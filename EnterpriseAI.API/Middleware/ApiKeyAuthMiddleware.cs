using System.Linq;
using EnterpriseAI.Core.Configuration;
using Microsoft.Extensions.Options;

namespace EnterpriseAI.API.Middleware;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiSecurityConfig _config;

    public ApiKeyAuthMiddleware(RequestDelegate next, IOptions<ApiSecurityConfig> config)
    {
        _next = next;
        _config = config.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip auth for swagger endpoints
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(_config.HeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        var apiKey = extractedApiKey.ToString();
        if (!_config.ApiKeys.Any(k => k == apiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        await _next(context);
    }
}

// Extension method for cleaner registration
public static class ApiKeyAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyAuthMiddleware>();
    }
}
