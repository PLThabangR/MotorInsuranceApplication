using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;
/// <summary>
/// The EF Core DbContext for the Motor Insurance application.
///
/// Responsibilities:
/// - Expose DbSet<T> properties for each aggregate root entity.
/// - Apply entity type configurations defined in the Configurations folder.
/// - Enforce conventions (UTC timestamps, sequential Guids).
///
/// It does NOT contain business logic. Queries and commands belong in the
/// Application layer (handlers). The DbContext is a thin persistence concern.
/// </summary>
public class ApplicationDbContext:DbContext
{   
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    // --- DbSets ---
    // Expose one DbSet per entity so handlers can query them.

    public DbSet<PolicyHolder> PolicyHolders => Set<PolicyHolder>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<VehicleDriver> VehicleDrivers => Set<VehicleDriver>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<Telematics> Telematics => Set<Telematics>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> classes in this assembly.
        // This keeps entity configuration close to the entity it configures
        // and avoids a giant OnModelCreating method.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
}