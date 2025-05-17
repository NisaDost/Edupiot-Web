namespace EduPilot.Web.DTOs
{
    public class QuestionDTO
    {
        public string QuestionContent { get; set; }
        public IFormFile? File { get; set; }
        public bool isActive { get; set; }
        public List<ChoiceDTO> Choices { get; set; }
    }
}
