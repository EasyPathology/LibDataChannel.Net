using System.Runtime.InteropServices;
using LibDataChannel.Native.Utils;

namespace LibDataChannel.Native;

public abstract class NativeRtcHandle : IDisposable
{
    protected IntPtr HandlePtr { get; }

    protected bool Disposed { get; private set; }
    protected object SyncRoot => this;

    private GCHandle handle;

    protected NativeRtcHandle()
    {
        handle = GCHandle.Alloc(this, GCHandleType.WeakTrackResurrection);
        HandlePtr = (IntPtr)handle;
    }

    ~NativeRtcHandle()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        lock (SyncRoot)
        {
            if (Disposed)
                return;

            Disposed = true;

            try
            {
                OnDispose();
            }
            finally
            {
                if (ThreadUtils.IsRtcThread())
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Free();
                        handle.Free();
                    });
                }
                else
                {
                    Free();
                    handle.Free();
                }
            }
        }
    }

    protected abstract void OnDispose();
    protected abstract void Free();
}