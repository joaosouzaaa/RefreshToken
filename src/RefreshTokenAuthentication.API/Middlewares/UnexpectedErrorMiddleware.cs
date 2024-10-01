using RefreshTokenAuthentication.API.Settings.NotificationSettings;
using System.Net.Mime;

namespace RefreshTokenAuthentication.API.Middlewares;

internal sealed class UnexpectedErrorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = new List<Notification>()
            {
                new("UnexpectedError",
                    "An unexpected error happened")
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
