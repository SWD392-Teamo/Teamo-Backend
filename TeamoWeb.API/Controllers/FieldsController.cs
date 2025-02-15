using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Fields;
using Teamo.Core.Specifications.SubjectFields;
using TeamoWeb.API.Errors;

namespace TeamoWeb.API.Controllers
{
    public class FieldsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public FieldsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Get fields
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<Field>>> GetFields([FromQuery] FieldParams fieldParams)
        {
            //Getting fields of a Subject if subjectId in params has value
            var subjectFieldSpec = new SubjectFieldSpecification(fieldParams.SubjectId);
            var subjectFields = await _unitOfWork.Repository<SubjectField>().ListAsync(subjectFieldSpec);
            var fields = subjectFields.Select(s => s.Field).ToList();

            //Getting fields with spec
            var fieldSpec = new FieldSpecification(fieldParams);
            var fieldsWithSpec = await _unitOfWork.Repository<Field>().ListAsync(fieldSpec);
            fields = fields.Where(f => fieldsWithSpec.Contains(f)).ToList();

            return Ok(fields);
        }

        //Get field by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetFieldById(int id)
        {
            var fieldSpec = new FieldSpecification(id);
            var field = await _unitOfWork.Repository<Field>().GetEntityWithSpec(fieldSpec);
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
            
            var existFieldSpec = new FieldSpecification(field.Name);
            var existField = await _unitOfWork.Repository<Field>().GetEntityWithSpec(existFieldSpec);
            if(existField != null) return BadRequest(new ApiErrorResponse(400, "Already exists field with this name."));

            _unitOfWork.Repository<Field>().Add(field);
            var result = await _unitOfWork.Complete();

            if(!result) return BadRequest(new ApiErrorResponse(400, "Failed to create new field."));
            return Ok(field);
        }
    }
}