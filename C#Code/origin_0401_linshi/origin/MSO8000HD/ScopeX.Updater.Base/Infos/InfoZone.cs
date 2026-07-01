using System;
using System.Collections.Generic;

namespace ScopeX.Updater.Base
{
    #region 枚举
    public enum InfoType
    {
        GoldenInfo,
        AppInfo,
        Product,
        Data
    }

    public enum InfoSize
    {
        Normal = 64,
        Bigger = 256,
        Extend_512K = 512,
        Extend_1M = 1024,
        Extend_2M = 2048,
        Extend_4M = 4096,
    }
    #endregion
    [Serializable]
    public class InfoIndex
    {
        public UInt32 ID { get; set; }
        public UInt32 InfoAddr { get; set; }

        public InfoType InfoType { get; set; } = InfoType.Data;
        public InfoSize InfoSize { get; set; } = InfoSize.Normal;
        public string CRC16 { get; set; }
    }
    [Serializable]
    public class InfoZone
    {
        public UInt32 BoardID { get; set; }
        public List<InfoIndex> InfoIndexs { get; set; }

        public void Init()
        {
            InfoIndexs = new();
            var golden = new InfoIndex
            {
                ID = 0,
                InfoAddr = UpdaterBaseConstants.FLASH_DATA_START_ADDR,
                InfoSize = InfoSize.Normal,
                InfoType = InfoType.GoldenInfo
            };
            var app = new InfoIndex
            {
                ID = 1,
                InfoAddr = UpdaterBaseConstants.FLASH_DATA_START_ADDR + UpdaterBaseConstants.FLASH_MIN_INFO_SIZE_BYTE,
                InfoSize = InfoSize.Normal,
                InfoType = InfoType.AppInfo
            };
            InfoIndexs.Add(golden);
            InfoIndexs.Add(app);
        }

        public bool IndexsCheck()
        {
            if (InfoIndexs == null || InfoIndexs.Count < 2)
            {
                return false;
            }
            if (InfoIndexs.Count > UpdaterBaseConstants.FLASH_TOTAL_INFO_COUNT)
            {
                return false;
            }
            return true;
        }
    }
}
