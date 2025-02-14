using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class GroupMemberPositionConfiguration : IEntityTypeConfiguration<GroupMemberPosition>
    {
        public void Configure(EntityTypeBuilder<GroupMemberPosition> builder)
        {
            builder.ToTable("GroupMemberPosition");

            builder.HasOne(o => o.GroupMember)
                .WithMany(m => m.GroupMemberPositions)
                .HasForeignKey(o => o.GroupMemberId);

            builder.HasOne(o => o.GroupPosition)
                .WithMany()
                .HasForeignKey(o => o.GroupPositionId);
        }
    }
}