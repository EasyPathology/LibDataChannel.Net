using System.Runtime.CompilerServices;
using System.Text;
using LibDataChannel.Native.Connections.Rtc;

namespace LibDataChannel.Native.Channels.Track;

public static class NativeRtcTrack
{
    /// <summary>
    ///     Adds a new Track on a Peer Connection. The Peer Connection does not need to be connected, however, the Track will be open only when the Peer Connection is connected.
    /// </summary>
    /// <param name="handle">the parent peer connection handle.</param>
    /// <param name="mediaDescriptionSdp">a null-terminated string specifying the corresponding media SDP. It must start with a m-line and include a mid parameter.</param>
    /// <returns></returns>
    public static unsafe int AddTrack(NativeRtcPeerConnectionHandle handle, string mediaDescriptionSdp)
    {
        ArgumentNullException.ThrowIfNull(mediaDescriptionSdp, nameof(mediaDescriptionSdp));
        Span<byte> utf8Kind = stackalloc byte[Encoding.UTF8.GetByteCount(mediaDescriptionSdp) + 1];
        Encoding.UTF8.GetBytes(mediaDescriptionSdp, utf8Kind);
        utf8Kind[^1] = 0;

        var ret = NativeRtc.AddTrack(handle.Id, (IntPtr) Unsafe.AsPointer(ref utf8Kind.GetPinnableReference()));
        if (ret < 0) NativeRtc.ThrowException(ret);
        return ret;
    }
    
    /// <summary>
    ///     Adds a new Track on a Peer Connection. The Peer Connection does not need to be connected, however, the Track will be open only when the Peer Connection is connected.
    /// </summary>
    /// <param name="handle">the parent peer connection handle.</param>
    /// <param name="init">the options.</param>
    /// <returns></returns>
    public static unsafe int AddTrack(NativeRtcPeerConnectionHandle handle, NativeRtcTrackInit init)
    {
        var ret = NativeRtc.AddTrackEx(handle.Id, (IntPtr) (&init));
        if (ret < 0) NativeRtc.ThrowException(ret);
        return ret;
    }
    
    /// <summary>
    ///     Deletes a Track.
    /// </summary>
    /// <param name="handle">the handle.</param>
    public static void Delete(NativeRtcChannelHandle handle)
    {
        NativeRtc.DeleteTrack(handle.Id);
    }
    
    /// <summary>
    ///     Retrieves the SDP media description of a Track.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the SDP media description.</returns>
    public static string GetDescription(NativeRtcChannelHandle handle)
    {
        return MarshalUtils.GetString(handle.Id, NativeRtc.GetTrackDescription);
    }
    
    /// <summary>
    ///     Retrieves the mid (media identifier) of a Track.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the mid (media identifier).</returns>
    public static string GetMediaIdentifier(NativeRtcChannelHandle handle)
    {
        return MarshalUtils.GetString(handle.Id, NativeRtc.GetTrackMediaIdentifier);
    }
    
    /// <summary>
    ///     Retrieves the direction of a Track.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the direction.</returns>
    public static unsafe RtcDirection GetDirection(NativeRtcChannelHandle handle)
    {
        var direction = RtcDirection.Unknown;
        var errorCode = NativeRtc.GetTrackDirection(handle.Id, &direction);
        if (errorCode < 0) NativeRtc.ThrowException(errorCode);
        return direction;
    }
}