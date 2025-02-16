using Teamo.Core.Enums;

namespace TeamoWeb.API.Dtos
{
    public class SemesterToUpsertDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public DateOnly? StartDate { get; set; }
        private DateOnly? _endDate;
        public DateOnly? EndDate
        {
            get => _endDate;
            set
            {
                if (StartDate != null && value <= StartDate)
                    throw new ArgumentException("End date must be greater than start date.");
                _endDate = value;
            }
        }        
    }
}
