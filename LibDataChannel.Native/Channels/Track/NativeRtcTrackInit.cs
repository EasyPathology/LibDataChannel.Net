using System.Runtime.InteropServices;

namespace LibDataChannel.Native.Channels.Track;

[StructLayout(LayoutKind.Sequential)]
public ref struct NativeRtcTrackInit(
    RtcDirection direction,
    RtcCodec codec,
    int payloadType,
    uint ssrc,
    IntPtr mid,
    IntPtr name,
    IntPtr msId,
    IntPtr trackId,
    IntPtr profile)
{
    public RtcDirection Direction = direction;
    public RtcCodec Codec = codec;
    public int PayloadType = payloadType;
    public uint Ssrc = ssrc;
    public IntPtr Mid = mid;
    public IntPtr Name = name;
    public IntPtr MsId = msId;
    public IntPtr TrackId = trackId;
    public IntPtr Profile = profile;

    public void Free()
    {
        Marshal.FreeHGlobal(Mid);
        Marshal.FreeHGlobal(Name);
        Marshal.FreeHGlobal(MsId);
        Marshal.FreeHGlobal(TrackId);
        Marshal.FreeHGlobal(Profile);
    }
}