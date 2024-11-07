using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanKalpa.Domain.Users;

namespace SanKalpa.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(key => key.Id);

        builder.OwnsOne(userName => userName.UserName, userName =>
        {
            userName.Property(name => name.Value)
            .HasColumnName("user_name")
            .HasMaxLength(50)
            .IsRequired();
        });

        builder.OwnsOne(emailAddress => emailAddress.EmailAddress, emailAddress =>
        {
            emailAddress.Property(email => email.Value)
            .HasColumnName("email_address")
            .HasMaxLength(50)
            .IsRequired();

            emailAddress.HasIndex(email => email.Value).IsUnique();
        });

        builder.OwnsOne(password => password.Password, password =>
        {
            password.Property(password => password.Value)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();
        });

        builder.Property(createdAt => createdAt.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(updatedAt => updatedAt.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}

