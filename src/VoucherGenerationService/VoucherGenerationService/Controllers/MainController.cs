using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WomPlatform.Connector;

namespace WomPlatform.Web.Generator.Controllers {
    
    [Route("")]
    public class MainController : Controller {

        private readonly Client _client;
        private readonly ILogger<MainController> _logger;

        public MainController(Client c, ILogger<MainController> logger) {
            _client = c;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult ShowHome() {
            return View("Login");
        }

        [RequireSession(Program.SessionKeyLogin)]
        [HttpGet("source")]
        public async Task<IActionResult> ShowInstruments() {
            var session = HttpContext.Session.GetObject<SessionInfo>(Program.SessionKeyLogin);

            var result = await _client.LoginAsMerchant(session.Username, session.Password);

            return View("Instruments", result);
        }

        [RequireSession(Program.SessionKeyLogin)]
        [HttpGet("source/{sourceId}")]
        public IActionResult ShowGenerationForm(
            [FromRoute] string sourceId
        ) {
            ViewBag.SourceId = sourceId;

            return View("SourceForm");
        }

        [RequireSession(Program.SessionKeyLogin)]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateVouchers(
            [FromForm] string sourceId,
            [FromForm] int genCount,
            [FromForm] int voucherCount,
            [FromForm] double latitude,
            [FromForm] double longitude,
            [FromForm] DateTime timestamp,
            [FromForm] string aimCode
        ) {
            return Content("ALALAL");
        }

    }

}
