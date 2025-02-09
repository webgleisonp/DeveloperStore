using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Infra.Data.Repositories;

internal sealed class ProductRepository(DeveloperStoreDbContext dbContext) : IProductRepository
{
    public async Task CreateProductsAsync(Product newProduct, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(newProduct, cancellationToken);
    }

    public Task DeleteProductAsync(Product productExists, CancellationToken cancellationToken)
    {
        dbContext.Remove(productExists);

        return Task.CompletedTask;
    }

    public async Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var data = await dbContext.Products.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

        return data!;
    }

    public Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<Product>>(dbContext.Products);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken)
    {
        var data = await dbContext.Products.Where(p => p.Category.ToLower() == category.ToLower()).ToListAsync();

        return data;
    }

    public async Task<Product> GetProductsByTitleAsync(string title, CancellationToken cancellationToken)
    {
        var data = await dbContext.Products.SingleOrDefaultAsync(p => p.Title == title, cancellationToken);

        return data!;
    }

    public Task UpdateProductAsync(Product productExists, CancellationToken cancellationToken)
    {
        dbContext.Update(productExists);

        return Task.CompletedTask;
    }
}
