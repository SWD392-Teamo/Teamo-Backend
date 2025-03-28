namespace Teamo.Core.Specifications.Fields
{
    public class FieldParams : PagingParams
    {
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }

        public int? SubjectId { get; set; }
    }
}