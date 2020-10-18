using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XTI_App.EF
{
    public sealed class AppVersionEntityConfiguration : IEntityTypeConfiguration<AppVersionRecord>
    {
        public void Configure(EntityTypeBuilder<AppVersionRecord> builder)
        {
            builder.HasKey(v => v.ID);
            builder.Property(v => v.ID).ValueGeneratedOnAdd();
            builder.Property(v => v.VersionKey).HasMaxLength(50);
            builder.HasIndex(v => v.VersionKey).IsUnique();
            builder
                .HasOne<AppRecord>()
                .WithMany()
                .HasForeignKey(v => v.AppID);
        }
    }
}
