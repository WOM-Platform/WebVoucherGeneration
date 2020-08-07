using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace VoucherGenerationService.Controllers {
    
    [Route("")]
    public class MainController : Controller {

        [HttpGet("")]
        public IActionResult ShowHome() {
            return Content("Hello!");
        }

        [HttpGet("set")]
        public IActionResult Set() {
            HttpContext.Session.SetString("Test", "Prova");
            return Content("All set");
        }

        [HttpGet("reset")]
        public IActionResult Reset() {
            HttpContext.Session.Clear();
            return Content("Cleared");
        }

        [HttpGet("check")]
        [RequireSession("Test")]
        public IActionResult Check() {
            return Content("Everything OK!");
        }

    }

}
