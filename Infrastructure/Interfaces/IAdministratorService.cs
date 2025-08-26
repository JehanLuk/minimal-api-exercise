using MinimalAPI.DTOs;
using MinimalAPI.Domain.Entities;

namespace MinimalAPI.Domain.Interfaces;

public interface IAdministratorService
{
    Administrator? Login(LoginDTO loginDTO);
}