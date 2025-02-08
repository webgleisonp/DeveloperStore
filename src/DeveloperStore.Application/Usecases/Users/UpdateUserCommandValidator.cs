using DeveloperStore.Application.GlobalValidators;
using FluentValidation;

namespace DeveloperStore.Application.Usecases.Users;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(p => p.Password)
            .NotEmpty()
            .MinimumLength(8);
        RuleFor(p => p.Name)
            .NotNull()
            .SetValidator(new NameValidator());
        RuleFor(p => p.Address)
            .NotNull()
            .SetValidator(new AddressValidator());
        RuleFor(p => p.Phone)
            .NotEmpty();
        RuleFor(p => p.Status)
            .NotEmpty();
        RuleFor(p => p.Role)
            .NotEmpty();
    }
}
