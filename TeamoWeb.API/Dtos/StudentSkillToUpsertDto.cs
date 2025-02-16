namespace TeamoWeb.API.Dtos
{
    public class StudentSkillToUpsertDto
    {
        public int SkillId { get; set; }
        public int StudentId { get; set; }
        public string? Level { get; set; }
    }
}