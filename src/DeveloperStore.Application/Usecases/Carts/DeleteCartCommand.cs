using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

public sealed record DeleteCartCommand(int Id) : IRequest<Result>;