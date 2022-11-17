using Microsoft.EntityFrameworkCore;
using PS.Master.Data.Database;
using PS.Master.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data.Definitions
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly AppDbContext _db;

        public ConfigRepository(AppDbContext appDbContext)
        {
            this._db = appDbContext;
        }

        public async Task<List<MasterConfig>> GetActiveConfig()
        {
            return await _db.MasterConfig.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<List<MasterConfig>> GetConfig()
        {
            return await _db.MasterConfig.ToListAsync();
        }

        public async Task<MasterConfig> GetConfig(string configKey)
        {
            MasterConfig config = await _db.MasterConfig.FirstOrDefaultAsync(x => x.Key.ToLower() == configKey.ToLower());
            return config;
        }
    }
}
