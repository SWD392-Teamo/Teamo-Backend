using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.Property(o => o.Name).HasColumnType("nvarchar(100)");
            builder.Property(o => o.Type).HasColumnType("nvarchar(100)");
        }
    }
}