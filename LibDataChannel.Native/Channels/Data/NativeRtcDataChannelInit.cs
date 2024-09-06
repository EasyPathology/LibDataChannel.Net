namespace LibDataChannel.Native.Channels.Data;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public ref struct NativeRtcDataChannelInit(in NativeRtcReliability reliability, IntPtr protocol, bool negotiated, bool manualStream, ushort streamId)
{
    public NativeRtcReliability Reliability = reliability;
    public IntPtr Protocol = protocol;
    public bool Negotiated = negotiated;
    public bool ManualStream = manualStream;
    public ushort StreamId = streamId;

    public void Free()
    {
        Marshal.FreeHGlobal(Protocol);
    }
}