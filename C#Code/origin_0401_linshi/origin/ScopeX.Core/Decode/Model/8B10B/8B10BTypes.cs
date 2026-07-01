using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace ScopeX.Core.Decode
{
    internal partial class D8B10BDecodeModel
    {
        public enum Kcode//K码类型
        { 
            KNone,
            K280,//K_28_0
            K281,//K_28_1
            K282,//K_28_2
            K283,//K_28_3
            K284,//K_28_4
            K285,//K_28_5
            K286,//K_28_6
            K287,//K_28_7
            K237,//K_23_7
            K277,//K_27_7
            K297,//K_29_7
            K307,//K_30_7
        }

        public enum SymbolPolarity//符号极性
        { 
            SymbolNone,//未判断
            SymbolPositive,//正极性
            SymbolNegative,//负极性
        }

        public enum BalanceCodeJudgment//编码平衡度
        { 
            BalanceNone,
            ImperfectlyBalanceCode,//不平衡编码
            PerfectlyBalanceCode,//平衡码
        }

        public struct SymbolCode//符号
        {
            public Kcode Kcode;
            public ushort Value;
            public SymbolPolarity Polarity;
            public BalanceCodeJudgment Balance;

            public SymbolCode()
            {
                Kcode = Kcode.KNone;
                Value = 0;
                Polarity = SymbolPolarity.SymbolNone;
                Balance = BalanceCodeJudgment.BalanceNone;
            }
        }

        public enum D8B10BStatusType//解码状态
        { 
            D8B10BNoError,
            D8B10B6bitDisparityError,//6bit极性错误
            D8B10B4bitDisparityError,//4bit极性错误
            D8B10BDSymbolError,//符号错误
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct D8B10BEvent//事件包
        {
            public SymbolCode Symbol;//解码符号

            public UInt64 StartIndex;//起始索引
            public UInt64 EndIndex;//结束索引

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] RawData;//未解码的原始数据

            public D8B10BStatusType Error;//错误状态

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct D8B10BOptions
        {
            public IntPtr CancelFlag;//取消解码标志

            public Double BaudRate;//波特率
        };
        

        public struct D8B10BResult
        {
            public IntPtr D8B10BEvent;
            public UInt32 EventCount;
            public SerialProtocolType ProtocolType; // 协议类型
        };
        

    }
}
