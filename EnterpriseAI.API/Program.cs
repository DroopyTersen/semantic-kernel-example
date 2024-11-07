using EnterpriseAI.Core.Configuration;
using EnterpriseAI.Core.Repositories;
using EnterpriseAI.Core.Services;
using Azure.Search.Documents;
using Microsoft.Extensions.Options;
using Azure;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AI services
builder.Services.Configure<AIServiceConfig>(
    builder.Configuration.GetSection("AIService"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<AIServiceConfig>>().Value);
builder.Services.AddSingleton<KernelService>();
builder.Services.AddSingleton<IChatRepository, InMemoryChatRepository>();

// Add Azure Search service
builder.Services.AddSingleton<AzureSearchService>(sp =>
{
    var config = sp.GetRequiredService<IOptions<AIServiceConfig>>().Value;
    var searchClient = new SearchClient(
        new Uri(config.SearchServiceEndpoint),
        config.SearchIndexName,
        new AzureKeyCredential(config.SearchServiceApiKey));
    return new AzureSearchService(searchClient);
});
builder.Services.AddSingleton<IHybridSearchService>(sp =>
    sp.GetRequiredService<AzureSearchService>());

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
