using Microsoft.EntityFrameworkCore;
using TreasuryTransfers.Api.Endpoints;
using TreasuryTransfers.Api.Middleware;
using TreasuryTransfers.Application;
using TreasuryTransfers.Infrastructure;
using TreasuryTransfers.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Application layer services
builder.Services.AddApplication();

// Register Infrastructure layer services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply pending migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

// Map endpoint groups
app.MapTransferEndpoints();
app.MapAccountEndpoints();

app.Run();
