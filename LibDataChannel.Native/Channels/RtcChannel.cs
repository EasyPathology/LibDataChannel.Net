﻿using System.Runtime.InteropServices;

namespace LibDataChannel.Native.Channels;

/// <summary>
///     Handle for a data channel.
/// </summary>
public abstract class RtcChannel : NativeRtcHandle
{
    /// <summary>
    ///     The channel id.
    /// </summary>
    public int Id { get; }

    public delegate void OpenCallback();

    public delegate void ClosedCallback();

    public delegate void ErrorCallback(string? error);

    public delegate void MessageCallback(ReadOnlySpan<byte> message);

    public delegate void BufferedAmountLowCallback();

    public delegate void AvailableCallback();

    private bool _isMessageEventSet;

    private event MessageCallback? OnMessagePrivateCallback;

    /// <summary>
    ///     Called when the channel is opened.
    /// </summary>
    public event OpenCallback? OnOpen;

    /// <summary>
    ///     Called when the channel is closed.
    /// </summary>
    public event ClosedCallback? OnClosed;

    /// <summary>
    ///     Called when an error occurs.
    /// </summary>
    public event ErrorCallback? OnError;

    /// <summary>
    ///     Called when the buffered amount of data is low.
    /// </summary>
    public event BufferedAmountLowCallback? OnBufferedAmountLow;

    /// <summary>
    ///     Called when data is available.
    /// </summary>
    public event AvailableCallback? OnAvailable;

    /// <summary>
    ///     Called when a message is received.
    /// </summary>
    public event MessageCallback? OnMessage
    {
        add
        {
            lock (SyncRoot)
            {
                if (!Disposed && !_isMessageEventSet)
                {
                    _isMessageEventSet = true;
                    NativeRtcChannel.AttachMessageCallback(this);
                }
            }

            OnMessagePrivateCallback += value;
        }
        remove
        {
            OnMessagePrivateCallback -= value;

            lock (SyncRoot)
            {
                if (!Disposed && _isMessageEventSet && OnMessagePrivateCallback == null)
                {
                    _isMessageEventSet = false;
                    NativeRtcChannel.DetachMessageCallback(this);
                }
            }
        }
    }

    protected override void OnDispose()
    {
        lock (SyncRoot)
        {
            if (_isMessageEventSet)
            {
                NativeRtcChannel.DetachMessageCallback(this);
            }
        }
    }

    /// <summary>
    ///     Gets whether the channel is open.
    /// </summary>
    public bool IsOpen => NativeRtcChannel.IsOpen(this);

    /// <summary>
    ///     Gets whether the channel is closed.
    /// </summary>
    public bool IsClosed => NativeRtcChannel.IsClosed(this);

    /// <summary>
    ///     Gets the buffered amount of data waiting to be sent.
    /// </summary>
    public int BufferedAmount => NativeRtcChannel.GetBufferedAmount(this);

    /// <summary>
    ///     Gets the available amount of data waiting to be received.
    /// </summary>
    public int AvailableAmount => NativeRtcChannel.GetAvailableAmount(this);

    /// <summary>
    ///     Sets the buffered amount low threshold before the <see cref="OnBufferedAmountLow"/> event is fired.
    /// </summary>
    public int BufferedAmountLowThreshold
    {
        set => NativeRtcChannel.SetBufferedAmountLowThreshold(this, value);
    }

    /// <summary>
    ///     Sends the given message.
    /// </summary>
    /// <param name="message">the message to send.</param>
    public void SendMessage(ReadOnlySpan<byte> message) => NativeRtcChannel.SendMessage(this, message);

    /// <summary>
    ///     Tries to receive a buffered message.
    /// </summary>
    /// <param name="buffer">the buffer to put the message content.</param>
    /// <param name="length">the length of the message received.</param>
    /// <returns>true if a message has been received.</returns>
    /// <exception cref="InvalidOperationException">throw if an event is already registered to <see cref="OnMessage"/>.</exception>
    public bool TryReceiveMessage(Span<byte> buffer, out int length)
    {
        lock (SyncRoot)
        {
            if (_isMessageEventSet)
                throw new InvalidOperationException("Cannot call TryReceiveMessage when OnMessage is set.");
        }

        return NativeRtcChannel.TryReceiveMessage(this, buffer, out length);
    }

    protected RtcChannel(int id)
    {
        Id = id;

        NativeRtcChannel.SetHandle(this, HandlePtr);
        NativeRtcChannel.AttachCallbacks(this);
    }

    internal void InternalHandleOnOpen()
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnOpen?.Invoke();
    }

    internal void InternalHandleOnClosed()
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnClosed?.Invoke();
    }

    internal void InternalHandleOnError(string? error)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnError?.Invoke(error);
    }

    internal void InternalHandleOnMessage(ReadOnlySpan<byte> message)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnMessagePrivateCallback?.Invoke(message);
    }

    internal void InternalHandleOnBufferedAmountLow()
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnBufferedAmountLow?.Invoke();
    }

    internal void InternalHandleOnAvailable()
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }

        OnAvailable?.Invoke();
    }

    internal static RtcChannel FromHandle(IntPtr ptr) =>
        GCHandle.FromIntPtr(ptr).Target as RtcChannel ?? throw new InvalidOperationException("Invalid handle");
}