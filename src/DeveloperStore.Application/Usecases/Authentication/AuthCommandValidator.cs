using FluentValidation;

namespace DeveloperStore.Application.Usecases.Authentication;

public sealed class AuthCommandValidator : AbstractValidator<AuthCommand>
{
    public AuthCommandValidator()
    {
        RuleFor(p => p.UserName).NotEmpty();
        RuleFor(p => p.Password).NotEmpty();
    }
}
