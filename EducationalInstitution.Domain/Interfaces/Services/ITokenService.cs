using EducationalInstitution.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        Task<string?> ValidateTokenAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task BlacklistTokenAsync(string token);
    }
}
