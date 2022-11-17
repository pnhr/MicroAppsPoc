using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Services.Interfaces
{
    public interface IAppManagerService
    {
        HttpRequest Request { get; set; }
        Task<string> DeployWebApplication(AppArtifacts appArtifacts);
    }
}
