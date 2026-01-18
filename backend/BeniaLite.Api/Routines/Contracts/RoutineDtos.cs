namespace BeniaLite.Api.Routines.Contracts;

public sealed record RoutineListItemDto(Guid Id, string Name, string Category, bool IsActive);

public sealed record RoutineStepDto(Guid Id, string Title, string? Notes, int SortOrder, string FrequencyType, int FrequencyValue);

public sealed record RoutineDetailDto(Guid Id, string Name, string Category, bool IsActive, List<RoutineStepDto> Steps);

public sealed record CreateRoutineRequest(string Name, string Category);
public sealed record UpdateRoutineRequest(string Name, string Category, bool IsActive);

public sealed record AddStepRequest(string Title, string? Notes, int SortOrder, string FrequencyType, int FrequencyValue);

public sealed record CompleteRoutineRequest(string? Notes);

public sealed record RoutineTodayDto(Guid Id, string Name, string Category, bool IsCompletedToday, DateTime? CompletedAtUtc, List<RoutineStepDto> Steps);