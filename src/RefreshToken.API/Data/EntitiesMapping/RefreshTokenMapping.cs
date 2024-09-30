using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefreshToken.API.Constants;
using RefreshToken.API.Entities;

namespace RefreshToken.API.Data.EntitiesMapping;

public sealed class RefreshTokenMapping : IEntityTypeConfiguration<ApplicationRefreshToken>
{
    public void Configure(EntityTypeBuilder<ApplicationRefreshToken> builder)
    {
        builder.ToTable(TableNamesConstants.RefreshTokenTableName);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired(true);

        builder.Property(r => r.Value)
            .IsRequired(true)
            .HasColumnType("varchar(100)");

        builder.Property(r => r.ExpiryDate)
            .IsRequired(true);

        builder.HasOne(r => r.User)
            .WithOne(u => u.RefreshToken)
            .HasForeignKey<ApplicationRefreshToken>(r => r.UserId)
            .IsRequired(true);
    }
}
