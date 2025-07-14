using FluentValidation;
using System.Net;
using System.Text.Json;

namespace R3E.WebGUI.Service.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.Details = validationEx.Errors.Select(e => new ErrorDetail
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();
                break;

            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid argument";
                errorResponse.Details = new List<ErrorDetail>
                {
                    new() { Message = argEx.Message }
                };
                break;

            case InvalidOperationException invOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid operation";
                errorResponse.Details = new List<ErrorDetail>
                {
                    new() { Message = invOpEx.Message }
                };
                break;

            case FileNotFoundException fileNotFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "File not found";
                errorResponse.Details = new List<ErrorDetail>
                {
                    new() { Message = fileNotFoundEx.Message }
                };
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                break;

            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Message = "Request timeout";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                break;
        }

        errorResponse.StatusCode = response.StatusCode;
        errorResponse.Timestamp = DateTime.UtcNow;
        errorResponse.Path = context.Request.Path;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Path { get; set; } = string.Empty;
    public List<ErrorDetail> Details { get; set; } = new();
}

public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}