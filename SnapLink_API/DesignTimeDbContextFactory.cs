using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SnapLink_Repository.DBContext;

namespace SnapLink_API
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SnaplinkDbContext>
    {
        public SnaplinkDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<SnaplinkDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("SnapLink_API"));

            return new SnaplinkDbContext(builder.Options);
        }
    }
} 