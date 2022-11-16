using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PS.Master.Api.Services.Interfaces;
using PS.Master.ViewModels.Auth;
using PS.Master.ViewModels.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace PS.Master.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;


        public UserController(IConfiguration config, IUserService userService, ILogger<UserController> logger)
        {
            this.config = config;
            this.userService = userService;
            this.logger = logger;
        }

        [HttpPost]
        [Route("login")]
        //[Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Login()
        {
            return await userService.Login(HttpContext);
        }

        [HttpPost]
        [Route("istokenexpired")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<bool>> IsTokenExpired()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(' ')[1];
                return await userService.IsTokenExpired(token);
            }
            return await Task.FromResult(true);
        }

        [HttpPost]
        [Route("getuserbyjwt")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserVM>> GetUserByToken()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Split(' ')[1];
                return await userService.GetUserByToken(token);
            }
            return null;
        }

        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}
