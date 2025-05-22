using ContactSystem.Dal.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ContactSystem.Dal.EntityConfigurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.LastName)
               .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
               .HasMaxLength(50);

        builder.Property(c => c.Email)
               .HasMaxLength(100);

        builder.Property(c => c.Address)
               .HasMaxLength(200);

        builder.Property(c => c.CreatedAt)
               .HasDefaultValueSql("GETDATE()");
    }
}