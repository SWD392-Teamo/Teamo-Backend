using Microsoft.AspNetCore.Mvc;
using Teamo.Core.Entities;
using Teamo.Core.Interfaces;

namespace TeamoWeb.API.Controllers
{
    public class SubjectsController : BaseApiController
    {
        private readonly IGenericRepository<Subject> _subjectsRepository;
        public SubjectsController(IGenericRepository<Subject> subjectsRepository)
        {
            _subjectsRepository = subjectsRepository;
        }
        public async Task<ActionResult<Subject>> GetSubjectsByMajorId(int id)
        {
            
        }
    }
}
