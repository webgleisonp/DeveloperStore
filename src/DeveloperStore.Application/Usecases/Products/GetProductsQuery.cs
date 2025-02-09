using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record GetProductsQuery(int? Page = null, int? PageSize = null, string? Order = null) : IRequest<PaginatedResult<IEnumerable<ProductResponse>>>;
