using Microsoft.EntityFrameworkCore;
using PracticoOrm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5434";
var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "fabricaPastas";
var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "admin";
var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
