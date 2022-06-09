namespace Khaos.Avalanche;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        foreach (var sequence in enumerable)
        {
            foreach (var element in sequence)
            {
                yield return element;
            }
        }
    }
}