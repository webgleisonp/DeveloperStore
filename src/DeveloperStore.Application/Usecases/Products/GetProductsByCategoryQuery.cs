using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record GetProductsByCategoryQuery(string Category) : IRequest<Result<IEnumerable<ProductResponse>>>;