namespace Khaos.Avalanche.Http;

public class DoHttpRequestTransform : ITransform<HttpRequestMessage, HttpResponseMessage>
{
    private readonly HttpClient _httpClient;

    public DoHttpRequestTransform(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public DoHttpRequestTransform()
        : this(new HttpClient())
    { }

    public IAsyncEnumerable<HttpResponseMessage> Expand(IAsyncEnumerable<HttpRequestMessage> enumerable) =>
        enumerable.SelectAwaitWithCancellation(
            async (request, cancellationToken) => await _httpClient.SendAsync(request, cancellationToken));
}