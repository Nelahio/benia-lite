using System;

namespace BeniaLite.Api.Entities;

public sealed class RoutineCompletion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid RoutineId { get; set; }
    public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
