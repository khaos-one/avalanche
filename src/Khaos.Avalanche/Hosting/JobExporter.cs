using System.Net.Http.Json;

using Lokad.ILPack;

namespace Khaos.Avalanche.Hosting;

public class JobExporter
{
    private readonly string _baseUrl;

    public JobExporter(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task Export(Type jobType)
    {
        var jobAssembly = jobType.Assembly;
        var assemblyGenerator = new AssemblyGenerator();

        var bytes = assemblyGenerator.GenerateAssemblyBytes(jobAssembly);

        var request = new SerializedJobStartInfo(bytes, jobType.FullName!);

        var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync(_baseUrl + "/jobs", request);

        response.EnsureSuccessStatusCode();
    }
}