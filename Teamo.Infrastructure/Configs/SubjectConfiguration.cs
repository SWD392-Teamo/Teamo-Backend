using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subject");
            builder.Property(o => o.Code).HasColumnType("varchar(20)");   
            builder.Property(o => o.Name).HasColumnType("nvarchar(100)");
            builder.Property(o => o.Description).HasColumnType("nvarchar(1000)");
            builder.Property(o => o.CreatedDate).HasColumnType("date");
            builder.HasMany(o => o.Fields).WithMany().UsingEntity<SubjectField>();
        }
    }
}