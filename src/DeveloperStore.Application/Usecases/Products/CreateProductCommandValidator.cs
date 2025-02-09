using FluentValidation;

namespace DeveloperStore.Application.Usecases.Products;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Title).NotEmpty();
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.Category).NotEmpty();
        RuleFor(p => p.Image).NotEmpty();
    }
}