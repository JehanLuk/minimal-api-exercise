using MinimalAPI.Domain.Enums;

namespace MinimalAPI.Models.ModelView;

public record AdminLogged
{
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
    public string Token { get; set; } = default!;
}