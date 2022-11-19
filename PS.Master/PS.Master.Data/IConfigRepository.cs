using PS.Master.Domain;
using PS.Master.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data
{
    public interface IConfigRepository
    {
        Task<List<MasterConfig>> GetConfig();
        Task<List<MasterConfig>> GetActiveConfig();
        Task<MasterConfig> GetConfig(string configKey);
    }
}
