namespace Astra.Domain.Abstractions
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }

        private Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);

        public Result Bind(Func<Result> next) => IsSuccess ? next() : this;

        public Result<T> Bind<T>(Func<Result<T>> next) => IsSuccess ? next() : Result<T>.Failure(Error!);

        public T Match<T>(Func<T> onSuccess, Func<string, T> onFailure) =>
            IsSuccess ? onSuccess() : onFailure(Error!);

        public override string ToString() =>
            IsSuccess ? "Success" : $"Failure: {Error}";
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; }
        public string? Error { get; }

        public Result(bool isSuccess, T? value, string? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string error) => new Result<T>(true, default, null);

        public Result<U> Map<U>(Func<T, U> func)
        {
            if (IsSuccess)
                return Result<U>.Success(func(Value!));
            return Result<U>.Failure(Error!);
        }

        public Result<U> Bind<U>(Func<T, Result<U>> func)
        {
            if (IsSuccess)
                return func(Value!);
            return Result<U>.Failure(Error!);
        }

        public TReturn Match<TReturn>(Func<T, TReturn> onSuccess, Func<T, TReturn> onFailure)
        {
            if (IsSuccess) return onSuccess(Value!);
            return onFailure(Value!);
        }

        public void Match(Action<T> onSuccess, Action<string> onFailure)
        {
            if (IsSuccess) onSuccess(Value!);
            else onFailure(Error!);
        }

        public override string ToString()
        {
            return IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
        }
    }
}
