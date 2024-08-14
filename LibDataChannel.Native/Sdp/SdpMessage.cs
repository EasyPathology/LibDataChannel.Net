namespace LibDataChannel.Native.Sdp;

[Serializable]
public readonly struct SdpMessage(SdpType type, string content)
{
    public SdpType Type { get; init; } = type;
    public string Content { get; init; } = content;

    public override string ToString()
    {
        return $"Type: {Type}, Content: {Content}";
    }
}