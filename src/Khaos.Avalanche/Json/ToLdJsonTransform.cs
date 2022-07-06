using System.Text.Json;

namespace Khaos.Avalanche.Json;

public class ToLdJsonTransform : ITransform<object, string>
{
    public IAsyncEnumerable<string> Expand(IAsyncEnumerable<object> enumerable) =>
        enumerable.Select(element => JsonSerializer.Serialize(element));
}