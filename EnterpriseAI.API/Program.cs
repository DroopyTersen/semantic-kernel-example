using EnterpriseAI.Core.Configuration;
using EnterpriseAI.Core.Repositories;
using EnterpriseAI.Core.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder
    .Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables();

// Standard API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AI Service Config values
builder.Services.Configure<AIServiceConfig>(builder.Configuration.GetSection("AIService"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AIServiceConfig>>().Value);

// Main Kernel setup, look at this for more details on AI services
builder.Services.AddSingleton<KernelService>();

// How to persist the chat history
builder.Services.AddSingleton<IChatRepository, InMemoryChatRepository>();

var app = builder.Build();

// Setup Swagger for DEV
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
