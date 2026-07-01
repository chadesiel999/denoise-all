using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum SMBusFieldType
    {
        FIELD_START,
        FIELD_RESTART,
        FIELD_STOP,

        FIELD_ADDRESS,//地址字段
        FIELD_ADDRESS_WR,//读写字段
        FIELD_COMMAND_CODE,//命令码字段
        FIELD_BYTE_COUNT, //块大小字段
        FIELD_DATA,      //数据字段

        FIELD_DATA_ADDRESS_DEVICE, //1 数据字段_ARP_不显示数值
        FIELD_DATA_UDID_VERSION_1, //1 数据字段_ARP_不显示数值
        FIELD_DATA_RESERVED,      // 2 数据字段_ARP_不显示数值
        FIELD_PEC,//PEC字段
        FIELD_ACKNACK,//应答位字段
        FIELD_ERROR, //错误字段
    };
}
