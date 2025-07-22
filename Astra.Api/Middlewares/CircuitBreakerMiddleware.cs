using Microsoft.AspNetCore.Mvc;

namespace Astra.Api.Middlewares
{
    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }

    public class CircuitBreakerMiddleware
    {
        private readonly RequestDelegate _next;
        private static CircuitBreakerState _state = CircuitBreakerState.Closed;
        private static int _failureCount = 0;
        private static readonly object _lock = new();
        private static DateTime _lastFailureTime;
        private static readonly int _failureThreshold = 3;
        private static readonly TimeSpan _openToHalfOpenWaitTime = TimeSpan.FromSeconds(30);

        public CircuitBreakerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(_state == CircuitBreakerState.Open)
            {
                if(DateTime.UtcNow  - _lastFailureTime > _openToHalfOpenWaitTime)
                {
                    lock (_lock) _state = CircuitBreakerState.HalfOpen;
                }
                else
                {
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Detail = "Service unavailable.  Try again later.",
                        Status = context.Response.StatusCode,
                        Title = "Service unavailable.",
                        Type = typeof(Exception).FullName
                    });
                    return;
                }
            }
            try
            {
                await _next(context);
                if(_state == CircuitBreakerState.HalfOpen)
                {
                    lock(_lock)
                    {
                        _state = CircuitBreakerState.Closed;
                        _failureCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                lock(_lock)
                {
                    _failureCount++;
                    _lastFailureTime = DateTime.UtcNow;
                    if (_failureCount > _failureThreshold)
                        _state = CircuitBreakerState.Open;
                    else if (_state == CircuitBreakerState.HalfOpen)
                        _state = CircuitBreakerState.Open;
                }
            }
        }
    }
}
