using PS.Master.ViewModels.Auth;
using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Login(HttpContext context);
        Task<UserVM> GetUserByToken(string token);
        Task<bool> IsTokenExpired(string token);
    }
}
