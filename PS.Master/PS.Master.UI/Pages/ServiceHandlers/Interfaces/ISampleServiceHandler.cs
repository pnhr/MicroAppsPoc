using PS.Master.ViewModels.Models;

namespace PS.Master.UI.Pages.ServiceHandlers.Interfaces
{
    public interface ISampleServiceHandler
    {
        Task<List<UserVM>> GetUsersAsync();
    }
}
