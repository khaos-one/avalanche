using System.Text.Json;

namespace Khaos.Avalanche.Json;

public class ToLdJsonTransform<T> : ITransform<T, string>
{
    public IAsyncEnumerable<string> Expand(IAsyncEnumerable<T> enumerable) =>
        enumerable.Select(element => JsonSerializer.Serialize(element));
}