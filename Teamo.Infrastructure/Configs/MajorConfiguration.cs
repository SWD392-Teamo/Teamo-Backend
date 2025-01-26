using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class MajorConfiguration : IEntityTypeConfiguration<Major>
    {
        public void Configure(EntityTypeBuilder<Major> builder)
        {
            builder.Property(u => u.Code)
                      .HasColumnType("varchar(20)")
                      .IsRequired();
            builder.Property(u => u.Name)
                      .HasColumnType("varchar(100)")
                      .IsRequired();
        }
    }
}
