using System.Runtime.InteropServices;
using LibDataChannel.Native.Channels.Track;

namespace LibDataChannel.Channels.Track;

public class RtcTrackInit
{
    public RtcDirection Direction { get; set; }
    
    public RtcCodec Codec { get; set; }
    
    public int PayloadType { get; set; }
    
    public uint Ssrc { get; set; }

    /// <summary>
    ///     optional, from codec
    /// </summary>
    public string? Mid { get; set; }

    /// <summary>
    ///     optional
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    ///     optional
    /// </summary>
    public string? MsId { get; set; }
    
    /// <summary>
    ///     optional, track ID used in MsId
    /// </summary>
    public string? TrackId { get; set; }
    
    /// <summary>
    ///     optional, codec profile
    /// </summary>
    public string? Profile { get; set; }
    
    internal NativeRtcTrackInit AllocNative()
    {
        var mid = Mid != null ? Marshal.StringToHGlobalAnsi(Mid) : IntPtr.Zero;
        var name = Name != null ? Marshal.StringToHGlobalAnsi(Name) : IntPtr.Zero;
        var msId = MsId != null ? Marshal.StringToHGlobalAnsi(MsId) : IntPtr.Zero;
        var trackId = TrackId != null ? Marshal.StringToHGlobalAnsi(TrackId) : IntPtr.Zero;
        var profile = Profile != null ? Marshal.StringToHGlobalAnsi(Profile) : IntPtr.Zero;
        return new NativeRtcTrackInit(Direction, Codec, PayloadType, Ssrc, mid, name, msId, trackId, profile);
    }
}