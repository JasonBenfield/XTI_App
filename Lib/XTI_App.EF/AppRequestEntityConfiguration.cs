using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XTI_App.EF
{
    public sealed class AppRequestEntityConfiguration : IEntityTypeConfiguration<AppRequestRecord>
    {
        public void Configure(EntityTypeBuilder<AppRequestRecord> builder)
        {
            builder.HasKey(r => r.ID);
            builder.Property(r => r.ID).ValueGeneratedOnAdd();
            builder.Property(r => r.Path).HasMaxLength(100);
            builder.Property(r => r.RequestKey).HasMaxLength(100);
            builder.HasIndex(s => s.RequestKey).IsUnique();
            builder
                .HasOne<AppSessionRecord>()
                .WithMany()
                .HasForeignKey(r => r.SessionID);
            builder
                .HasOne<AppVersionRecord>()
                .WithMany()
                .HasForeignKey(r => r.VersionID);
        }
    }
}
