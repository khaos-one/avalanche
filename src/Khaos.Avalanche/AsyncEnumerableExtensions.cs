namespace Khaos.Avalanche;

public static class AsyncEnumerableExtensions
{ 
    public static async IAsyncEnumerable<T> Flatten<T>(this IAsyncEnumerable<IEnumerable<T>> enumerable)
    {
        await foreach (var sequence in enumerable)
        {
            foreach (var element in sequence)
            {
                yield return element;
            }
        }
    }

    public static async IAsyncEnumerable<TOut> Aggregate<TIn, TOut>(
        this IAsyncEnumerable<TIn> source,
        TOut seed,
        Func<TOut, TIn, TOut> aggregator)
    {
        var result = await source.AggregateAsync(seed, aggregator);
        
        yield return result;
    }

    public static async IAsyncEnumerable<TOut> AggregateMutable<TIn, TOut>(
        this IAsyncEnumerable<TIn> source,
        TOut seed,
        Action<TOut, TIn> aggregator)
    {
        await foreach (var element in source)
        {
            aggregator(seed, element);
        }

        yield return seed;
    }

    public static IAsyncEnumerable<T> DistinctBy<T, TKey>(
        this IAsyncEnumerable<T> source,
        Func<T, TKey> selector) =>
        source.GroupBy(selector).SelectAwait(async grouping => await grouping.FirstAsync());

    public static async IAsyncEnumerable<IEnumerable<T>> Batch<T>(this IAsyncEnumerable<T> source, int batchSize = 1000)
    {
        if (batchSize == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        var added = 0;
        var buffer = new List<T>(batchSize);

        await foreach (var item in source)
        {
            buffer.Add(item);
            added++;

            if (added % batchSize == 0)
            {
                yield return buffer.ToArray();
                
                buffer.Clear();
                added = 0;
            }
        }

        if (added != 0)
        {
            yield return buffer.ToArray();
        }
    }

    public static ISink<T> SinkInto<T>(this IAsyncEnumerable<T> enumerable, ISink<T> sink)
    {
        sink.SetEnumerable(enumerable);

        return sink;
    }

    public static FunctionalSink<T> SinkIntoFn<T>(this IAsyncEnumerable<T> enumerable, Func<T, Task> action)
    {
        var sink = new FunctionalSink<T>(action);
        sink.SetEnumerable(enumerable);

        return sink;
    }

    public static IAsyncEnumerable<TOut> Apply<TIn, TOut>(
        this IAsyncEnumerable<TIn> enumerable,
        ITransform<TIn, TOut> transform) =>
        transform.Expand(enumerable);
}