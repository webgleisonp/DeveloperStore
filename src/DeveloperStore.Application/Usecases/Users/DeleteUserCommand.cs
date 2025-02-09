using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

public sealed record DeleteUserCommand(int Id) : IRequest<Result<UserResponse>>;