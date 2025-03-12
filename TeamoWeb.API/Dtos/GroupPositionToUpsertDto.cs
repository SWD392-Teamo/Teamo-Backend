
﻿using System.ComponentModel.DataAnnotations;
using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class GroupPositionToUpsertDto
    {
        public string? Name { get; set; }
        [Range(1, 100, ErrorMessage = "Count must be between 1 and 50.")]
        public int? Count { get; set; }
        public GroupPositionStatus? Status { get; set; }
        public IEnumerable<int> SkillIds { get; set; } = Enumerable.Empty<int>();
    }
}
