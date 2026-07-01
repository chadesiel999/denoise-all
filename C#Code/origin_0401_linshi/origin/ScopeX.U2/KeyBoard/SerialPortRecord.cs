using System.ComponentModel;

namespace ScopeX.U2
{
    /// <summary>
    /// 串口设备变更类型枚举
    /// </summary>
    internal enum SerialPortChangeType
    {
        /// <summary>
        /// 插入
        /// </summary>
        [Description("插入")]
        Insert,
        /// <summary>
        /// 拔出
        /// </summary>
        [Description("拔出")]
        Remove
    }

    /// <summary>
    /// 串口设备变更类型枚举
    /// </summary>
    internal record SerialPortRecord(SerialPortChangeType Direction, string SerialPortName);
}
