using EducationalInstitution.Application.DTOs.Common;
using EducationalInstitution.Application.DTOs.Professors;
using EducationalInstitution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalInstitution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProfessorsController : ControllerBase
    {
        private readonly ProfessorService _professorService;

        public ProfessorsController(ProfessorService professorService)
        {
            _professorService = professorService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProfessorDto>>>> GetAll()
        {
            try
            {
                var professors = await _professorService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<ProfessorDto>>.SuccessResult(professors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<ProfessorDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProfessorDto>>> GetById(int id)
        {
            try
            {
                var professor = await _professorService.GetByIdAsync(id);
                if (professor == null)
                {
                    return NotFound(ApiResponse<ProfessorDto>.ErrorResult("Profesor no encontrado"));
                }
                return Ok(ApiResponse<ProfessorDto>.SuccessResult(professor, "Profesor actualizado exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProfessorDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _professorService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Profesor no encontrado"));
                }
                return Ok(ApiResponse<object>.SuccessResult(null, "Profesor eliminado exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }
    }
}
