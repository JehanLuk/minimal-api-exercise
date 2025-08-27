using MinimalAPI.DTOs;
using MinimalAPI.Domain.Entities;

namespace MinimalAPI.Domain.Interfaces;

public interface IVehicleService
{
    List<Vehicle> All(int? page, string? name = null, string? brand = null);
    Vehicle? SearchById(int id);
    Vehicle Include(Vehicle vehicle);
    Vehicle Update(Vehicle vehicle);
    Vehicle Delete(Vehicle vehicle);
}