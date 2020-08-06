using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoucherGenerationService.Controllers {
    
    [Route("")]
    public class MainController : Controller {

        [HttpGet("")]
        public IActionResult ShowHome() {
            return Content("Hello!");
        }

    }

}
