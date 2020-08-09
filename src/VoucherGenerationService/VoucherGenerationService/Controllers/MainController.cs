using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;
using WomPlatform.Web.Generator.ViewModels;

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
            var login = await _client.LoginAsSource(session.Username, session.Password);

            return View("Instruments", login);
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [RequireSession(Program.SessionKeyLogin)]
        [HttpGet("source/{sourceId}")]
        public async Task<IActionResult> ShowGenerationForm(
            [FromRoute] string sourceId
        ) {
            ViewBag.SourceId = sourceId;

            var aims = await _client.GetAims();

            return View("SourceForm", new GenerationFormViewModel {
                Aims = aims.Aims
            });
        }

        [RequireSession(Program.SessionKeyLogin)]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateVouchers(
            [FromForm] string sourceId,
            [FromForm] int genCount,
            [FromForm] int voucherCount,
            [FromForm] double latitude,
            [FromForm] double longitude,
            [FromForm] string date,
            [FromForm] string time,
            [FromForm] string aimCode
        ) {
            if(sourceId == null) {
                _logger.LogError("SourceID not set");
                return RedirectToAction(nameof(ShowInstruments));
            }
            if(genCount <= 0) {
                ErrorMessage = "Generation count cannot be 0 or negative";
                return RedirectToAction(nameof(ShowGenerationForm), new { sourceId });
            }
            if(voucherCount <= 0) {
                ErrorMessage = "Number of vouchers cannot be 0 or negative";
                return RedirectToAction(nameof(ShowGenerationForm), new { sourceId });
            }

            var tsString = $"{date}T{time}";
            if(!DateTime.TryParseExact(tsString, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var timestamp)) {
                _logger.LogError("Timestamp string {0} not parsable", tsString);

                ErrorMessage = "Date or time not set correctly";
                return RedirectToAction(nameof(ShowGenerationForm), new { sourceId });
            }

            var session = HttpContext.Session.GetObject<SessionInfo>(Program.SessionKeyLogin);
            var login = await _client.LoginAsSource(session.Username, session.Password);

            string key = GetKey(login, sourceId);
            if(key == null) {
                _logger.LogError("SourceID does not match logged-in user's authorized sources");
                return RedirectToAction(nameof(ShowInstruments));
            }

            using var keyStream = new MemoryStream(Encoding.ASCII.GetBytes(key));
            var instrument = _client.CreateInstrument(sourceId, keyStream);

            _logger.LogInformation("Requesting {0} vouchers for aim {1}, in {2:F3}x{3:F3} at {4} ({5} reps)",
                voucherCount, aimCode, latitude, longitude, timestamp, genCount);

            var voucherInfo = new VoucherCreatePayload.VoucherInfo {
                Aim = aimCode,
                Count = voucherCount,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = timestamp
            };
            try {
                var result = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                    voucherInfo
                });
                return Content($"{result.Link} {result.Password}");
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Failed to request vouchers");
                
                ErrorMessage = "Failed to request vouchers";
                return RedirectToAction(nameof(ShowGenerationForm), new { sourceId });
            }
        }

        private static string GetKey(SourceLoginResultV1 loginResult, string sourceId) {
            foreach (var s in loginResult.Sources) {
                if (s.Id == sourceId) {
                    return s.PrivateKey;
                }
            }

            return null;
        }

    }

}
