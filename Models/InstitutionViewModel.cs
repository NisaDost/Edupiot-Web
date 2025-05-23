﻿using EduPilot.Web.DTOs;

namespace EduPilot.Web.Models
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
        public List<StudentDTO> Students { get; set; } = new List<StudentDTO>();
        public List<SupervisorDTO> Supervisors { get; set; } = new List<SupervisorDTO>();

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }


        public IFormFile? LogoFile { get; set; }
    }
}