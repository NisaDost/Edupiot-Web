namespace EduPilot_Web.Models.DTOs
{
    public class InstitutionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public string? Website { get; set; }
    }
}
