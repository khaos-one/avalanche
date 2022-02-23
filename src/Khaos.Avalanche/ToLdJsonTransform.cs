using Newtonsoft.Json;

namespace Khaos.Avalanche;

public class ToLdJsonTransform<T> : ITransform<T, string>
{
    public IAsyncEnumerable<string> Expand(IAsyncEnumerable<T> enumerable) =>
        enumerable.Select(element => JsonConvert.SerializeObject(element));
}