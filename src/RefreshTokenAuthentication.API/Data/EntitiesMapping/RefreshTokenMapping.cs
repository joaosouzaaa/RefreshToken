using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RefreshTokenAuthentication.API.Constants;
using RefreshTokenAuthentication.API.Entities;

namespace RefreshTokenAuthentication.API.Data.EntitiesMapping;

public sealed class RefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
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
            .HasForeignKey<RefreshToken>(r => r.UserId)
            .IsRequired(true);
    }
}
