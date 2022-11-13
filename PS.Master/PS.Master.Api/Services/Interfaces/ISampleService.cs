using PS.Master.Domain;
using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Services.Interfaces
{
    public interface ISampleService
    {
        Task<List<UserVM>> GetUsers();
    }
}
