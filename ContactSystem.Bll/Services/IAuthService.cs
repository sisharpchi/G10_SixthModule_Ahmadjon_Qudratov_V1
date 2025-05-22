using ContactSystem.Bll.Dtos;

namespace ContactSystem.Bll.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto request);
    Task<long> SignUpUserAsync(UserCreateDto userCreateDto);
    Task LogOutAsync(string token);
}