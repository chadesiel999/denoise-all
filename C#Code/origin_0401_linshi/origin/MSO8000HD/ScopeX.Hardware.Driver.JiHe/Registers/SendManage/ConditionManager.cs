using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Registers.SendManage
{
    /// <summary>
    /// 寄存器发送条件管理器
    /// </summary>
    internal static class ConditionManager
    {
        /// <summary>
        /// 属性更改事件
        /// </summary>
        public static event Action<String>? ConditionChanged;

        private static Boolean _TriggerCtrlEn= false;
        /// <summary>
        /// 触发是否打开的状态；True-有触发，Flase-无触发；
        /// </summary>
        public static Boolean TriggerCtrlEn 
        { 
            get => _TriggerCtrlEn;
            set
            {
                _TriggerCtrlEn = value;
                OnConditionChanged();
            }
        } 

        private static Boolean _ToolAcqDigitalTrigEn = false;
        /// <summary>
        /// Tool工具设置的触发是否打开状态；True-使能，Flase-关闭；
        /// </summary>
        public static Boolean ToolAcqDigitalTrigEn 
        { 
            get => _ToolAcqDigitalTrigEn;
            set
            {
                _ToolAcqDigitalTrigEn = value;
                OnConditionChanged();
            }
        }

        private static Boolean _ToolProcDigitalTrigEn = false;
        /// <summary>
        /// Tool工具设置的触发是否打开状态；True-使能，Flase-关闭；
        /// </summary>
        public static Boolean ToolProcDigitalTrigEn
        {
            get => _ToolProcDigitalTrigEn;
            set
            {
                _ToolProcDigitalTrigEn = value;
                OnConditionChanged();
            }
        }

        private static Boolean _IsFromDDR = false;
        /// <summary>
        /// 数据是否从DDR来；True-DDR，Flase-FIFO；
        /// </summary>
        public static Boolean IsFromDDR 
        { 
            get => _IsFromDDR;
            set
            {
                _IsFromDDR = value;
                OnConditionChanged();
            }
        }

        private static Boolean _IsExtractEn = false;
        /// <summary>
        /// 是否有前抽；True-前抽打开，Flase-前抽关闭；
        /// </summary>
        public static Boolean IsExtractEn 
        { 
            get => _IsExtractEn; 
            set
            {
                _IsExtractEn = value;
                OnConditionChanged();
            }
        }

        private static ChannelId _TriggerSrc = ChannelId.C1;
        /// <summary>
        /// 当前的触发源通道号；ChannelId-触发源的通道号；
        /// </summary>
        public static ChannelId TriggerSrc 
        { 
            get => _TriggerSrc; 
            set
            {
                _TriggerSrc = value;
                OnConditionChanged();
            }
        }
//cij_0403
        private static Boolean _ToolDspEn = true;
        /// <summary>
        /// Tool工具是否打开Dsp;True-使能，Flase-关闭；
        /// </summary>
        public static Boolean ToolDspEn 
        { 
            get => _ToolDspEn;
            set
            {
                _ToolDspEn = value;
                OnConditionChanged();
            }
        }

        private static Boolean _ToolDspProEn = true;
        /// <summary>
        /// Tool工具是否打开Dsp;True-使能，Flase-关闭；
        /// </summary>
        public static Boolean ToolDspProEn
        {
            get => _ToolDspProEn;
            set
            {
                _ToolDspProEn = value;
                OnConditionChanged();
            }
        }

        private static Boolean _InterpEn = false;
        /// <summary>
        /// 插值是否打开的状态; True-插值打开，Flase-插值关闭；
        /// </summary>
        public static Boolean InterpEn 
        {
            get  => _InterpEn;
            set
            {
                _InterpEn = value;
                OnConditionChanged();
            }
        }

        private static void OnConditionChanged([CallerMemberName] String propertyName = "")
        {
            ConditionChanged?.Invoke(propertyName);
        }
    }
}
