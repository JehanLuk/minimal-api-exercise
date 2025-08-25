using Npgsql.EntityFrameworkCore.PostgreSQL;
using MinimalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(options => {
    var connString = builder.Configuration.GetConnectionString("psql");
    options.UseNpgsql(connString);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Password == "1234")
        return Results.Ok("Login success!");
    else
        return Results.Unauthorized();
});

app.Run();