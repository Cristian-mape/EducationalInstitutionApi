using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Infrastructure.Repositories
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly EducationalContext _context;

        public ProfessorRepository(EducationalContext context)
        {
            _context = context;
        }

        public async Task<Professor?> GetByIdAsync(int id)
        {
            return await _context.Professors
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Professor?> GetByEmployeeCodeAsync(string employeeCode)
        {
            return await _context.Professors
                .FirstOrDefaultAsync(p => p.EmployeeCode == employeeCode && p.IsActive);
        }

        public async Task<IEnumerable<Professor>> GetAllAsync()
        {
            return await _context.Professors
                .Where(p => p.IsActive)
                .OrderBy(p => p.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Professor>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Professors
                .Where(p => p.IsActive)
                .OrderBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Professor> CreateAsync(Professor professor)
        {
            _context.Professors.Add(professor);
            await _context.SaveChangesAsync();
            return professor;
        }

        public async Task<Professor> UpdateAsync(Professor professor)
        {
            _context.Professors.Update(professor);
            await _context.SaveChangesAsync();
            return professor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var professor = await GetByIdAsync(id);
            if (professor == null) return false;

            professor.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Professors.AnyAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<bool> EmployeeCodeExistsAsync(string employeeCode)
        {
            return await _context.Professors.AnyAsync(p => p.EmployeeCode == employeeCode && p.IsActive);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Professors.CountAsync(p => p.IsActive);
        }
    }
}
