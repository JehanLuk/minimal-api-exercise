using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.OpenApi.Models;
using MinimalAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Models.ModelView;
using MinimalAPI.Domain.Enums;

#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<AppDbContext>(options => {
    var connString = builder.Configuration.GetConnectionString("psql");
    options.UseNpgsql(connString);
});

var app = builder.Build();

#endregion

app.UseSwagger();
app.UseSwaggerUI();

#region Home

app.MapGet("/home", () => Results.Json(new Home())).WithTags("Home");

#endregion

#region Administrators

app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Login success!");
    else
        return Results.Unauthorized();
}).WithTags("Administrators");

app.MapGet("/admin", ([FromQuery] int? page, IAdministratorService administratorService)=> {
    var adms = new List<AdminModelView>();
    var admins = administratorService.All(page);
    foreach( var adm in admins)
    {
        adms.Add(new AdminModelView{
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Profile
        });
    }
    return Results.Ok(adms);
}).WithTags("Administrators");

app.MapPost("/admin", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) => {
    var validation = new ValidationErrors{
        Messages = new List<string>()
    };

    if ( string.IsNullOrEmpty(administratorDTO.Email ))
        validation.Messages.Add("Email can not be blank!");
    if ( string.IsNullOrEmpty(administratorDTO.Password ))
        validation.Messages.Add("Password can not be blank!");
    if ( administratorDTO.Profile == null)
        validation.Messages.Add("Profile can not be blank!");

    if ( validation.Messages.Count > 0 )
        return Results.BadRequest(validation);

    var admin = new Administrator {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Profile = administratorDTO.Profile?.ToString() ?? Profile.Editor.ToString()
    };

    administratorService.Include(admin);

    return Results.Created($"/admin/{admin.Id}", new AdminModelView{
            Id = admin.Id,
            Email = admin.Email,
            Profile = admin.Profile
    });
}).WithTags("Administrators");

app.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) => {
    var admin = administratorService.SearchById(id);

    if ( admin == null ) return Results.NotFound();

    return Results.Ok(new AdminModelView{
            Id = admin.Id,
            Email = admin.Email,
            Profile = admin.Profile
    });
}).WithTags("Administrators");

#endregion

#region Vehicles

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
    
    var vehicle = new Vehicle{
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    vehicleService.Include(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) => {
    var vehicles = vehicleService.All(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.SearchById(id);

    if ( vehicle == null ) return Results.NotFound();

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
    var vehicle = vehicleService.SearchById(id);
    if ( vehicle == null ) return Results.NotFound();

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.SearchById(id);
    if ( vehicle == null ) return Results.NotFound();

    vehicleService.Delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");

#endregion

app.Run();