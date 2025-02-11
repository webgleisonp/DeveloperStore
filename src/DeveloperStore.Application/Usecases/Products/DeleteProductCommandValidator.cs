using FluentValidation;

namespace DeveloperStore.Application.Usecases.Products;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
    }
}