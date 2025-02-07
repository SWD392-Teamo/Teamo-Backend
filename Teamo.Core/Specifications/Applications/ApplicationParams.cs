namespace Teamo.Core.Specifications.Applications
{
    public class ApplicationParams : PagingParams
    {
        public int? GroupId { get; set; }
        public int? LeaderId { get; set; }
        public int? SenderId { get; set; }
        public int? PositionId { get; set; }
        public string Status { get; set; }

        //Sort by latest or earliest date
        public string Sort { get; set; }
    }
}