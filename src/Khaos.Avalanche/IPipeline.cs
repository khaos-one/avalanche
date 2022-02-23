namespace Khaos.Avalanche;

public interface IPipeline
{
    Task Run(CancellationToken cancellationToken = default);
}