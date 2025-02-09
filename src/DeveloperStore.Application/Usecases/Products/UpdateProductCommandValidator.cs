using FluentValidation;

namespace DeveloperStore.Application.Usecases.Products;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Title).NotEmpty();
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.Category).NotEmpty();
        RuleFor(p => p.Image).NotEmpty();
    }
}