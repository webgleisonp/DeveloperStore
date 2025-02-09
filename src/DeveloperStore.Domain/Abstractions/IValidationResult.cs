using DeveloperStore.Domain.Shared;

namespace DeveloperStore.Domain.Abstractions;

internal interface IValidationResult
{
    public static readonly Error ValidationError = new("ValidationError", "An error occurred during execution");

    Error[] Errors { get; }
}