using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PolicyConfiguration:IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");    
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<SequentialGuidValueGenerator>();
        
        builder.Property(p=> p.PolicyNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.StartDate)
            .IsRequired();
        builder.Property(p => p.EndDate)
            .IsRequired();
        
        // decimal(18,2) is the standard for monetary amounts.
        // 18 digits total, 2 after the decimal point.
        builder.Property(p=>p.Premium)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Store enum as int. Explicit conversion prevents accidental
        // string storage which is harder to query and index.
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(p=> p.CreatedAt).IsRequired();
        builder.Property(p=> p.UpdatedAt).IsRequired();
        
        // Foreign key to PolicyHolder.
        // Restrict: we do NOT cascade-delete policies when a policyholder is deleted.
        // In insurance we never hard-delete customers; if that ever happens, the
        // operation must be a deliberate manual step, not a cascade.
        builder.HasOne(p => p.PolicyHolder)
            .WithMany(ph => ph.Policies)
            .HasForeignKey(p=>p.PolicyHolderId)
            //n a regulated system, deleting a policyholder should not be possible while policies exist.
            //Restrict enforces this at the database level, throwing an error if someone tries.
            .OnDelete(DeleteBehavior.Restrict);
        
            // Index on FK — required for join performance.
        builder.HasIndex(p=>p.PolicyHolderId)
            .HasDatabaseName("IX_PolicyHolderId");
        
        // Query performance index for common filters.
        builder.HasIndex(p => new{p.Status,p.EndDate})
            .HasDatabaseName("IX_Policies_Status_EndDate");
        
    }
}