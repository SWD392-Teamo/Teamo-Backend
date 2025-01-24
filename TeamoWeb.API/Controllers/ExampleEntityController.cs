using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications;
using TeamoWeb.API.Dtos;
using TeamoWeb.API.Extensions;

namespace TeamoWeb.API.Controllers
{
    public class ExampleEntityController : BaseApiController
    {
        private readonly IUnitOfWork _unit;

        public ExampleEntityController(IUnitOfWork unit) 
        {
            _unit = unit;
        }

        [HttpGet("example")]
        public async Task<ActionResult<IReadOnlyList<ExampleEntityDto>>> GetEntities([FromQuery] ExampleEntitySpecParams specParams)
        {
            var spec = new ExampleEntitySpecification(specParams);

            // Return a paginated result of ExampleEntityDto
            // Noticed that the ToDto() extension method
            // was called by the entity class itself
            return await CreatePagedResult(_unit.Repository<ExampleEntity>(), spec, specParams.PageIndex,
                specParams.PageSize, e => e.ToDto());
        }
    }
}
