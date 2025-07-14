using Microsoft.Extensions.Primitives;

namespace R3E.WebGUI.Service.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Security headers for production
        context.Response.Headers.Add("X-Content-Type-Options", new StringValues("nosniff"));
        context.Response.Headers.Add("X-Frame-Options", new StringValues("DENY"));
        context.Response.Headers.Add("X-XSS-Protection", new StringValues("1; mode=block"));
        context.Response.Headers.Add("Referrer-Policy", new StringValues("strict-origin-when-cross-origin"));
        
        // Content Security Policy - adjust based on your needs
        var csp = "default-src 'self'; " +
                  "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
                  "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net; " +
                  "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net; " +
                  "img-src 'self' data: https:; " +
                  "connect-src 'self' https://*.neo.org wss://*.neo.org; " +
                  "frame-ancestors 'none';";
        
        context.Response.Headers.Add("Content-Security-Policy", new StringValues(csp));

        // HSTS for HTTPS
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Add("Strict-Transport-Security", 
                new StringValues("max-age=31536000; includeSubDomains"));
        }

        await _next(context);
    }
}

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyAuthenticationMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for health checks and public endpoints
        var path = context.Request.Path.Value ?? "";
        if (path.StartsWith("/health") || 
            path.StartsWith("/swagger") || 
            path.StartsWith("/subdomain") ||
            context.Request.Method == "GET")
        {
            await _next(context);
            return;
        }

        // Check if API key authentication is enabled
        var requireApiKey = _configuration.GetValue<bool>("R3EWebGUI:Security:RequireApiKey", false);
        
        if (requireApiKey && IsProtectedEndpoint(context))
        {
            var providedApiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
            var validApiKey = _configuration["R3EWebGUI:Security:ApiKey"];

            if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != validApiKey)
            {
                _logger.LogWarning("Invalid API key attempt from {IP}", 
                    context.Connection.RemoteIpAddress);
                
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new { error = "Invalid or missing API key" }));
                return;
            }
        }

        await _next(context);
    }

    private bool IsProtectedEndpoint(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";

        // Protect POST, PUT, DELETE operations on API endpoints
        return (method == "POST" || method == "PUT" || method == "DELETE") && 
               path.StartsWith("/api/");
    }
}