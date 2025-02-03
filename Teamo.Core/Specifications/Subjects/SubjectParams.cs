namespace Teamo.Core.Specifications.Subjects
{
    public class SubjectParams : PagingParams
    {
        private string _search;

        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }

        public int? MajorId { get; set; }
        public string Sort { get; set; }
    }
}
