using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponse>>;
