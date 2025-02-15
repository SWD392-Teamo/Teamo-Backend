using Teamo.Core.Enums;

namespace Teamo.Core.Specifications.Semesters
{
    public class SemesterParams
    {
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
        public SemesterStatus? Status { get; set; }
    }
}
