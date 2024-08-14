namespace LibDataChannel.Native.Exceptions;

using System;

public class RtcException(string message) : Exception(message);