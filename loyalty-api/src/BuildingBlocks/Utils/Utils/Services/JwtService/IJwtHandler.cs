using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Utils
{
    public interface IJwtHandler
    {
        string SecretKey { get; set; }
        bool IsTokenValid(string token, TokenValidationParameters tokenValidationParameters);
        string GenerateToken(IJwtContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}