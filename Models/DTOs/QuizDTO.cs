namespace EduPilot_Web.Models.DTOs
{
    public class QuizDTO
    {
        public Guid SubjectId { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }
}
