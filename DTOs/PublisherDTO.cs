﻿namespace EduPilot.Web.DTOs
{
    public class PublisherDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public int QuizCount { get; set; }
        public int QuestionCount { get; set; }
    }
}
