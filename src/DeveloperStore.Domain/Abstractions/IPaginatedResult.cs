namespace DeveloperStore.Domain.Abstractions;

internal interface IPaginatedResult
{
    int TotalRecords { get; }
    int? PageNumber { get; }
    int? PageSize { get; }
}