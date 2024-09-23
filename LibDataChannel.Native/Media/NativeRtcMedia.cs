using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibDataChannel.Native.Channels;
using LibDataChannel.Native.Utils;

namespace LibDataChannel.Native.Media;

public static class NativeRtcMedia
{
    public static unsafe void SetH264Packetizer(RtcTrack track, NativeRtcPacketizerInit init)
    {
        try
        {
            NativeRtc.SetH264Packetizer(track.Id, (IntPtr)(&init)).ThrowIfError();
        }
        finally
        {
            init.Free();
        }
    }

    public static unsafe void SetH2654Packetizer(RtcTrack track, NativeRtcPacketizerInit init)
    {
        try
        {
            NativeRtc.SetH265Packetizer(track.Id, (IntPtr)(&init)).ThrowIfError();
        }
        finally
        {
            init.Free();
        }
    }

    public static unsafe void SetAv1Packetizer(RtcTrack track, NativeRtcPacketizerInit init)
    {
        try
        {
            NativeRtc.SetAv1Packetizer(track.Id, (IntPtr)(&init)).ThrowIfError();
        }
        finally
        {
            init.Free();
        }
    }

    public static unsafe void SetOpusPacketizer(RtcTrack track, NativeRtcPacketizerInit init)
    {
        try
        {
            NativeRtc.SetOpusPacketizer(track.Id, (IntPtr)(&init)).ThrowIfError();
        }
        finally
        {
            init.Free();
        }
    }

    public static unsafe void SetAacPacketizer(RtcTrack track, NativeRtcPacketizerInit init)
    {
        try
        {
            NativeRtc.SetAacPacketizer(track.Id, (IntPtr)(&init)).ThrowIfError();
        }
        finally
        {
            init.Free();
        }
    }

    public static void ChainRtcpReceivingSession(RtcTrack track)
    {
        NativeRtc.ChainRtcpReceivingSession(track.Id).ThrowIfError();
    }

    public static void ChainRtcpSrReporter(RtcTrack track)
    {
        NativeRtc.ChainRtcpSrReporter(track.Id).ThrowIfError();
    }

    public static void ChainRtcpNackResponder(RtcTrack track, uint maxStoredPacketsCount)
    {
        NativeRtc.ChainRtcpNackResponder(track.Id, maxStoredPacketsCount).ThrowIfError();
    }

    public static unsafe void ChainPliHandler(RtcTrack track)
    {
        NativeRtc.ChainPliHandler(track.Id, &PliCallback).ThrowIfError();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void PliCallback(int id, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        RtcTrack.FromHandle(handle).InternalHandlePli();
    }

    public static unsafe void ChainRembHandler(RtcTrack track)
    {
        NativeRtc.ChainRembHandler(track.Id, &RembCallback).ThrowIfError();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void RembCallback(int id, uint maxBitrate, IntPtr handle)
    {
        ThreadUtils.SetRtcThread();
        RtcTrack.FromHandle(handle).InternalHandleRemb(maxBitrate);
    }

    public static unsafe uint TransformSecondsToTimestamp(RtcTrack track, double seconds)
    {
        var timestamp = 0U;
        NativeRtc.TransformSecondsToTimestamp(track.Id, seconds, &timestamp).ThrowIfError();
        return timestamp;
    }

    public static unsafe double TransformTimestampToSeconds(RtcTrack track, uint timestamp)
    {
        var seconds = 0D;
        NativeRtc.TransformTimestampToSeconds(track.Id, timestamp, &seconds).ThrowIfError();
        return seconds;
    }

    public static unsafe uint GetCurrentTrackTimestamp(RtcTrack track)
    {
        var timestamp = 0U;
        NativeRtc.GetCurrentTrackTimestamp(track.Id, &timestamp).ThrowIfError();
        return timestamp;
    }

    public static void SetTrackRtpTimestamp(RtcTrack track, uint timestamp)
    {
        NativeRtc.SetTrackRtpTimestamp(track.Id, timestamp).ThrowIfError();
    }

    public static unsafe uint GetLastTrackSenderReportTimestamp(RtcTrack track)
    {
        var timestamp = 0U;
        NativeRtc.GetLastTrackSenderReportTimestamp(track.Id, &timestamp).ThrowIfError();
        return timestamp;
    }

    public static void SetNeedsToSendRtcpSr(RtcTrack track)
    {
        NativeRtc.SetNeedsToSendRtcpSr(track.Id).ThrowIfError();
    }
}