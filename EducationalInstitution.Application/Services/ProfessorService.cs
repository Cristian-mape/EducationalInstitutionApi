using AutoMapper;
using EducationalInstitution.Application.DTOs.Professors;
using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.Services
{
    public class ProfessorService
    {
        private readonly IProfessorRepository _professorRepository;
        private readonly IMapper _mapper;

        public ProfessorService(IProfessorRepository professorRepository, IMapper mapper)
        {
            _professorRepository = professorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProfessorDto>> GetAllAsync()
        {
            var professors = await _professorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProfessorDto>>(professors);
        }

        public async Task<ProfessorDto?> GetByIdAsync(int id)
        {
            var professor = await _professorRepository.GetByIdAsync(id);
            return professor != null ? _mapper.Map<ProfessorDto>(professor) : null;
        }

        public async Task<ProfessorDto> CreateAsync(CreateProfessorDto createProfessorDto)
        {
            if (await _professorRepository.EmployeeCodeExistsAsync(createProfessorDto.EmployeeCode))
            {
                throw new InvalidOperationException("El código de empleado ya existe");
            }

            var professor = _mapper.Map<Professor>(createProfessorDto);
            var createdProfessor = await _professorRepository.CreateAsync(professor);
            return _mapper.Map<ProfessorDto>(createdProfessor);
        }

        public async Task<ProfessorDto?> UpdateAsync(int id, UpdateProfessorDto updateProfessorDto)
        {
            var existingProfessor = await _professorRepository.GetByIdAsync(id);
            if (existingProfessor == null) return null;

            _mapper.Map(updateProfessorDto, existingProfessor);
            var updatedProfessor = await _professorRepository.UpdateAsync(existingProfessor);
            return _mapper.Map<ProfessorDto>(updatedProfessor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _professorRepository.DeleteAsync(id);
        }
    }
}
