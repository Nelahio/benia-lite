namespace BeniaLite.Api.Entities;

public sealed class SymptomLogTrigger
{
    public Guid SymptomLogId { get; set; }
    public SymptomLog? SymptomLog { get; set; }

    public Guid TriggerTagId { get; set; }
    public TriggerTag? TriggerTag { get; set; }
}
