using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class GroupPositionConfiguration : IEntityTypeConfiguration<GroupPosition>
    {
        public void Configure(EntityTypeBuilder<GroupPosition> builder)
        {
            builder.Property(o => o.Name).HasColumnType("nvarchar(100)");

            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (GroupPositionStatus)Enum.Parse(typeof(GroupPositionStatus), s))
                .HasColumnType("varchar(50)");

            builder.HasOne(o => o.Group)
                .WithMany(g => g.GroupPositions)
                .HasForeignKey(o => o.GroupId);
            
            builder.HasMany(o => o.Skills)
                .WithMany()
                .UsingEntity<GroupPositionSkill>();
        }
    }
}