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
  }
}