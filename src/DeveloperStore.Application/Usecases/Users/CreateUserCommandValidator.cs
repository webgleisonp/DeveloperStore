using DeveloperStore.Application.GlobalValidators;
using DeveloperStore.Domain.Enums;
using FluentValidation;

namespace DeveloperStore.Application.Usecases.Users;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(p => p.UserName)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(12);
        RuleFor(p => p.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(12);
        RuleFor(p => p.Name)
            .NotNull()
            .SetValidator(new NameValidator());
        RuleFor(p => p.Address)
            .NotNull()
            .SetValidator(new AddressValidator());
        RuleFor(p => p.Phone)
            .NotEmpty();
        RuleFor(p => p.Status)
            .IsInEnum();
        RuleFor(p => p.Role)
            .IsInEnum();
    }
}
