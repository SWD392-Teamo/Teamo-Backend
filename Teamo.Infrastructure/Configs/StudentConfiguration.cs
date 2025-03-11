using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Student");

            builder.HasIndex(u => u.Phone).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Code).IsUnique();
            builder.HasIndex(o => o.Major).IsUnique();

            builder.Property(u => u.UserName).HasColumnType("varchar(50)");
            builder.Property(u => u.Phone).HasColumnType("varchar(20)");
            builder.Property(u => u.Email).HasColumnType("varchar(100)");
            builder.Property(u => u.Code).HasColumnType("varchar(20)");
            builder.Property(u => u.FirstName) .HasColumnType("varchar(100)");
            builder.Property(u => u.LastName).HasColumnType("varchar(100)");
            builder.Property(u => u.ImgUrl).HasColumnType("varchar(200)");
            builder.Property(u => u.Major).HasColumnType("varchar(20)");

            builder.Property(c => c.Gender)
                .HasConversion(
                    s => s.ToString(),
                    s => (Gender)Enum.Parse(typeof(Gender), s))
                .HasColumnType("varchar(20)");
        }
    }
}