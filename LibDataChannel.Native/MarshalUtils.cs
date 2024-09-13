using System.Runtime.InteropServices;

namespace LibDataChannel.Native;

internal static class MarshalUtils
{
    public delegate int GetStringFunction(int id, IntPtr buffer, int size);

    public static unsafe string GetString(int id, GetStringFunction function)
    {
        var size = function(id, IntPtr.Zero, 0);
        var buffer = stackalloc byte[size];
        var length = function(id, (IntPtr)buffer, size);
        if (length <= 0) NativeRtc.ThrowException(length);
        return Marshal.PtrToStringAnsi((IntPtr) buffer, length - 1);
    }
}