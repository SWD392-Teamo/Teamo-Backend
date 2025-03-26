namespace Teamo.Core.Specifications.Users
{
    public class UserSpecParams : PagingParams
    {
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }

        public int? UserId { get; set; }
        public int? MajorId { get; set; }
        public string Status { get; set; }

        //Sort alphabetically by first names
        public string Sort { get; set; }
    }
}