using EducationalInstitution.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Interfaces.Repositories
{
    public interface IProfessorRepository
    {
        Task<Professor?> GetByIdAsync(int id);
        Task<Professor?> GetByEmployeeCodeAsync(string employeeCode);
        Task<IEnumerable<Professor>> GetAllAsync();
        Task<IEnumerable<Professor>> GetPagedAsync(int page, int pageSize);
        Task<Professor> CreateAsync(Professor professor);
        Task<Professor> UpdateAsync(Professor professor);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmployeeCodeExistsAsync(string employeeCode);
        Task<int> CountAsync();
    }
}
