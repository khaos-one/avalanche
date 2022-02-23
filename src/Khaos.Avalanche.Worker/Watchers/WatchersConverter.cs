using System.Text.Json;
using System.Text.Json.Serialization;

using Khaos.Avalanche.Watchers;

namespace Khaos.Avalanche.Worker.Watchers;

public class WatchersConverter : JsonConverter<IWatcher>
{
    public override IWatcher? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, IWatcher value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, null, options);

                break;
            
            default:
                var type = value.GetType();
                JsonSerializer.Serialize(
                    writer,
                    value,
                    type,
                    options);

                break;
        }
    }
}