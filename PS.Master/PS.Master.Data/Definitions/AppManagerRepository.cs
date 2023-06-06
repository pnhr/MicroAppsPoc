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
using Microsoft.Data.SqlClient;

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

        public async Task<bool> ExecuteDbScript(string sqlFilePath, string dbName, string connStr)
        {
            Deploy(sqlFilePath, dbName, connStr);
            return true;
        }

        private string Deploy(string sqlFilePath, string dbName, string connStr)
        {
            List<string> batches = GetBatches(sqlFilePath, dbName);
            SqlConnectionStringBuilder sqlConnBuilder = new SqlConnectionStringBuilder(connStr);
            sqlConnBuilder.TrustServerCertificate = true;
            SqlConnection conn = null;
            bool isDbCreated = false;
            try
            {
                foreach (var ste in batches)
                {
                    if (string.IsNullOrEmpty(ste))
                        continue;
                    if (isDbCreated == false)
                    {
                        sqlConnBuilder.InitialCatalog = "master";
                        using (conn = new SqlConnection(sqlConnBuilder.ConnectionString))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(ste, conn);
                            cmd.ExecuteNonQuery();
                        }
                        if (conn != null)
                        {
                            conn.Close();
                        }
                        isDbCreated = true;
                    }
                    else
                    {
                        sqlConnBuilder.InitialCatalog = dbName;
                        using (conn = new SqlConnection(sqlConnBuilder.ConnectionString))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(ste, conn);
                            cmd.ExecuteNonQuery();
                        }
                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
            return "Database have been complted successfully!";
        }

        private List<string> GetBatches(string sqlFilePath, string dbName)
        {
            List<string> batches = new List<string>();
            if (File.Exists(sqlFilePath))
            {
                string script = File.ReadAllText(sqlFilePath);
                if (!string.IsNullOrEmpty(script))
                {
                    batches = script.Split("GO")?.ToList();
                }
                batches.RemoveAll(x => x.ToUpper().Contains($"USE [MASTER]"));
                batches.RemoveAll(x => x.ToUpper().Contains($"USE {dbName.ToUpper()}"));
                batches.RemoveAll(x => x.ToUpper().Contains($"USE [{dbName.ToUpper()}]"));

            }
            return batches;
        }
    }
}
