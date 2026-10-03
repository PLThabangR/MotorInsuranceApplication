using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistence.Configurations;


public class TelematicsConfiguration : IEntityTypeConfiguration<Telematics>
{
    public void Configure(EntityTypeBuilder<Telematics> builder)
    {
        builder.ToTable("Telematics");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
               .ValueGeneratedOnAdd()
               .HasValueGenerator<SequentialGuidValueGenerator>();

        builder.Property(t => t.RecordedAt).IsRequired();

        builder.Property(t => t.SpeedKmh)
               .IsRequired()
               .HasColumnType("numeric(6,2)"); // up to 9999.99 km/h

        builder.Property(t => t.OdometerKm)
               .HasColumnType("numeric(12,2)"); // up to ~10 billion km

        builder.Property(t => t.Latitude)
               .HasColumnType("numeric(9,6)"); // -90 to 90 with 6 decimals (~0.1m precision)

        builder.Property(t => t.Longitude)
               .HasColumnType("numeric(9,6)"); // -180 to 180 with 6 decimals

        builder.Property(t => t.HarshAcceleration).IsRequired();
        builder.Property(t => t.HarshBraking).IsRequired();
        builder.Property(t => t.HarshCornering).IsRequired();

        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt);

        // The single most important index for this table:
        // queries will almost always be "readings for vehicle X between time A and B".
        // A composite index on (VehicleId, RecordedAt) supports this efficiently.
        builder.HasIndex(t => new { t.VehicleId, t.RecordedAt })
               .HasDatabaseName("IX_Telematics_VehicleId_RecordedAt");

        builder.HasOne(t => t.Vehicle)
               .WithMany(v => v.Telematics)
               .HasForeignKey(t => t.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.VehicleId).HasDatabaseName("IX_Telematics_VehicleId");
    }
}