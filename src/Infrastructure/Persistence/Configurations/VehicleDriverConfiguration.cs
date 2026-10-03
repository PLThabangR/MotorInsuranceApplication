using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class VehicleDriverConfiguration:IEntityTypeConfiguration<VehicleDriver>
{

    public void Configure(EntityTypeBuilder<VehicleDriver> builder)
    {
        builder.ToTable("VehicleDrivers");

        builder.HasKey(vd => vd.Id);

        builder.Property(vd => vd.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.Property(vd => vd.IsPrimaryDriverForVehicle).IsRequired();
        builder.Property(vd => vd.EffectiveFrom).IsRequired();
        builder.Property(vd => vd.EffectiveTo);

        builder.Property(vd => vd.CreatedAt).IsRequired();
        builder.Property(vd => vd.UpdatedAt);
        
            //Composite unique: a given driver can only be assigned to a given
        // vehicle on a given policy once per effective period.
        builder.HasIndex(vd => new { vd.VehicleId, vd.DriverId, vd.EffectiveFrom })
            .IsUnique()
            .HasDatabaseName("IX_VehicleDrivers_Vehicle_Driver_From");
        
        // Relationship with Vehicle.
        builder.HasOne(vd => vd.Vehicle)
            .WithMany(v => v.VehicleDrivers)
            .HasForeignKey(vd => vd.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Driver.
        builder.HasOne(vd => vd.Driver)
            .WithMany(d => d.VehicleDrivers)
            .HasForeignKey(vd => vd.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Policy (denormalized for fast policy-scoped queries).
        builder.HasOne(vd => vd.Policy)
            .WithMany(p => p.VehicleDrivers)
            .HasForeignKey(vd => vd.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for foreign keys.
        builder.HasIndex(vd => vd.VehicleId).HasDatabaseName("IX_VehicleDrivers_VehicleId");
        builder.HasIndex(vd => vd.DriverId).HasDatabaseName("IX_VehicleDrivers_DriverId");
        builder.HasIndex(vd => vd.PolicyId).HasDatabaseName("IX_VehicleDrivers_PolicyId");
        
        
        
        
    }
}