using MinimalAPI.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.Domain.Services;

public class AdministratorService : IAdministratorService
{
    private readonly AppDbContext _context;

    public AdministratorService (AppDbContext context)     
    {
        this._context = context;
    }
    public Administrator? Login(LoginDTO loginDTO)
    {
        return _context.Administrators.FirstOrDefault(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
    }
}