using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Entities
{
    public class Qualification
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        [Range(0, 5)]
        public decimal Grade { get; set; }

        public DateTime QualificationDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación
        public virtual Student Student { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;

        public bool IsPassing => Grade >= 3.0m;
    }
}
