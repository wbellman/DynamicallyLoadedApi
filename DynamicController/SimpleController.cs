#region imports

using System;
using DynamicDependency;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DynamicController.v2 {

  [Route("api/v2/[controller]")]
  [ApiController]
  public class SimpleController : ControllerBase {
    [HttpGet("test/")]
    public IActionResult Direction() {
      return Ok(new {message = "Controller loaded.", timestamp = DateTime.Now});
    }

    [HttpGet("speak/")]
    public IActionResult Speak() {
      var to = new TestObject();
      return Ok(new {message = to.Speak(), timestamp = DateTime.Now});
    }
  }
}