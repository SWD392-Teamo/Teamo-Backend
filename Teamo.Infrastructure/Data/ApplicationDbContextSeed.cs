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
            if (!context.Majors.Any())
            {
                var maData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/majors.json");

                var majors = JsonSerializer.Deserialize<List<Major>>(maData);

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
                    }, "admin123456", "Admin")
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

                var subjects = JsonSerializer.Deserialize<List<Subject>>(subData);

                if (subjects == null) return;

                context.Subjects.AddRange(subjects);

                await context.SaveChangesAsync();
            }

            // Seed semesters
            if (!context.Semesters.Any())
            {
                var semData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/semesters.json");

                var semesters = JsonSerializer.Deserialize<List<Semester>>(semData);

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
        }
    }
}
