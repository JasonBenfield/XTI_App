using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XTI_App.Entities;

namespace XTI_App.DB
{
    public sealed class AppUserModifierEntityConfiguration : IEntityTypeConfiguration<AppUserModifierRecord>
    {
        public void Configure(EntityTypeBuilder<AppUserModifierRecord> builder)
        {
            builder.HasKey(um => um.ID);
            builder.Property(um => um.ID).ValueGeneratedOnAdd();
            builder
                .HasOne<AppUserRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(um => um.UserID);
            builder
                .HasOne<ModifierRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(um => um.ModifierID);
        }
    }
}
