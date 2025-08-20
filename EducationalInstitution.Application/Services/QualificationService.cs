using AutoMapper;
using EducationalInstitution.Application.DTOs.Qualifications;
using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.Services
{
    public class QualificationService
    {
        private readonly IQualificationRepository _qualificationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public QualificationService(
            IQualificationRepository qualificationRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IMapper mapper)
        {
            _qualificationRepository = qualificationRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QualificationDto>> GetAllAsync()
        {
            var qualifications = await _qualificationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<QualificationDto>>(qualifications);
        }

        public async Task<QualificationDto?> GetByIdAsync(int id)
        {
            var qualification = await _qualificationRepository.GetByIdAsync(id);
            return qualification != null ? _mapper.Map<QualificationDto>(qualification) : null;
        }

        public async Task<IEnumerable<QualificationDto>> GetByStudentIdAsync(int studentId)
        {
            var qualifications = await _qualificationRepository.GetByStudentIdAsync(studentId);
            return _mapper.Map<IEnumerable<QualificationDto>>(qualifications);
        }

        public async Task<IEnumerable<QualificationDto>> GetByCourseIdAsync(int courseId)
        {
            var qualifications = await _qualificationRepository.GetByCourseIdAsync(courseId);
            return _mapper.Map<IEnumerable<QualificationDto>>(qualifications);
        }

        public async Task<QualificationDto> CreateAsync(CreateQualificationDto createQualificationDto)
        {
            if (!await _studentRepository.ExistsAsync(createQualificationDto.StudentId))
            {
                throw new InvalidOperationException("El estudiante especificado no existe");
            }

            if (!await _courseRepository.ExistsAsync(createQualificationDto.CourseId))
            {
                throw new InvalidOperationException("El curso especificado no existe");
            }

            // Verificar si ya existe una calificación para este estudiante y curso
            var existingQualification = await _qualificationRepository.GetByStudentAndCourseAsync(
                createQualificationDto.StudentId, createQualificationDto.CourseId);

            if (existingQualification != null)
            {
                throw new InvalidOperationException("Ya existe una calificación para este estudiante en este curso");
            }

            var qualification = _mapper.Map<Qualification>(createQualificationDto);
            var createdQualification = await _qualificationRepository.CreateAsync(qualification);
            return _mapper.Map<QualificationDto>(createdQualification);
        }

        public async Task<QualificationDto?> UpdateAsync(int id, UpdateQualificationDto updateQualificationDto)
        {
            var existingQualification = await _qualificationRepository.GetByIdAsync(id);
            if (existingQualification == null) return null;

            _mapper.Map(updateQualificationDto, existingQualification);
            var updatedQualification = await _qualificationRepository.UpdateAsync(existingQualification);
            return _mapper.Map<QualificationDto>(updatedQualification);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _qualificationRepository.DeleteAsync(id);
        }
    }
}
