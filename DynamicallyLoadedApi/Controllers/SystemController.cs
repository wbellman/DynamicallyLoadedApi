#region imports

using System;
using System.Net;
using DynamicallyLoadedApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DynamicallyLoadedApi.Controllers {

  [Route("api/[controller]")]
  [ApiController]
  public class SystemController : ControllerBase {

    public SystemController(Watcher watcher) {
      Watcher = watcher;
    }

    private Watcher Watcher { get; }

    // GET
    [HttpGet("assemblies/")]
    public IActionResult Assemblies() {
      return Ok(new {assemblies = Watcher.Assemblies});
    }

    [HttpGet("assemblies/unload")]
    public IActionResult Unload([FromQuery] string name) {
      try {
        Watcher.UnloadAssembly(name);
        return Ok(new {message = "Unloaded", timestamp = DateTime.Now});
      } catch (Exception ex) {
        //Report.Exception(ex);
        return StatusCode((int) HttpStatusCode.InternalServerError,
                          new {
                            message   = $"{ex.GetType().Name}: {ex.Message}", stacktrace = ex.StackTrace,
                            timestamp = DateTime.Now
                          });
      }
    }
  }
}