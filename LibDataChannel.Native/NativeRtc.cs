using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using LibDataChannel.Native.Channels.Track;
using LibDataChannel.Native.Connections.Rtc;
using LibDataChannel.Native.Exceptions;

namespace LibDataChannel.Native;

internal static unsafe class NativeRtc
{
    private const string LibraryName = "libdatachannel";
    
    private static string LibraryPath
    {
	    get
	    {
		    if (libraryPath != null) return libraryPath;
            
		    var platform = Environment.OSVersion.Platform switch
		    {
			    PlatformID.Win32NT => "win",
			    PlatformID.Unix => "linux",
			    PlatformID.MacOSX => "osx",
			    _ => throw new PlatformNotSupportedException()
		    };
		    var arch = Environment.Is64BitProcess ? "x64" : "x86";
		    libraryPath = Path.Combine(AppContext.BaseDirectory, "runtimes", $"{platform}-{arch}", "native");
            
		    return libraryPath;
	    }
    }

    private static string? libraryPath;

    static NativeRtc()
    {
	    NativeLibrary.SetDllImportResolver(typeof(NativeRtc).Assembly, static (libraryName, _, _) =>
		    NativeLibrary.Load(Path.Combine(LibraryPath, libraryName)));
    }

    internal enum Error
    {
        InvalidArgument = -1,
        Failure = -2,
        NotAvailable = -3,
        BufferTooSmall = -4
    }

    #region Optional global preload and cleanup

