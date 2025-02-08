namespace DeveloperStore.Application.Abstractions;

public interface IUnityOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}