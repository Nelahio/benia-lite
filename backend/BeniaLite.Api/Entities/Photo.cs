namespace BeniaLite.Api.Entities;

public sealed class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public Guid SymptomLogId { get; set; }
    public SymptomLog? SymptomLog { get; set; }

    public string Url { get; set; } = string.Empty; // ex: /uploads/xxx.jpg
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
