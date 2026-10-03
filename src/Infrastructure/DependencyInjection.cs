using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Extension method for registering Infrastructure-layer services with the
/// DI container. Keeps Program.cs thin and lets the Infrastructure project
/// own the details of how its services are configured.
/// </summary>
public static class DependencyInjection
{   // composition root pattern: the outermost layer wires everything, but each layer owns its own wiring.


    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {   
        //
        var connectionString = configuration.GetConnectionString("DefaultConnection")?? throw  new  InvalidOperationException("Please set connection string in appsettings.json");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });


        return services;
    }
    
}