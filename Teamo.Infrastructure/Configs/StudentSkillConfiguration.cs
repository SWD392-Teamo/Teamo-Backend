using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
    {
        public void Configure(EntityTypeBuilder<StudentSkill> builder)
        {
            builder.Property(o => o.Level)
                .HasConversion(
                    s => s.ToString(),
                    s => (StudentSkillLevel)Enum.Parse(typeof(StudentSkillLevel), s))
                .HasColumnType("varchar(50)");

            builder.HasOne(o => o.Student)
                .WithMany(u => u.StudentSkills)
                .HasForeignKey(o => o.StudentId);

            builder.HasOne(o => o.Skill)
                .WithMany()
                .HasForeignKey(o => o.SkillId);
        }
    }
}