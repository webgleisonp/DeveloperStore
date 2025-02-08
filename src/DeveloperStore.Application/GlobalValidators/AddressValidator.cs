using DeveloperStore.Domain.ValueObjects;
using FluentValidation;

namespace DeveloperStore.Application.GlobalValidators;

public sealed class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(p => p.City).NotEmpty();
        RuleFor(p => p.Street).NotEmpty();
        RuleFor(p => p.Number).NotEmpty();
        RuleFor(p => p.PostCode).NotEmpty();
    }
}