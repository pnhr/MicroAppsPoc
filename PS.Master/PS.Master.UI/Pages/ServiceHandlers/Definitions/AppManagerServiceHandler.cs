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

        public async Task<string> Deploy(AppArtifacts appArtifacts)
        {
            string uri = await PostAndGetString<AppArtifacts>(appArtifacts, Helpers.UriHelper.DeployWebApp);
            return uri;
        }
    }
}
