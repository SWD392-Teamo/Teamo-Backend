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
            builder.ToTable("GroupMember");

            builder.HasOne(o => o.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(o => o.GroupId)
                .OnDelete(DeleteBehavior.Restrict); 
            
            builder.HasOne(o => o.Student)
                .WithMany()
                .HasForeignKey(o => o.StudentId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(o => o.Role)
                .HasConversion(
                    s => s.ToString(),
                    s => (GroupMemberRole)Enum.Parse(typeof(GroupMemberRole), s))
                .HasColumnType("varchar(50)");

            builder.HasMany(o => o.GroupPositions)
                .WithMany()
                .UsingEntity<GroupMemberPosition>();              
        }
    }
}