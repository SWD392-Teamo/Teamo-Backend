using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Configs
{
    public class SubjectFieldConfiguration : IEntityTypeConfiguration<SubjectField>
    {
        public void Configure(EntityTypeBuilder<SubjectField> builder)
        {
            builder.ToTable("SubjectField");
            
            builder.HasOne(o => o.Subject)
                .WithMany(s => s.SubjectFields)
                .HasForeignKey(o => o.SubjectId);

            builder.HasOne(o => o.Field)
                .WithMany()
                .HasForeignKey(o => o.FieldId);
        }
    }
}