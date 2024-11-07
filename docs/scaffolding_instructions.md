# SegalAI Project Setup Documentation

This document outlines the complete setup process for the SegalAI solution, including the initial project scaffolding and Semantic Kernel integration.

## Initial Solution Setup

First, create the solution and project structure:

```bash
# Create solution and directory
mkdir SegalAI
cd SegalAI
dotnet new sln -n SegalAI

# Create projects
dotnet new classlib -n SegalAI.Core
dotnet new webapi -n SegalAI.API
dotnet new console -n SegalAI.CLI

# Add projects to solution
dotnet sln add SegalAI.Core/SegalAI.Core.csproj
dotnet sln add SegalAI.API/SegalAI.API.csproj
dotnet sln add SegalAI.CLI/SegalAI.CLI.csproj

# Add project references
cd SegalAI.API
dotnet add reference ../SegalAI.Core/SegalAI.Core.csproj
cd ../SegalAI.CLI
dotnet add reference ../SegalAI.Core/SegalAI.Core.csproj

# Add Swagger packages to API project
cd ../SegalAI.API
dotnet add package Swashbuckle.AspNetCore

# Return to solution root
cd ..

# Restore and build
dotnet restore
dotnet build
```

## Project Structure

After running these commands, you'll have the following structure:

```
SegalAI/
├── SegalAI.sln
├── SegalAI.Core/
├── SegalAI.API/
└── SegalAI.CLI/
```

- **SegalAI.Core**: Class library containing shared business logic
- **SegalAI.API**: Web API with Swagger integration
- **SegalAI.CLI**: Console application for testing Core functionality

## Running the Projects

You can run the projects using these commands:

```bash
# Run the API
dotnet run --project SegalAI.API

# Run the CLI
dotnet run --project SegalAI.CLI

# Run API with watch mode (auto-recompile)
dotnet watch run --project SegalAI.API
```

## VS Code Integration

Create `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "run-api",
      "command": "dotnet",
      "type": "process",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/SegalAI.API/SegalAI.API.csproj"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "run-cli",
      "command": "dotnet",
      "type": "process",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/SegalAI.CLI/SegalAI.CLI.csproj"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "watch-api",
      "command": "dotnet",
      "type": "process",
      "args": [
        "watch",
        "run",
        "--project",
        "${workspaceFolder}/SegalAI.API/SegalAI.API.csproj"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

## Semantic Kernel Integration

Add required packages to the Core project:

```bash
cd SegalAI.Core
dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
```

Enable user secrets for API and CLI projects:

```bash
cd ../SegalAI.API
dotnet user-secrets init
cd ../SegalAI.CLI
dotnet user-secrets init
```

### Configuration Files

#### AIServiceConfig.cs

```csharp
namespace SegalAI.Core.Configuration;

public class AIServiceConfig
{
    public required string OpenAIApiKey { get; set; }
    public required string AzureOpenAIEndpoint { get; set; }
    public required string AzureOpenAIKey { get; set; }
    public required string ModelDeploymentName { get; set; }
    public required string EmbeddingDeploymentName { get; set; }
}
```

#### KernelService.cs

```csharp
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

namespace SegalAI.Core.Services;

public class KernelService
{
    private readonly Kernel _kernel;
    private readonly AIServiceConfig _config;

    public KernelService(IConfiguration configuration)
    {
        _config = configuration.GetSection("AIService").Get<AIServiceConfig>()
            ?? throw new InvalidOperationException("AIService configuration is missing");

        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: _config.ModelDeploymentName,
                endpoint: _config.AzureOpenAIEndpoint,
                apiKey: _config.AzureOpenAIKey
            );

        if (!string.IsNullOrEmpty(_config.OpenAIApiKey))
        {
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4-turbo-preview",
                apiKey: _config.OpenAIApiKey
            );
        }

        _kernel = builder.Build();
    }

    public Kernel Kernel => _kernel;
}
```

### Configuration Settings

#### appsettings.json (API & CLI)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AIService": {
    "ModelDeploymentName": "gpt-4",
    "EmbeddingDeploymentName": "text-embedding-ada-002"
  }
}
```

### Managing Secrets

Each project (API and CLI) needs its own local settings file for secrets.

1. For the API:

   ```bash
   cd SegalAI.API
   cp appsettings.local.template.json appsettings.local.json
   # Edit appsettings.local.json with your secrets
   ```

2. For the CLI:

   ```bash
   cd SegalAI.CLI
   cp appsettings.local.template.json appsettings.local.json
   # Edit appsettings.local.json with your secrets
   ```

The `appsettings.local.json` files are gitignored and won't be committed.

### Program.cs Updates

#### API Program.cs

```csharp
using SegalAI.Core.Configuration;
using SegalAI.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Add services
builder.Services.Configure<AIServiceConfig>(
    builder.Configuration.GetSection("AIService"));
builder.Services.AddSingleton<KernelService>();

// ... rest of your Program.cs setup
```

#### CLI Program.cs

```csharp
using Microsoft.Extensions.Configuration;
using SegalAI.Core.Configuration;
using SegalAI.Core.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var kernelService = new KernelService(configuration);

// Your CLI logic here
```

## Verification

To verify the setup:

1. Build the solution:

```bash
dotnet build
```

2. Run the API:

```bash
dotnet run --project SegalAI.API
```

3. Check Swagger UI at http://localhost:5000/swagger or https://localhost:5001/swagger

4. Run the CLI:

```bash
dotnet run --project SegalAI.CLI
```

## Next Steps

1. Add your OpenAI and Azure OpenAI credentials using user secrets
2. Create your first Semantic Kernel plugin in the Core project
3. Implement API endpoints that utilize the Semantic Kernel service
4. Create CLI commands to test Semantic Kernel functionality
