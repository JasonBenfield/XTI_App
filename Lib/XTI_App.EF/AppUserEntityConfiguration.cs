using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XTI_App.EF
{
    public sealed class AppUserEntityConfiguration : IEntityTypeConfiguration<AppUserRecord>
    {
        public void Configure(EntityTypeBuilder<AppUserRecord> builder)
        {
            builder.HasKey(u => u.ID);
            builder.Property(u => u.ID).ValueGeneratedOnAdd();
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.Property(u => u.UserName).HasMaxLength(100);
            builder.Property(u => u.Password).HasMaxLength(100);
        }
    }
}
