using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Subjects;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class SubjectsController : BaseApiController
    {
        private readonly ISubjectService _subjectService;
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;

        public SubjectsController(ISubjectService subjectService, 
            IUploadService uploadService, IConfiguration config)
        {
            _subjectService = subjectService;
            _uploadService = uploadService;
            _config = config;
        }

        //Get subjects with spec
        [Cache(1000)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetSubjects([FromQuery] SubjectParams subjectParams)
        {
            var subjects = await _subjectService.GetSubjectsAsync(subjectParams);
            var count = await _subjectService.CountSubjectsAsync(subjectParams);
            var subjectsToDto = subjects.Select(s => s.ToDto()).ToList();
            var pagination = new Pagination<SubjectDto>(subjectParams.PageIndex,subjectParams.PageSize,count,subjectsToDto);

            return Ok(pagination);
        }

        //Get subject by id
        [Cache(1000)]
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubjectById(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);

            if (subject == null) 
                return NotFound(new ApiErrorResponse(404, "Subject not found"));

            return Ok(subject.ToDto());
        }


        //Create a new subject
        [InvalidateCache("/subjects")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubjectDto>> CreateNewSubject([FromForm] SubjectToUpsertDto subjectDto)
        {
            if(string.IsNullOrEmpty(subjectDto.Name) || string.IsNullOrEmpty(subjectDto.Code))
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));

            var duplicateCode = await _subjectService.CheckDuplicateCodeSubject(subjectDto.Code);
            if(!duplicateCode) return BadRequest(new ApiErrorResponse(400, "Subject code must be unique."));
            
            var subject = subjectDto.ToEntity();
            if(subjectDto.Image != null)
            {
                var image = subjectDto.Image;
                // Upload and get download url
                var imgUrl = await _uploadService.UploadFileAsync(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    _config["Firebase:SubjectImagesUrl"]);

                // Update image url
                subject.ImgUrl = imgUrl;
            }

            var newSubject = await _subjectService.CreateSubjectAsync(subject);

            if(newSubject == null) return BadRequest(new ApiErrorResponse(400, "Failed to create new subject."));
            else return Ok(newSubject.ToDto());
        }

        //Update subject, update name or description only
        [InvalidateCache("/subjects")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubjectDto>> UpdateSubject(int id, [FromForm] SubjectToUpsertDto subjectDto)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if(subject == null) return NotFound(new ApiErrorResponse(404, "Subject not found."));

            subject = subjectDto.ToEntity(subject);
            if (subjectDto.Image != null)
            {
                var image = subjectDto.Image;
                // Upload and get download url
                var imgUrl = await _uploadService.UploadFileAsync(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    _config["Firebase:SubjectImagesUrl"]);

                // Update image url
                subject.ImgUrl = imgUrl;
            }

            var result = await _subjectService.UpdateSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to update subject."));
            else return Ok(subject.ToDto());
        }


        //Delete subject, change subject status to inactive
        [InvalidateCache("/subjects")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if(subject == null) return BadRequest(new ApiErrorResponse(400, "Subject not found."));
            if(subject.Status == SubjectStatus.Inactive)
                return BadRequest(new ApiErrorResponse(400, "Subject is already inactive."));

            var result = await _subjectService.DeleteSubjectAsync(subject);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete subject"));
            return Ok(new ApiErrorResponse(200, "Deleted subject successfully."));
        }
    }
}
