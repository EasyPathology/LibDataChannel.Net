namespace LibDataChannel.Native.Connections.Rtc;

[Serializable]
public readonly struct RtcIceCandidate(string candidate, string mid)
{
    public string Candidate { get; init; } = candidate;
    public string Mid { get; init; } = mid;
}