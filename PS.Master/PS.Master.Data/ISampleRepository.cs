using PS.Master.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data
{
    public interface ISampleRepository
    {
        Task<List<Employee>> GetEmployeesAsync();
    }
}
