namespace EduPilot.Web.DTOs
{
    public class QuizDetailDTO
    {
        public Guid Id { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Duration { get; set; }
        public bool IsActive { get; set; }
        public int QuestionCount { get; set; }
        public List<PublisherQuestionDTO> Questions { get; set; }
    }
}
