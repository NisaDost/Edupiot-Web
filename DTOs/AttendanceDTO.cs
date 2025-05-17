namespace EduPilot_Web.DTOs
{
    public class AttendanceDTO
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid LessonId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public string Emotion { get; set; }

    }
}
