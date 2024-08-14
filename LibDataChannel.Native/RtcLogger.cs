namespace LibDataChannel.Native;

public abstract class RtcLogger
{
    public abstract void Log(RtcLogLevel level, string? message);
}