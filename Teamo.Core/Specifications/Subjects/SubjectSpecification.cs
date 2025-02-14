﻿using Teamo.Core.Entities;
using Teamo.Core.Specifications.Groups;

namespace Teamo.Core.Specifications.Subjects
{
    public class SubjectSpecification : BaseSpecification<Subject>
    {
        public SubjectSpecification(SubjectParams subjectParams)
            : base(x => (string.IsNullOrEmpty(subjectParams.Search)
            || x.Name.ToLower().Contains(subjectParams.Search)
            || x.Code.ToLower().Contains(subjectParams.Search)) &&
            (!subjectParams.MajorId.HasValue || subjectParams.MajorId.HasValue))
        {
            ApplyPaging(subjectParams.PageSize * (subjectParams.PageIndex - 1),
                subjectParams.PageSize);
        }
        
        public SubjectSpecification(int id) : base(x => x.Id == id)
        {
        }

        public SubjectSpecification(string code) 
            : base(x => x.Code.ToLower().Equals(code.ToLower()))
        {
        }
    }
}
