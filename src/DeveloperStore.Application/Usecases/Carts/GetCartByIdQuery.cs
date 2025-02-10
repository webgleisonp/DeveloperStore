using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed record GetCartByIdQuery(int Id) : IRequest<Result<CartsResponse>>;