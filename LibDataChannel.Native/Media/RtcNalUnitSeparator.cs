namespace LibDataChannel.Native.Media;

/// <summary>
///     Define how NAL units are separated in a H264/H265 sample
/// </summary>
public enum RtcNalUnitSeparator
{
    /// <summary>
    ///     first 4 bytes are NAL unit length
    /// </summary>
    Length = 0,
    
    /// <summary>
    ///     0x00, 0x00, 0x00, 0x01
    /// </summary>
    LongStartSequence = 1,
    
    /// <summary>
    ///     0x00, 0x00, 0x01
    /// </summary>
    ShortStartSequence = 2,
    
    /// <summary>
    ///     long or short start sequence
    /// </summary>
    StartSequence = 3
}