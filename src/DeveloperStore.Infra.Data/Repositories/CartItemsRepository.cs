using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Infra.Data.Repositories;

internal sealed class CartItemsRepository(DeveloperStoreDbContext dbContext) : ICartItemsRepository
{
    public async Task CreateCartItemAsync(CartItem cartItem, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(cartItem, cancellationToken);
    }

    public Task DeleteCartItemAsync(CartItem cartItem, CancellationToken cancellationToken)
    {
        dbContext.Remove(cartItem);

        return Task.CompletedTask;
    }

    public async Task<CartItem> GetCartItemByProductIdAsync(int cartId, int productId, CancellationToken cancellationToken)
    {
        var data = await dbContext.CartItems.SingleOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId, cancellationToken);

        return data!;
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(CancellationToken cancellationToken)
    {
        var data = await dbContext.CartItems.ToListAsync(cancellationToken);

        return data!;
    }

    public async Task<CartItem> GetItemByIdAsync(int cartItemId, CancellationToken cancellation)
    {
        var data = await dbContext.CartItems.SingleOrDefaultAsync(ci => ci.Id == cartItemId, cancellation);

        return data!;
    }

    public async Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(int id, CancellationToken cancellationToken)
    {
        var data = await dbContext.CartItems.Where(ci => ci.CartId == id).ToListAsync(cancellationToken);

        return data!;
    }
}
