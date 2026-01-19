namespace BeniaLite.Api.Symptoms.Contracts;

public sealed record PhotoDto(Guid Id, string Url, DateTime CreatedAtUtc);

public sealed record TriggerTagDto(Guid Id, string Name);

public sealed record SymptomLogListItemDto(
    Guid Id,
    string Category,
    int Severity0to10,
    DateTime LoggedAtUtc
);

public sealed record SymptomLogDetailDto(
    Guid Id,
    string Category,
    int Severity0to10,
    string? Notes,
    DateTime LoggedAtUtc,
    List<TriggerTagDto> Triggers,
    List<PhotoDto> Photos
);

public sealed record CreateSymptomLogRequest(
    string Category,
    int Severity0to10,
    string? Notes,
    DateTime? LoggedAtUtc,
    List<string>? TriggerNames
);

public sealed record SymptomDailyPointDto(DateTime DayUtc, double AvgSeverity, int Count);

public sealed record SymptomStatsDto(
    int Days,
    string? Category,
    int TotalCount,
    double AvgSeverity,
    int MinSeverity,
    int MaxSeverity,
    List<SymptomDailyPointDto> Series
);