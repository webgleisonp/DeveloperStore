using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Authentication;

public sealed record AuthCommand(string UserName, string Password) : IRequest<Result<AuthResponse>>;
