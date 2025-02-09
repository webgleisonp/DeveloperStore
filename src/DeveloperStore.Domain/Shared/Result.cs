using DeveloperStore.Domain.Abstractions;

namespace DeveloperStore.Domain.Shared;

public class Result : IBaseResult
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue?> Create<TValue>(TValue? value) => Success(value);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default!, false, error);
    public static Result FirstFailureOrSuccess(params Result[] results)
    {
        foreach (var result in results)
            if (result.IsFailure)
                return result;

        return Success();
    }
}

public class Result<TValue> : Result, IBaseResult
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) => _value = value;

    public TValue Value => IsSuccess
        ? _value!
        : default!;

    public static implicit operator Result<TValue?>(TValue value) => Create(value);
}

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}