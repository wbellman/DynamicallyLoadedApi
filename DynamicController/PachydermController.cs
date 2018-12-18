#region imports

using System;
using DataSteward.Persistence.Stub.Infrastructure;
using DynamicDependency;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DynamicController.v2 {

  [Route("api/v2/[controller]")]
  [ApiController]
  public class PachydermController : ControllerBase {
    [HttpGet("direction/")]
    public IActionResult Direction() {
      return Ok(new {message = "North moving elephant.", timestamp = DateTime.Now});
    }

    [HttpGet("speak/")]
    public IActionResult Speak() {
      var to = new TestObject();
      return Ok(new {message = to.Speak(), timestamp = DateTime.Now});
    }

    [HttpGet("random/")]
    public IActionResult Random() {
      return Ok(new {message = $"We all got a ${Generator.Words.Next()} ${Generator.Words.Next()} ${Generator.Words.Next()} thing waiting for us...", timestamp = DateTime.Now});
    }
  }
}