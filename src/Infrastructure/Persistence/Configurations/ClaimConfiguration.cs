using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Persistence.Configurations;



public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd()
               .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.Property(c => c.ClaimNumber).IsRequired().HasMaxLength(50);
        builder.Property(c => c.IncidentDate).IsRequired();
        builder.Property(c => c.FiledDate).IsRequired();
        builder.Property(c => c.Description).IsRequired().HasMaxLength(2000);

        builder.Property(c => c.ClaimAmount)
               .IsRequired()
               .HasColumnType("numeric(18,2)");

        builder.Property(c => c.ApprovedAmount)
               .HasColumnType("numeric(18,2)");

        builder.Property(c => c.Status)
               .IsRequired()
               .HasConversion<int>();

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);

        builder.HasIndex(c => c.ClaimNumber)
               .IsUnique()
               .HasDatabaseName("IX_Claims_ClaimNumber");

        // Relationship with Policy (required).
        builder.HasOne(c => c.Policy)
               .WithMany(p => p.Claims)
               .HasForeignKey(c => c.PolicyId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Vehicle (optional).
        builder.HasOne(c => c.Vehicle)
               .WithMany(v => v.Claims)
               .HasForeignKey(c => c.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Driver (optional).
        builder.HasOne(c => c.Driver)
               .WithMany(d => d.Claims)
               .HasForeignKey(c => c.DriverId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PolicyId).HasDatabaseName("IX_Claims_PolicyId");
        builder.HasIndex(c => c.VehicleId).HasDatabaseName("IX_Claims_VehicleId");
        builder.HasIndex(c => c.DriverId).HasDatabaseName("IX_Claims_DriverId");

        // Common query: "open claims" or "claims by status".
        builder.HasIndex(c => c.Status).HasDatabaseName("IX_Claims_Status");
    }
}