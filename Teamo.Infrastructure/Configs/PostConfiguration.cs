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
            builder.ToTable("Post");

            builder.Property(o => o.Content).HasColumnType("nvarchar(1000)");

            builder.HasOne(o => o.Student)
                .WithMany()
                .HasForeignKey(o => o.StudentId)
                .OnDelete(DeleteBehavior.NoAction); 
            builder.HasOne(o => o.Group)
                .WithMany(o => o.Posts)
                .HasForeignKey(o => o.GroupId);

            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (PostStatus)Enum.Parse(typeof(PostStatus), s))
                .HasColumnType("varchar(20)");
        }
    }
}