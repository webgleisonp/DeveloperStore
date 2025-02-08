namespace DeveloperStore.Domain.Shared;

public class PagedResult<TValue>
{
    private readonly TValue[] _items;
    private readonly Error[] _errors;
    private static readonly Error DefaultError = new("DefaultError", "An error occurred while processing the request");

    protected internal PagedResult(
        TValue[] items,
        int totalItems,
        int currentPage,
        int totalPages,
        bool isSuccess,
        Error[] errors)
    {
        _items = items;
        TotalItems = totalItems;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        IsSuccess = isSuccess;
        _errors = errors.Length == 0 && !isSuccess
            ? new[] { DefaultError }
            : errors;
    }

    public IReadOnlyCollection<TValue> Items => _items.AsReadOnly();
    public int TotalItems { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors => _errors;

    public static PagedResult<TValue> Success(
        IEnumerable<TValue> items,
        int totalItems,
        int currentPage,
        int totalPages) =>
        new(
            items.ToArray(),
            totalItems,
            currentPage,
            totalPages,
            true,
            Array.Empty<Error>());

    public static PagedResult<TValue> Failure(params Error[] errors) =>
        new(
            Array.Empty<TValue>(),
            0,
            0,
            0,
            false,
            errors);

    public static PagedResult<TValue> Failure() =>
        new(
            Array.Empty<TValue>(),
            0,
            0,
            0,
            false,
            new[] { DefaultError });
} 