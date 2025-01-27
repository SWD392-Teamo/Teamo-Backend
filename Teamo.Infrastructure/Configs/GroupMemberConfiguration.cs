using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.HasOne(o => o.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(o => o.GroupId);
            
            builder.HasOne(o => o.Student)
                .WithMany()
                .HasForeignKey(o => o.StudentId);

            builder.HasOne(o => o.GroupPosition)
                .WithMany()
                .HasForeignKey(o => o.GroupPositionId);

            builder.Property(o => o.Role)
                .HasConversion(
                    s => s.ToString(),
                    s => (GroupMemberRole)Enum.Parse(typeof(GroupMemberRole), s))
                .HasColumnType("varchar(50)");                
        }
    }
}