using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed record DeleteCartItemCommand(int CartId, int CartItemId) : IRequest<Result>;