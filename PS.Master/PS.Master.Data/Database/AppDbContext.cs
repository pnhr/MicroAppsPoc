using Microsoft.EntityFrameworkCore;
using PS.Master.Domain;
using PS.Master.Domain.DbModels;
using System.Reflection.Metadata;

namespace PS.Master.Data.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ApplicationHost> ApplicationHosts { get; set; }
        public DbSet<MasterConfig> MasterConfig { get; set; }
        public DbSet<AppServer> AppServers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppServer>().HasIndex(b => b.ServerCode).IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}