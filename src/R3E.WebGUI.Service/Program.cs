using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using R3E.WebGUI.Service.API.Middleware;
using R3E.WebGUI.Service.API.Validation;
using R3E.WebGUI.Service.Core.Services;
using R3E.WebGUI.Service.Infrastructure.Data;
using R3E.WebGUI.Service.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "R3E WebGUI Service API",
        Version = "v1",
        Description = @"Professional Neo N3 Smart Contract WebGUI Hosting Service

Features:
• Auto-generate WebGUIs from contract manifests
• Real-time contract data (balances, transactions, events)
• Wallet integration (NeoLine, O3, WalletConnect)
• Contract method invocation with gas estimation
• Multi-template system (Standard, NEP-17, NEP-11)
• Subdomain-based hosting (contract.domain.com)
• Asset tracking and transaction history
• Production-ready with rate limiting and security",
        Contact = new OpenApiContact
        {
            Name = "R3E Community",
            Url = new Uri("https://github.com/r3e-network")
        }
    });
    
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints",
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey
    });
});

// Database
builder.Services.AddDbContext<WebGUIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Core Services
builder.Services.AddScoped<IWebGUIRepository, WebGUIRepository>();
builder.Services.AddScoped<IWebGUIService, WebGUIService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();
builder.Services.AddScoped<IContractConfigService, ContractConfigService>();
builder.Services.AddScoped<ISignatureValidationService, SignatureValidationService>();

// Neo Integration Services
builder.Services.AddHttpClient<INeoRpcService, SimpleNeoRpcService>();
builder.Services.AddScoped<INeoRpcService, SimpleNeoRpcService>();
builder.Services.AddScoped<IWebGUIGeneratorService, SimpleWebGUIGeneratorService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<DeployWebGUIRequestValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsBuilder =>
    {
        corsBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Health checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline

// Security headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// Rate limiting
app.UseMiddleware<RateLimitingMiddleware>();

// API Key authentication (for production)
if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
}

// Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Request logging
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "R3E WebGUI Service API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "R3E WebGUI Service API Documentation";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

// Handle subdomain routing
app.Use(async (context, next) =>
{
    var host = context.Request.Host.Host;
    var path = context.Request.Path.Value ?? "";
    
    // Extract subdomain - remove port first
    var hostWithoutPort = host.Split(':')[0];
    var parts = hostWithoutPort.Split('.');
    var hasSubdomain = parts.Length >= 2 && parts[^1] == "localhost";
    
    if (hasSubdomain && !path.StartsWith("/api/") && !path.StartsWith("/health") && !path.StartsWith("/swagger"))
    {
        // This is a subdomain request, route to subdomain controller
        context.Request.Path = "/subdomain" + path;
    }
    
    await next();
});

// Map controllers with subdomain support
app.MapControllers();

// Start the application without waiting for database
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("R3E WebGUI Service started successfully");
logger.LogInformation("API documentation available at /swagger");

// Initialize database in background
_ = Task.Run(async () =>
{
    var maxRetries = 30;
    var retryDelay = TimeSpan.FromSeconds(2);
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WebGUIDbContext>();
            await context.Database.EnsureCreatedAsync();
            
            var bgLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            bgLogger.LogInformation("Database connection established and tables created");
            break;
        }
        catch (Exception ex)
        {
            var bgLogger = app.Services.GetRequiredService<ILogger<Program>>();
            bgLogger.LogWarning($"Database connection attempt {attempt}/{maxRetries} failed: {ex.Message}");
            
            if (attempt < maxRetries)
            {
                await Task.Delay(retryDelay);
            }
            else
            {
                bgLogger.LogError("Failed to connect to database after {MaxRetries} attempts", maxRetries);
            }
        }
    }
});

app.Run();