using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Skills;
using Teamo.Core.Specifications.StudentSkills;

namespace Teamo.Infrastructure.Services
{
    public class SkillService : ISkillService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SkillService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Skill> GetSkillByIdAsync(int id)
        {
            var skillSpec = new SkillSpecification(id);
            return await _unitOfWork.Repository<Skill>().GetEntityWithSpec(skillSpec);
        }

        public async Task<IReadOnlyList<Skill>> GetSkillsWithSpecAsync(SkillParams skillParams)
        {
            var skillSpec = new SkillSpecification(skillParams);
            var skills = await _unitOfWork.Repository<Skill>().ListAsync(skillSpec);

            if(!skillParams.StudentId.HasValue)
            {
                return skills;
            }

            var compareParams = new StudentSkillForCompareParams{ StudentId = skillParams.StudentId };
            var compareSpec = new StudentSkillForCompareSpecification(compareParams);
            var compareStudentSkills = await _unitOfWork.Repository<StudentSkill>().ListAsync(compareSpec);
            
            var compareSkills = (IReadOnlyList<Skill>) compareStudentSkills
                .Select(async s => await _unitOfWork.Repository<Skill>().GetByIdAsync(s.SkillId)).ToList();

            skills = skills.Where(s => !compareSkills.Contains(s)).ToList();

            return skills;
        }
    }
}