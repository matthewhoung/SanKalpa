using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanKalpa.Domain.Users;

namespace SanKalpa.Infrastructure.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(key => key.Id);

        builder.Property(token => token.Token)
            .HasColumnName("token")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(expiryDate => expiryDate.ExpiryDate)
            .HasColumnName("expiry_date")
            .IsRequired();

        builder.Property(isRevoked => isRevoked.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();

        builder.Property(userId => userId.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(createdAt => createdAt.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(token => token.Token).IsUnique();

        builder.HasIndex(userId => userId.UserId).IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
