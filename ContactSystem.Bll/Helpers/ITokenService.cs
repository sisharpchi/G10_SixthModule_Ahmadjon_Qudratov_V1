using ContactSystem.Bll.Dtos;
using System.Security.Claims;

namespace ContactSystem.Bll.Helpers;

public interface ITokenService
{
    public string GenerateToken(UserGetDto user);
    public string GenerateRefreshToken();
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}