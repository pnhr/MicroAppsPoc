using PS.Master.ViewModels.Auth;
using PS.Master.ViewModels.Models;

namespace PS.Master.UI.Pages.ServiceHandlers.Interfaces
{
    public interface IUserServiceHandler
    {
        Task<AuthenticationResponse> LoginAsync();
        Task<bool> IsTokenExpiredAsync();
        Task<UserVM> GetLoginUserDetailsAsync();
    }
}
