using DeveloperStore.Domain.Entities;

namespace DeveloperStore.Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task CreateNewUserAsync(User newUser, CancellationToken cancellationToken);
    Task DeleteUserAsync(User userExists, CancellationToken cancellationToken);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);
    Task UpdateUserAsync(User userExists, CancellationToken cancellationToken);
}
