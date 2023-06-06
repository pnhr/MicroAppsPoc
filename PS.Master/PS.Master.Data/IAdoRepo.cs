using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data
{
    public interface IAdoRepo
    {
        Task<bool> CreateDatabase(List<string> batches, string dbName = "");
    }
}
