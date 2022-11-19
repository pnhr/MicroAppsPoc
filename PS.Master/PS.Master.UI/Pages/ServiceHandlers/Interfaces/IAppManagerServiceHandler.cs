using PS.Master.ViewModels.Models;

namespace PS.Master.UI.Pages.ServiceHandlers.Interfaces
{
    public interface IAppManagerServiceHandler
    {
        public Task<DeployResult> Deploy(AppArtifacts appArtifacts);
        public Task<string> UploadAppFile(MultipartFormDataContent fileContent, string appName);
        public Task<List<DeployResult>> GetDeployedApplications();
    }
}
