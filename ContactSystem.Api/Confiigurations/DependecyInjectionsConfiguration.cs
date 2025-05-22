using ContactSystem.Bll.Dtos;
using ContactSystem.Bll.Helpers;
using ContactSystem.Bll.Services;
using ContactSystem.Bll.Validators;
using FluentValidation;

namespace ContactSystem.Api.Confiigurations;

public static class DependecyInjectionsConfiguration
{
    public static void ConfigureDependecies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserLoginDto>, UserLoginDtoValidator>();
        services.AddScoped<IValidator<ContactCreateDto>, ContactCreateDtoValidator>();
        services.AddScoped<IValidator<ContactDto>, ContactDtoValidator>();
    }
}