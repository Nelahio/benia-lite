namespace BeniaLite.Api.Entities;

public sealed class SymptomLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Category { get; set; } = "Skin"; // Skin/Hair/General...
    public int Severity0to10 { get; set; } // 0..10
    public string? Notes { get; set; }

    public DateTime LoggedAtUtc { get; set; } = DateTime.UtcNow;

    public List<SymptomLogTrigger> Triggers { get; set; } = new();
    public List<Photo> Photos { get; set; } = new();
}
