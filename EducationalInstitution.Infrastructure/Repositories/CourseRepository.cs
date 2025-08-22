using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using EducationalInstitution.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly EducationalContext _context;

        public CourseRepository(EducationalContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Professor)
                .Include(c => c.Qualifications)
                    .ThenInclude(q => q.Student)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Course?> GetByCodeAsync(string courseCode)
        {
            return await _context.Courses
                .Include(c => c.Professor)
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode && c.IsActive);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Professor)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByProfessorIdAsync(int professorId)
        {
            return await _context.Courses
                .Include(c => c.Professor)
                .Where(c => c.ProfessorId == professorId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Course> CreateAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await GetByIdAsync(id);
            if (course == null) return false;

            course.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<bool> CourseCodeExistsAsync(string courseCode)
        {
            return await _context.Courses.AnyAsync(c => c.CourseCode == courseCode && c.IsActive);
        }
    }
}
