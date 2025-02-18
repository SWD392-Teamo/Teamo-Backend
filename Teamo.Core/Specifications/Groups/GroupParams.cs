
using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Groups
{
    public class GroupParams : PagingParams
    {
        private string _search;

        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }

        public int? SubjectId { get; set; }
        public string Sort { get; set; } 
        public GroupStatus? Status { get; set; }
        public int? SemesterId { get; set; }
        public int? FieldId { get; set; }
        public int? StudentId { get; set; }
    }
}
