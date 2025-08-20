using EducationalInstitution.Application.DTOs.Common;
using EducationalInstitution.Application.DTOs.Qualifications;
using EducationalInstitution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalInstitution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class QualificationsController : ControllerBase
    {
        private readonly QualificationService _qualificationService;

        public QualificationsController(QualificationService qualificationService)
        {
            _qualificationService = qualificationService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QualificationDto>>>> GetAll()
        {
            try
            {
                var qualifications = await _qualificationService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<QualificationDto>>.SuccessResult(qualifications));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QualificationDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<QualificationDto>>> GetById(int id)
        {
            try
            {
                var qualification = await _qualificationService.GetByIdAsync(id);
                if (qualification == null)
                {
                    return NotFound(ApiResponse<QualificationDto>.ErrorResult("Calificación no encontrada"));
                }
                return Ok(ApiResponse<QualificationDto>.SuccessResult(qualification));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QualificationDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QualificationDto>>>> GetByStudentId(int studentId)
        {
            try
            {
                var qualifications = await _qualificationService.GetByStudentIdAsync(studentId);
                return Ok(ApiResponse<IEnumerable<QualificationDto>>.SuccessResult(qualifications));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QualificationDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Admin,Coordinator,Professor")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QualificationDto>>>> GetByCourseId(int courseId)
        {
            try
            {
                var qualifications = await _qualificationService.GetByCourseIdAsync(courseId);
                return Ok(ApiResponse<IEnumerable<QualificationDto>>.SuccessResult(qualifications));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QualificationDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Coordinator,Professor")]
        public async Task<ActionResult<ApiResponse<QualificationDto>>> Create([FromBody] CreateQualificationDto createQualificationDto)
        {
            try
            {
                var qualification = await _qualificationService.CreateAsync(createQualificationDto);
                return CreatedAtAction(nameof(GetById), new { id = qualification.Id },
                    ApiResponse<QualificationDto>.SuccessResult(qualification, "Calificación creada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<QualificationDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QualificationDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Coordinator,Professor")]
        public async Task<ActionResult<ApiResponse<QualificationDto>>> Update(int id, [FromBody] UpdateQualificationDto updateQualificationDto)
        {
            try
            {
                var qualification = await _qualificationService.UpdateAsync(id, updateQualificationDto);
                if (qualification == null)
                {
                    return NotFound(ApiResponse<QualificationDto>.ErrorResult("Calificación no encontrada"));
                }
                return Ok(ApiResponse<QualificationDto>.SuccessResult(qualification, "Calificación actualizada exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QualificationDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _qualificationService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Calificación no encontrada"));
                }
                return Ok(ApiResponse<object>.SuccessResult(null, "Calificación eliminada exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }
    }
}
