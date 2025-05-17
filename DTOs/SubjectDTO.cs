namespace EduPilot.Web.DTOs
{
    public class SubjectDTO
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
    }
}
