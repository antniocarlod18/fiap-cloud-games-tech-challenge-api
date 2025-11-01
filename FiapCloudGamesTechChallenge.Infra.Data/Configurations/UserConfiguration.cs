using FiapCloudGamesTechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiapCloudGamesTechChallenge.Infra.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(u => u.Id);
            builder.Property(p => p.DateCreated).HasColumnType("DATETIME").IsRequired();
            builder.Property(p => p.DateUpdated).HasColumnType("DATETIME");

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.HashPassword)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Active).IsRequired();

            builder.Property(u => u.IsAdmin).IsRequired();

            builder.HasMany(u => u.GameLibrary)
                .WithMany(g => g.UserLibrary)
                .UsingEntity("GameUserLibrary");

            builder.HasMany(u => u.GameCart)
                .WithMany(g => g.UserCart)
                .UsingEntity("GameUserCart");

            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User);
        }
    }
}
