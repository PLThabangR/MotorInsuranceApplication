using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DriverConfiguration: IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Drivers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.Property(d => d.FullName).IsRequired().HasMaxLength(200);
        builder.Property(d => d.LicenceNumber).IsRequired().HasMaxLength(50);
        builder.Property(d => d.DateOfBirth).IsRequired();
        builder.Property(d => d.YearsOfExperience).IsRequired();
        builder.Property(d => d.IsPrimaryDriver).IsRequired();
        builder.Property(d => d.EffectiveFrom).IsRequired();
        builder.Property(d => d.EffectiveTo);

        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.UpdatedAt);
        
            // Licence number is unique within a policy.
            builder.HasIndex(d => new { d.PolicyId, d.LicenceNumber })
            .IsUnique()
            .HasDatabaseName("IX_Drivers_PolicyId_LicenceNumber");
            
            
            //Each vehicle has one policy with many drivers
            builder.HasOne(d => d.Policy)
                .WithMany(p => p.Drivers)
                .HasForeignKey(d => d.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            //for fster query index PolicyId
            builder.HasIndex(d => d.PolicyId)
                .HasDatabaseName("IX_Drivers_PolicyId");
            
    }
}