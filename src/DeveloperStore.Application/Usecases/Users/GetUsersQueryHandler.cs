using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Users;

internal sealed class GetUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, PaginatedResult<IEnumerable<UserResponse>>>, IPagedResultHandler<User>
{
    public async Task<PaginatedResult<IEnumerable<UserResponse>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersAsync(cancellationToken);

        if (users is null || !users.Any())
            return PaginatedResult.Failure<IEnumerable<UserResponse>>(DomainErrors.User.UsersTableIsEmpty);

        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            var orderParts = request.Order.Split(',');

            IOrderedEnumerable<User>? orderedResult = null;

            foreach (var part in orderParts)
            {
                var trimmedPart = part.Trim();
                var isDescending = trimmedPart.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);
                var propertyName = trimmedPart
                    .Replace(" asc", "", StringComparison.OrdinalIgnoreCase)
                    .Replace(" desc", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                if (orderedResult == null)
                {
                    orderedResult = isDescending
                        ? users.OrderByDescending(u => GetPropertyValue(u, propertyName))
                        : users.OrderBy(u => GetPropertyValue(u, propertyName));
                }
                else
                {
                    orderedResult = isDescending
                        ? orderedResult.ThenByDescending(u => GetPropertyValue(u, propertyName))
                        : orderedResult.ThenBy(u => GetPropertyValue(u, propertyName));
                }
            }

            users = (orderedResult is not null || orderedResult.Count() == 0) ? orderedResult.AsEnumerable() : users;
        }

        var totalItems = users.Count();
        var currentPage = request.Page ?? 1;
        var pageSize = request.PageSize ?? totalItems;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            if (currentPage <= 0)
                return PaginatedResult.Failure<IEnumerable<UserResponse>>(DomainErrors.Pagination.InvalidPage);

            if (pageSize <= 0)
                return PaginatedResult.Failure<IEnumerable<UserResponse>>(DomainErrors.Pagination.InvalidPageSize);

            if (currentPage > totalPages)
                return PaginatedResult.Failure<IEnumerable<UserResponse>>(DomainErrors.Pagination.PageExceedsLimit(currentPage, totalPages));

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

        return PaginatedResult.Success(response, totalItems, currentPage, pageSize);
    }

    public object? GetPropertyValue(User value, string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "email" => value.Email,
            "password" => value.Password,
            "name" => value.Name.FirstName,
            "address" => value.Address.Street,
            "phone" => value.Phone,
            "status" => value.Status,
            "role" => value.Role,
            _ => null
        };
    }
}