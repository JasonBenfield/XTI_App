using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XTI_App.Entities;

namespace XTI_App.DB
{
    public sealed class AppUserRoleEntityConfiguration : IEntityTypeConfiguration<AppUserRoleRecord>
    {
        public void Configure(EntityTypeBuilder<AppUserRoleRecord> builder)
        {
            builder.HasKey(ur => ur.ID);
            builder.Property(ur => ur.ID).ValueGeneratedOnAdd();
            builder
                .HasOne<AppUserRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(ur => ur.UserID);
            builder
                .HasOne<AppRoleRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(ur => ur.RoleID);
        }
    }
}
