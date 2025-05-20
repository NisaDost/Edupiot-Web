namespace EduPilot.Web.DTOs
{
    public class PublisherQuizzesDTO
    {
        public Guid Id { get; set; }
        public String SubjectName { get; set; }
        public int Duration { get; set; }
        public Difficulty Difficulty { get; set; }
        public bool IsActive { get; set; }
        public int QuestionCount { get; set; }
    }
}
