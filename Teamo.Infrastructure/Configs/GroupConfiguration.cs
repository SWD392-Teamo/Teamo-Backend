using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Group");
            
            builder.Property(o => o.Name).HasColumnType("nvarchar(100)");
            builder.Property(o => o.Title).HasColumnType("nvarchar(200)");
            builder.Property(o => o.Description).HasColumnType("nvarchar(1000)");
            builder.Property(o => o.ImgUrl).HasColumnType("varchar(200)");

            builder.HasOne(o => o.Semester)
                .WithMany()
                .HasForeignKey(o => o.SemesterId);

            builder.HasOne(o => o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o => o.CreatedById);

            builder.HasOne(o => o.Field)
                .WithMany()
                .HasForeignKey(o => o.FieldId);

            builder.HasOne(o => o.Subject)
                .WithMany()
                .HasForeignKey(o => o.SubjectId);

            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (GroupStatus)Enum.Parse(typeof(GroupStatus), s))
                .HasColumnType("varchar(50)");
        }
    }
}