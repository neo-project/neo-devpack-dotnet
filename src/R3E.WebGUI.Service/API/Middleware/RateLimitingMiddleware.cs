using System.Collections.Concurrent;
using System.Net;

namespace R3E.WebGUI.Service.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly IConfiguration _configuration;
    
    // Simple in-memory store - in production, use Redis or similar distributed cache
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();
    
    // Configuration
    private readonly int _maxRequestsPerMinute;
    private readonly int _maxRequestsPerHour;
    private readonly int _maxDeploymentsPerDay;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        ILogger<RateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
        
        _maxRequestsPerMinute = configuration.GetValue("RateLimit:MaxRequestsPerMinute", 60);
        _maxRequestsPerHour = configuration.GetValue("RateLimit:MaxRequestsPerHour", 1000);
        _maxDeploymentsPerDay = configuration.GetValue("RateLimit:MaxDeploymentsPerDay", 50);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var endpoint = GetEndpointIdentifier(context);
        
        if (!await CheckRateLimit(clientId, endpoint, context))
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get real IP address from headers
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string GetEndpointIdentifier(HttpContext context)
    {
        return $"{context.Request.Method}:{context.Request.Path}";
    }

    private async Task<bool> CheckRateLimit(string clientId, string endpoint, HttpContext context)
    {
        var now = DateTime.UtcNow;
        var key = $"{clientId}:{endpoint}";
        
        var rateLimitInfo = _rateLimitStore.AddOrUpdate(key, 
            new RateLimitInfo { FirstRequest = now, RequestCount = 1, LastRequest = now },
            (k, existing) =>
            {
                // Reset counters if time windows have passed
                if (ShouldResetMinuteCounter(existing, now))
                {
                    existing.RequestsPerMinute = 0;
                    existing.MinuteWindowStart = now;
                }
                
                if (ShouldResetHourCounter(existing, now))
                {
                    existing.RequestsPerHour = 0;
                    existing.HourWindowStart = now;
                }
                
                if (ShouldResetDayCounter(existing, now))
                {
                    existing.DeploymentsPerDay = 0;
                    existing.DayWindowStart = now;
                }

                existing.RequestCount++;
                existing.RequestsPerMinute++;
                existing.RequestsPerHour++;
                existing.LastRequest = now;

                // Track deployments separately
                if (IsDeploymentEndpoint(endpoint))
                {
                    existing.DeploymentsPerDay++;
                }

                return existing;
            });

        // Check rate limits
        if (rateLimitInfo.RequestsPerMinute > _maxRequestsPerMinute)
        {
            _logger.LogWarning($"Rate limit exceeded for client {clientId}: {rateLimitInfo.RequestsPerMinute} requests per minute");
            return false;
        }

        if (rateLimitInfo.RequestsPerHour > _maxRequestsPerHour)
        {
            _logger.LogWarning($"Rate limit exceeded for client {clientId}: {rateLimitInfo.RequestsPerHour} requests per hour");
            return false;
        }

        if (IsDeploymentEndpoint(endpoint) && rateLimitInfo.DeploymentsPerDay > _maxDeploymentsPerDay)
        {
            _logger.LogWarning($"Deployment rate limit exceeded for client {clientId}: {rateLimitInfo.DeploymentsPerDay} deployments per day");
            return false;
        }

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit-Minute"] = _maxRequestsPerMinute.ToString();
        context.Response.Headers["X-RateLimit-Remaining-Minute"] = Math.Max(0, _maxRequestsPerMinute - rateLimitInfo.RequestsPerMinute).ToString();
        context.Response.Headers["X-RateLimit-Reset-Minute"] = rateLimitInfo.MinuteWindowStart.AddMinutes(1).ToString("o");

        return true;
    }

    private static bool ShouldResetMinuteCounter(RateLimitInfo info, DateTime now)
    {
        return now.Subtract(info.MinuteWindowStart).TotalMinutes >= 1;
    }

    private static bool ShouldResetHourCounter(RateLimitInfo info, DateTime now)
    {
        return now.Subtract(info.HourWindowStart).TotalHours >= 1;
    }

    private static bool ShouldResetDayCounter(RateLimitInfo info, DateTime now)
    {
        return now.Subtract(info.DayWindowStart).TotalDays >= 1;
    }

    private static bool IsDeploymentEndpoint(string endpoint)
    {
        return endpoint.Contains("/api/webgui/deploy", StringComparison.OrdinalIgnoreCase);
    }

    // Clean up old entries periodically
    public static void CleanupOldEntries()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);
        var keysToRemove = _rateLimitStore
            .Where(kvp => kvp.Value.LastRequest < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _rateLimitStore.TryRemove(key, out _);
        }
    }
}

public class RateLimitInfo
{
    public DateTime FirstRequest { get; set; }
    public DateTime LastRequest { get; set; }
    public long RequestCount { get; set; }
    
    public DateTime MinuteWindowStart { get; set; } = DateTime.UtcNow;
    public int RequestsPerMinute { get; set; }
    
    public DateTime HourWindowStart { get; set; } = DateTime.UtcNow;
    public int RequestsPerHour { get; set; }
    
    public DateTime DayWindowStart { get; set; } = DateTime.UtcNow;
    public int DeploymentsPerDay { get; set; }
}