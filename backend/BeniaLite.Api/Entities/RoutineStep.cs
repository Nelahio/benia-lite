using System;

namespace BeniaLite.Api.Entities;

public sealed class RoutineStep
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoutineId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int SortOrder { get; set; }

    public string FrequencyType { get; set; } = "Daily"; // Weekly...
    public int FrequencyValue { get; set; } = 1; // 1 = tous les jours, 2 = 1 jour sur 2 (si Daily)

    public Routine? Routine { get; set; }
}
