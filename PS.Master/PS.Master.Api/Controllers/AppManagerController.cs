using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> DeployWebApp(AppArtifacts appArtifacts)
        {
            _appManagerService.Request = Request;
            string link = await _appManagerService.DeployWebApplication(appArtifacts);
            return link;
        }
    }
}
