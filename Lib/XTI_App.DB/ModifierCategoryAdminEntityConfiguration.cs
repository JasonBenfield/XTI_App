using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XTI_App.Entities;

namespace XTI_App.DB
{
    public sealed class ModifierCategoryAdminEntityConfiguration : IEntityTypeConfiguration<ModifierCategoryAdminRecord>
    {
        public void Configure(EntityTypeBuilder<ModifierCategoryAdminRecord> builder)
        {
            builder.HasKey(c => c.ID);
            builder.Property(c => c.ID).ValueGeneratedOnAdd();
            builder.HasIndex(c => new { c.ModCategoryID, c.UserID }).IsUnique();
            builder
                .HasOne<ModifierCategoryRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(c => c.ModCategoryID);
            builder
                .HasOne<AppUserRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(c => c.UserID);
        }
    }
}
