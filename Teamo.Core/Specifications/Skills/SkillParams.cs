namespace Teamo.Core.Specifications.Skills
{
    public class SkillParams
    {
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }

        public int? StudentId { get; set; }
    }
}