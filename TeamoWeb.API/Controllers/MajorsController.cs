using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Enums;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Majors;
using Teamo.Core.Specifications.Subjects;
using Teamo.Infrastructure.Data;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;
using static System.Net.Mime.MediaTypeNames;

namespace TeamoWeb.API.Controllers
{
    public class MajorsController : BaseApiController
    {
        private readonly IGenericRepository<Major> _majorRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IConfiguration _config;

        public MajorsController(IGenericRepository<Major> majorRepo, 
            IUnitOfWork unitOfWork, IUploadService uploadService, IConfiguration config)
        {
            _majorRepo = majorRepo;
            _unitOfWork = unitOfWork;
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
        public async Task<ActionResult<MajorDto>> CreateMajor([FromForm] MajorToUpsertDto majorDto)
        {
            var majorSpec = new MajorSpecification(majorDto.Code);
            var duplicateCode = await _majorRepo.GetEntityWithSpec(majorSpec);
            if (duplicateCode != null) return BadRequest(new ApiErrorResponse(400, "Major code already existed"));
        
            var major = majorDto.toEntity();
            if(majorDto.Image != null)
            {
                var image = majorDto.Image;
                // Upload and get download url
                var imgUrl = await _uploadService.UploadFileAsync(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    _config["Firebase:MajorImagesUrl"]);

                // Update image url
                major.ImgUrl = imgUrl;
            }
                
            _unitOfWork.Repository<Major>().Add(major);
            var result = await _unitOfWork.Complete();

            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to create new major"));

            var newSpec = new MajorSpecification(major.Id);
            var createdMajor = await _majorRepo.GetEntityWithSpec(newSpec);

            return Ok(createdMajor.ToDto());
        }

        [InvalidateCache("/majors")]
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorDto>> UpdateMajor(int id,[FromForm] MajorToUpsertDto majorDto)
        {
            var major = await _majorRepo.GetEntityWithSpec(new MajorSpecification(id));
            if (major == null) return NotFound();
            major = majorDto.toEntity(major);

            if (majorDto.Image != null)
            {
                var image = majorDto.Image;
                // Upload and get download url
                var imgUrl = await _uploadService.UploadFileAsync(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    _config["Firebase:MajorImagesUrl"]);

                // Update image url
                major.ImgUrl = imgUrl;
            }

            _unitOfWork.Repository<Major>().Update(major);
            var result = await _unitOfWork.Complete();

            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to update new major"));

            var newSpec = new MajorSpecification(id);
            var updatedMajor = await _unitOfWork.Repository<Major>().GetEntityWithSpec(newSpec);

            return Ok(updatedMajor.ToDto());
        }

        [InvalidateCache("/majors")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MajorDto>> DeleteMajor(int id)
        {
            var major = await _majorRepo.GetEntityWithSpec(new MajorSpecification(id));
            if (major == null) return NotFound();

            major.Status = MajorStatus.Inactive;
            _majorRepo.Update(major);
            var result = await _majorRepo.SaveAllAsync();

            if (!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete major"));

            return Ok(new ApiErrorResponse(200, "Major deleted successfully."));
        }
    }
}