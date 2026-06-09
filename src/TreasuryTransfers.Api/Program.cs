using TreasuryTransfers.Api.Endpoints;
using TreasuryTransfers.Application;
using TreasuryTransfers.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Railway sets PORT env var — configure Kestrel to listen on it
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Application layer services
builder.Services.AddApplication();

// Register Infrastructure layer services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Map endpoint groups
app.MapTransferEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
