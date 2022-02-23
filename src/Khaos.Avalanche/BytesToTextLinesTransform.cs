using System.Text;

using Microsoft.Toolkit.HighPerformance;

namespace Khaos.Avalanche;

public class BytesToTextLinesTransform : ITransform<ReadOnlyMemory<byte>, IEnumerable<string>>
{
    public IAsyncEnumerable<IEnumerable<string>> Expand(IAsyncEnumerable<ReadOnlyMemory<byte>> enumerable) =>
        enumerable.SelectAwait(
            async element =>
            {
                await using var ms = element.AsStream();
                using var sr = new StreamReader(ms, Encoding.UTF8);

                var accumulator = new List<string>();

                while (true)
                {
                    var nextElement = sr.ReadLine();

                    if (nextElement is null || nextElement.Length == 0)
                    {
                        break;
                    }

                    if (nextElement[0] == 0x00)
                    {
                        break;
                    }

                    accumulator.Add(nextElement);
                }

                return accumulator;
            });
}