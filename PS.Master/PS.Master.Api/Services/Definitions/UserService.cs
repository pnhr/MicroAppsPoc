using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PS.Master.Api.Services.Interfaces;
using PS.Master.ViewModels.Auth;
using PS.Master.ViewModels.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace PS.Master.Api.Services.Definitions
{
    public class UserService : IUserService
    {
        private readonly IConfiguration config;
        private readonly ILogger<UserService> logger;
        private readonly byte[]? secureKeyBytes;
        public UserService(IConfiguration config, ILogger<UserService> logger)
        {
            this.config = config;
            this.logger = logger;
            string secureKey = config["Authentication:JWTSettings:SecretKey"];
            secureKeyBytes = Encoding.ASCII.GetBytes(secureKey);
        }

        public async Task<UserVM> GetUserByToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secureKeyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = (JwtSecurityToken)securityToken;

            if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    UserVM user = new UserVM();
                    user.UserId = userId;

                    //Uncomment below to add few more claims
                    //user.FirstName = "<first name>";
                    //user.LastName = "<last name>";

                    return await Task.FromResult(user);
                }
            }
            return null;
        }

        public async Task<bool> IsTokenExpired(string jwtToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtToken);
                var hasExpired = token.ValidTo < DateTime.UtcNow;
                return await Task.FromResult(hasExpired);
            }
            catch (SecurityTokenException ex)
            {
                logger.LogError(ex.Message, ex);
            }
            return await Task.FromResult(true);
        }

        public async Task<AuthenticationResponse> Login(HttpContext context)
        {
            if (context == null)
                return null;

            UserVM userVm = new UserVM();

            if (context.User.Identity.IsAuthenticated)
                userVm.UserId = context.User.Identity.Name;
            else
                userVm.UserId = Environment.UserName; //WindowsIdentity.GetCurrent().Name;
            AuthenticationResponse authenticationResponse = new AuthenticationResponse();
            if (!string.IsNullOrWhiteSpace(userVm.UserId))
            {
                authenticationResponse.Token = GenerateJwtToken(userVm);
            }

            return await Task.FromResult(authenticationResponse);
        }

        private string GenerateJwtToken(UserVM userVm)
        {
            var claimUserId = new Claim(ClaimTypes.NameIdentifier, userVm.UserId);
            var claimEmail = new Claim(ClaimTypes.Email, userVm.Email ?? "");
            var claimFirstName = new Claim(AppClaimTypes.FirstName, userVm.FirstName ?? "");
            var claimLastName = new Claim(AppClaimTypes.LastName, userVm.LastName ?? "");

            var claimsIdentity = new ClaimsIdentity(new[] { claimUserId, claimEmail, claimFirstName, claimLastName }, "JwtServerAuth");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secureKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }
    }
}
