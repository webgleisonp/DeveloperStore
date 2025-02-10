using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed record GetCartsQuery(int? Page = null, int? PageSize = null, string? Order = null) : IRequest<PaginatedResult<IEnumerable<CartsResponse>>>;