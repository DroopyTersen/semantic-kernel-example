using System;
using EnterpriseAI.Core.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EnterpriseAI.API;

public static class SwaggerConfiguration
{
    public static void ConfigureSwaggerGen(
        SwaggerGenOptions options,
        ApiSecurityConfig apiSecurityConfig
    )
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(apiSecurityConfig);

        options.SwaggerDoc("v1", new OpenApiInfo { Title = "EnterpriseAI API", Version = "v1" });

        // Add security definition for API key
        options.AddSecurityDefinition(
            "ApiKey",
            new OpenApiSecurityScheme
            {
                Description = "API key needed to access the endpoints. X-API-Key: My_API_Key",
                In = ParameterLocation.Header,
                Name = apiSecurityConfig.HeaderName,
                Type = SecuritySchemeType.ApiKey,
            }
        );

        // Add security requirement for all operations
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey",
                        },
                    },
                    Array.Empty<string>()
                },
            }
        );
    }
}
