using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Services.Interfaces
{
    public interface IAppManagerService
    {
        Task<DeployResult> DeployWebApplication(AppArtifacts appArtifacts);
        Task<DeployResult> DeployWebApplication(AppArtifacts appArtifacts, string masterAppBaseUri);
        Task<List<DeployResult>> GetDeployedApps();
    }
}
