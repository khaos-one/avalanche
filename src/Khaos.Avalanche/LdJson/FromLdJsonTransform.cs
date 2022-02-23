using System.Text.Json;

namespace Khaos.Avalanche.LdJson;

public class FromLdJsonTransform<T> : ITransform<string, T>
{
    public IAsyncEnumerable<T> Expand(IAsyncEnumerable<string> enumerable) =>
        enumerable.Select(
            element => JsonSerializer.Deserialize<T>(element)
                ?? throw new Exception("Failed to deserialize a row."));
}