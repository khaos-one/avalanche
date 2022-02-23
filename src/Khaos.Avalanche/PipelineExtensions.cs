namespace Khaos.Avalanche;

public static class PipelineExtensions
{
    public static IPipeline RunSequentially(IEnumerable<IPipeline> pipelines) =>
        new SequentialPipeline(pipelines);
}