using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Abstractions.Repositories;

public interface ICartsRepository
{
    Task CreateCartAsync(Cart cart, CancellationToken cancellationToken);
    Task DeleteCartAsync(Cart cart, CancellationToken cancellationToken);
    Task<Cart> GetCartByIdAsync(int id, CancellationToken cancellationToken);
    Task<Cart> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<Cart>> GetCartsAsync(CancellationToken cancellationToken);
}
