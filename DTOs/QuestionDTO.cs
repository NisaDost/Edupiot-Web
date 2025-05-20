namespace EduPilot.Web.DTOs
{
    public class QuestionDTO
    {
        public Guid QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string QuestionImage { get; set; }
        public IFormFile? File { get; set; }
        public bool IsActive { get; set; }
        public List<ChoiceDTO> Choices { get; set; } = new List<ChoiceDTO>();
    }
}