    [DllImport(LibraryName, EntryPoint = "rtcPreload", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Preload();

    [DllImport(LibraryName, EntryPoint = "rtcCleanup", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Cleanup();

    #endregion

    #region Log

    /// <summary>
    ///     NULL cb on the first call will log to stdout
    /// </summary>
    /// <param name="level"></param>
    /// <param name="callback"></param>
    [DllImport(LibraryName, EntryPoint = "rtcInitLogger", CallingConvention = CallingConvention.Cdecl)]
    public static extern void InitLogger(RtcLogLevel level, delegate* unmanaged[Cdecl]<RtcLogLevel, IntPtr, void> callback);

    #endregion

    #region User pointer

    [DllImport(LibraryName, EntryPoint = "rtcSetUserPointer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUserPointer(int id, IntPtr userPointer);

    [DllImport(LibraryName, EntryPoint = "rtcGetUserPointer", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetUserPointer(int id);

    #endregion

    #region PeerConnection

    [DllImport(LibraryName, EntryPoint = "rtcCreatePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CreatePeerConnection(IntPtr configuration);

    [DllImport(LibraryName, EntryPoint = "rtcClosePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClosePeerConnection(int peerConnectionId);

    [DllImport(LibraryName, EntryPoint = "rtcDeletePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeletePeerConnection(int peerConnectionId);

    [DllImport(LibraryName, EntryPoint = "rtcSetLocalDescriptionCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalDescriptionCallback(
        int peerConnectionId,
        delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetLocalCandidateCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalCandidateCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcState, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetGatheringStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetGatheringStateChangeCallback(
        int peerConnectionId,
        delegate* unmanaged[Cdecl]<int, RtcGatheringState, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetSignalingStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetSignalingStateChangeCallback(
        int peerConnectionId,
        delegate* unmanaged[Cdecl]<int, RtcSignalingState, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetLocalDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalDescription(int peerConnectionId, IntPtr type);

    [DllImport(LibraryName, EntryPoint = "rtcSetRemoteDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetRemoteDescription(int peerConnectionId, IntPtr sdp, IntPtr type);

    [DllImport(LibraryName, EntryPoint = "rtcAddRemoteCandidate", CallingConvention = CallingConvention.Cdecl)]
    public static extern int AddRemoteCandidate(int peerConnectionId, IntPtr candidate, IntPtr mid);

    [DllImport(LibraryName, EntryPoint = "rtcGetLocalDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocalDescription(int peerConnectionId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetRemoteDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteDescription(int peerConnectionId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetLocalDescriptionType", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocalDescriptionType(int peerConnectionId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetRemoteDescriptionType", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteDescriptionType(int peerConnectionId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetLocalAddress", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLocalAddress(int peerConnectionId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetRemoteAddress", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteAddress(int peerConnectionId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetSelectedCandidatePair", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetSelectedCandidatePair(int peerConnectionId, IntPtr local, int localSize, IntPtr remote, int remoteSize);

    [DllImport(LibraryName, EntryPoint = "rtcIsNegotiationNeeded", CallingConvention = CallingConvention.Cdecl)]
    public static extern int IsNegotiationNeeded(int peerConnectionId);

    [DllImport(LibraryName, EntryPoint = "rtcGetMaxDataChannelStream", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetMaxDataChannelStream(int peerConnectionId);

    [DllImport(LibraryName, EntryPoint = "rtcGetRemoteMaxMessageSize", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetRemoteMaxMessageSize(int peerConnectionId);

    #endregion

    #region DataChannel, Track, and WebSocket common API

    [DllImport(LibraryName, EntryPoint = "rtcSetOpenCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetOpenCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetClosedCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetClosedCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetErrorCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetErrorCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetMessageCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetMessageCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, int, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSendMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SendMessage(int channelId, IntPtr message, int size);

    [DllImport(LibraryName, EntryPoint = "rtcClose", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Close(int channelId);

    [DllImport(LibraryName, EntryPoint = "rtcDelete", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Delete(int channelId);

    [DllImport(LibraryName, EntryPoint = "rtcIsOpen", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsOpen(int channelId);

    [DllImport(LibraryName, EntryPoint = "rtcIsClosed", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsClosed(int channelId);

    [DllImport(LibraryName, EntryPoint = "rtcGetMaxMessageSize", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetMaxMessageSize(int peerConnectionId);

    [DllImport(LibraryName, EntryPoint = "rtcGetBufferedAmount", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetBufferedAmount(int channelId);

    [DllImport(LibraryName, EntryPoint = "rtcSetBufferedAmountLowThreshold", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetBufferedAmountLowThreshold(int channelId, int amount);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetBufferedAmountLowCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetBufferedAmountLowCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);

    #endregion

    #region DataChannel, Track, and WebSocket common extended API

    [DllImport(LibraryName, EntryPoint = "rtcGetAvailableAmount", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetAvailableAmount(int channelId);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetAvailableCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetAvailableCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcReceiveMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ReceiveMessage(int channelId, IntPtr buffer, IntPtr pSize);

    #endregion
    
    #region DataChannel

    [DllImport(LibraryName, EntryPoint = "rtcSetDataChannelCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetDataChannelCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcCreateDataChannel", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CreateDataChannel(int peerConnectionId, IntPtr label);

    [DllImport(LibraryName, EntryPoint = "rtcCreateDataChannelEx", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CreateDataChannelEx(int peerConnectionId, IntPtr label, IntPtr init);

    [DllImport(LibraryName, EntryPoint = "rtcDeleteDataChannel", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeleteDataChannel(int dataChannelId);

    [DllImport(LibraryName, EntryPoint = "rtcGetDataChannelStream", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetDataChannelStream(int dataChannelId);

    [DllImport(LibraryName, EntryPoint = "rtcGetDataChannelLabel", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetDataChannelLabel(int dataChannelId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetDataChannelProtocol", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetDataChannelProtocol(int dataChannelId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetDataChannelReliability", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetDataChannelReliability(int dataChannelId, IntPtr reliability);

    #endregion

    #region Track

    [DllImport(LibraryName, EntryPoint = "rtcSetTrackCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetTrackCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, int, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcAddTrack", CallingConvention = CallingConvention.Cdecl)]
    public static extern int AddTrack(int peerConnectionId, IntPtr mediaDescriptionSdp);

    [DllImport(LibraryName, EntryPoint = "rtcAddTrackEx", CallingConvention = CallingConvention.Cdecl)]
    public static extern int AddTrackEx(int peerConnectionId, IntPtr init);

    [DllImport(LibraryName, EntryPoint = "rtcDeleteTrack", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeleteTrack(int trackId);

    [DllImport(LibraryName, EntryPoint = "rtcGetTrackDescription", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetTrackDescription(int trackId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetTrackMid", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetTrackMediaIdentifier(int trackId, IntPtr buffer, int size);

    [DllImport(LibraryName, EntryPoint = "rtcGetTrackDirection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetTrackDirection(int trackId, RtcDirection* direction);

    [DllImport(LibraryName, EntryPoint = "rtcRequestKeyframe", CallingConvention = CallingConvention.Cdecl)]
    public static extern int RequestKeyframe(int trackId);

    [DllImport(LibraryName, EntryPoint = "rtcRequestBitrate", CallingConvention = CallingConvention.Cdecl)]
    public static extern int RequestBitrate(int trackId, uint bitrate);

    #endregion

    #region Media

    /// <summary>
    /// 	Allocate a new opaque message.
    /// 	Must be explicitly freed by rtcDeleteOpaqueMessage() unless
    /// 	explicitly returned by a media interceptor callback;
    /// </summary>
    /// <param name="data"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcCreateOpaqueMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateOpaqueMessage(IntPtr data, int size);

    [DllImport(LibraryName, EntryPoint = "rtcDeleteOpaqueMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern void DeleteOpaqueMessage(IntPtr msg);

    /// <summary>
    ///		Set MediaInterceptor on peer connection
    /// </summary>
    /// <param name="peerConnectionId"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcSetMediaInterceptorCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetMediaInterceptorCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, IntPtr, int, IntPtr, IntPtr> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetH264Packetizer", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetH264Packetizer(int trackId, IntPtr init);

    [DllImport(LibraryName, EntryPoint = "rtcSetH265Packetizer", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetH265Packetizer(int trackId, IntPtr init);

    [DllImport(LibraryName, EntryPoint = "rtcSetAV1Packetizer", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetAv1Packetizer(int trackId, IntPtr init);

    [DllImport(LibraryName, EntryPoint = "rtcSetOpusPacketizer", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetOpusPacketizer(int trackId, IntPtr init);

    [DllImport(LibraryName, EntryPoint = "rtcSetAACPacketizer", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetAacPacketizer(int trackId, IntPtr init);
    
    /// <summary>
    ///     Chain RtcpReceivingSession on track
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcChainRtcpReceivingSession", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ChainRtcpReceivingSession(int trackId);
    
    /// <summary>
    ///     Chain RtcpSrReporter on track
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcChainRtcpSrReporter", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ChainRtcpSrReporter(int trackId);
    
    /// <summary>
    ///     Chain RtcpNackResponder on track
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="maxStoredPacketsCount"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcChainRtcpNackResponder", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ChainRtcpNackResponder(int trackId, uint maxStoredPacketsCount);
    
    /// <summary>
    ///     Chain PliHandler on track
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcChainPliHandler", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ChainPliHandler(int trackId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    /// <summary>
    ///     Chain RembHandler on track
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcChainRembHandler", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ChainRembHandler(int trackId, delegate* unmanaged[Cdecl]<int, uint, IntPtr, void> callback);
    
    /// <summary>
    ///     Transform seconds to timestamp using track's clock rate, result is written to timestamp
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="seconds"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcTransformSecondsToTimestamp", CallingConvention = CallingConvention.Cdecl)]
    public static extern int TransformSecondsToTimestamp(int trackId, double seconds, uint* timestamp);
    
    /// <summary>
    ///     Transform timestamp to seconds using track's clock rate, result is written to seconds
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="timestamp"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcTransformTimestampToSeconds", CallingConvention = CallingConvention.Cdecl)]
    public static extern int TransformTimestampToSeconds(int trackId, uint timestamp, double* seconds);
    
    /// <summary>
    ///     Get current timestamp, result is written to timestamp
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcGetCurrentTrackTimestamp", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetCurrentTrackTimestamp(int trackId, uint* timestamp);
    
    /// <summary>
    ///     Set RTP timestamp for track identified by given id
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcSetTrackRtpTimestamp", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetTrackRtpTimestamp(int trackId, uint timestamp);
    
    /// <summary>
    ///     Get timestamp of last RTCP SR, result is written to timestamp
    /// </summary>
    /// <param name="trackId"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcGetLastTrackSenderReportTimestamp", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLastTrackSenderReportTimestamp(int trackId, uint* timestamp);
    
    /// <summary>
    ///     Set NeedsToReport flag in RtcpSrReporter handler identified by given track id
    /// </summary>
    /// <param name="trackId"></param>
    /// <returns></returns>
    [DllImport(LibraryName, EntryPoint = "rtcSetNeedsToSendRtcpSr", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetNeedsToSendRtcpSr(int trackId);

    #endregion

    // TODO: WEBSOCKET

    [DoesNotReturn]
    public static void ThrowException(int errorCode)
    {
        throw (Error)errorCode switch
        {
            Error.InvalidArgument => new RtcArgumentException(),
            Error.Failure => new RtcFailureException(),
            Error.NotAvailable => new RtcNotAvailableException(),
            Error.BufferTooSmall => new RtcBufferTooSmallException(),
            _ => new RtcException($"RTC error code: {errorCode}"),
        };
    }
    
    public static int ThrowIfError(this int errorCode)
    {
        if (errorCode < 0)
        {
            ThrowException(errorCode);
        }

        return errorCode;
    }
}