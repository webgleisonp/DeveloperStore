using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;
public sealed record GetUsersQuery(
    int? Page = null,
    int? PageSize = null,
    string? Order = null) : IRequest<PagedResult<UserResponse>>;
