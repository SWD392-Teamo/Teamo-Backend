
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
    }
}
