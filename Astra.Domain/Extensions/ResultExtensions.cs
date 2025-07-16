using Astra.Domain.Abstractions;

namespace Astra.Domain.Extensions
{
    public static class ResultExtensions
    {
        public static async Task<Result<U>> BindAsync<T, U>(this Result<T> resultTask, Func<T, Task<Result<U>>> next)
        {
            var result = resultTask;
            return result.IsSuccess ? await next(result.Value!).ConfigureAwait(false) :
                Result<U>.Failure(result.Error!);
        }
        
        public static async Task<Result<U>> BindAsync<T, U>(this Task<Result<T>> resultTask, Func<T, Task<Result<U>>> next)
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.IsSuccess ? await next(result.Value!).ConfigureAwait(false) :
                Result<U>.Failure(result.Error!);
        }

        public static async Task<Result<U>> MapAsync<T, U>(this Task<Result<T>> resultTask, Func<T, Task<U>> next)
        {
            var result = await resultTask.ConfigureAwait(false);
            return result.IsSuccess ? 
                Result<U>.Success(await next(result.Value!).ConfigureAwait(false)) :
                Result<U>.Failure(result.Error!);
        }
    }
}
