using ContactSystem.Bll.Helpers;

namespace ContactSystem.Api.Confiigurations;

public static class JwtConfigurationDI
{
    public static void ConfigureJwtSettings(this WebApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");

        var lifetime = jwtSection["Lifetime"];
        var securityKey = jwtSection["SecurityKey"];
        var audience = jwtSection["Audience"];
        var issuer = jwtSection["Issuer"];

        var jwtSettings = new JwtConfigure(issuer, audience, securityKey, lifetime);

        builder.Services.AddSingleton<JwtConfigure>(jwtSettings);
    }
}
