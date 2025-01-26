using Microsoft.EntityFrameworkCore;
using Teamo.Core.Entities;

namespace Teamo.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     modelBuilder.Entity<User>().ToTable(nameof(Users), t => t.ExcludeFromMigrations());
        //     modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // }
    }
}
