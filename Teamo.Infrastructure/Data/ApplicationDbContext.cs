
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Link> Link { get; set; }
        public DbSet<Major> Major { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentSkill> StudentSkill { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<MajorSubject> MajorSubject { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<SubjectField> SubjectField { get; set; }
        public DbSet<Semester> Semester { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupPosition> GroupPosition { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<GroupPositionSkill> GroupPositionSkill { get; set; }
        public DbSet<Application> Application { get; set; }
        public DbSet<GroupMemberPosition> GroupMemberPosition { get; set; }
        public DbSet<Post> Post { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
