using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class LinkConfiguration : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.ToTable("Link");
            builder.Property(l => l.Name).HasColumnType("nvarchar(100)");
            builder.Property(l => l.Url).HasColumnType("varchar(200)");

            builder.HasOne(l => l.Student)
                .WithMany(u => u.Links)
                .HasForeignKey(l => l.StudentId);
        }
    }
}