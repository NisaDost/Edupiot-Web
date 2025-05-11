namespace EduPilot_Web.Models
{
    public class InstitutionSupervisorViewModel
    {
        public Guid SupervisorId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
