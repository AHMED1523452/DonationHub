using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DonationHub.Middleware
{
    public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger) : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                await next(context);
                stopWatch.Stop();

                var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;

                logger.LogInformation("Request {Method} {Path} executed in {ElapsedMilliseconds} ms",
                    context.Request.Method, context.Request.Path, elapsedMilliseconds);
            }
            catch (ValidationException exception)
            {
                logger.LogWarning(exception, "Request validation failed for {Method} {Path}.",
                context.Request.Method, context.Request.Path);

                context.Response.Clear(); //. don't make it like a response
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    IsSuccess = false,
                    Message = "Request validation failed."
                }, context.RequestAborted);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception for {Method} {Path}.",
                context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)
                    throw;

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    TraceId = context.TraceIdentifier
                }, context.RequestAborted);
            }
        }
    }
}
