using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(o => o.Content).HasColumnType("nvarchar(1000)");

            builder.HasOne(o => o.GroupMember)
                .WithMany()
                .HasForeignKey(o => o.GroupMemberId);

            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (PostStatus)Enum.Parse(typeof(PostStatus), s))
                .HasColumnType("varchar(20)");
            
            builder.Property(o => o.Privacy)
                .HasConversion(
                    p => p.ToString(),
                    p => (PostPrivacy)Enum.Parse(typeof(PostPrivacy), p))
                .HasColumnType("varchar(20)");
        }
    }
}