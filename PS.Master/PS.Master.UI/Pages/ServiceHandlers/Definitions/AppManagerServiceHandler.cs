using PS.Master.UI.Pages.ServiceHandlers.Interfaces;
using PS.Master.ViewModels.Models;
using Helpers = PS.Master.UI.Helpers;

namespace PS.Master.UI.Pages.ServiceHandlers.Definitions
{
    public class AppManagerServiceHandler : ServiceHandlerBase, IAppManagerServiceHandler
    {
        public AppManagerServiceHandler(IConfiguration configuration, HttpClient http) : base(configuration, http)
        {
        }

        public async Task<DeployResult> Deploy(AppArtifacts appArtifacts)
        {
            DeployResult deployResult = await Post<AppArtifacts, DeployResult>(appArtifacts, Helpers.UriHelper.DeployWebApp);
            return deployResult;
        }

        public async Task<string> UploadAppFile(MultipartFormDataContent fileContent, string appName)
        {
            Dictionary<string, string> queryStringsToBeApended = new Dictionary<string, string>();
            queryStringsToBeApended.Add("appName", appName);
            string stageFolderPath = await PostFile(fileContent, Helpers.UriHelper.AppFileUpload, queryStringsToBeApended);
            return stageFolderPath;
        }
    }
}
