using System;

namespace BeniaLite.Api.Entities;

public sealed class Routine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "General"; // Skin, Hair, Food, Sleep...
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<RoutineStep> Steps { get; set; } = new();
}
