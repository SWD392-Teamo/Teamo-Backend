using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Entities.Identity;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService _profileService;
        
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userProfile = await _profileService.GetProfileAsync(User.GetEmail());
            return Ok(userProfile.ToProfileDto());
        }

        //Update description in user profile
        [HttpPatch("des-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileDescription([FromBody] ProfileUpdateDto profile)
        {
            var result = await _profileService.UpdateProfileDescriptionAsync(profile.UserId, profile.Description);
            if(result.Succeeded) return Ok();
            else return BadRequest();
        }

        //Add a new skill to user profile
        [HttpPost("skill-add")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileSkill([FromBody] ProfileUpdateDto profile)
        {
            var newSkill = profile.StudentSkill;
            if(newSkill == null) return BadRequest();
            Enum.TryParse(newSkill.SkillLevel, out StudentSkillLevel level);
            var studentSkill = new StudentSkill()
                {
                    SkillId = newSkill.SkillId, 
                    StudentId = profile.UserId,
                    Level = level
                };
            var result = await _profileService.AddProfileSkillAsync(studentSkill);
            if(result) return Ok();
            else return BadRequest();
        }

        //Update a skill level in user profile
        [HttpPatch("skill-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileSkill([FromBody] ProfileUpdateDto profile)
        {
            var updateSkill = profile.StudentSkill;
            if(updateSkill == null) return BadRequest();
            Enum.TryParse(updateSkill.SkillLevel, out StudentSkillLevel updateLevel);
            var result = await _profileService.UpdateProfileSkillAsync(profile.UserId, updateSkill.SkillId, updateLevel);
            if(result) return Ok();
            else return BadRequest();
        }

        //Add a new link to user profile
        [HttpPost("link-add")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> AddProfileLink([FromBody] ProfileUpdateDto profile)
        {
            var newLink = profile.Link;
            if(newLink == null) return BadRequest();
            var link = new Link()
                {
                    Name = newLink.Name,
                    Url = newLink.Url,
                    StudentId = profile.UserId
                };
            var result = await _profileService.AddProfileLinkAsync(link);
            if(result) return Ok();
            else return BadRequest();
        }

        //Update a link in user profile
        [HttpPatch("link-update")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> UpdateProfileLink([FromBody] ProfileUpdateDto profile)
        {
            var updateLink = profile.Link;
            if(updateLink == null) return BadRequest();
            var result = await _profileService.UpdateProfileLinkAsync(updateLink.Id, updateLink.Name, updateLink.Url);
            if(result) return Ok();
            else return BadRequest();
        }

        //Remove a link from user profile
        [HttpDelete("link-del")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult> RemoveProfileLink([FromBody] ProfileUpdateDto profile)
        {
            var delLink = profile.Link;
            if(delLink == null) return BadRequest();
            var result = await _profileService.RemoveProfileLinkAsync(delLink.Id);
            if(result) return Ok();
            else return BadRequest();
        }
    }
}