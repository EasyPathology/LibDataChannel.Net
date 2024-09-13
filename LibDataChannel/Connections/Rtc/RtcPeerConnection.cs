using System.Diagnostics;
using System.Net;
using LibDataChannel.Channels.Data;
using LibDataChannel.Channels.Track;
using LibDataChannel.Native.Connections.Rtc;
using LibDataChannel.Native.Sdp;

namespace LibDataChannel.Connections.Rtc;

/// <summary>
///     Represents a RTC Peer Connection.
/// </summary>
[DebuggerDisplay("RtcPeerConnection({Id})")]
public class RtcPeerConnection : NativeRtcPeerConnectionHandle
{
    public delegate void DataChannelCallback(RtcDataChannel channel);
    public delegate void TrackCallback(RtcTrack track);
    public delegate void LocalDescriptionCallback(SdpMessage sdp);
    public delegate void LocalCandidateCallback(RtcIceCandidate candidate);
    public delegate void StateChangeCallback(RtcState state);
    public delegate void GatheringStateChangeCallback(RtcGatheringState state);
    public delegate void SignalingStateChangeCallback(RtcSignalingState state);
    
    private readonly List<RtcDataChannel> _dataChannels;
    private readonly List<RtcTrack> _tracks;

    /// <summary>
    ///     State of the peer connection.
    /// </summary>
    public RtcState State { get; private set; }
    
    /// <summary>
    ///     State of the gathering process.
    /// </summary>
    public RtcGatheringState GatheringState { get; private set; }
    
    /// <summary>
    ///     State of the signaling process.
    /// </summary>
    public RtcSignalingState SignalingState { get; private set; }
    
    /// <summary>
    ///     List of data channels created by this peer connection. Thread safe.
    /// </summary>
    public IReadOnlyList<RtcDataChannel> DataChannels
    {
        get
        {
            lock (SyncRoot)
            {
                return _dataChannels.ToList();
            }
        }
    }
    
    /// <summary>
    ///    List of tracks created by this peer connection. Thread safe.
    /// </summary>
    public IReadOnlyList<RtcTrack> Tracks
    {
        get
        {
            lock (SyncRoot)
            {
                return _tracks.ToList();
            }
        }
    }
    
    /// <summary>
    ///     Retrieves the maximum message size for data channels on the peer connection as negotiated with the remote peer.
    /// </summary>
    public int RemoteMaxMessageSize => NativeRtcPeerConnection.GetRemoteMaxMessageSize(this);
    
    /// <summary>
    ///     Retrieves the maximum message size for the channel.
    /// </summary>
    public int MaxMessageSize => NativeRtcPeerConnection.GetMaxMessageSize(this);

    /// <summary>
    ///     Callback for when a new data channel is created by the host.
    /// </summary>
    public event DataChannelCallback? OnDataChannel;

    /// <summary>
    ///     Callback for when a new track is created by the host.
    /// </summary>
    public event TrackCallback? OnTrack;
    
    /// <summary>
    ///     Callback for when the local description is set.
    /// </summary>
    public event LocalDescriptionCallback? OnLocalDescription;
    
    /// <summary>
    ///     Callback for when the local candidate is set.
    /// </summary>
    public event LocalCandidateCallback? OnLocalCandidate;
    
    /// <summary>
    ///     Callback for when the state of the peer connection changes.
    /// </summary>
    public event StateChangeCallback? OnStateChange;
    
    /// <summary>
    ///     Callback for when the state of the gathering process changes.
    /// </summary>
    public event GatheringStateChangeCallback? OnGatheringStateChange;
    
    /// <summary>
    ///     Callback for when the state of the signaling process changes.
    /// </summary>
    public event SignalingStateChangeCallback? OnSignalingStateChange;

    /// <summary>
    ///     Creates a new RTC Peer Connection.
    /// </summary>
    /// <param name="configuration">the configuration.</param>
    public RtcPeerConnection(RtcPeerConfiguration? configuration = null) 
        : base(NativeRtcPeerConnection.Create(configuration != null ? configuration.AllocNative() : default))
    {
        _dataChannels = new List<RtcDataChannel>();
        _tracks = new List<RtcTrack>();
    }

    protected override void OnDispose()
    {
        _dataChannels.ForEach(c => c.Dispose());
        _dataChannels.Clear();
        
        State = RtcState.Closed;
        GatheringState = RtcGatheringState.New;
        SignalingState = RtcSignalingState.Stable;
        
        base.OnDispose();
    }
    
    public bool HasLocalDescription => NativeRtcPeerConnection.HasLocalDescription(this);

    /// <summary>
    ///     Local description of the peer connection.
    /// </summary>
    public SdpMessage LocalDescription => new SdpMessage(
        NativeRtcPeerConnection.GetLocalDescriptionType(this),
        NativeRtcPeerConnection.GetLocalDescription(this));

    public bool HasRemoteDescription => NativeRtcPeerConnection.HasRemoteDescription(this);
    
