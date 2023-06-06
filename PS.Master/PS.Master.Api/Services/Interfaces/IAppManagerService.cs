using PS.Master.Domain.DbModels;
using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Services.Interfaces
{
    public interface IAppManagerService
    {
        Task<DeployResult> DeployWebApplication(AppArtifacts appArtifacts);
        Task<List<DeployResult>> GetDeployedApps();
        Task<string> CreateWebsite(AppServer appServer, string siteName, bool isDefaultAppPoolNeeded = true);
        Task<bool> ExecuteDbScript(string scriptPath);
    }
}
