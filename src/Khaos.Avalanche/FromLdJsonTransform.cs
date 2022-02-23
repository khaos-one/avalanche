using System.Text.Json;

namespace Khaos.Avalanche;

public class FromLdJsonTransform<T> : ITransform<string, T>
{
    public IAsyncEnumerable<T> Expand(IAsyncEnumerable<string> enumerable) =>
        enumerable.Select(element => JsonSerializer.Deserialize<T>(element));
}