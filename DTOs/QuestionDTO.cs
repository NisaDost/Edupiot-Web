namespace EduPilot_Web.Models.DTOs
{
    public class QuestionDTO
    {
        public string QuestionContent { get; set; }
        public string? QuestionImage { get; set; }
        public bool isActive { get; set; }
        public List<ChoiceDTO> Choices { get; set; }
    }
}
