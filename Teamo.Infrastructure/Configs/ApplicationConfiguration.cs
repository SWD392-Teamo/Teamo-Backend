using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

namespace Teamo.Infrastructure.Configs
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.Property(o => o.RequestContent).HasColumnType("nvarchar(1000)");
            
            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), s))
                .HasColumnType("varchar(50)");

            builder.HasOne(o => o.Group)
                .WithMany(g => g.Applications)
                .HasForeignKey(o => o.GroupId);
            
            builder.HasOne(o => o.DestStudent)
                .WithMany()
                .HasForeignKey(o => o.DestStudentId);

            builder.HasOne(o => o.SrcStudent)
                .WithMany()
                .HasForeignKey(o => o.SrcStudentId);

            builder.HasOne(o => o.GroupPosition)
                .WithMany()
                .HasForeignKey(o => o.GroupPositionId);    
        }
    }
}