using DeveloperStore.Application.Abstractions;
using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using DeveloperStore.Domain.Errors;
using DeveloperStore.Domain.Shared;
using MediatR;

namespace DeveloperStore.Application.Usecases.Products;

internal sealed class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PaginatedResult<IEnumerable<ProductResponse>>>, IPagedResultHandler<Product>
{
    public async Task<PaginatedResult<IEnumerable<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsAsync(cancellationToken);

        if (products is null || !products.Any())
            return PaginatedResult.Failure<IEnumerable<ProductResponse>>(DomainErrors.Product.ProductsTableIsEmpty);

        if (!string.IsNullOrWhiteSpace(request.Order))
        {
            var orderParts = request.Order.Split(',');

            IOrderedEnumerable<Product>? orderedResult = null;

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
                        ? products.OrderByDescending(u => GetPropertyValue(u, propertyName))
                        : products.OrderBy(u => GetPropertyValue(u, propertyName));
                }
                else
                {
                    orderedResult = isDescending
                        ? orderedResult.ThenByDescending(u => GetPropertyValue(u, propertyName))
                        : orderedResult.ThenBy(u => GetPropertyValue(u, propertyName));
                }
            }

            products = (orderedResult is not null || orderedResult.Count() == 0) ? orderedResult.AsEnumerable() : products;
        }

        var totalItems = products.Count();
        var currentPage = request.Page ?? 1;
        var pageSize = request.PageSize ?? totalItems;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            if (currentPage <= 0)
                return PaginatedResult.Failure<IEnumerable<ProductResponse>>(DomainErrors.Pagination.InvalidPage);

            if (pageSize <= 0)
                return PaginatedResult.Failure<IEnumerable<ProductResponse>>(DomainErrors.Pagination.InvalidPageSize);

            if (currentPage > totalPages)
                return PaginatedResult.Failure<IEnumerable<ProductResponse>>(DomainErrors.Pagination.PageExceedsLimit(currentPage, totalPages));

            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
        }

        var response = products.Select(p => new ProductResponse(p.Id, p.Title, p.Price, p.Description, p.Category, p.Image, p.Rating));

        return PaginatedResult.Success(response, totalItems, currentPage, totalPages);
    }

    public object? GetPropertyValue(Product value, string propertyName)
    {
        return propertyName.ToLower() switch
        {
            "title" => value.Title,
            "price" => value.Price,
            "category" => value.Category,
            "image" => value.Image,
            "rate" => value.Rating.Rate,
            _ => null
        };
    }
}