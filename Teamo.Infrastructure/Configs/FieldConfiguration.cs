using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.ToTable("Field");
            builder.Property(o => o.Name).HasColumnType("nvarchar(100)");
            builder.Property(o => o.Description).HasColumnType("nvarchar(1000)");
        }
    }
}