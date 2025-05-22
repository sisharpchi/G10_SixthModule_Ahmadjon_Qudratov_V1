using ContactSystem.Bll.Dtos;
using ContactSystem.Bll.Helpers;
using ContactSystem.Bll.Helpers.Security;
using ContactSystem.Core.Errors;
using ContactSystem.Dal;
using ContactSystem.Dal.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ContactSystem.Bll.Services;

public class AuthService(MainContext mainContext,IValidator<UserCreateDto> validator, ITokenService tokenService, JwtConfigure jwtConfigure, IValidator<UserCreateDto> validatorForLogin) : IAuthService
{
    private readonly int Expires = int.Parse(jwtConfigure.Lifetime);

    public async Task<long> SignUpUserAsync(UserCreateDto userCreateDto)
    {
        var validatorResult = await validator.ValidateAsync(userCreateDto);
        if (!validatorResult.IsValid)
        {
            string errorMessages = string.Join("; ", validatorResult.Errors.Select(e => e.ErrorMessage));
            throw new AuthException(errorMessages);
        }

        var tupleFromHasher = PasswordHasher.Hasher(userCreateDto.Password);
        var user = new User()
        {
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            UserName = userCreateDto.UserName,
            Email = userCreateDto.Email,
            PhoneNumber = userCreateDto.PhoneNumber,
            Password = tupleFromHasher.Hash,
            Salt = tupleFromHasher.Salt,
        };
        user.RoleId = await GetRoleIdAsync("User");

        return await InsertUserAync(user);
    }
    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto request)
    {
        ClaimsPrincipal? principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) throw new ForbiddenException("Invalid access token.");


        var userClaim = principal.FindFirst(c => c.Type == "UserId");
        var userId = long.Parse(userClaim.Value);


        var refreshToken = await SelectRefreshTokenAsync(request.RefreshToken, userId);
        if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow || refreshToken.IsRevoked)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        refreshToken.IsRevoked = true;

        var user = await SelectUserByIdAync(userId);

        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.Name,
        };

        var newAccessToken = tokenService.GenerateToken(userGetDto);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        var refreshTokenToDB = new RefreshToken()
        {
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(21),
            IsRevoked = false,
            UserId = user.UserId
        };

        await InsertRefreshTokenAsync(refreshTokenToDB);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            Expires = 24
        };
    }
    public async Task<LoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto)
    {
        var user = await SelectUserByUserNameAync(userLoginDto.UserName);

        var checkUserPassword = PasswordHasher.Verify(userLoginDto.Password, user.Password, user.Salt);

        if (checkUserPassword == false)
        {
            throw new UnauthorizedException("UserName or password incorrect");
        }

        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.Name,
        };

        var token = tokenService.GenerateToken(userGetDto);
        var refreshToken = tokenService.GenerateRefreshToken();

        var refreshTokenToDB = new RefreshToken()
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(21),
            IsRevoked = false,
            UserId = user.UserId
        };

        await InsertRefreshTokenAsync(refreshTokenToDB);

        var loginResponseDto = new LoginResponseDto()
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            Expires = 24
        };


        return loginResponseDto;
    }
    public async Task LogOutAsync(string token) => await RemoveRefreshTokenAsync(token);


    private async Task InsertRefreshTokenAsync(RefreshToken refreshToken)
    {
        await mainContext.RefreshTokens.AddAsync(refreshToken);
        await mainContext.SaveChangesAsync();
    }
    private async Task RemoveRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenToRemove = await mainContext.RefreshTokens.FindAsync(refreshToken);
        if (refreshTokenToRemove is null)
            throw new EntityNotFoundException($"Refresh token {refreshToken} not found");
        mainContext.RefreshTokens.Remove(refreshTokenToRemove);
        await mainContext.SaveChangesAsync();
    }
    private async Task<RefreshToken> SelectRefreshTokenAsync(string refreshToken, long userId)
    {
        var refreshInUser = await mainContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken && r.UserId == userId);
        return refreshInUser;
    }


    private async Task<User> SelectUserByUserNameAync(string userName)
    {
        var user = await mainContext.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.UserName == userName);
        if (user is null)
            throw new EntityNotFoundException($"Entity with {userName} not found");
        return user;
    }
    private async Task<User> SelectUserByIdAync(long id)
    {
        var user = await mainContext.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.UserId == id);
        if (user is null)
            throw new EntityNotFoundException($"Entity with {id} not found");
        return user;
    }
    private async Task<long> InsertUserAync(User user)
    {
        await mainContext.Users.AddAsync(user);
        await mainContext.SaveChangesAsync();
        return user.UserId;
    }
    private async Task<long> GetRoleIdAsync(string role)
    {
        var foundRole = await mainContext.UserRoles.FirstOrDefaultAsync(u => u.Name == role);
        if (foundRole is null)
            throw new EntityNotFoundException(role + " - not found");
        return foundRole.Id;
    }
}
