using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PS.Master.Data.Database;
using PS.Master.Domain.DbModels;
using PS.Master.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data.Definitions
{
    public class AppManagerRepository : IAppManagerRepository
    {
        private readonly AppDbContext _dbContext;
        public AppManagerRepository(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<List<AppServer>> GetActiveAppServers()
        {
            return await _dbContext.AppServers.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<AppServer> GetActiveAppServers(string serverCode)
        {
            return await _dbContext.AppServers.FirstOrDefaultAsync(x => x.ServerCode == serverCode);
        }

        public async Task AddApplication(ApplicationHost applicationHost)
        {
            _dbContext.ApplicationHosts.Add(applicationHost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<DeployedAppInfo>> GetDeployedApps()
        {
            var apps = await _dbContext.ApplicationHosts.Where(x => x.IsActive)
                                                    .Select(x => new DeployedAppInfo { AppLogo = x.AppLogo, 
                                                                                        AppName = x.AppName,
                                                                                        AppUrl = x.AppVPath,
                                                                                        AppDisplayName = x.AppDisplayName,
                                                                                        AppDiscription = x.AppDiscription
                                                    }).ToListAsync();

            return apps;
        }
    }
}
