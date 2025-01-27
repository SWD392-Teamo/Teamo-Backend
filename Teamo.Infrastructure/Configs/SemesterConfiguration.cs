using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
    {
        public void Configure(EntityTypeBuilder<Semester> builder)
        {
            builder.Property(o => o.Name).HasColumnType("nvarchar(100)");
            builder.Property(o => o.Code).HasColumnType("varchar(20)");
            builder.Property(o => o.StartDate).HasColumnType("date");
            builder.Property(o => o.EndDate).HasColumnType("date");
        }
    }
}