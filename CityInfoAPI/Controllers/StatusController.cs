using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/cities")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private IStatusService _statusSvc;

        public StatusController(IStatusService statusSvc)
        {
            _statusSvc = statusSvc;
        }

        //https://localhost:44313/api/cities/async/status
        //https://cityinfoapi2.azurewebsites.net/api/cities/async/status
        [HttpGet("async/status")]
        public async Task<IActionResult> GetStatusAsync(CancellationToken cancellationToken)
        {
            var result = await _statusSvc.GetStatusAsync(cancellationToken);
            if (result == "Health Check OK")
                return Ok(result);
            else
                return StatusCode(500);
        }

        //https://localhost:44313/api/cities/async/version
        //https://cityinfoapi2.azurewebsites.net/api/cities/async/version
        [HttpGet("async/version")]
        public async Task<IActionResult> GetVersion()
        {
            string versionResult = await _statusSvc.GetVersion();
            return Ok($"Version: {versionResult}");
        }
    }
}