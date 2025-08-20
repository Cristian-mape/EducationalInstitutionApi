using EducationalInstitution.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(User? user, string? token)> LoginAsync(string email, string password);
        Task<User> RegisterAsync(string firstName, string lastName, string email, string password, Domain.Enums.UserRole role);
        Task<bool> ValidatePasswordAsync(string password, string hash);
        string HashPassword(string password);
    }
}
