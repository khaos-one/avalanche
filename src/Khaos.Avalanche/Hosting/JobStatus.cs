namespace Khaos.Avalanche.Hosting;

public record JobStatus(TaskStatus Status, ExceptionInfo? Exception);