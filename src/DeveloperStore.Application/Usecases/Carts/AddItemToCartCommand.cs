using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed record AddItemToCartCommand(int CartId, int ProductId, int Quantity, decimal ItemPrice) : IRequest<Result<CartsResponse>>;