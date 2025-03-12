using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Majors;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class MajorsController : BaseApiController
    {
        private readonly IGenericRepository<Major> _majorRepo;
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;

        public MajorsController(IGenericRepository<Major> majorRepo, 
            IUploadService uploadService, IConfiguration config)
        {
            _majorRepo = majorRepo;
            _uploadService = uploadService;
            _config = config;
        }

        //Get list of majors with spec
        [Cache(1000)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MajorDto>>> GetMajors([FromQuery] MajorSpecParams majorSpecParams)
        {
            var majorSpec = new MajorSpecification(majorSpecParams);
            return await CreatePagedResult(_majorRepo, majorSpec, majorSpecParams.PageIndex,
                majorSpecParams.PageSize, m => m.ToDto());
        }

        //Get major by Id
        [Cache(1000)]
        [HttpGet("{id}")]
        public async Task<ActionResult<MajorDto?>> GetMajorById(int id)
        {
            var majorSpec = new MajorSpecification(id);
            var major = await _majorRepo.GetEntityWithSpec(majorSpec);

            if (major == null) return NotFound(new ApiErrorResponse(404, "Major not found."));
            
            return Ok(major.ToDto());
        }

        [InvalidateCache("/majors")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorDto>> CreateMajor(MajorToUpsertDto majorDto)
        {
            try
            {
                var major = majorDto.toEntity();
                _majorRepo.Add(major);
                await _majorRepo.SaveAllAsync();
                return Ok(major.ToDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }           
        }

        [InvalidateCache("/majors")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorDto>> UpdateMajor(int id, MajorToUpsertDto majorDto)
        {
            try
            {
                var major = await _majorRepo.GetEntityWithSpec(new MajorSpecification(id));
                if (major == null) return NotFound();

                major = majorDto.toEntity(major);
                _majorRepo.Update(major);
                await _majorRepo.SaveAllAsync();
                return Ok(major.ToDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }

        [InvalidateCache("/majors")]
        [HttpPost("{id}/image")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UploadMajorImage(int id, IFormFile image)
        {
            // Check if an image was chosen
            if (image == null) return BadRequest(new ApiErrorResponse(400, "No image found"));

            var major = await _majorRepo.GetEntityWithSpec(new MajorSpecification(id));
            if (major == null) return NotFound();

            // Upload and get download url
            var imgUrl = await _uploadService.UploadFileAsync(
                image.OpenReadStream(),
                image.FileName,
                image.ContentType,
                _config["Firebase:MajorImagesUrl"]);

            // Update image url
            major.ImgUrl = imgUrl;

            _majorRepo.Update(major);
            var result = await _majorRepo.SaveAllAsync();

            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to upload image."));

            return Ok(new ApiErrorResponse(200, "Image uploaded successfully.", imgUrl));
        }

        [InvalidateCache("/majors")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorDto>> DeleteMajor(int id)
        {
            try
            {
                var major = await _majorRepo.GetEntityWithSpec(new MajorSpecification(id));
                if (major == null) return NotFound();

                major.Status = MajorStatus.Inactive;
                _majorRepo.Update(major);
                return Ok(new ApiErrorResponse(200, "Major deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponse(400, ex.Message, ex.InnerException?.Message));
            }
        }
    }
}