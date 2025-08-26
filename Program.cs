using Npgsql.EntityFrameworkCore.PostgreSQL;
using MinimalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddDbContextPool<AppDbContext>(options => {
    var connString = builder.Configuration.GetConnectionString("psql");
    options.UseNpgsql(connString);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Login success!");
    else
        return Results.Unauthorized();
});

app.Run();