using ContactSystem.Api.Confiigurations;
using ContactSystem.Api.Endpoints;
using ContactSystem.Api.Middlewares;

namespace ContactSystem.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Configuration();
        builder.ConfigurationJwtAuth();
        builder.ConfigureJwtSettings();
        builder.ConfigureSerilog();
        builder.Services.ConfigureDependecies();
        builder.Services.AddMemoryCache();
        builder.Services.AddResponseCaching();
        builder.Services.AddLogging();

        builder.Configuration();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<NightBlockMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapAuthEndpoints();
        app.MapContactEndpoints();
        app.MapRoleEndpoints();
        app.MapUserEndpoints();

        app.MapControllers();

        app.Run();
    }
}
