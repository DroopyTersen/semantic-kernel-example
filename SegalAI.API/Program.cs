using SegalAI.Core.Configuration;
using SegalAI.Core.Repositories;
using SegalAI.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AI services
builder.Services.Configure<AIServiceConfig>(
    builder.Configuration.GetSection("AIService"));
builder.Services.AddSingleton<KernelService>();
builder.Services.AddSingleton<IChatRepository, InMemoryChatRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
