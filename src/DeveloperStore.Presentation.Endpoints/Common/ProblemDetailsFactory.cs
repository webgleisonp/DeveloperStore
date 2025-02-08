using DeveloperStore.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class ProblemDetailsFactory
{
    public static ProblemDetails CreateValidationProblemDetails(
        Error[] errors,
        string instance,
        string? title = null,
        int? statusCode = null,
        string? type = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ?? StatusCodes.Status400BadRequest,
            Title = title ?? "Validation Error",
            Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = string.Join("; ", errors.Select(e => e.Message)),
            Instance = instance
        };

        problemDetails.Extensions["errors"] = errors.Select(e => new 
        {
            code = e.Code,
            message = e.Message
        }).ToArray();

        return problemDetails;
    }
} 