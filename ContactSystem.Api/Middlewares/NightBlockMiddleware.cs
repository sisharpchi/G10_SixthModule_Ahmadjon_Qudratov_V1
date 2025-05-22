namespace ContactSystem.Api.Middlewares;

public class NightBlockMiddleware
{
    private readonly RequestDelegate _next;

    public NightBlockMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var currentHour = DateTime.Now.Hour;

        if (currentHour <= 9 || currentHour >= 18)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "The API is available only from 9:00 to 18:00. Please try again during working hours."
            });

            return;
        }

        await _next(context);
    }
}
