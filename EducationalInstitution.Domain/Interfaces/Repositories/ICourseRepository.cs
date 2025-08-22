using EducationalInstitution.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(int id);
        Task<Course?> GetByCodeAsync(string courseCode);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> GetByProfessorIdAsync(int professorId);
        Task<Course> CreateAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> CourseCodeExistsAsync(string courseCode);
    }
}
