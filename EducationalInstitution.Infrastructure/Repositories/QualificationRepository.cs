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
    public class QualificationRepository : IQualificationRepository
    {
        private readonly EducationalContext _context;

        public QualificationRepository(EducationalContext context)
        {
            _context = context;
        }

        public async Task<Qualification?> GetByIdAsync(int id)
        {
            return await _context.Qualifications
                .Include(q => q.Student)
                .Include(q => q.Course)
                    .ThenInclude(c => c.Professor)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            return await _context.Qualifications
                .Include(q => q.Student)
                .Include(q => q.Course)
                .OrderByDescending(q => q.QualificationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualification>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Qualifications
                .Include(q => q.Course)
                    .ThenInclude(c => c.Professor)
                .Where(q => q.StudentId == studentId)
                .OrderByDescending(q => q.QualificationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Qualification>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Qualifications
                .Include(q => q.Student)
                .Where(q => q.CourseId == courseId)
                .OrderByDescending(q => q.QualificationDate)
                .ToListAsync();
        }

        public async Task<Qualification?> GetByStudentAndCourseAsync(int studentId, int courseId)
        {
            return await _context.Qualifications
                .Include(q => q.Student)
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.StudentId == studentId && q.CourseId == courseId);
        }

        public async Task<Qualification> CreateAsync(Qualification qualification)
        {
            _context.Qualifications.Add(qualification);
            await _context.SaveChangesAsync();
            return qualification;
        }

        public async Task<Qualification> UpdateAsync(Qualification qualification)
        {
            _context.Qualifications.Update(qualification);
            await _context.SaveChangesAsync();
            return qualification;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var qualification = await GetByIdAsync(id);
            if (qualification == null) return false;

            _context.Qualifications.Remove(qualification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Qualifications.AnyAsync(q => q.Id == id);
        }
    }
}
