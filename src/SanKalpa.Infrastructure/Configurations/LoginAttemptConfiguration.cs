using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanKalpa.Domain.Services;

namespace SanKalpa.Infrastructure.Configurations;

internal sealed class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.ToTable("login_attempts");

        builder.HasKey(key => key.Id);

        builder.Property(email => email.EmailAddress)
            .HasColumnName("email_address")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(success => success.IsSuccessful)
            .HasColumnName("is_success")
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(attemptat => attemptat.AttemptedAt)
            .HasColumnName("attempt_at")
            .IsRequired();

        builder.Property(ip => ip.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(255);
    }
}
