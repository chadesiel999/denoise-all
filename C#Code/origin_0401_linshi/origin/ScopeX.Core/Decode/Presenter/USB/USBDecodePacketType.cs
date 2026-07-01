namespace ScopeX.Core.Decode
{
    public enum USBDecodePacketType
    {
        SYNC,
        Packet,
        PacketData,
        CRC,
        EOP
    }

    /// <Author>
    /// ZXL
    /// </Author>
    public enum USBDecodePacketTypeNew
    {
        SYNC,
        PID,
        ADDR,
        ENDP,
        FRAMENUB,
        DATA,
        CRC,
        EOP
    }


}
