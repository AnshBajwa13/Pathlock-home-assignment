namespace Application.Common.Models;

/// <summary>
/// Represents the result of an operation
/// </summary>
public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    public static Result Success()
    {
        return new Result { Succeeded = true };
    }

    public static Result Failure(params string[] errors)
    {
        return new Result
        {
            Succeeded = false,
            Errors = errors
        };
    }
}

/// <summary>
/// Represents the result of an operation with a return value
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            Succeeded = true,
            Data = data
        };
    }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = errors
        };
    }
}
