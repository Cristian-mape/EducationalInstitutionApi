using EducationalInstitution.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Interfaces.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByStudentCodeAsync(string studentCode);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<IEnumerable<Student>> GetPagedAsync(int page, int pageSize);
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> StudentCodeExistsAsync(string studentCode);
        Task<int> CountAsync();
    }
}
