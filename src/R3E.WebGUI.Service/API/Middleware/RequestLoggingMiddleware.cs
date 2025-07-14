using System.Diagnostics;
using System.Text;

namespace R3E.WebGUI.Service.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Add request ID to response headers for tracking
        context.Response.Headers["X-Request-ID"] = requestId;
        
        var request = context.Request;
        var requestInfo = new StringBuilder();
        requestInfo.AppendLine($"[{requestId}] Starting request: {request.Method} {request.Path}{request.QueryString}");
        requestInfo.AppendLine($"[{requestId}] Host: {request.Host}");
        requestInfo.AppendLine($"[{requestId}] User-Agent: {request.Headers.UserAgent}");
        requestInfo.AppendLine($"[{requestId}] Content-Type: {request.ContentType}");
        requestInfo.AppendLine($"[{requestId}] Content-Length: {request.ContentLength}");

        if (request.Headers.ContainsKey("X-Forwarded-For"))
        {
            requestInfo.AppendLine($"[{requestId}] X-Forwarded-For: {request.Headers["X-Forwarded-For"]}");
        }

        _logger.LogInformation(requestInfo.ToString());

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var response = context.Response;
            
            var responseInfo = new StringBuilder();
            responseInfo.AppendLine($"[{requestId}] Completed request in {stopwatch.ElapsedMilliseconds}ms");
            responseInfo.AppendLine($"[{requestId}] Status: {response.StatusCode}");
            responseInfo.AppendLine($"[{requestId}] Content-Type: {response.ContentType}");
            
            if (response.StatusCode >= 400)
            {
                _logger.LogWarning(responseInfo.ToString());
            }
            else
            {
                _logger.LogInformation(responseInfo.ToString());
            }

            // Log performance metrics
            if (stopwatch.ElapsedMilliseconds > 5000) // 5 seconds
            {
                _logger.LogWarning($"[{requestId}] Slow request detected: {request.Method} {request.Path} took {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}