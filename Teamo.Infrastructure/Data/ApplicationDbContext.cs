
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities.Identity;

namespace Teamo.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "User");

                entity.Property(u => u.UserName)
                      .HasColumnType("varchar(50)")
                      .IsRequired();

                entity.Property(u => u.Phone)
                      .HasColumnType("varchar(20)")
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(u => u.Gender)
                      .HasColumnType("varchar(20)")
                      .IsRequired();

                entity.Property(u => u.FirstName)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(u => u.LastName)
                      .HasColumnType("varchar(100)")
                      .IsRequired();

                entity.Property(u => u.ImgUrl)
                      .HasColumnType("varchar(500)");
            });

            builder.Entity<IdentityRole<int>>(entity =>
            {
                entity.ToTable(name: "Role");
                entity.Property(r => r.Name).HasColumnType("varchar(50)").IsRequired();
            });
        }
    }
}
