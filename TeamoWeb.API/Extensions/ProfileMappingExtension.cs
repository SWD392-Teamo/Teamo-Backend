using Teamo.Core.Entities.Identity;
using TeamoWeb.API.Dtos;

namespace TeamoWeb.API.Extensions
{
    public static class ProfileMappingExtension
    {
        public static ProfileDto? ToProfileDto(this User? user)
        {
            if(user == null) return null;
            return new ProfileDto
            {
                Id = user.Id,
                Code = user.Code,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Gender = user.Gender.ToString(),
                Dob = user.Dob,
                ImgUrl = user.ImgUrl ?? null,
                Description = user.Description ?? null,
                MajorCode = user.Major.Code ?? null,
                Links = (user.Links != null) ? 
                    user.Links.Select(l => l.ToDto()).ToList() : new List<LinkDto?>(),
                StudentSkills = (user.StudentSkills != null) ? 
                    user.StudentSkills.Select(s => s.ToDto()).ToList() : new List<StudentSkillDto?>()
            };
        }

        //Update description (profile image and description)
        public static User UpdateDescription(this ProfileDto profileDto, User user)
        {
            user.ImgUrl = string.IsNullOrEmpty(profileDto.ImgUrl) ? user.ImgUrl : profileDto.ImgUrl;
            user.Description = string.IsNullOrEmpty(profileDto.Description) ? user.Description : profileDto.Description;
            return user;
        }
    }
}
