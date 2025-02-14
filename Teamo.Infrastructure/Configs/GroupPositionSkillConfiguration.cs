using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class GroupPositionSkillConfiguration : IEntityTypeConfiguration<GroupPositionSkill>
    {
        public void Configure(EntityTypeBuilder<GroupPositionSkill> builder)
        {
            builder.ToTable("GroupPositionSkill");
            builder.HasOne(o => o.GroupPosition)
                .WithMany(g => g.GroupPositionSkills)
                .HasForeignKey(o => o.GroupPositionId);

            builder.HasOne(o => o.Skill)
                .WithMany()
                .HasForeignKey(o => o.SkillId);
        }
    }
}