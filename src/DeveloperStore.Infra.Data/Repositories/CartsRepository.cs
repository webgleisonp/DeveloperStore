using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Infra.Data.Repositories;

internal sealed class CartsRepository(DeveloperStoreDbContext dbContext) : ICartsRepository
{
    public async Task CreateCartAsync(Cart cart, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(cart, cancellationToken);
    }

    public Task DeleteCartAsync(Cart cart, CancellationToken cancellationToken)
    {
        dbContext.Remove(cart);

        return Task.CompletedTask;
    }

    public async Task<Cart> GetCartByIdAsync(int id, CancellationToken cancellationToken)
    {
        var data = await dbContext.Carts.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

        return data!;
    }

    public async Task<Cart> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var data = await dbContext.Carts.SingleOrDefaultAsync(c => c.UserId == userId && c.Active, cancellationToken);

        return data!;
    }

    public async Task<IEnumerable<Cart>> GetCartsAsync(CancellationToken cancellationToken)
    {
        var data = await dbContext.Carts.ToListAsync(cancellationToken);

        return data;
    }
}
