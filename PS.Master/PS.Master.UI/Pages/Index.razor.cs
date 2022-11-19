using PS.Master.ViewModels.Models;

namespace PS.Master.UI.Pages
{
    public partial class Index
    {
        public List<DeployResult> DeployedApps { get; set; }
        protected override async Task OnInitializedAsync()
        {
            DeployedApps = await appMangerServiceHandler.GetDeployedApplications();
        }
    }
}
