using System.Drawing;
using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class VehicleConfiguration:IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        //this tell of the primary key
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<SequentialGuidValueGenerator>();
        
        builder.Property(v => v.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(20);
        //IN is tied to the physical car and is never reused.
        builder.Property(v => v.Vin)
            .IsRequired()
            .HasMaxLength(17);
        
        builder.Property(v => v.Make)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Year)
            .IsRequired();
        
        builder.Property(v=> v.Colour)
            .HasMaxLength(20);
        
        builder.Property(v => v.VehicleType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.EffectiveFrom)
            .IsRequired();
        
        builder.Property(v => v.EffectiveTo);

        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.UpdatedAt);

// VIN is globally unique for a physical car.

        builder.HasIndex(v => v.Vin)
               .IsUnique()
               .HasDatabaseName("IX_Vehicles_Vin");
        
        // Registration is unique within a policy.
        builder.HasIndex(v => new { v.PolicyId, v.RegistrationNumber })
            .IsUnique()
            .HasDatabaseName("IX_Vehicles_PolicyId_RegistrationNumber");
        
        // Relationship with Policy.
        // Restrict: deleting a policy must not silently delete its vehicles.
        builder.HasOne(v => v.Policy)
            .WithMany(p => p.Vehicles)
            .HasForeignKey(v => v.PolicyId)
            //Why restrict delete from Policy to Vehicle? Because deleting a policy should not delete vehicles.
            //In insurance, policies are never hard-deleted; they are marked as expired/cancelled
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.PolicyId)
            .HasDatabaseName("IX_Vehicles_PolicyId");
    }
}