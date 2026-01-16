namespace BiografOpgave.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            // Path to the API folder (where appsettings.json lives)
            var apiBasePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "BiografOpgave.API");

            var config = new ConfigurationBuilder()
                .SetBasePath(apiBasePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new DatabaseContext(options);
        }
    }
}
