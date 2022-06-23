using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Khaos.Avalanche.Parallel;

public static class ParallelExtensions
{
    public static async IAsyncEnumerable<List<T>> ParallelInterleave<T>(
        this IAsyncEnumerable<IAsyncEnumerable<T>> enumerable,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var revealedEnumerables = enumerable
            .Select(sequence => sequence.ToListAsync(cancellationToken));

        var tasks = await revealedEnumerables.ToListAsync(cancellationToken);

        while (tasks.Count > 0)
        {
            foreach (var task in tasks.ToImmutableArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (task.IsCompletedSuccessfully)
                {
                    tasks.Remove(task);
                    
                    yield return task.Result;
                }
            }
        }
    }
}