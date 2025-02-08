using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, PagedResult<UserResponse>>
{
    public async Task<PagedResult<UserResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersAsync(cancellationToken);

        if (users is null || !users.Any())
            return PagedResult<UserResponse>.Failure(DomainErrors.User.UsersTableIsEmpty);

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

        var totalItems = users.Count();
        var currentPage = request.Page ?? 1;
        var pageSize = request.PageSize ?? totalItems;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            if (currentPage <= 0)
                return PagedResult<UserResponse>.Failure(DomainErrors.Pagination.InvalidPage);

            if (pageSize <= 0)
                return PagedResult<UserResponse>.Failure(DomainErrors.Pagination.InvalidPageSize);

            if (currentPage > totalPages)
                return PagedResult<UserResponse>.Failure(
                    DomainErrors.Pagination.PageExceedsLimit(currentPage, totalPages));

            users = users
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        var response = users.Select(user => new UserResponse(
            user.Id,
            user.Email,
            user.UserName,
            user.Password,
            user.Name,
            user.Address,
            user.Phone,
            user.Status,
            user.Role
        ));

        return PagedResult<UserResponse>.Success(
            response,
            totalItems,
            currentPage,
            totalPages);
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