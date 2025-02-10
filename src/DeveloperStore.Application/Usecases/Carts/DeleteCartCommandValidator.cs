using FluentValidation;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed class DeleteCartCommandValidator : AbstractValidator<DeleteCartCommand>
{
    public DeleteCartCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
    }
}