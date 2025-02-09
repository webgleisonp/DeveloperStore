namespace DeveloperStore.Application.Abstractions;

internal interface IPagedResultHandler<in T> where T : class
{
    object? GetPropertyValue(T value, string propertyName);
}
