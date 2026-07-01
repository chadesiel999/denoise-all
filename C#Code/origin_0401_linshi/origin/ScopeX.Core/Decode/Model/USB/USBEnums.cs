namespace ScopeX.Core.Decode;

public class USBEnums
{
    public enum EventInfoTitles
    {
        UNKNOW,
        INDEX,
        START_TIME,
        SYNC,
        SOF,
        SETUP,
        PID,
        DATA0,
        DATA1,
        DATA2,
        MDATA,
        ADDR,
        ACK,
        //IN,关键字
        TIN,
        //OUT,关键字
        TOUT,
        NAK,
        STALL,
        NYET,
        FNUM,
        CRC5,
        CRC16,
        EOP,
        //ERROR, 关键字
        ERR
    }
}
