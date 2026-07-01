using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal record DpxAcqParameters
    {
        //从最低位（2位）为优先级最低的开始，到高位依次补齐，最高为一定是最高优先级。
        internal Int32 ChannelLevel = 0xf;//每个通道占2位，最高位是优先级是最高的（也就是最Top）。最后一个通道要全部填充后续的优先级为最后一个通道
        internal Int32 PersistenceType = 0;//余晖时间。0=最小（自动）,1=50MS，2=100MS，3=200MS，4=500MS，5=1S，6=2S,7=5S，8=10S，9=20S，10=无限持续
        internal Int32 DrawMode = 0;//bit0:1,矢量  0，点
        internal Int32 WinMode = 0; //2bit:bit0,表示是否zoom;bit1：是否XY
        internal Int32 XYMode_Source = 0;//共4bit.bit3bit2:x的源，bit1bit0:y的源

        internal Int32 InternalTimeByms = 0;//录制播放用的间隔时间，暂时没有使用
        internal Int32 UIMainWinDdr_XStartPos = 0;//目前以一行1250为基数计算。从0开始，0~1249
        internal Int32 UIMainWinDdr_XEndPos = 0;
        internal Int32 UIMainWinExtractNum = 1;//应该以1250为基准进行计算，以插值后的数据为基准进行计算
        internal Int32 UIMainWinMaxHitTimes = 255;//回读的
        internal Int32 UIMainWinMinHitTimes = 0;//回读的

        internal Int32 UISubWinDdr_XStartPos = 0;
        internal Int32 UISubWinDdr_XEndPos = 0;
        internal Int32 UISubWinExtractNum = 1;
        internal Int32 UISubWinMaxHitTimes = 255;
        internal Int32 UISubWinMinHitTimes = 0;

        internal ReadParams? DdrReadParam;
    }
}
