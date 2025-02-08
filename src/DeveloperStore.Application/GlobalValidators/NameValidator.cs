using DeveloperStore.Domain.ValueObjects;
using FluentValidation;

namespace DeveloperStore.Application.GlobalValidators;

public sealed class NameValidator : AbstractValidator<Name>
{
    public NameValidator()
    {
        RuleFor(p => p.FirstName).NotEmpty();
        RuleFor(p => p.LastName).NotEmpty();
        RuleFor(p => p.LastName).NotEqual(p => p.FirstName);
    }
}