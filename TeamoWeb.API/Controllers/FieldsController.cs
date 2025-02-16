using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Interfaces.Services;
using Teamo.Core.Specifications.Fields;
using Teamo.Core.Specifications.SubjectFields;
using TeamoWeb.API.Errors;

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
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<Field>>> GetFields([FromQuery] FieldParams fieldParams)
        {
            var fields = await _fieldService.GetFieldsWithSpecAsync(fieldParams);
            return Ok(fields);
        }

        //Get field by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetFieldById(int id)
        {
            var field = await _fieldService.GetFieldByIdAsync(id);
            if(field == null) return NotFound(new ApiErrorResponse(404, "Field not found."));
            return Ok(field);
        }

        //Create new field
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Field>> CreateField([FromBody] Field field)
        {
            if(field == null || string.IsNullOrEmpty(field.Name))
                return BadRequest(new ApiErrorResponse(400, "Please input all required fields."));
            
            var existField = await _fieldService.CheckDuplicateNameField(field.Name);
            if(!existField) return BadRequest(new ApiErrorResponse(400, "Already exists field with this name."));

            var result = await _fieldService.CreateFieldAsync(field);

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create new field."));
            return Ok(field);
        }

        //Delete field
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