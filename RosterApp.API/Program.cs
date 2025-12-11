using Microsoft.EntityFrameworkCore;
using RosterApp.API.Middlewares;
using RosterApp.Application.Abstractions;
using RosterApp.Application.Abstractions.Repositories.Commands;
using RosterApp.Application.Abstractions.Repositories.Queries;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.Services;
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

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ITipDistributionService, TipDistributionService>();
builder.Services.AddScoped<ITipsService, TipsService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
