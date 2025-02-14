
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

        public DbSet<Link> Links { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentSkill> StudentSkills { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<MajorSubject> MajorSubjects { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<SubjectField> SubjectFields { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupPosition> GroupPositions { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupPositionSkill> GroupPositionSkills { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<GroupMemberPosition> GroupMemberPositions { get; set; }
        public DbSet<Post> Posts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
