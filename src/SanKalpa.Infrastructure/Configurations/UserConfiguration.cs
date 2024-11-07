using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanKalpa.Domain.Users;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(key => key.Id);

        builder.Property(firstName => firstName.FirstName)
            .HasColumnName("first_name")
            .HasConversion(firstName => firstName.Value,
                           value => new FirstName(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(lastName => lastName.LastName)
            .HasColumnName("last_name")
            .HasConversion(lastName => lastName.Value,
                           value => new LastName(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(emailAddress => emailAddress.EmailAddress)
            .HasColumnName("email_address")
            .HasConversion(emailAddress => emailAddress.Value,
                           value => new EmailAddress(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(emailAddress => emailAddress.EmailAddress).IsUnique();

        builder.HasIndex(identity => identity.IdentityId).IsUnique();
    }
}
