using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XTI_App.Entities;

namespace XTI_App.DB
{
    public sealed class ResourceEntityConfiguration : IEntityTypeConfiguration<ResourceRecord>
    {
        public void Configure(EntityTypeBuilder<ResourceRecord> builder)
        {
            builder.HasKey(r => r.ID);
            builder.Property(r => r.ID).ValueGeneratedOnAdd();
            builder.Property(r => r.Name).HasMaxLength(100);
            builder
                .HasIndex(r => new { r.GroupID, r.Name })
                .IsUnique();
            builder
                .HasOne<ResourceGroupRecord>()
                .WithMany()
                .HasForeignKey(r => r.GroupID);
        }
    }
}
