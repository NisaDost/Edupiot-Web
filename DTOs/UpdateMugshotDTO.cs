namespace EduPilot.Web.DTOs
{
    public class UpdateMugshotDTO
    {
        public string InstitutionId { get; set; }
        public IFormFile Mugshot { get; set; }
    }
}
