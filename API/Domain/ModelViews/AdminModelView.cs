using MinimalAPI.Domain.Enums;

namespace MinimalAPI.Models.ModelView;

public record AdminModelView
{
    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Profile { get; set; } = default!;
}