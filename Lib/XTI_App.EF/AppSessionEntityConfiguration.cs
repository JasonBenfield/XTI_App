using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XTI_App.EF
{
    public sealed class AppSessionEntityConfiguration : IEntityTypeConfiguration<AppSessionRecord>
    {
        public void Configure(EntityTypeBuilder<AppSessionRecord> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.ID).ValueGeneratedOnAdd();
            builder.Property(s => s.SessionKey).HasMaxLength(100);
            builder.HasIndex(s => s.SessionKey).IsUnique();
            builder.Property(s => s.RequesterKey).HasMaxLength(100);
            builder.Property(s => s.RemoteAddress).HasMaxLength(20);
            builder.Property(s => s.UserAgent).HasMaxLength(1000);
            builder
                .HasOne<AppUserRecord>()
                .WithMany()
                .HasForeignKey(s => s.UserID);
        }
    }
}
