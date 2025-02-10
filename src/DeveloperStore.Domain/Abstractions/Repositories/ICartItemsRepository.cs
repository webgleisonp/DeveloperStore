
using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Abstractions.Repositories;

public interface ICartItemsRepository
{
    Task CreateCartItemAsync(CartItem cartItem, CancellationToken cancellationToken);
    Task DeleteCartItemAsync(CartItem cartItem, CancellationToken cancellationToken);
    Task<CartItem> GetCartItemByProductIdAsync(int cartId, int productId, CancellationToken cancellationToken);
    Task<IEnumerable<CartItem>> GetCartItemsAsync(CancellationToken cancellationToken);
    Task<CartItem> GetItemByIdAsync(int cartItemId, CancellationToken cancellation);
    Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(int id, CancellationToken cancellationToken);
}
