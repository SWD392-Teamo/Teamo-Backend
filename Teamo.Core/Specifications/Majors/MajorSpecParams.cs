namespace Teamo.Core.Specifications.Majors
{
    public class MajorSpecParams : PagingParams
    {
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
    }
}