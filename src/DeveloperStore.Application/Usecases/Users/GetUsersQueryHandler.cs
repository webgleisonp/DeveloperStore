using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserResponse>>>
{
    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersAsync(cancellationToken);

        if (users is null || users.Count() == 0)
            return Result.Failure<IEnumerable<UserResponse>>(DomainErrors.User.UsersTableIsEmpty);

        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            var orderParts = request.Order.Split(',');

            IOrderedEnumerable<User>? orderedUsers = null;

            foreach (var part in orderParts)
            {
                var trimmedPart = part.Trim();
                var isDescending = trimmedPart.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);
                var propertyName = trimmedPart
                    .Replace(" asc", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(" desc", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                if (orderedUsers == null)
                {
                    orderedUsers = isDescending
                        ? users.OrderByDescending(u => GetPropertyValue(u, propertyName))
                        : users.OrderBy(u => GetPropertyValue(u, propertyName));
                }
                else
                {
                    orderedUsers = isDescending
                        ? orderedUsers.ThenByDescending(u => GetPropertyValue(u, propertyName))
                        : orderedUsers.ThenBy(u => GetPropertyValue(u, propertyName));
                }
            }

            users = (orderedUsers is not null || orderedUsers.Count() == 0) ? orderedUsers.AsEnumerable() : users;
        }

        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            if (request.Page.Value <= 0)
                return Result.Failure<IEnumerable<UserResponse>>(DomainErrors.Pagination.InvalidPage);

            if (request.PageSize.Value <= 0)
                return Result.Failure<IEnumerable<UserResponse>>(DomainErrors.Pagination.InvalidPageSize);

            var totalItems = users.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize.Value);

            if (request.Page.Value > totalPages)
                return Result.Failure<IEnumerable<UserResponse>>(
                    DomainErrors.Pagination.PageExceedsLimit(request.Page.Value, totalPages));

            users = users
                .Skip((request.Page.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value);
        }

        var response = users.Select(user => new UserResponse(
            user.Id,
            user.Email,
            user.Password,
            user.Name,
            user.Address,
            user.Phone,
            user.Status,
            user.Role
        ));

        return Result.Success(response);
    }

    private static object? GetPropertyValue(User user, string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "email" => user.Email,
            "password" => user.Password,
            "name" => user.Name.FirstName,
            "address" => user.Address.Street,
            "phone" => user.Phone,
            "status" => user.Status,
            "role" => user.Role,
            _ => null
        };
    }
}