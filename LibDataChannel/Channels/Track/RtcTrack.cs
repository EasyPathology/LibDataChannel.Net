using LibDataChannel.Connections.Rtc;
using LibDataChannel.Native.Channels.Track;

namespace LibDataChannel.Channels.Track;

public class RtcTrack : RtcChannel
{
    /// <summary>
    ///     The parent peer connection.
    /// </summary>
    public RtcPeerConnection PeerConnection { get; }

    private string? _description;
    private string? _mediaIdentifier;
    private RtcDirection? _direction;

    internal RtcTrack(RtcPeerConnection peerConnection, string mediaDescriptionSdp) 
        : base(NativeRtcTrack.AddTrack(peerConnection, mediaDescriptionSdp))
    {
        PeerConnection = peerConnection;
        PeerConnection.OnTrackAdded(this);
    }
    
    internal RtcTrack(RtcPeerConnection peerConnection, RtcTrackInit init) 
        : base(NativeRtcTrack.AddTrack(peerConnection, init.AllocNative()))
    {
        PeerConnection = peerConnection;
        PeerConnection.OnTrackAdded(this);
    }
    
    internal RtcTrack(RtcPeerConnection peerConnection, int id) : base(id)
    {
        PeerConnection = peerConnection;
        PeerConnection.OnTrackAdded(this);
    }

    protected override void OnDispose()
    {
        PeerConnection.OnTrackClosed(this);
        base.OnDispose();
    }

    protected override void Free()
    {
        NativeRtcTrack.Delete(this);
    }

    /// <summary>
    ///     The description of the track.
    /// </summary>
    public string Description => _description ??= NativeRtcTrack.GetDescription(this);

    /// <summary>
    ///     The mid (media identifier) of the track.
    /// </summary>
    public string MediaIdentifier => _mediaIdentifier ??= NativeRtcTrack.GetMediaIdentifier(this);

    /// <summary>
    ///     The direction of the track.
    /// </summary>
    public RtcDirection Direction => _direction ??= NativeRtcTrack.GetDirection(this);
}