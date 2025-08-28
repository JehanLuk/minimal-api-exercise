using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MinimalAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Domain.Services;
using MinimalAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Models.ModelView;
using MinimalAPI.Domain.Enums;
using MinimalAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
            if (configuration != null)
            {
                Configuration = configuration;
                key = Configuration.GetSection("Jwt")?.ToString() ?? "";
            }
        }

    private string key = "";
    public IConfiguration Configuration { get; set; } = default!;

    public void Configureservices(IServiceCollection services)
    {
        #region Builder

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert the JWT Token here"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        services.AddDbContextPool<AppDbContext>(options =>
        {
            var connString = Configuration.GetConnectionString("psql");
            options.UseNpgsql(connString);
        });

    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
                        
            #region Home

            endpoints.MapGet("/home", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

            #endregion

            #region Administrators

            string GenerateTokenJwt(Administrator administrator){
                if (string.IsNullOrEmpty(key)) return string.Empty;
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrator.Email),
                    new Claim("Profile", administrator.Profile),
                    new Claim(ClaimTypes.Role, administrator.Profile),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            endpoints.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
                var adm = administratorService.Login(loginDTO);
                if (adm != null)
                {
                    string token = GenerateTokenJwt(adm);
                    return Results.Ok(new AdminLogged
                    {
                        Email = adm.Email,
                        Profile = adm.Profile,
                        Token = token
                    });
                }
                else
                    return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Administrators");

            endpoints.MapGet("/admin", ([FromQuery] int? page, IAdministratorService administratorService)=> {
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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            endpoints.MapPost("/admin", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) => {
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
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            endpoints.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) => {
                var admin = administratorService.SearchById(id);

                if ( admin == null ) return Results.NotFound();

                return Results.Ok(new AdminModelView{
                        Id = admin.Id,
                        Email = admin.Email,
                        Profile = admin.Profile
                });
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            #endregion

            #region Vehicles

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
                
                var vehicle = new Vehicle{
                    Name = vehicleDTO.Name,
                    Brand = vehicleDTO.Brand,
                    Year = vehicleDTO.Year
                };

                vehicleService.Include(vehicle);

                return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) => {
                var vehicles = vehicleService.All(page);

                return Results.Ok(vehicles);
            }).RequireAuthorization().WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.SearchById(id);

                if ( vehicle == null ) return Results.NotFound();

                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
                var vehicle = vehicleService.SearchById(id);
                if ( vehicle == null ) return Results.NotFound();

                vehicle.Name = vehicleDTO.Name;
                vehicle.Brand = vehicleDTO.Brand;
                vehicle.Year = vehicleDTO.Year;

                vehicleService.Update(vehicle);

                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.SearchById(id);
                if ( vehicle == null ) return Results.NotFound();

                vehicleService.Delete(vehicle);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Vehicles");

            #endregion
        });
    }

    #endregion
}