using Teamo.Core.Entities;
using Teamo.Core.Interfaces;
using Teamo.Core.Specifications.Groups;
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

            var compareSkills = new List<Skill>();
            foreach (var s in compareStudentSkills) 
            {
                var skill = await _unitOfWork.Repository<Skill>().GetByIdAsync(s.SkillId);
                compareSkills.Add(skill);
            }

            skills = skills.Where(s => !compareSkills.Contains(s)).ToList();

            return skills;
        }

        public async Task<Skill> CreateSkillAsync(Skill skill)
        {
            _unitOfWork.Repository<Skill>().Add(skill);
            await _unitOfWork.Complete();
            return await GetSkillByIdAsync(skill.Id);
        }

        public async Task<Skill> UpdateSkillAsync(Skill skill)
        {
            _unitOfWork.Repository<Skill>().Update(skill);
            await _unitOfWork.Complete();
            return await GetSkillByIdAsync(skill.Id);
        }

        public async Task<bool> DeleteSkillAsync(Skill skill)
        {
            //Check if skill is used in students' profile
            var studentSkillSpec = new StudentSkillSpecification(skill.Id);
            var studentSkill = await _unitOfWork.Repository<StudentSkill>().GetEntityWithSpec(studentSkillSpec);

            //Check if skill is used in group positions
            var positionSkillSpec = new GroupPositionSkillBySkillIdSpecification(skill.Id);
            var positionSkill = await _unitOfWork.Repository<GroupPositionSkill>().GetEntityWithSpec(positionSkillSpec);

            if(studentSkill != null || positionSkill != null) return false;

            _unitOfWork.Repository<Skill>().Delete(skill);
            return await _unitOfWork.Complete();
        }

        public async Task<bool> CheckDuplicateSkillAsync(string name)
        {
            var result = true;

            var skillSpec = new SkillSpecification(name);
            var existSkill = await _unitOfWork.Repository<Skill>().GetEntityWithSpec(skillSpec);
            
            if(existSkill != null) result = false;

            return result;
        }
    }
}