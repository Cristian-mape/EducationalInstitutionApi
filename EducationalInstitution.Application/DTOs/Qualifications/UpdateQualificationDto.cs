using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.DTOs.Qualifications
{
    public class UpdateQualificationDto
    {
        [Required]
        [Range(0, 5)]
        public decimal Grade { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime QualificationDate { get; set; }
    }
}
