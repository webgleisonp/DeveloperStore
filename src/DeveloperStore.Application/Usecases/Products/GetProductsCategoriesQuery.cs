using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record GetProductsCategoriesQuery : IRequest<Result<IEnumerable<string>>>;