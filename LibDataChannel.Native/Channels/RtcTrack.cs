using System.Runtime.InteropServices;

namespace LibDataChannel.Native.Channels;

public abstract class RtcTrack(int id) : RtcChannel(id)
{
    public delegate void PliCallback();

    public delegate void RembCallback(uint maxBitrate);
    
    /// <summary>
    ///     Called when a PLI is received.
    /// </summary>
    public event PliCallback? OnPil;

    /// <summary>
    ///     Called when a REMB is received.
    /// </summary>
    public event RembCallback? OnRemb;
    
    internal void InternalHandlePli()
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnPil?.Invoke();
    }
    
    internal void InternalHandleRemb(uint maxBitrate)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnRemb?.Invoke(maxBitrate);
    }
    
    internal static new RtcTrack FromHandle(IntPtr ptr) =>
        GCHandle.FromIntPtr(ptr).Target as RtcTrack ?? throw new InvalidOperationException("Invalid handle");
}