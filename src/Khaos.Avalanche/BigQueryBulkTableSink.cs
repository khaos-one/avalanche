using Google.Cloud.BigQuery.V2;

using Polly;

namespace Khaos.Avalanche;

public class BigQueryBulkTableSink : ISink<IEnumerable<BigQueryInsertRow>>
{
    private readonly BigQueryClient _client;
    private readonly string _dataset;
    private readonly string _tableName;
    private readonly bool _truncate;

    private readonly IAsyncPolicy _insertPolicy;

    private IAsyncEnumerable<IEnumerable<BigQueryInsertRow>>? _enumerable;

    public BigQueryBulkTableSink(BigQueryClient client, string dataset, string tableName, bool truncate = false)
    {
        _client = client;
        _dataset = dataset;
        _tableName = tableName;
        _truncate = truncate;

        _insertPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, attempt => TimeSpan.FromSeconds(attempt));
    }

    public void SetEnumerable(IAsyncEnumerable<IEnumerable<BigQueryInsertRow>> enumerable)
    {
        _enumerable = enumerable;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        var isInitialized = false;
        
        await foreach (var element in _enumerable!.WithCancellation(cancellationToken))
        {
            if (!isInitialized && _truncate)
            {
                await _client.ExecuteQueryAsync(
                    $"TRUNCATE TABLE {_dataset}.{_tableName}",
                    Array.Empty<BigQueryParameter>(),
                    cancellationToken: cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                isInitialized = true;
            }

            await _insertPolicy.ExecuteAsync(
                async () =>
                    await _client.InsertRowsAsync(
                        _dataset,
                        _tableName,
                        element,
                        cancellationToken: cancellationToken));
        }
    }
}