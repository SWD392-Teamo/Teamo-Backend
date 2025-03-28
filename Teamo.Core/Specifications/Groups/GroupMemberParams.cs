﻿using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupMemberParams : PagingParams
    {
        public int? StudentId { get; set; }
        public int? GroupId { get; set; }
        public GroupMemberRole? Role { get; set; }
    }
}
