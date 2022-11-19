using PS.Master.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data
{
    public interface IAppManagerRepository
    {
        Task<List<AppServer>> GetActiveAppServers();
        Task<AppServer> GetActiveAppServers(string serverCode);
        Task AddApplication(ApplicationHost applicationHost);
    }
}
