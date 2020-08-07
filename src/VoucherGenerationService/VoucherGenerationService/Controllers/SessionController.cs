using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WomPlatform.Connector;

namespace VoucherGenerationService.Controllers {
    
    [Route("session")]
    public class SessionController : Controller {

        private readonly Client _client;
        private readonly ILogger<SessionController> _logger;

        public SessionController(Client c, ILogger<SessionController> logger) {
            _client = c;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromForm] string username,
            [FromForm] string password
        ) {
            try {
                await _client.LoginAsMerchant(username, password);
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Failed to login");
                return RedirectToAction(nameof(MainController.ShowHome), "Main");
            }

            HttpContext.Session.SetString(Program.SessionKeyLogin, JsonSerializer.Serialize(
                new SessionInfo {
                    Username = username,
                    Password = password
                }
            ));
        }

    }

}
