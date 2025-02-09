using DeveloperStore.Domain.ValueObjects;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record ProductResponse(int Id, string Title, decimal Price, string Description, string Category, string Image, Rating Rating);
