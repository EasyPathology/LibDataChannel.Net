using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibDataChannel.Native;

public static class NativeRtcLogger
{
    private static RtcLogger? _logger; // Keep reference to avoid gc.

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static unsafe void Attach(RtcLogger logger, RtcLogLevel level)
    {
        if (_logger == null) NativeRtc.InitLogger(level, &LogCallback);
        _logger = logger;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static unsafe void Detach()
    {
        NativeRtc.InitLogger(RtcLogLevel.None, null);
    }

    [UnmanagedCallersOnly(CallConvs = new []{ typeof(CallConvCdecl) })]
    [MethodImpl(MethodImplOptions.Synchronized)]
    private static void LogCallback(RtcLogLevel level, IntPtr message)
    {
        _logger?.Log(level, Marshal.PtrToStringAnsi(message));
    }
}