namespace EduPilot.Web.DTOs
{
    public class PublisherQuestionDTO
    {
        public Guid QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string? QuestionImage { get; set; }
        public List<ChoiceDTO> Choices { get; set; }
    }
}
