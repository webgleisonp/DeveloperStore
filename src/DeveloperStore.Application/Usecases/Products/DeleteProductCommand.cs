using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

public sealed record DeleteProductCommand(int Id) : IRequest<Result>;