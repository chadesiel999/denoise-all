using System;

namespace ScopeX.Updater.Base
{
    public class UpdaterBaseConstants
    {
        public const UInt32 FLASH_DATA_START_ADDR = 0x0270_0000;
        //public const UInt32 FLASH_INFO_INDEX_START_ADDR = 0x0260_0000;
        //public const UInt32 FLASH_IMAGE_APP_START_ADDR = 0x0120_0000;
        public const UInt32 FLASH_MIN_INFO_SIZE_BYTE = SIZE_64K_SECTOR;
        public const UInt32 FLASH_1M_SIZE_BYTE = 1024 * 1024; // 0x10_0000
        public const UInt32 FLASH_TOTAL_INFO_COUNT = FLASH_TOTAL_INFO_SIZE_BYTE / FLASH_MIN_INFO_SIZE_BYTE;
        public const UInt32 FLASH_TOTAL_INFO_SIZE_BYTE = FLASH_1M_SIZE_BYTE;
        public const UInt32 FLASH_IMAGE_APP_MAX_SIZE_BYTE = FLASH_1M_SIZE_BYTE * 16;
        public const UInt32 FLASH_IMAGE_GOLDEN_MAX_SIZE_BYTE = FLASH_1M_SIZE_BYTE * 16;
//#if UPO7000L
//        public const string NOW_PACKAGE_VERSION = "0.0.2";
//        public const string LAST_SUPPORT_PACKAGE_VERSION = "0.0.2";
//#else
        public const string NOW_PACKAGE_VERSION = "2.0.0";
        public const string LAST_SUPPORT_PACKAGE_VERSION = "2.0.0";
//#endif

        public const UInt32 SIZE_64K_SECTOR = 64 * 1024;

    }
}
