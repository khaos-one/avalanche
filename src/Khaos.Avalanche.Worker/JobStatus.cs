namespace Khaos.Avalanche.Worker;

public record JobStatus(TaskStatus Status, string? ExceptionMessage);