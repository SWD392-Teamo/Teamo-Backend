using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Fields;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Errors;
using TeamoWeb.API.Extensions;
using TeamoWeb.API.RequestHelpers;

namespace TeamoWeb.API.Controllers
{
    public class FieldsController : BaseApiController
    {
        private readonly IFieldService _fieldService;
        
        public FieldsController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        //Get fields
        [Cache(1000)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<Field>>> GetFields([FromQuery] FieldParams fieldParams)
        {
            var fields = await _fieldService.GetFieldsWithSpecAsync(fieldParams);
            var fieldsToDtos = fields.Select(f => f.ToDto()).ToList();
            var count = await _fieldService.CountAsync(fieldParams);
            var pagination = new Pagination<Field>(fieldParams.PageIndex,fieldParams.PageSize,count,fields);
            return Ok(pagination);
        }

        //Get field by id
        [Cache(1000)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetFieldById(int id)
        {
            var field = await _fieldService.GetFieldByIdAsync(id);
            if(field == null) return NotFound(new ApiErrorResponse(404, "Field not found."));
            return Ok(field.ToDto());
        }

        //Create new field
        [InvalidateCache("/fields")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Field>> CreateField([FromBody] FieldDto field)
        {
            if(field == null || string.IsNullOrEmpty(field.Name))
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));
            
            var existField = await _fieldService.CheckDuplicateNameField(field.Name);
            if(!existField) return BadRequest(new ApiErrorResponse(400, "Already exists field with this name."));

            var result = await _fieldService.CreateFieldAsync(field.ToEntity());

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create new field."));
            return Ok(new ApiErrorResponse(200, "Created new field successfully."));
        }

        //Delete field
        [InvalidateCache("/fields")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteField(int id)
        {
            var field = await _fieldService.GetFieldByIdAsync(id);
            if(field == null) return NotFound(new ApiErrorResponse(404, "Field not found"));
            
            var result = await _fieldService.DeleteFieldAsync(field);
            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to delete field."));
            return Ok(new ApiErrorResponse(200, "Deleted field successfully."));            
        }
    }
}