using PS.Master.UI.Pages.ServiceHandlers.Interfaces;
using PS.Master.ViewModels.Models;
using Helpers = PS.Master.UI.Helpers;

namespace PS.Master.UI.Pages.ServiceHandlers.Definitions
{
    public class SampleServiceHandler : ServiceHandlerBase, ISampleServiceHandler
    {
        public SampleServiceHandler(IConfiguration configuration, HttpClient http) : base(configuration, http)
        {
        }

        public async Task<List<UserVM>> GetUsersAsync()
        {
            return await Get<List<UserVM>>(Helpers.UriHelper.SampleUsers);
        }
    }
}
