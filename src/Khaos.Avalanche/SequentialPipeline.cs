namespace Khaos.Avalanche;

public class SequentialPipeline : IPipeline
{
    private readonly IEnumerable<IPipeline> _pipelines;

    public SequentialPipeline(IEnumerable<IPipeline> pipelines)
    {
        _pipelines = pipelines;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        foreach (var pipeline in _pipelines)
        {
            await pipeline.Run(cancellationToken);
        }
    }
}