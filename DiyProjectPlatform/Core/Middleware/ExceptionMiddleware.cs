using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Exceptions;
using System.Text.Json;

namespace Core.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IHostEnvironment env)
    {
        _next = next;
        _serviceProvider = serviceProvider;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            var logService = context.RequestServices.GetRequiredService<ILogService>();
            await logService.AddLogAsync(ex.Message, Shared.Enums.LogLevel.Warning);

            context.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ForbiddenException or PermissionDeniedException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                BadRequestException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

            await WriteErrorResponse(context, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            var logService = context.RequestServices.GetRequiredService<ILogService>();
            await logService.AddLogAsync("Unauthorized access attempt", Shared.Enums.LogLevel.Warning);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await WriteErrorResponse(context, "Unauthorized access");
        }
        catch (Exception ex)
        {
            var logService = context.RequestServices.GetRequiredService<ILogService>();
            await logService.AddLogAsync("Unhandled exception: " + ex.Message, Shared.Enums.LogLevel.Error);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred";
            await WriteErrorResponse(context, message);
        }
    }

    private async Task WriteErrorResponse(HttpContext context, string message)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }
        else
        {
            var statusCode = context.Response.StatusCode;
            context.Response.Redirect($"/Error/{statusCode}");
        }
    }
}
