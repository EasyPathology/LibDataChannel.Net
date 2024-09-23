using System.Runtime.CompilerServices;
using System.Text;
using LibDataChannel.Native.Connections.Rtc;

namespace LibDataChannel.Native.Channels.Data;

public static class NativeRtcDataChannel
{
    /// <summary>
    ///     Creates a new native RTCDataChannel object with the given label and default options.
    /// </summary>
    /// <param name="handle">the parent peer connection handle.</param>
    /// <param name="label">the label.</param>
    /// <returns>the data channel id.</returns>
    public static unsafe int Create(NativeRtcPeerConnectionHandle handle, string label)
    {
        ArgumentNullException.ThrowIfNull(label, nameof(label));
        Span<byte> utf8Label = stackalloc byte[Encoding.UTF8.GetByteCount(label) + 1];
        Encoding.UTF8.GetBytes(label, utf8Label);
        utf8Label[^1] = 0;

        return NativeRtc.CreateDataChannel(handle.Id, (IntPtr)Unsafe.AsPointer(ref utf8Label.GetPinnableReference())).ThrowIfError();
    }

    /// <summary>
    ///     Creates a new native RTCDataChannel object with the given label and default options.
    /// </summary>
    /// <param name="handle">the parent peer connection handle.</param>
    /// <param name="label">the label.</param>
    /// <param name="init">the options.</param>
    /// <returns>the data channel id.</returns>
    public static unsafe int Create(NativeRtcPeerConnectionHandle handle, string label, NativeRtcDataChannelInit init)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(label, nameof(label));
            Span<byte> utf8Label = stackalloc byte[Encoding.UTF8.GetByteCount(label) + 1];
            Encoding.UTF8.GetBytes(label, utf8Label);
            utf8Label[^1] = 0;

            return NativeRtc.CreateDataChannelEx(
                handle.Id,
                (IntPtr)Unsafe.AsPointer(ref utf8Label.GetPinnableReference()),
                (IntPtr)(&init)
            ).ThrowIfError();
        }
        finally
        {
            init.Free();
        }
    }

    /// <summary>
    ///     Deletes the native RTCDataChannel object.
    /// </summary>
    /// <param name="handle">the handle.</param>
    public static void Delete(RtcDataChannel handle)
    {
        NativeRtc.DeleteDataChannel(handle.Id);
    }

    /// <summary>
    ///     Gets the stream id.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the stream id.</returns>
    public static int GetStream(RtcDataChannel handle)
    {
        return NativeRtc.GetDataChannelStream(handle.Id);
    }

    /// <summary>
    ///     Gets the channel label.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the label.</returns>
    public static string GetLabel(RtcDataChannel handle)
    {
        return MarshalUtils.GetString(handle.Id, NativeRtc.GetDataChannelLabel);
    }

    /// <summary>
    ///     Gets the channel sub-protocol.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the sub-protocol name.</returns>
    public static string GetProtocol(RtcDataChannel handle)
    {
        return MarshalUtils.GetString(handle.Id, NativeRtc.GetDataChannelProtocol);
    }

    /// <summary>
    ///     Gets the reliability of the channel.
    /// </summary>
    /// <param name="handle">the handle.</param>
    /// <returns>the reliability options.</returns>
    public static unsafe NativeRtcReliability GetReliability(RtcDataChannel handle)
    {
        NativeRtcReliability reliability;
        NativeRtc.GetDataChannelReliability(handle.Id, (IntPtr)(&reliability)).ThrowIfError();
        return reliability;
    }
}