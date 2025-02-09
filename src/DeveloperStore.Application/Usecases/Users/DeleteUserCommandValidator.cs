using FluentValidation;

namespace DeveloperStore.Application.Usecases.Users;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}