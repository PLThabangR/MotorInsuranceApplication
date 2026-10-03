using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;
/// <summary>
/// Allows EF Core CLI tools (dotnet ef) to construct the DbContext at design
/// time without booting the whole API. This is the recommended pattern when
/// the DbContext lives in a class library separate from the startup project.
///
/// It reads the connection string from the API project's appsettings files
/// using an explicit base path, because the CLI may run from the solution root
/// or the Infrastructure project folder.
/// </summary>
public class DesignTimeDbContextFactory:IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Locate the API project's appsettings files relative to the solution.
        // We walk up from the current directory until we find the API folder.
        var basePath = Directory.GetCurrentDirectory();
        // If running from Infrastructure folder, walk up to the solution root,
        // then into the API folder.
        var apiProjectPath = Path.Combine(basePath, "src", "API");
        if (!Directory.Exists(apiProjectPath))
        {
            // Fallback: try one level up (when running from inside src/*)
            apiProjectPath = Path.GetFullPath(Path.Combine(basePath, "..", "API"));
        }
        
        //We are using the package configurationbuilder from microsoft 
        //to get onfo from appSettings.json
        // Build the application's configuration manually because the API
        // application's normal startup process is not executed by
        // Entity Framework Core at design time.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in appsettings.");
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // Create the options used to configure ApplicationDbContext.
        // Configure Entity Framework Core to use PostgreSQL through Npgsql.
        optionsBuilder.UseNpgsql(connectionString, npgsql =>
        {
            // Migrations are stored in Infrastructure, not API.
            npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        });
        // Create and return the fully configured DbContext.
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}