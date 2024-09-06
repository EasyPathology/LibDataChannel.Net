using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibDataChannel.Native.Connections.Rtc;
using LibDataChannel.Native.Exceptions;

namespace LibDataChannel.Native;

public unsafe class NativeRtc
{
#if LINUX
    private const string LibraryName = "libdatachannel.so";
    private const string LibRelativePath = @"runtimes\linux-x64\native\";
#elif OSX
    private const string LibraryName = "libdatachannel.dylib";
    private const string LibRelativePath = @"runtimes\osx-x64\native\";
#else
    private const string LibraryName = "datachannel.dll";
    private const string LibraryRelativePath = @"runtimes\win-x64\native\";
#endif

    static NativeRtc() 
    {
	    NativeLibrary.SetDllImportResolver(typeof(NativeRtc).Assembly, ImportResolver);
    }

    private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
	    return NativeLibrary.Load(Path.Combine(AppContext.BaseDirectory, LibraryRelativePath, libraryName));
    }

    internal const int ErrorInvalidArgument = -1;
    internal const int ErrorFailure = -2;
    internal const int ErrorNotAvailable = -3;
    internal const int ErrorBufferTooSmall = -4;

    [DllImport(LibraryName, EntryPoint = "rtcInitLogger", CallingConvention = CallingConvention.Cdecl)]
    public static extern void InitLogger(RtcLogLevel level, delegate* unmanaged[Cdecl]<RtcLogLevel, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcPreload", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Preload();

    [DllImport(LibraryName, EntryPoint = "rtcCleanup", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Cleanup();

    [DllImport(LibraryName, EntryPoint = "rtcSetUserPointer", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetUserPointer(int id, IntPtr userPointer);
    
    [DllImport(LibraryName, EntryPoint = "rtcCreatePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CreatePeerConnection(IntPtr configuration);
    
    [DllImport(LibraryName, EntryPoint = "rtcDeletePeerConnection", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeletePeerConnection(int peerConnectionId);

    [DllImport(LibraryName, EntryPoint = "rtcSetLocalDescriptionCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalDescription(int peerConnectionId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetLocalCandidateCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetLocalCandidateCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcState, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetGatheringStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetGatheringStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcGatheringState, IntPtr, void> callback);

    [DllImport(LibraryName, EntryPoint = "rtcSetSignalingStateChangeCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetSignalingStateChangeCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, RtcSignalingState, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetDataChannelCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetDataChannelCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetTrackCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetTrackCallback(int peerConnectionId, delegate* unmanaged[Cdecl]<int, int, IntPtr> callback);
    
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
    
    [DllImport(LibraryName, EntryPoint = "rtcSetOpenCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetOpenCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetClosedCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetClosedCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetErrorCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetErrorCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetMessageCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetMessageCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetBufferedAmountLowCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetBufferedAmountLowCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetAvailableCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetAvailableCallback(int channelId, delegate* unmanaged[Cdecl]<int, IntPtr, void> callback);
    
    [DllImport(LibraryName, EntryPoint = "rtcIsOpen", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsOpen(int channelId);
    
    [DllImport(LibraryName, EntryPoint = "rtcIsClosed", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool IsClosed(int channelId);
    
    [DllImport(LibraryName, EntryPoint = "rtcSendMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SendMessage(int channelId, IntPtr message, int size);
    
    [DllImport(LibraryName, EntryPoint = "rtcGetBufferedAmount", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetBufferedAmount(int channelId);
    
    [DllImport(LibraryName, EntryPoint = "rtcSetBufferedAmountLowThreshold", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetBufferedAmountLowThreshold(int channelId, int amount);
    
    [DllImport(LibraryName, EntryPoint = "rtcReceiveMessage", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ReceiveMessage(int channelId, IntPtr buffer, IntPtr pSize);
    
	[DllImport(LibraryName, EntryPoint = "rtcGetAvailableAmount", CallingConvention = CallingConvention.Cdecl)]
	public static extern int GetAvailableAmount(int channelId);
	
	[DllImport(LibraryName, EntryPoint = "rtcCreateDataChannel", CallingConvention = CallingConvention.Cdecl)]
	public static extern int CreateDataChannel(int peerConnectionId, IntPtr label);
	
	[DllImport(LibraryName, EntryPoint = "rtcCreateDataChannelEx", CallingConvention = CallingConvention.Cdecl)]
	public static extern int CreateDataChannel(int peerConnectionId, IntPtr label, IntPtr init);
	
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
	
	[DllImport(LibraryName, EntryPoint = "rtcAddTrack", CallingConvention = CallingConvention.Cdecl)]
	public static extern int AddTrack(int dataChannelId, IntPtr mediaDescriptionSdp);
	
	[DllImport(LibraryName, EntryPoint = "rtcAddTrackEx", CallingConvention = CallingConvention.Cdecl)]
	public static extern int AddTrackEx(int dataChannelId, IntPtr init);
	
	[DllImport(LibraryName, EntryPoint = "rtcDeleteTrack", CallingConvention = CallingConvention.Cdecl)]
	public static extern int DeleteTrack(int trackId);
	
	
	
	// TODO: TRACK & MEDIA & WEBSOCKET
	
	[DoesNotReturn]
	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void ThrowException(int errorCode)
	{
		throw errorCode switch
		{
			ErrorInvalidArgument => new RtcArgumentException(),
			ErrorFailure => new RtcFailureException(),
			ErrorNotAvailable => new RtcNotAvailableException(),
			ErrorBufferTooSmall => new RtcBufferTooSmallException(),
			_ => new RtcException($"RTC error code: {errorCode}"),
		};
	}
}