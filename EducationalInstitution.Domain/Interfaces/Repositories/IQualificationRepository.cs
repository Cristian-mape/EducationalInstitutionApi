using EducationalInstitution.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Interfaces.Repositories
{
    public interface IQualificationRepository
    {
        Task<Qualification?> GetByIdAsync(int id);
        Task<IEnumerable<Qualification>> GetAllAsync();
        Task<IEnumerable<Qualification>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Qualification>> GetByCourseIdAsync(int courseId);
        Task<Qualification?> GetByStudentAndCourseAsync(int studentId, int courseId);
        Task<Qualification> CreateAsync(Qualification qualification);
        Task<Qualification> UpdateAsync(Qualification qualification);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
