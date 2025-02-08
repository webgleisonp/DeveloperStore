using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

public sealed record GetUserByIdQuery(int Id) : IRequest<Result<UserResponse>>;