using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed record CreateCartCommand(int UserId, DateTime CreateDate, IEnumerable<CartItemsRequest> CartItems) : IRequest<Result<CartsResponse>>;