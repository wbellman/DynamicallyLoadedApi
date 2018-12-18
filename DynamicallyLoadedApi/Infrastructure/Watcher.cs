#region imports

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#endregion

namespace DynamicallyLoadedApi.Infrastructure {
  public class Watcher : IDisposable {
    public Watcher(
      ApplicationPartManager manager,
      IHostingEnvironment environment,
      IApplicationLifetime lifetime,
      ILogger<Watcher> log,
      IOptionsMonitor<WatcherSettings> monitor
    ) {
      ChangeProvider = DynamicControllerChangeProvider.Instance;
      Manager        = manager;
      Environment    = environment;
      Lifetime       = lifetime;
      Log            = log;
      Monitor        = monitor;

      Semaphore = new SemaphoreSlim(1);

      Monitor.OnChange(OnSettingsChange);

      InitializePlugins(Settings.PluginDir);

      UpdateFileWatch(Settings.PluginDir);
    }

    private IOptionsMonitor<WatcherSettings> Monitor        { get; }
    private ILogger<Watcher>                 Log            { get; }
    private IApplicationLifetime             Lifetime       { get; }
    private ControllerFeature                ControllerFeature    { get; }
    private ApplicationPartManager           Manager        { get; }
    private IHostingEnvironment              Environment    { get; }
    private DynamicControllerChangeProvider  ChangeProvider { get; }
    private FileSystemWatcher                FileWatch      { get; set; }
    private SemaphoreSlim                    Semaphore      { get; }


    private WatcherSettings Settings => Monitor.CurrentValue;

    private void UpdateFileWatch(string path) {
      try {
        if (!Directory.Exists(path)) {
          Directory.CreateDirectory(path);
        }

        if (FileWatch != null) {
          Log.LogInformation($"Redirecting Watcher to path: {path}");
          FileWatch.Path = path;
          return;
        }

        Log.LogInformation($"Setting up Watcher on path: {path}");
        FileWatch = new FileSystemWatcher(path);

        Log.LogInformation("Setting up actions...");
        FileWatch.Changed  += OnChanged;
        FileWatch.Created  += OnCreated;
        FileWatch.Deleted  += OnDeleted;
        FileWatch.Error    += OnError;
        FileWatch.Renamed  += OnRenamed;
        FileWatch.Disposed += OnDisposed;

        Log.LogInformation("Enabling events...");
        FileWatch.EnableRaisingEvents = true;
      } catch (Exception ex) {
        Log.LogError(ex, "Error occured setting up file watch. File watch disabled.");
        FileWatch?.Dispose();
        FileWatch = null;
      }
    }

    private void InitializePlugins(string path) {
      foreach (var file in Directory.EnumerateFiles(path)) {
        LoadAssembly(file);
      }
    }

    private void OnSettingsChange(WatcherSettings settings) {
      Log.LogInformation("Plugin directory changed.  New Dir: {path}", settings.PluginDir);
      UpdateFileWatch(settings.PluginDir);
      InitializePlugins(settings.PluginDir);
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
      Log.LogInformation("CREATED:  {@args}", e);
      LoadAssembly(e.FullPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) {
      Log.LogInformation("DELETED:  {@args}", e);
    }

    private void OnError(object sender, ErrorEventArgs e) {
      Log.LogInformation("ERROR:    {@args}", e);
    }

    private void OnRenamed(object sender, RenamedEventArgs e) {
      Log.LogInformation("RENAMED:  {@args}", e);
    }

    private void OnDisposed(object sender, EventArgs e) {
      Log.LogInformation("DISPOSED: {@args}", e);
    }

    private void OnChanged(object sender, FileSystemEventArgs e) {
      Log.LogInformation("CHANGED:  {@args}", e);
    }

    private void LoadAssembly(string path) {
      if (!path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)) {
        return;
      }

      Semaphore.Wait();
      try {
        Log.LogInformation("Loading Assembly: {path}", path);

        var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        if (asm == null) {
          Log.LogWarning("Failed to load: {path}", path);
          //return;
        }

        UnloadAssembly(asm.GetName().Name);
        Manager.ApplicationParts.Add(new AssemblyPart(asm));

        var controllerFeature = new ControllerFeature();
        Manager.PopulateFeature(controllerFeature);

      } catch (Exception ex) {
        //Report.Exception(ex);
        Log.LogError(ex,"Load failed");
        Log.LogError("Failed to load assembly. Path {path}", path);
      } finally {
        Semaphore.Release();
      }
    }

    public IEnumerable<string> Assemblies {
      get { return Manager.ApplicationParts.Select(part => part.Name); }
    }

    public void UnloadAssembly(string name) {
      var unload = false;
      var index  = -1;
      foreach (var part in Manager.ApplicationParts) {
        if (!part.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
          continue;
        }

        unload = true;
        index  = Manager.ApplicationParts.IndexOf(part);
      }

      if (unload) {
        Manager.ApplicationParts.RemoveAt(index);
      }
    }

    public void Dispose() {
      FileWatch?.Dispose();
    }
  }

  public class WatcherSettings {
    public string PluginDir { get; set; }
  }
}