using DeveloperStore.Application.Abstractions;

namespace DeveloperStore.Infra.Data;

internal sealed class UnityOfWork(DeveloperStoreDbContext dbContext) : IUnityOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}