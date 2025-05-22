namespace EduPilot.Web.DTOs
{
    public class UpdateMugshotDTO
    {
        public string StudentId { get; set; }
        public IFormFile Mugshot { get; set; }
    }
}
