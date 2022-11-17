using PS.Master.ViewModels.Models;

namespace PS.Master.UI.Pages.ServiceHandlers.Interfaces
{
    public interface IAppManagerServiceHandler
    {
        public Task<string> Deploy(AppArtifacts appArtifacts);
    }
}
