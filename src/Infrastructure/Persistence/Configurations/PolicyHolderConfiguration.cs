using Domain.Entities;
using Infrastructure.Persistence.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

//We are enforcing the constraints to our Policyholder table 
public class PolicyHolderConfiguration: IEntityTypeConfiguration<PolicyHolder>
{
    public void Configure(EntityTypeBuilder<PolicyHolder> builder)
    {   //specifically applied to Policy holder table
        //EF core will pluralize the the DbSet name by defualt
        //Being explicit avoid surprises
        builder.ToTable("PolicyHolders");
        builder.HasKey(p => p.Id);
        //We sequential Guid value generator for index locality
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<SequentialGuidValueGenerator>();
        
        builder.Property(ph => ph.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ph => ph.NationalId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ph => ph.Email)
            .IsRequired()
            .HasMaxLength(254); // RFC 5321 maximum email length

        builder.Property(ph => ph.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);
        
        // Unique constraints.
        builder.HasIndex(ph => ph.NationalId)
            .IsUnique()
            .HasDatabaseName("IX_PolicyHolders_NationalId");

        builder.HasIndex(ph => ph.Email)
            .IsUnique()
            .HasDatabaseName("IX_PolicyHolders_Email");
        
        
    }
}