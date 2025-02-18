
﻿using System.ComponentModel.DataAnnotations;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupToUpsertDto
    {
        public string? Name { get; set; }
        public string? Title { get; set; } 
        public int? SemesterId { get; set; }
        public string? Description { get; set; }
        [Range(1, 100, ErrorMessage = "MaxMember must be between 1 and 100.")]
        public int? MaxMember { get; set; }
        public int? FieldId { get; set; }
        public int? SubjectId { get; set; }
        public GroupStatus? Status { get; set; }    
        public IEnumerable<GroupPositionToUpsertDto> GroupPositions { get; set; } = Enumerable.Empty<GroupPositionToUpsertDto>();
    }
}
