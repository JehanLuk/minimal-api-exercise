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
    public Administrator Include(Administrator administrator)
    {
        _context.Administrators.Add(administrator);
        _context.SaveChanges();

        return administrator;
    }
    public Administrator? SearchById(int id)
    {
        return _context.Administrators.Where(a => a.Id == id).FirstOrDefault();
    }
    public List<Administrator> All(int? page)
    {
        var query = _context.Administrators.AsQueryable();
        
        int pageSize = 10;
        int currentPage = page ?? 1;

        return query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}