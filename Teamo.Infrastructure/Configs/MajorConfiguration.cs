using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class MajorConfiguration : IEntityTypeConfiguration<Major>
    {
        public void Configure(EntityTypeBuilder<Major> builder)
        {
            builder.ToTable("Major");
            builder.Property(m => m.Code).HasColumnType("varchar(20)");
            builder.Property(m => m.Name).HasColumnType("varchar(100)");
            builder.Property(m => m.CreatedDate).HasColumnType("date");

            builder.HasMany(m => m.Subjects)
                .WithMany()
                .UsingEntity<MajorSubject>();
        }
    }
}
