using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class MajorSubjectConfiguration : IEntityTypeConfiguration<MajorSubject>
    {
        public void Configure(EntityTypeBuilder<MajorSubject> builder)
        {
            builder.HasOne(o => o.Major)
                .WithMany(m => m.MajorSubjects)
                .HasForeignKey(o => o.MajorId);
            
            builder.HasOne(o => o.Subject)
                .WithMany()
                .HasForeignKey(o => o.SubjectId);
        }
    }
}