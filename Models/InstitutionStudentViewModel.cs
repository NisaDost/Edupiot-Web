namespace EduPilot_Web.Models
{
    public class InstitutionStudentViewModel
    {
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public int Grade { get; set; }
        public string Email { get; set; }
        public List<string>? SupervisorName { get; set; }
        public string InstitutionId { get; set; }
        public string InstitutionName { get; set; }
    }
}
