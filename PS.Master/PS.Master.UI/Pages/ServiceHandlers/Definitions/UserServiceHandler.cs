using PS.Master.UI.Auth.Services;
using PS.Master.UI.Pages.ServiceHandlers.Interfaces;
using PS.Master.ViewModels.Auth;
using PS.Master.ViewModels.Models;
using Helpers = PS.Master.UI.Helpers;

namespace PS.Master.UI.Pages.ServiceHandlers.Definitions
{
    public class UserServiceHandler : ServiceHandlerBase, IUserServiceHandler
    {
        private readonly IAccessTokenService accessTokenService;

        public UserServiceHandler(IAccessTokenService accessTokenService, IConfiguration configuration, HttpClient http) : base(configuration, http)
        {
            this.accessTokenService = accessTokenService;
        }

        public async Task<UserVM> GetLoginUserDetailsAsync()
        {
            bool isExpired = await IsTokenExpiredAsync();

            if (isExpired)
                return null;

            return await Post<UserVM>(Helpers.UriHelper.LoginUserDetails);
        }

        public async Task<bool> IsTokenExpiredAsync()
        {
            return await Post<bool>(Helpers.UriHelper.IsTokenExpired);
        }

        public async Task<AuthenticationResponse> LoginAsync()
        {
            return await Post<AuthenticationResponse>(Helpers.UriHelper.Login);
        }
    }
}
