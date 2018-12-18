#region imports

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

#endregion

namespace DynamicallyLoadedApi {
  public class Program {
    public static void Main(string[] args) {
      CreateWebHostBuilder(args).Build().Run();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args) {
      return new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((ctx, config) => {
               config.AddJsonFile("appsettings.json", false, true);
               config.AddJsonFile("watcher.json", false, true);
             })
            .ConfigureLogging(logging => {
               logging.ClearProviders();
               logging.SetMinimumLevel(LogLevel.Trace);
             })
            .UseNLog()
            .UseStartup<Startup>();
    }
  }
}