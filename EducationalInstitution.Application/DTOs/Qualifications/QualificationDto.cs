using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.DTOs.Qualifications
{
    public class QualificationDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public decimal Grade { get; set; }
        public DateTime QualificationDate { get; set; }
        public string? Comments { get; set; }
        public bool IsPassing { get; set; }
    }
}
