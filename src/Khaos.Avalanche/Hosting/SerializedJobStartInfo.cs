using Lokad.ILPack;

namespace Khaos.Avalanche.Hosting;

public record SerializedJobStartInfo(byte[] Assembly, string EntryType)
{
    public static SerializedJobStartInfo FromJobType(Type jobType)
    {
        var jobAssembly = jobType.Assembly;
        var assemblyGenerator = new AssemblyGenerator();

        var bytes = assemblyGenerator.GenerateAssemblyBytes(jobAssembly);

        return new SerializedJobStartInfo(bytes, jobType.FullName!);
    }

    public static SerializedJobStartInfo FromJobType<T>() => FromJobType(typeof(T));
}