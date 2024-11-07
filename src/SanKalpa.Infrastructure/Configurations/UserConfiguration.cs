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

        builder.Property(userName => userName.UserName)
            .HasColumnName("user_name")
            .HasConversion(firstName => firstName.Value,
                           value => new UserName(value))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(emailAddress => emailAddress.EmailAddress)
            .HasColumnName("email_address")
            .HasConversion(emailAddress => emailAddress.Value,
                           value => new EmailAddress(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(userPassword => userPassword.UserPassword)
            .HasColumnName("user_password")
            .HasConversion(userPassword => userPassword.Value,
                           value => new Password(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(emailAddress => emailAddress.EmailAddress).IsUnique();

        builder.Property(createdAt => createdAt.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(updatedAt => updatedAt.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
