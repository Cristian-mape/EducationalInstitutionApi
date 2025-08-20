using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly EducationalContext _context;

        public StudentRepository(EducationalContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Qualifications)
                    .ThenInclude(q => q.Course)
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<Student?> GetByStudentCodeAsync(string studentCode)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode && s.IsActive);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await GetByIdAsync(id);
            if (student == null) return false;

            student.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<bool> StudentCodeExistsAsync(string studentCode)
        {
            return await _context.Students.AnyAsync(s => s.StudentCode == studentCode && s.IsActive);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Students.CountAsync(s => s.IsActive);
        }
    }
}
