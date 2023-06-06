using PS.Master.ViewModels.Models;
using static System.Net.WebRequestMethods;

namespace PS.Master.UI.Pages
{
    public partial class Index
    {
        public List<DeployResult> DeployedApps { get; set; }
        private System.Threading.Timer? timer;
        protected override async Task OnInitializedAsync()
        {
            DeployedApps = await appMangerServiceHandler.GetDeployedApplications();
            //timer = new System.Threading.Timer(async (object? stateInfo) =>
            //{
            //    DeployedApps = await appMangerServiceHandler.GetDeployedApplications();
            //    StateHasChanged(); // NOTE: MUST CALL StateHasChanged() BECAUSE THIS IS TRIGGERED BY A TIMER INSTEAD OF A USER EVENT
            //}, new System.Threading.AutoResetEvent(false), (1000), 500); // fire every 1000 milliseconds
        }
    }
}
