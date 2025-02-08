using DeveloperStore.Domain.Abstractions.Repositories;
using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeveloperStore.Infra.Data.Repositories;

internal sealed class UserRepository(DeveloperStoreDbContext dbContext) : IUserRepository
{
    public async Task CreateNewUserAsync(User newUser, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(newUser, cancellationToken);
    }

    public Task DeleteUserAsync(User userExists, CancellationToken cancellationToken)
    {
        dbContext.Remove(userExists);

        return Task.CompletedTask;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var data = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        return data!;
    }

    public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var data = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        return data!;
    }

    public async Task<User> GetUserByUserNameAndPasswordAsync(string userName, string password, CancellationToken cancellationToken)
    {
        var data = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == userName && u.Password == password, cancellationToken);

        return data!;
    }

    public Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<User>>(dbContext.Users);
    }

    public Task UpdateUserAsync(User userExists, CancellationToken cancellationToken)
    {
        dbContext.Update(userExists);

        return Task.CompletedTask;
    }
}
