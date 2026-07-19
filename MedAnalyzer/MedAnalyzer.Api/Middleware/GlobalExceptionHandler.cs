using MedAnalyzer.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MedAnalyzer.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (status, title) = exception switch
            {
                DomainValidationException => (StatusCodes.Status400BadRequest, "Validación fallida"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "No autorizado"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message,
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem, ct);

            return true;
        }
    }
}
