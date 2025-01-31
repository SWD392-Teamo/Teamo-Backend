﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities.Identity;

namespace Teamo.Infrastructure.Configs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.UserName)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

            builder.Property(u => u.Phone)
                      .HasColumnType("varchar(20)")
                      .IsRequired();

            builder.Property(u => u.Email)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

            builder.Property(u => u.Code)
                      .HasColumnType("varchar(20)")
                      .IsRequired();

            builder.Property(u => u.Gender)
                      .HasColumnType("varchar(20)")
                      .IsRequired();

            builder.Property(u => u.FirstName)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

            builder.Property(u => u.LastName)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

            builder.Property(u => u.ImgUrl)
                      .HasColumnType("varchar(500)");

            builder.Property(u => u.Description)
                      .HasColumnType("varchar(1000)");

            builder.Property(c => c.Status)
                .HasConversion(
                s => s.ToString(),
                s => (UserStatus)Enum.Parse(typeof(UserStatus), s))
                .HasColumnType("varchar(50)");

            builder.Property(c => c.Gender)
                .HasConversion(
                s => s.ToString(),
                s => (Gender)Enum.Parse(typeof(Gender), s))
                .HasColumnType("varchar(50)");

            builder.HasOne(u => u.Major)
                      .WithMany()
                      .HasForeignKey(r => r.MajorID)
                      .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
