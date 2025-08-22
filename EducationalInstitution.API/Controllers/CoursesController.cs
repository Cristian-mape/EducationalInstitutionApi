using EducationalInstitution.Application.DTOs.Common;
using EducationalInstitution.Application.DTOs.Courses;
using EducationalInstitution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalInstitution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CoursesController(CourseService courseService)
        {
            _courseService = courseService;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseDto>>>> GetAll()
        {
            try
            {
                var courses = await _courseService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<CourseDto>>.SuccessResult(courses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<CourseDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> GetById(int id)
        {
            try
            {
                var course = await _courseService.GetByIdAsync(id);
                if (course == null)
                {
                    return NotFound(ApiResponse<CourseDto>.ErrorResult("Curso no encontrado"));
                }
                return Ok(ApiResponse<CourseDto>.SuccessResult(course));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CourseDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpGet("professor/{professorId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseDto>>>> GetByProfessorId(int professorId)
        {
            try
            {
                var courses = await _courseService.GetByProfessorIdAsync(professorId);
                return Ok(ApiResponse<IEnumerable<CourseDto>>.SuccessResult(courses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<CourseDto>>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Create([FromBody] CreateCourseDto createCourseDto)
        {
            try
            {
                var course = await _courseService.CreateAsync(createCourseDto);
                return CreatedAtAction(nameof(GetById), new { id = course.Id },
                    ApiResponse<CourseDto>.SuccessResult(course, "Curso creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CourseDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<CourseDto>>> Update(int id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            try
            {
                var course = await _courseService.UpdateAsync(id, updateCourseDto);
                if (course == null)
                {
                    return NotFound(ApiResponse<CourseDto>.ErrorResult("Curso no encontrado"));
                }
                return Ok(ApiResponse<CourseDto>.SuccessResult(course, "Curso actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CourseDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CourseDto>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _courseService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Curso no encontrado"));
                }
                return Ok(ApiResponse<object>.SuccessResult(null, "Curso eliminado exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult($"Error interno: {ex.Message}"));
            }
        }
    }
}
