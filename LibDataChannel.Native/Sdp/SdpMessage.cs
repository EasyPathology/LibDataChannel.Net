using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibDataChannel.Native.Sdp;

public readonly struct SdpMessage(SdpType type, string content)
{
    public SdpType Type { get; init; } = type;
    public string Content { get; init; } = content;

    public override string ToString()
    {
        return $"Type: {Type}, Content: {Content}";
    }
    
    public string JsonSerialize() => JsonSerializer.Serialize(this, SdpMessageJsonSerializerContext.Default.SdpMessage);

    public static bool TryJsonDeserialize(string json, out SdpMessage result)
    {
        try
        {
            result = JsonSerializer.Deserialize(json, SdpMessageJsonSerializerContext.Default.SdpMessage);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}

[JsonSerializable(typeof(SdpMessage))]
public partial class SdpMessageJsonSerializerContext : JsonSerializerContext;