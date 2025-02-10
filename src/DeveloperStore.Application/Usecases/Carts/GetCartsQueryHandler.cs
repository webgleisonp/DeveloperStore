using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Carts;

internal sealed class GetCartsQueryHandler(ICartsRepository cartsRepository, ICartItemsRepository cartItemsRepository) : IRequestHandler<GetCartsQuery, PaginatedResult<IEnumerable<CartsResponse>>>, IPagedResultHandler<Cart>
{
    public async Task<PaginatedResult<IEnumerable<CartsResponse>>> Handle(GetCartsQuery request, CancellationToken cancellationToken)
    {
        var carts = await cartsRepository.GetCartsAsync(cancellationToken);

        if (carts is null || !carts.Any())
            return PaginatedResult.Failure<IEnumerable<CartsResponse>>(DomainErrors.Cart.CartsTableIsEmpty);

        var cartItems = await cartItemsRepository.GetCartItemsAsync(cancellationToken);

        if (cartItems is null || !cartItems.Any())
            return PaginatedResult.Failure<IEnumerable<CartsResponse>>(DomainErrors.CartItem.CartItemsTableIsEmpty);

        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            var orderParts = request.Order.Split(',');

            IOrderedEnumerable<Cart>? orderedResult = null;

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
                        ? carts.OrderByDescending(u => GetPropertyValue(u, propertyName))
                        : carts.OrderBy(u => GetPropertyValue(u, propertyName));
                }
                else
                {
                    orderedResult = isDescending
                        ? orderedResult.ThenByDescending(u => GetPropertyValue(u, propertyName))
                        : orderedResult.ThenBy(u => GetPropertyValue(u, propertyName));
                }
            }
            carts = (orderedResult is not null || orderedResult.Count() == 0) ? orderedResult.AsEnumerable() : carts;
        }

        var totalItems = carts.Count();
        var currentPage = request.Page ?? 1;
        var pageSize = request.PageSize ?? totalItems;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            if (currentPage <= 0)
                return PaginatedResult.Failure<IEnumerable<CartsResponse>>(DomainErrors.Pagination.InvalidPage);

            if (pageSize <= 0)
                return PaginatedResult.Failure<IEnumerable<CartsResponse>>(DomainErrors.Pagination.InvalidPageSize);

            if (currentPage > totalPages)
                return PaginatedResult.Failure<IEnumerable<CartsResponse>>(DomainErrors.Pagination.PageExceedsLimit(currentPage, totalPages));

            carts = carts.Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        var cartItemsResponse = cartItems.Select(ci => new CartItemsResponse(ci.CartId, ci.ProductId, ci.Quantity, ci.Price));
        var cartsResponse = carts.Select(c => new CartsResponse(c.Id, c.UserId, c.CreateDate, cartItemsResponse.Where(ci => ci.CartId == c.Id)));

        return PaginatedResult.Success(cartsResponse, totalItems, currentPage, pageSize);
    }

    public object? GetPropertyValue(Cart value, string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "id" => value.Id,
            "userId" => value.UserId,
            "createDate" => value.CreateDate,
            _ => null
        };
    }
}