    /// <summary>
    ///     Remote description of the peer connection.
    /// </summary>
    public SdpMessage RemoteDescription
    {
        get => new SdpMessage(
            NativeRtcPeerConnection.GetRemoteDescriptionType(this),
            NativeRtcPeerConnection.GetRemoteDescription(this));
        set => NativeRtcPeerConnection.SetRemoteDescription(this, value);
    }
    
    /// <summary>
    ///     Local ip endpoint of the peer connection.
    /// </summary>
    public IPEndPoint LocalAddress => NativeRtcPeerConnection.GetLocalAddress(this);
    
    /// <summary>
    ///     Remote ip endpoint of the peer connection.
    /// </summary>
    public IPEndPoint RemoteAddress => NativeRtcPeerConnection.GetRemoteAddress(this);

    /// <summary>
    ///     Gets the selected candidate pair (local and remote) for the peer connection.
    /// </summary>
    /// <param name="local">the local candidate.</param>
    /// <param name="remote">the remote candidate.</param>
    public void GetSelectedCandidatePair(out string local, out string remote)
    {
        NativeRtcPeerConnection.GetSelectedCandidatePair(this, out local, out remote);
    }
    
    /// <summary>
    ///     Adds a new ice candidate to the peer connection.
    /// </summary>
    /// <param name="candidate">the candidate.</param>
    public void AddRemoteCandidate(RtcIceCandidate candidate)
    {
        NativeRtcPeerConnection.AddRemoteCandidate(this, candidate);
    }

    /// <summary>
    ///     Creates a new data channel with the given label.
    /// </summary>
    /// <param name="label">the label.</param>
    /// <returns>the data channel created.</returns>
    public RtcDataChannel CreateDataChannel(string label) => new RtcDataChannel(this, label);
    
    /// <summary>
    ///     Creates a new data channel with the given label and configuration.
    /// </summary>
    /// <param name="label">the label.</param>
    /// <param name="configuration">the options.</param>
    /// <returns>the data channel created.</returns>
    public RtcDataChannel CreateDataChannel(string label, RtcDataChannelInit configuration) => new RtcDataChannel(this, label, configuration);

    /// <summary>
    ///    Adds a new track to the peer connection.
    /// </summary>
    /// <param name="mediaDescriptionSdp">the media Description sdp.</param>
    /// <returns></returns>
    public RtcTrack AddTrack(string mediaDescriptionSdp) => new RtcTrack(this, mediaDescriptionSdp);
    
    /// <summary>
    ///   Adds a new track to the peer connection.
    /// </summary>
    /// <param name="init">the options.</param>
    /// <returns></returns>
    public RtcTrack AddTrack(RtcTrackInit init) => new RtcTrack(this, init);
    
    /// <summary>
    ///     Creates a new offer to be sent to the remote peer.
    /// </summary>
    public void CreateOffer() => NativeRtcPeerConnection.SetLocalDescription(this, SdpType.Offer);
    
    /// <summary>
    ///     Creates an answer to the remote offer.
    /// </summary>
    public void CreateAnswer() => NativeRtcPeerConnection.SetLocalDescription(this, SdpType.Answer);

    /// <summary>
    ///   Closes the peer connection.
    /// </summary>
    public void Close() => NativeRtcPeerConnection.Close(this);
    
    protected override void Internal_OnLocalDescription(SdpMessage description)
    {
        OnLocalDescription?.Invoke(description);
    }

    protected override void Internal_OnLocalCandidate(RtcIceCandidate candidate)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
        }
        
        OnLocalCandidate?.Invoke(candidate);
    }

    protected override void Internal_StateChangeCallback(RtcState state)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
            
            State = state;
        }

        OnStateChange?.Invoke(state);
    }

    protected override void Internal_GatheringStateCallback(RtcGatheringState state)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;

            GatheringState = state;
        }

        OnGatheringStateChange?.Invoke(state);
    }
    
    protected override void Internal_SignalingStateCallback(RtcSignalingState state)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;

            SignalingState = state;
        }

        OnSignalingStateChange?.Invoke(state);
    }

    protected override void Internal_DataChannelCallback(int dataChannelId)
    {
        RtcDataChannel dataChannel;
        
        lock (SyncRoot)
        {
            if (Disposed)
                return;

            dataChannel = new RtcDataChannel(this, dataChannelId);
        }
        
        OnDataChannel?.Invoke(dataChannel);
    }

    protected override void Internal_TrackCallback(int trackId)
    {
        RtcTrack track;
        
        lock (SyncRoot)
        {
            if (Disposed)
                return;

            track = new RtcTrack(this, trackId);
        }
        
        OnTrack?.Invoke(track);
    }

    internal void OnDataChannelClosed(RtcDataChannel channel)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
            
            _dataChannels.Remove(channel);
        }
    }
    
    internal void OnDataChannelAdded(RtcDataChannel channel)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
            
            _dataChannels.Add(channel);
        }
    }

    internal void OnTrackClosed(RtcTrack track)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
            
            _tracks.Remove(track);
        }
    }
    
    internal void OnTrackAdded(RtcTrack track)
    {
        lock (SyncRoot)
        {
            if (Disposed)
                return;
            
            _tracks.Add(track);
        }
    }
}