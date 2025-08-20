using AutoMapper;
using EducationalInstitution.Application.DTOs.Students;
using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.Services
{
    public class StudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<PagedStudentsDto> GetPagedAsync(int page, int pageSize)
        {
            var students = await _studentRepository.GetPagedAsync(page, pageSize);
            var totalCount = await _studentRepository.CountAsync();

            return new PagedStudentsDto
            {
                Students = _mapper.Map<IEnumerable<StudentDto>>(students),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            return student != null ? _mapper.Map<StudentDto>(student) : null;
        }

        public async Task<StudentDto> CreateAsync(CreateStudentDto createStudentDto)
        {
            // Verificar si el código de estudiante ya existe
            if (await _studentRepository.StudentCodeExistsAsync(createStudentDto.StudentCode))
            {
                throw new InvalidOperationException("El código de estudiante ya existe");
            }

            var student = _mapper.Map<Student>(createStudentDto);
            var createdStudent = await _studentRepository.CreateAsync(student);
            return _mapper.Map<StudentDto>(createdStudent);
        }

        public async Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto updateStudentDto)
        {
            var existingStudent = await _studentRepository.GetByIdAsync(id);
            if (existingStudent == null) return null;

            _mapper.Map(updateStudentDto, existingStudent);
            var updatedStudent = await _studentRepository.UpdateAsync(existingStudent);
            return _mapper.Map<StudentDto>(updatedStudent);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _studentRepository.DeleteAsync(id);
        }
    }
}
