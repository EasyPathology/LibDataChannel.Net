using System.Runtime.InteropServices;

namespace LibDataChannel.Native.Media;

[StructLayout(LayoutKind.Sequential)]
public struct NativeRtcPacketizerInit
{
    public uint Ssrc;
    public IntPtr Cname;
    public byte PayloadType;
    public uint ClockRate;
    public ushort SequenceNumber;
    public uint Timestamp;

    /// <summary>
    ///     H264, H265, AV1
    ///     Maximum fragment size, 0 means default
    /// </summary>
    public ushort MaxFragmentSize;

    /// <summary>
    ///     H264/H265 only
    ///     NAL unit separator
    /// </summary>
    public RtcNalUnitSeparator NalSeparator;

    /// <summary>
    ///     AV1 only
    ///     OBU packetization for AV1 samples
    /// </summary>
    public RtcObuPacketization ObuPacketization;

    public byte PlayoutDelayId;
    public ushort PlayoutDelayMin;
    public ushort PlayoutDelayMax;
    
    public void Free()
    {
        Marshal.FreeHGlobal(Cname);
    }
}