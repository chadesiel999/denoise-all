using System;
using ScopeX.ComModel;

namespace ScopeX.Updater.Base
{
    #region 枚举 
    public enum FirmwareType
    {
        GoldenImage = 0,
        AppImage = 1,
        None = 999
    }
    #endregion
    [Serializable]
    public class ImageBlock : BaseDataBlock
    {
        public FirmwareType FirmwareType { get; set; }
        public UInt32 StartAddr { get; set; }
        public UInt32 SizeBytes { get; set; }
        public HardwareVersionInfo Version { get; set; }
        public HardwareVersionInfo RequiredDriveVersion { get; set; }
    }
}
