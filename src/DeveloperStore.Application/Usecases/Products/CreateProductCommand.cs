using DeveloperStore.Domain.Shared;
using DeveloperStore.Domain.ValueObjects;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record CreateProductCommand(string Title, decimal Price, string Description, string Category, string Image, Rating Rating) : IRequest<Result<ProductResponse>>;