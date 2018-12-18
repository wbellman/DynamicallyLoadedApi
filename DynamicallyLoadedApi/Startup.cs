#region imports

using DynamicallyLoadedApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endregion

namespace DynamicallyLoadedApi {
  public class Startup {
    public Startup(IConfiguration configuration, ILogger<Startup> log, ILoggerFactory factory) {
      Configuration = configuration;
      Log           = log;
    }

    private ILogger<Startup> Log           { get; }
    private IConfiguration   Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services
       .Configure<WatcherSettings>(Configuration.GetSection("watcher"));

      services
       .AddMvc();

      services
        .AddSingleton<Watcher>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    // Watcher is included here for purposes of instantiation.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, Watcher watch, ILoggerFactory factory) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      } else {
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();

      //Shared.Logging = factory;
    }
  }
}