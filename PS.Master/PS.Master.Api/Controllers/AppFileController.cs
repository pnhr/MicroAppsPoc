using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PS.Master.Api.Services.Definitions;
using PS.Master.Api.Services.Interfaces;
using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAppFileService _appFileService;
        private readonly IConfiguration _config;
        private readonly ILogger<AppManagerController> _logger;

        public AppFileController(IAppFileService appFileService, IConfiguration config, ILogger<AppManagerController> logger, IWebHostEnvironment env)
        {
            this._appFileService = appFileService;
            this._config = config;
            this._logger = logger;
            this._env = env;
        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<string>> PostFile([FromForm] IFormFile files, [FromQuery(Name = "appName")] string appName)
        {
            try
            {
                string stageFolder = await _appFileService.StageAppFile(files, appName);
                return stageFolder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "Something went wrong while uploading file");
            }
        }
    }
}
