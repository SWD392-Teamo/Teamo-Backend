using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Entities;
using Teamo.Core.Enums;

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
            if (!context.Major.Any())
            {
                var maData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/majors.json");

                var majors = JsonSerializer.Deserialize<List<Major>>(maData);

                if (majors == null) return;

                context.Major.AddRange(majors);

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
                        Code = "SE183448",
                        Description = "I have experienced in ASPNET development",
                        Email = "my@test.com",
                        UserName = "my@test.com",
                        Gender = Gender.Female,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/gemify-d7e93.appspot.com/o/images%2Fusers%2Ffemale-user.png",
                        PhoneNumber = "0034988493",
                        MajorID = 1
                    },"my123456","Student"),
                    (new User
                    {
                        FirstName = "Khánh",
                        LastName = "Ngô",
                        Code = "SE181509",
                        Description = "I have experienced in React development",
                        Email = "khanhcnp@test.com",
                        UserName = "khanhcnp@test.com",
                        Gender = Gender.Female,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/gemify-d7e93.appspot.com/o/images%2Fusers%2Ffemale-user.png",
                        PhoneNumber = "0034988493",
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
                        Code = "SE182420",
                        Description = "I am experienced with web app development. Languages: Java, C#, ReactJS, Angular",
                        Email = "khanhlq@test.com",
                        UserName = "khanhlq@test.com",
                        Gender = Gender.Male,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/gemify-d7e93.appspot.com/o/images%2Fusers%2Ffemale-user.png",
                        PhoneNumber = "0034988493",
                        MajorID = 1
                    },"khanhle123456","Student"),
                    (new User
                    {
                        FirstName = "Quốc Thành",
                        LastName = "Chu",
                        Code = "SS180001",
                        Description = "Marketing student with exceptional PR and event organization skills!",
                        Email = "thanhcq@test.com",
                        UserName = "thanhcq@test.com",
                        Gender = Gender.Male,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/gemify-d7e93.appspot.com/o/images%2Fusers%2Ffemale-user.png",
                        PhoneNumber = "0034988493",
                        MajorID = 7
                    },"thanh123456","Student"),
                    (new User
                    {
                        FirstName = "Nguyên Hậu",
                        LastName = "Đỗ Thị",
                        Code = "SE180002",
                        Description = "Specialize in event managements and video production.",
                        Email = "haudtn@test.com",
                        UserName = "haudtn@test.com",
                        Gender = Gender.Female,
                        Dob = new DateOnly(2000, 2, 1),
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/gemify-d7e93.appspot.com/o/images%2Fusers%2Ffemale-user.png",
                        PhoneNumber = "0034988493",
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
                        ImgUrl = "https://firebasestorage.googleapis.com/v0/b/gemify-d7e93.appspot.com/o/images%2Fusers%2Ffemale-user.png",
                        PhoneNumber = "0034988493",
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
            if (!context.Subject.Any())
            {
                var subData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/subjects.json");

                var subjects = JsonSerializer.Deserialize<List<Subject>>(subData);

                if (subjects == null) return;

                context.Subject.AddRange(subjects);

                await context.SaveChangesAsync();
            }

            // Seed semesters
            if (!context.Semester.Any())
            {
                var semData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/semesters.json");

                var semesters = JsonSerializer.Deserialize<List<Semester>>(semData, options);

                if (semesters == null) return;

                context.Semester.AddRange(semesters);

                await context.SaveChangesAsync();
            }

            // Seed fields
            if (!context.Field.Any())
            {
                var fieldData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/fields.json");

                var fields = JsonSerializer.Deserialize<List<Field>>(fieldData);

                if (fields == null) return;

                context.Field.AddRange(fields);

                await context.SaveChangesAsync();
            }

            // Seed skills
            if (!context.Skill.Any())
            {
                var skillData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/skills.json");

                var skills = JsonSerializer.Deserialize<List<Skill>>(skillData);

                if (skills == null) return;

                context.Skill.AddRange(skills);

                await context.SaveChangesAsync();
            }

            // Seed major subjects
            if (!context.MajorSubject.Any())
            {
                var maSubData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/majorSubjects.json");

                var majorSubjects = JsonSerializer.Deserialize<List<MajorSubject>>(maSubData);


                if (majorSubjects == null) return;

                context.MajorSubject.AddRange(majorSubjects);

                await context.SaveChangesAsync();
            }

            // Seed subject fields
            if (!context.SubjectField.Any())
            {
                var subFieldData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/subjectFields.json");

                var subjectFields = JsonSerializer.Deserialize<List<SubjectField>>(subFieldData);


                if (subjectFields == null) return;

                context.SubjectField.AddRange(subjectFields);

                await context.SaveChangesAsync();
            }

            // Seed Group 
            if (!context.Group.Any())
            {
                var groupData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groups.json");

                var groups = JsonSerializer.Deserialize<List<Group>>(groupData);


                if (groups == null) return;

                context.Group.AddRange(groups);

                await context.SaveChangesAsync();
            }
            // Seed Group Position
            if (!context.GroupPosition.Any())
            {
                var groupPositionData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupPositions.json");

                var groupPositions = JsonSerializer.Deserialize<List<GroupPosition>>(groupPositionData);


                if (groupPositions == null) return;

                context.GroupPosition.AddRange(groupPositions);

                await context.SaveChangesAsync();
            }

            // Seed Group Member
            if (!context.GroupMember.Any())
            {
                var groupMemberData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupMembers.json");

                var groupMembers = JsonSerializer.Deserialize<List<GroupMember>>(groupMemberData);


                if (groupMembers == null) return;

                context.GroupMember.AddRange(groupMembers);

                await context.SaveChangesAsync();
            }

            //Seed GroupMemberPositions
            if (!context.GroupMemberPosition.Any())
            {
                var groupMemberPositionsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/groupMemberPositions.json");

                var groupMemberPositions = JsonSerializer.Deserialize<List<GroupMemberPosition>>(groupMemberPositionsData);


                if (groupMemberPositions == null) return;

                context.GroupMemberPosition.AddRange(groupMemberPositions);

                await context.SaveChangesAsync();
            }

            //Seed Applications
            if (!context.Application.Any())
            {
                var applicationData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/applications.json");

                var applications = JsonSerializer.Deserialize<List<Application>>(applicationData, options);


                if (applications == null) return;

                context.Application.AddRange(applications);

                await context.SaveChangesAsync();
            }

            //Seed Student Skills
            if (!context.StudentSkill.Any())
            {
                var studentSkillsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/studentSkills.json");

                var studentSkills = JsonSerializer.Deserialize<List<StudentSkill>>(studentSkillsData, options);


                if (studentSkills == null) return;

                context.StudentSkill.AddRange(studentSkills);

                await context.SaveChangesAsync();
            }
        }
    }
}
