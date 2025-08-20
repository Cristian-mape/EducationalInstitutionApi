using AutoMapper;
using EducationalInstitution.Application.DTOs.Auth;
using EducationalInstitution.Application.DTOs.Common;
using EducationalInstitution.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalInstitution.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, ITokenService tokenService, IMapper mapper)
        {
            _authService = authService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var (user, token) = await _authService.LoginAsync(loginRequest.Email, loginRequest.Password);

                if (user == null || token == null)
                {
                    return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResult("Credenciales inválidas"));
                }

                var refreshToken = _tokenService.GenerateRefreshToken();
                var response = _mapper.Map<AuthResponseDto>(user);
                response.Token = token;
                response.RefreshToken = refreshToken;
                response.TokenExpiration = DateTime.UtcNow.AddHours(1);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResult(response, "Login exitoso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult($"Error durante el login: {ex.Message}"));
            }
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin,Coordinator")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                var user = await _authService.RegisterAsync(
                    registerRequest.FirstName,
                    registerRequest.LastName,
                    registerRequest.Email,
                    registerRequest.Password,
                    registerRequest.Role);

                var token = _tokenService.GenerateToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                var response = _mapper.Map<AuthResponseDto>(user);
                response.Token = token;
                response.RefreshToken = refreshToken;
                response.TokenExpiration = DateTime.UtcNow.AddHours(1);

                return Ok(ApiResponse<AuthResponseDto>.SuccessResult(response, "Usuario registrado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResult($"Error interno del servidor: {ex.Message}"));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _tokenService.BlacklistTokenAsync(token);
                return Ok(ApiResponse<object>.SuccessResult(null, "Sesión cerrada exitosamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult($"Error durante el logout: {ex.Message}"));
            }
        }
    }
}
