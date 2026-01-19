namespace BeniaLite.Api.Entities;

public sealed class TriggerTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty; // "stress", "sucre", etc.

    public List<SymptomLogTrigger> SymptomLogs { get; set; } = new();
}
