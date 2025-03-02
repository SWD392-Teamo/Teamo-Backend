namespace Teamo.Core.Specifications.Posts
{
    public class PostParams : PagingParams
    {
        public int? GroupId { get; set; }
        public string Sort {  get; set; }

    }
}
