using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Net.Sockets;

namespace Astra.Api.Middlewares
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (IsInfrastructureException(ex))
            {
                _logger.LogError(ex, "Infrastructure failure detected.");

                context.Response.StatusCode = StatusCodes.Status424FailedDependency;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Detail = ex.Message,
                    Status = context.Response.StatusCode,
                    Title = "Infrastructure Exception",
                    Type = ex.GetType().FullName
                });
            }
        }

        private bool IsInfrastructureException(Exception ex)
        {
            return ex switch
            {
                IOException => true,
                DbException => true,
                HttpRequestException => true,
                SocketException => true,
                AggregateException agg when agg.InnerExceptions.All(IsInfrastructureException) => true,
                _ => false
            };
        }
    }
}
