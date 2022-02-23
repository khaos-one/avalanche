using System.Runtime.CompilerServices;

using Google.Cloud.Storage.V1;

namespace Khaos.Avalanche.GoogleCloudStorage;

public class GcsFileSource : Source<FileContent>
{
    private readonly StorageClient _client;
    private readonly string _bucket;
    private readonly string _path;

    public GcsFileSource(StorageClient client, string bucket, string path)
    {
        _client = client;
        _bucket = bucket;
        _path = path;
    }

    protected override async IAsyncEnumerable<FileContent> GetEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var objectsIterator = _client.ListObjectsAsync(_bucket, _path).AsRawResponses();

        await foreach (var page in objectsIterator.WithCancellation(cancellationToken))
        {
            if (!page.Items?.Any() ?? false)
            {
                break;
            }

            foreach (var item in page.Items)
            {
                if (item.Size == 0)
                {
                    continue;
                }

                await using var ms = new MemoryStream();
                await _client.DownloadObjectAsync(item, ms, cancellationToken: cancellationToken);

                var content = ms.GetBuffer();

                yield return new(item.Name, new ReadOnlyMemory<byte>(content));
            }
        }
    }
}