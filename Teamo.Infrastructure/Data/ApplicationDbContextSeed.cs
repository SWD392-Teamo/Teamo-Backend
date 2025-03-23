using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Teamo.Infrastructure.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context, 
            UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());

            // Seed roles
            if (!roleManager.Roles.Any())
            {
                var data = await File.ReadAllTextAsync(path + @"/Data/SeedData/roles.json");

                var roles = JsonSerializer.Deserialize<List<IdentityRole<int>>>(data, options);

                if (roles == null) return;

                foreach (var item in roles)
                {
                    await roleManager.CreateAsync(item);
                }
            }

            // Seed majors
            if (!context.Majors.Any())
            {
                var maData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/majors.json");

                var majors = JsonSerializer.Deserialize<List<Major>>(maData, options);

                if (majors == null) return;

                context.Majors.AddRange(majors);

                await context.SaveChangesAsync();
            }

            // Seed users
            if (!userManager.Users.Any())
            {
                var userList = new List<(User user, string password, string role)>
                {
                    (new User
                    {
                        FirstName = "My",
                        LastName = "Lâm",
                        Code = "SE179997",
                        Description = "I have experienced in ASPNET development",
                        Email = "my@test.com",
                        UserName = "my@test.com",
                        Gender = Gender.Female,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/mechat-926e4.appspot.com/o/teamo%2Fimages%2Fplaceholders%2Ffemale-user.jpg?alt=media",
                        PhoneNumber = "0034988493",
                        MajorID = 1
                    },"my123456","Student"),
                    (new User
                    {
                        FirstName = "Khánh",
                        LastName = "Ngô",
                        Code = "SE179998",
                        Description = "I have experienced in React development",
                        Email = "khanhcnp@test.com",
                        UserName = "khanhcnp@test.com",
                        Gender = Gender.Female,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/mechat-926e4.appspot.com/o/teamo%2Fimages%2Fplaceholders%2Ffemale-user.jpg?alt=media",
                        PhoneNumber = "0034988494",
                        MajorID = 2
                    },"khanh123456","Student"),
                    (new User
                    {
                        UserName = "admin@test.com",
                        Email = "admin@test.com"
                    }, "admin123456", "Admin"),
                    (new User
                    {
                        FirstName = "Khánh",
                        LastName = "Lê",
                        Code = "SE179999",
                        Description = "I am experienced with web app development. Languages: Java, C#, ReactJS, Angular",
                        Email = "khanhlq@test.com",
                        UserName = "khanhlq@test.com",
                        Gender = Gender.Male,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/mechat-926e4.appspot.com/o/teamo%2Fimages%2Fplaceholders%2Fmale-user.jpg?alt=media",
                        PhoneNumber = "0034988495",
                        MajorID = 1
                    },"khanhle123456","Student"),
                    (new User
                    {
                        FirstName = "Thành",
                        LastName = "Chu",
                        Code = "SS180001",
                        Description = "Marketing student with exceptional PR and event organization skills!",
                        Email = "thanhcq@test.com",
                        UserName = "thanhcq@test.com",
                        Gender = Gender.Male,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/mechat-926e4.appspot.com/o/teamo%2Fimages%2Fplaceholders%2Fmale-user.jpg?alt=media",
                        PhoneNumber = "0034988496",
                        MajorID = 7
                    },"thanh123456","Student"),
                    (new User
                    {
                        FirstName = "Hậu",
                        LastName = "Đỗ",
                        Code = "SE180002",
                        Description = "Specialize in event managements and video production.",
                        Email = "haudtn@test.com",
                        UserName = "haudtn@test.com",
                        Gender = Gender.Female,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/mechat-926e4.appspot.com/o/teamo%2Fimages%2Fplaceholders%2Ffemale-user.jpg?alt=media",
                        PhoneNumber = "0034988497",
                        MajorID = 6
                    },"hau123456","Student"),
                    (new User
                    {
                        FirstName = "Ân",
                        LastName = "Thiên",
                        Code = "SE180003",
                        Description = "Similar with web app and mobile app dev.",
                        Email = "ant@test.com",
                        UserName = "ant@test.com",
                        Gender = Gender.Male,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/mechat-926e4.appspot.com/o/teamo%2Fimages%2Fplaceholders%2Fmale-user.jpg?alt=media",
                        PhoneNumber = "0034988498",
                        MajorID = 1
                    },"an123456","Student")
                };

                foreach (var account in userList)
                {
                    await userManager.CreateAsync(account.user, account.password);
                    await userManager.AddToRoleAsync(account.user, account.role);
                }
            }

            // Seed subjects
            if (!context.Subjects.Any())
            {
                var subData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/subjects.json");

                var subjects = JsonSerializer.Deserialize<List<Subject>>(subData, options);

                if (subjects == null) return;

                context.Subjects.AddRange(subjects);

                await context.SaveChangesAsync();
            }

            // Seed semesters
            if (!context.Semesters.Any())
            {
                var semData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/semesters.json");

                var semesters = JsonSerializer.Deserialize<List<Semester>>(semData, options);

                if (semesters == null) return;

                context.Semesters.AddRange(semesters);

                await context.SaveChangesAsync();
            }

            // Seed fields
            if (!context.Fields.Any())
            {
                var fieldData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/fields.json");

                var fields = JsonSerializer.Deserialize<List<Field>>(fieldData);

                if (fields == null) return;

                context.Fields.AddRange(fields);

                await context.SaveChangesAsync();
            }

            // Seed skills
            if (!context.Skills.Any())
            {
                var skillData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/skills.json");

                var skills = JsonSerializer.Deserialize<List<Skill>>(skillData);

                if (skills == null) return;

                context.Skills.AddRange(skills);

                await context.SaveChangesAsync();
            }

            // Seed major subjects
            if (!context.MajorSubjects.Any())
            {
                var maSubData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/majorSubjects.json");

                var majorSubjects = JsonSerializer.Deserialize<List<MajorSubject>>(maSubData);


                if (majorSubjects == null) return;

                context.MajorSubjects.AddRange(majorSubjects);

                await context.SaveChangesAsync();
            }

            // Seed subject fields
            if (!context.SubjectFields.Any())
            {
                var subFieldData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/subjectFields.json");

                var subjectFields = JsonSerializer.Deserialize<List<SubjectField>>(subFieldData);


                if (subjectFields == null) return;

                context.SubjectFields.AddRange(subjectFields);

                await context.SaveChangesAsync();
            }

            // Seed Group 
            if (!context.Groups.Any())
            {
                var groupData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groups.json");

                var groups = JsonSerializer.Deserialize<List<Group>>(groupData);


                if (groups == null) return;

                context.Groups.AddRange(groups);

                await context.SaveChangesAsync();
            }
            // Seed Group Position
            if (!context.GroupPositions.Any())
            {
                var groupPositionData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupPositions.json");

                var groupPositions = JsonSerializer.Deserialize<List<GroupPosition>>(groupPositionData);


                if (groupPositions == null) return;

                context.GroupPositions.AddRange(groupPositions);

                await context.SaveChangesAsync();
            }

            // Seed Group Member
            if (!context.GroupMembers.Any())
            {
                var groupMemberData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupMembers.json");

                var groupMembers = JsonSerializer.Deserialize<List<GroupMember>>(groupMemberData);


                if (groupMembers == null) return;

                context.GroupMembers.AddRange(groupMembers);

                await context.SaveChangesAsync();
            }

            //Seed GroupMemberPositions
            if (!context.GroupMemberPositions.Any())
            {
                var groupMemberPositionsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupMemberPositions.json");

                var groupMemberPositions = JsonSerializer.Deserialize<List<GroupMemberPosition>>(groupMemberPositionsData);


                if (groupMemberPositions == null) return;

                context.GroupMemberPositions.AddRange(groupMemberPositions);

                await context.SaveChangesAsync();
            }

            //Seed Applications
            if (!context.Applications.Any())
            {
                var applicationData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/applications.json");

                var applications = JsonSerializer.Deserialize<List<Application>>(applicationData, options);


                if (applications == null) return;

                context.Applications.AddRange(applications);

                await context.SaveChangesAsync();
            }

            //Seed Student Skills
            if (!context.StudentSkills.Any())
            {
                var studentSkillsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/studentSkills.json");

                var studentSkills = JsonSerializer.Deserialize<List<StudentSkill>>(studentSkillsData, options);


                if (studentSkills == null) return;

                context.StudentSkills.AddRange(studentSkills);

                await context.SaveChangesAsync();
            }

            //Seed Students
            if (!context.Students.Any())
            {
                var studentsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/students.json");

                var students = JsonSerializer.Deserialize<List<Student>>(studentsData, options);

                if (students == null) return;

                context.Students.AddRange(students);

                await context.SaveChangesAsync();
            }

            // seed post
            if (!context.Posts.Any())
            {
                var postsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/posts.json");

                var posts = JsonSerializer.Deserialize<List<Post>>(postsData, options);

                if (posts == null) return;

                context.Posts.AddRange(posts);

                await context.SaveChangesAsync();
            }

            // seed link
            if (!context.Links.Any())
            {
                var linksData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/links.json");

                var links = JsonSerializer.Deserialize<List<Link>>(linksData, options);

                if (links == null) return;

                context.Links.AddRange(links);

                await context.SaveChangesAsync();
            }

            // Seed position skills
            if (!context.GroupPositionSkills.Any())
            {
                var groupPositionSkillsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupPositionSkills.json");

                var groupPositionSkills = JsonSerializer.Deserialize<List<GroupPositionSkill>>(groupPositionSkillsData, options);

                if (groupPositionSkills == null) return;

                context.GroupPositionSkills.AddRange(groupPositionSkills);

                await context.SaveChangesAsync();
            }
        }
    }
}
