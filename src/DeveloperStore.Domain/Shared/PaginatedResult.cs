using DeveloperStore.Domain.Abstractions;

namespace DeveloperStore.Domain.Shared;

public sealed class PaginatedResult : Result, IPaginatedResult
{
    public PaginatedResult(bool isSuccess,
        int totalRecords,
        int? pageNumber,
        int? pageSize,
        Error error) : base(isSuccess, error)
    {
        TotalRecords = totalRecords;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int TotalRecords { get; }

    public int? PageNumber { get; }

    public int? PageSize { get; }

    public static PaginatedResult Success(
        int totalRecords,
        int pageNumber,
        int pageSize) => new(true, totalRecords, pageNumber, pageSize, Error.None);

    public static PaginatedResult<TValue> Success<TValue>(
        TValue? value,
        int totalRecords,
        int? pageNumber,
        int? pageSize) => new(value, true, totalRecords, pageNumber, pageSize, Error.None);

    public static new PaginatedResult Failure(Error error) => new(false, 0, 0, 0, error);

    public static new PaginatedResult<TValue> Failure<TValue>(Error error) => new(default, false, 0, 0, 0, error);
}

public sealed class PaginatedResult<TValue> : Result<TValue>, IPaginatedResult
{
    public PaginatedResult(TValue? value,
        bool isSuccess,
        int totalRecords,
        int? pageNumber,
        int? pageSize,
        Error error) : base(value, isSuccess, error)
    {
        TotalRecords = totalRecords;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int TotalRecords { get; }

    public int? PageNumber { get; }

    public int? PageSize { get; }
}