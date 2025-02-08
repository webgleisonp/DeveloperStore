namespace DeveloperStore.Domain.Shared;

public class Result
{
    private static readonly Error DefaultError = new(
        "DefaultError",
        "An error occurred while processing the request");

    protected internal Result() { }

    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);

    public static Result<TValue> Failure<TValue>(params Error[] errors) => Result<TValue>.Failure(errors);

    // Sobrecarga para falha sem erros específicos
    public static Result<TValue> Failure<TValue>() => Result<TValue>.Failure(DefaultError);
}

public class Result<TValue>
{
    private readonly TValue? _value;
    private readonly Error[] _errors;

    private static readonly Error DefaultError = new(
        "DefaultError",
        "An error occurred while processing the request");

    protected internal Result(TValue? value, bool isSuccess, Error[] errors)
    {
        _value = value;
        IsSuccess = isSuccess;
        _errors = errors.Length == 0 && !isSuccess 
            ? new[] { DefaultError } 
            : errors;
    }

    public TValue? Value => IsSuccess 
        ? _value 
        : default;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors => _errors;

    public static Result<TValue> Success(TValue value) => new(value, true, Array.Empty<Error>());

    public static Result<TValue> Failure(params Error[] errors) => new(default, false, errors);

    // Sobrecarga para falha sem erros específicos
    public static Result<TValue> Failure() => new(default, false, new[] { DefaultError });
}

public record Error(string Code, string Message)
{
    public static Error Default() => new(
        "DefaultError",
        "An error occurred while processing the request");

    public static Error Validation(string code, string message) => new(code, message);

    public static readonly Error None = new(string.Empty, string.Empty);
}