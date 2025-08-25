using MinimalAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Password == "1234")
        return Results.Ok("Login success!");
    else
        return Results.Unauthorized();
});

app.Run();