using MinimalAPI.DTOs;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interfaces;
using MinimalAPI.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.Domain.Services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;

    public VehicleService (AppDbContext context)     
    {
        this._context = context;
    }
    public List<Vehicle> All(int? page = 1, string? name = null, string? brand = null)
    {
        var query = _context.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => v.Name.ToLower().Contains(name.ToLower()));
        }
        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(v => v.Name.ToLower().Contains(brand.ToLower()));
        }
        
        int pageSize = 10;
        int currentPage = page ?? 1;

        return query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
    }
    public Vehicle? SearchById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }
    public Vehicle Include(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
        return vehicle;
    }
    public Vehicle Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
        return vehicle;
    }
    public Vehicle Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
        return vehicle;
    }
}