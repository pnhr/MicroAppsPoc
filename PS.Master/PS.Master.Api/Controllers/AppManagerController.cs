using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PS.Master.Api.Services.Interfaces;
using PS.Master.ViewModels.Models;
using System.Text.Json.Nodes;

namespace PS.Master.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppManagerController : ControllerBase
    {
        private readonly IAppManagerService _appManagerService;
        private readonly IConfiguration _config;
        private readonly ILogger<AppManagerController> _logger;

        public AppManagerController(IAppManagerService adminService, IConfiguration config, ILogger<AppManagerController> logger)
        {
            this._appManagerService = adminService;
            this._config = config;
            this._logger = logger;
        }

        [HttpPost]
        [Route("deploywebapp")]
        [Authorize]
        public async Task<ActionResult<DeployResult>> DeployWebApp(AppArtifacts appArtifacts)
        {
            try
            {
                #if DEBUG
                DeployResult result = await _appManagerService.DeployWebApplication(appArtifacts);
                #endif

                #if !DEBUG
                string masterAppUrl = $"{Request.Scheme}://{Request.Host}:{Request.Host.Port ?? 80}";
                DeployResult result = await _appManagerService.DeployWebApplication(appArtifacts, masterAppUrl);
                #endif

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "Something went wrong while deploying files");
            }
        }

        [HttpGet]
        [Route("getapps")]
        [Authorize]
        public async Task<ActionResult<List<DeployResult>>> GetDeployedApps()
        {
            try
            {
                return await _appManagerService.GetDeployedApps();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "Something went wrong while deploying files");
            }
        }
    }
}
