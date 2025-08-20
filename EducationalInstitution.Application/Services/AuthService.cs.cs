using EducationalInstitution.Domain.Entities;
using EducationalInstitution.Domain.Interfaces.Repositories;
using EducationalInstitution.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<(User? user, string? token)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !ValidatePasswordAsync(password, user.PasswordHash).Result)
            {
                return (null, null);
            }

            var token = _tokenService.GenerateToken(user);
            return (user, token);
        }

        public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password, Domain.Enums.UserRole role)
        {
            // Verificar si el email ya existe
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("El email ya está registrado");
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> ValidatePasswordAsync(string password, string hash)
        {
            return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
