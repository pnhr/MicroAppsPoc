using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PS.Master.Api.Services.Interfaces;
using PS.Master.Data;
using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ISampleService _sampleService;
        public SampleController(ISampleService sampleService)
        {
            this._sampleService = sampleService;
        }

        [HttpGet]
        [Route("users")]
        [Authorize]
        public async Task<ActionResult<List<UserVM>>> GetUsers()
        {
            return await _sampleService.GetUsers();
        }
    }
}
