using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.DTOs.Courses
{
    public class UpdateCourseDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(1, 10)]
        public int Credits { get; set; }

        [Required]
        public int ProfessorId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
