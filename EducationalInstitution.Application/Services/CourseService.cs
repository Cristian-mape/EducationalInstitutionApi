using AutoMapper;
using EducationalInstitution.Application.DTOs.Courses;
using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.Services
{
    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IProfessorRepository _professorRepository;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository, IProfessorRepository professorRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _professorRepository = professorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            return course != null ? _mapper.Map<CourseDto>(course) : null;
        }

        public async Task<IEnumerable<CourseDto>> GetByProfessorIdAsync(int professorId)
        {
            var courses = await _courseRepository.GetByProfessorIdAsync(professorId);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto createCourseDto)
        {
            if (await _courseRepository.CourseCodeExistsAsync(createCourseDto.CourseCode))
            {
                throw new InvalidOperationException("El código de curso ya existe");
            }

            if (!await _professorRepository.ExistsAsync(createCourseDto.ProfessorId))
            {
                throw new InvalidOperationException("El profesor especificado no existe");
            }

            var course = _mapper.Map<Course>(createCourseDto);
            var createdCourse = await _courseRepository.CreateAsync(course);
            return _mapper.Map<CourseDto>(createdCourse);
        }

        public async Task<CourseDto?> UpdateAsync(int id, UpdateCourseDto updateCourseDto)
        {
            var existingCourse = await _courseRepository.GetByIdAsync(id);
            if (existingCourse == null) return null;

            if (!await _professorRepository.ExistsAsync(updateCourseDto.ProfessorId))
            {
                throw new InvalidOperationException("El profesor especificado no existe");
            }

            _mapper.Map(updateCourseDto, existingCourse);
            var updatedCourse = await _courseRepository.UpdateAsync(existingCourse);
            return _mapper.Map<CourseDto>(updatedCourse);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _courseRepository.DeleteAsync(id);
        }
    }
}
