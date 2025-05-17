namespace EduPilot.Web.DTOs
{
    public class QuizDTO
    {
        public Guid SubjectId { get; set; }

        public int Duration { get; set; }
        public Difficulty Difficulty { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }
}
