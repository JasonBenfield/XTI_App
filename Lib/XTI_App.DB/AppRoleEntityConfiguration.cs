using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XTI_App.Entities;

namespace XTI_App.DB
{
    public sealed class AppRoleEntityConfiguration : IEntityTypeConfiguration<AppRoleRecord>
    {
        public void Configure(EntityTypeBuilder<AppRoleRecord> builder)
        {
            builder.HasKey(r => r.ID);
            builder.Property(r => r.ID).ValueGeneratedOnAdd();
            builder.Property(r => r.Name).HasMaxLength(100);
            builder
                .HasIndex(r => new { r.AppID, r.Name })
                .IsUnique();
            builder
                .HasOne<AppRecord>()
                .WithMany()
                .HasForeignKey(r => r.AppID);
        }
    }
}
