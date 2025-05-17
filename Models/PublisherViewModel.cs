namespace EduPilot.Web.Models
{
    public class PublisherViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }

        public int QuizCount { get; set; }
        public int QuestionCount { get; set; }
    }
}
