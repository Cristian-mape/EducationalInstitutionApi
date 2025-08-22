using EducationalInstitution.Application.DTOs.Common;
using EducationalInstitution.Application.DTOs.Students;
using EducationalInstitution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalInstitution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinator,Professor")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudentDto>>>> GetAll()
        {
            try
            {
                var students = await _studentService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<StudentDto>>.SuccessResult(students));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<StudentDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        // Obtener estudiantes paginados
        [HttpGet("paged")]
        [Authorize(Roles = "Admin,Coordinator,Professor")]
        public async Task<ActionResult<ApiResponse<PagedStudentsDto>>> GetPaged([FromQuery] PaginationRequest request)
        {
            try
            {
                request.Validate();
                var students = await _studentService.GetPagedAsync(request.Page, request.PageSize);
                return Ok(ApiResponse<PagedStudentsDto>.SuccessResult(students));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PagedStudentsDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(int id)
        {
            try
            {
                var student = await _studentService.GetByIdAsync(id);
                if (student == null)
                {
                    return NotFound(ApiResponse<StudentDto>.ErrorResult("Estudiante no encontrado"));
                }
                return Ok(ApiResponse<StudentDto>.SuccessResult(student));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<StudentDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Create([FromBody] CreateStudentDto createStudentDto)
        {
            try
            {
                var student = await _studentService.CreateAsync(createStudentDto);
                return CreatedAtAction(nameof(GetById), new { id = student.Id },
                    ApiResponse<StudentDto>.SuccessResult(student, "Estudiante creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<StudentDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<StudentDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> Update(int id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            try
            {
                var student = await _studentService.UpdateAsync(id, updateStudentDto);
                if (student == null)
                {
                    return NotFound(ApiResponse<StudentDto>.ErrorResult("Estudiante no encontrado"));
                }
                return Ok(ApiResponse<StudentDto>.SuccessResult(student, "Estudiante actualizado exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<StudentDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _studentService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Estudiante no encontrado"));
                }
                return Ok(ApiResponse<object>.SuccessResult(null, "Estudiante eliminado exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }
    }
}
