using EduPilot_Web.Models.DTOs;
namespace EduPilot_Web.Models
{
    public class InstitutionViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public string? Website { get; set; }
        public List<StudentDTO> Students { get; set; }
        public List<SupervisorDTO> Supervisors { get; set; }
    }
}
