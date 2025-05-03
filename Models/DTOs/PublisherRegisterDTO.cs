namespace EduPilot_Web.Models.DTOs
{
    public class PublisherRegisterDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Address { get; set; }
        public string? Logo { get; set; }
        public string? Website { get; set; }
    }
}
