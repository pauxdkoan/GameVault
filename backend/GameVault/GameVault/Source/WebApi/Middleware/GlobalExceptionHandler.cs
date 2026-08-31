using GameVault.Source.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Source.WebApi.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException &&
            httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("La solicitud fue cancelada por el cliente.");

            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.StatusCode = 499;
            }

            return true;
        }

        logger.LogError(
            exception,
            "Unhandled exception occurred: {Message}",
            exception.Message);

        var problemDetails = exception switch
        {
            ApiException apiException =>
                CreateApiProblemDetails(apiException),

            UnauthorizedAccessException =>
                new ProblemDetails
                {
                    Type = "Unauthorized",
                    Title = "No autorizado",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = exception.Message
                },

            _ =>
                new ProblemDetails
                {
                    Type = "InternalServerError",
                    Title = "Error interno del servidor",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Ocurrió un error inesperado."
                }
        };

        httpContext.Response.StatusCode =
            problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    private static ProblemDetails CreateApiProblemDetails(
        ApiException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Type = GetProblemType(exception.StatusCode),
            Title = GetProblemTitle(exception.StatusCode),
            Status = exception.StatusCode,
            Detail = exception.Message
        };

        if (exception.Errors is not null)
        {
            problemDetails.Extensions["errors"] = exception.Errors;
        }

        return problemDetails;
    }

    private static string GetProblemTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Solicitud inválida",
            StatusCodes.Status401Unauthorized => "No autorizado",
            StatusCodes.Status403Forbidden => "Acceso prohibido",
            StatusCodes.Status404NotFound => "Recurso no encontrado",
            StatusCodes.Status409Conflict => "Conflicto",
            _ => "Error interno del servidor"
        };
    }

    private static string GetProblemType(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "BadRequest",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "NotFound",
            StatusCodes.Status409Conflict => "Conflict",
            _ => "InternalServerError"
        };
    }
}
