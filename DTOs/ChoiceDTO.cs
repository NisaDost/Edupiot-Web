namespace EduPilot.Web.DTOs
{
    public class ChoiceDTO
    {
        public Guid ChoiceId { get; set; }
        public string ChoiceContent { get; set; }
        public bool IsCorrect { get; set; }
    }
}
