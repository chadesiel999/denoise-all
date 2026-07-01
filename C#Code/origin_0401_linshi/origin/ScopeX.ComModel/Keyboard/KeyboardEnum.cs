using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    /// <summary>
    /// LED的控制字
    /// </summary>
    public enum LedControlCommand : Byte
    {
        /// <summary>
        /// 控制所有LED的状态
        /// </summary>
        LedStatesControl = 0x01,
        /// <summary>
        /// 单个LED颜色控制
        /// 如果按键板不支持颜色控制，下位机会接受此指令但不会有任何响应
        /// </summary>
        LedColorControl,
        /// <summary>
        /// 单个LED状态控制
        /// </summary>
        LedStateControl,
        /// <summary>
        /// 示波器状态控制
        /// </summary>
        ScopeStateControl,
        /// <summary>
        /// 示波器除了模拟通道外其他通道的状态控制
        /// </summary>
        ScopeOtherControl,
        /// <summary>
        /// 示波器模拟通道状态控制
        /// </summary>
        ScopeAnalogControl,
    }

    /// <summary>
    /// 示波器状态枚举值修改成固定值，避免因示波器软件或按键板单方便增减枚举项导致值不匹配
    /// </summary>
    public enum ScopeStateType : Byte
    {
        RunStop          = 0,
        TriggerModel     = 1,        
        TriggerLedControl= 8, //added by 8000HD触发LED控制         
        TriggerChannel   = 2,
        SelectedChannel  = 3,
        UltraAcq         = 4,
        TouchSceen       = 5,
        Print            = 6,
        Cursor           = 7,
        Close            = 255,
    }

    public enum ControlChannelType : Byte
    {
        Ref,
        Math,
        Bus,
        AWG,
        Digital,
    };
}
