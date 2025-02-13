using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Errors;
using FluentValidation;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed class CreateCartCommandValidator : AbstractValidator<CreateCartCommand>
{
    public CreateCartCommandValidator(IUserRepository userRepository, IProductRepository productRepository)
    {
        RuleFor(p => p.UserId)
            .MustAsync(async (userId, cancelation) =>
            {
                var user = await userRepository.GetUserByIdAsync(userId, cancelation);

                return user is not null;
            })
            .WithErrorCode(DomainErrors.User.UserNotFound.Code)
            .WithMessage(DomainErrors.User.UserNotFound.Message);
        RuleFor(p => p.CreateDate).NotEmpty();
        RuleFor(p => p.CartItems)
            .NotEmpty();

        RuleForEach(p => p.CartItems)
            .NotNull()
            .SetValidator(new CartItemsRequestValidator(productRepository));
    }
}