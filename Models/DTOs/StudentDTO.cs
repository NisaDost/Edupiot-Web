namespace EduPilot_Web.Models.DTOs
{
    public class StudentDTO
    {
        public Guid StudentId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Grade { get; set; }
        public List<string>? SupervisorName { get; set; }
    }
}
