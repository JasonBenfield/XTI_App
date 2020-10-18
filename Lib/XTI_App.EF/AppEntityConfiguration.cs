using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XTI_App.EF
{
    public sealed class AppEntityConfiguration : IEntityTypeConfiguration<AppRecord>
    {
        public void Configure(EntityTypeBuilder<AppRecord> builder)
        {
            builder.HasKey(a => a.ID);
            builder.Property(a => a.ID).ValueGeneratedOnAdd();
            builder.Property(a => a.Key).HasMaxLength(50);
            builder.HasIndex(a => a.Key).IsUnique();
            builder.Property(a => a.Title)
                .HasMaxLength(100)
                .HasDefaultValue("");
        }
    }
}
