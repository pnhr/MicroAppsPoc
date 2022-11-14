using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PS.Master.Api.Services.Interfaces;

namespace PS.Master.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppManagerController : ControllerBase
    {
        private readonly IAppManagerService _adminService;
        private readonly IConfiguration _config;
        private readonly ILogger<AppManagerController> _logger;

        public AppManagerController(IAppManagerService adminService, IConfiguration config, ILogger<AppManagerController> logger)
        {
            this._adminService = adminService;
            this._config = config;
            this._logger = logger;
        }

        [HttpGet]
        [Route("creatvdir")]
        public async Task<bool> CreateVDir(string name)
        {
            bool isCreated = await _adminService.CreateVirtualDir(name);
            return isCreated;
        }
    }
}
