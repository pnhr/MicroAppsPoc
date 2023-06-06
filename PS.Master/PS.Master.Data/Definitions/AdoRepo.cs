using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PS.Master.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Data.Definitions
{
    public class AdoRepo : IAdoRepo
    {
        private readonly IConfiguration _config;

        public AdoRepo(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> CreateDatabase(List<string> batches, string dbName = "")
        {
            SqlConnection conn = null;
            string dbConnStr = _config.GetConnectionString("AppMasterDbConnection");
            try
            {
                foreach(var ste in batches)
                {
                    //string temp = ste.Replace("\n", "");
                    //temp = temp.Replace("\t", " ");
                    //temp = temp.Replace("\r", "");

                    if(string.IsNullOrEmpty(dbName))
                    {
                        using (conn = new SqlConnection(dbConnStr))
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
                    else
                    {
                        if (ste.ToUpper().Contains($"USE {dbName.ToUpper()}"))
                            continue;

                        SqlConnectionStringBuilder sqlConnBuilder = new SqlConnectionStringBuilder();
                        sqlConnBuilder.ConnectionString = dbConnStr;
                        sqlConnBuilder.DataSource = dbName;

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
            catch(Exception ex)
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

            return await Task.FromResult(true);
        }
    }
}
