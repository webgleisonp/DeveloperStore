
using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Abstractions.Repositories;

public interface IProductRepository
{
    Task CreateProductsAsync(Product newProduct, CancellationToken cancellationToken);
    Task DeleteProductAsync(Product productExists, CancellationToken cancellationToken);
    Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken);
    Task<Product> GetProductsByTitleAsync(string title, CancellationToken cancellationToken);
    Task UpdateProductAsync(Product productExists, CancellationToken cancellationToken);
}
