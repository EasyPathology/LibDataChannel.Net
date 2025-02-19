using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibDataChannel.Native.Connections.Rtc;

public readonly struct RtcIceCandidate(string candidate, string mid)
{
    public string Candidate { get; init; } = candidate;
    public string Mid { get; init; } = mid;

    public string JsonSerialize() => JsonSerializer.Serialize(this, RtcIceCandidateJsonSerializerContext.Default.RtcIceCandidate);

    public static bool TryJsonDeserialize(string json, out RtcIceCandidate result)
    {
        try
        {
            result = JsonSerializer.Deserialize(json, RtcIceCandidateJsonSerializerContext.Default.RtcIceCandidate);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}

[JsonSerializable(typeof(RtcIceCandidate))]
public partial class RtcIceCandidateJsonSerializerContext : JsonSerializerContext;