using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subject");

            builder.HasIndex(o => o.Code).IsUnique();

            builder.Property(o => o.Code).HasColumnType("varchar(20)");   
            builder.Property(o => o.Name).HasColumnType("varchar(100)");
            builder.Property(o => o.Description).HasColumnType("varchar(1000)");
            builder.Property(o => o.CreatedDate).HasColumnType("date");
            builder.Property(o => o.ImgUrl).HasColumnType("varchar(200)");
            builder.HasMany(o => o.Fields).WithMany().UsingEntity<SubjectField>();
            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (SubjectStatus)Enum.Parse(typeof(SubjectStatus), s))
                .HasColumnType("varchar(50)");
        }
    }
}