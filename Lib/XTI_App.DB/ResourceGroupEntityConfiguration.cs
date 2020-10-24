using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XTI_App.Entities;

namespace XTI_App.DB
{
    public sealed class ResourceGroupEntityConfiguration : IEntityTypeConfiguration<ResourceGroupRecord>
    {
        public void Configure(EntityTypeBuilder<ResourceGroupRecord> builder)
        {
            builder.HasKey(g => g.ID);
            builder.Property(g => g.ID).ValueGeneratedOnAdd();
            builder.Property(g => g.Name).HasMaxLength(100);
            builder
                .HasIndex(g => new { g.AppID, g.Name })
                .IsUnique();
            builder
                .HasOne<AppRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(g => g.AppID);
            builder
                .HasOne<ModifierCategoryRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(g => g.ModCategoryID);
        }
    }
}
