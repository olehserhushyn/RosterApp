using Microsoft.EntityFrameworkCore;
using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Infrastructure.Context;
using RosterApp.Infrastructure.Repositories;
using RosterApp.Infrastructure.Repositories.Commands;
using RosterApp.Infrastructure.Repositories.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

// Repositories
builder.Services.AddScoped<ICurrencyQueryRepository, CurrencyQueryRepository>();
builder.Services.AddScoped<IEmployeeQueryRepository, EmployeeQueryRepository>();
builder.Services.AddScoped<IShiftQueryRepository, ShiftQueryRepository>();
builder.Services.AddScoped<IWeeklyTipsQueryRepository, WeeklyTipsQueryRepository>();


builder.Services.AddScoped<IEmployeeCommandRepository, EmployeeCommandRepository>();
builder.Services.AddScoped<IShiftCommandRepository, ShiftCommandRepository>();
builder.Services.AddScoped<IWeeklyTipsCommandRepository, WeeklyTipsCommandRepository>();

builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
