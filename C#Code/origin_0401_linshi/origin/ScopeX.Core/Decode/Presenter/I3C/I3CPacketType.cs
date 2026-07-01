using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum I3CFieldType
    {
        FIELD_TYPE_START,       // 包头 绿色
        FIELD_TYPE_RESTART,     // 包头 绿色
        FIELD_TYPE_END,         // 停止 红色
        FIELD_TYPE_ADDR,        // 地址 黄色
        FIELD_TYPE_COMMAND,     // 命令 黄色
        FIELD_TYPE_DATA,        // 数据 青色
        FIELD_TYPE_EXPAND,      // 扩展字段 紫色  
        FIELD_TYPE_ERROR,
    };

    //FIELD_ARBITRATION_START,
    //    FIELD_CONNECT_BIT,
    //    FIELD_ALERT_BIT,
}
