using EduPilot_Web.Models.DTOs;

namespace EduPilot_Web.Models
{
    public class QuizViewModel
    {
        public int SelectedGrade { get; set; }
        public Guid SelectedLessonId { get; set; }
        public Guid SelectedSubjectId { get; set; }

        public List<int> Grades { get; set; } = Enumerable.Range(1, 12).ToList();
        public List<LessonsByGradeDTO> Lessons { get; set; } = new();
        public List<SubjectDTO> Subjects { get; set; } = new();

        public Difficulty Difficulty { get; set; }
        public bool IsActive { get; set; }
    }
}
