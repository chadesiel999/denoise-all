using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal class Abstract_LongStorage
    {
        public LongStorageAcquirerAttribute? AcqAttribute;

        protected virtual UInt64 MaxPerDataByps
        {
            get;
            set;
        } = 50;//20G 采样

        public virtual Int32 ReadCollectedFrameCnt()
        {
            return 0;
        }

        public virtual void CreateAcquireAttribute(AbstractAcquirer? curAcquire) { }

        public readonly List<TrigAddrInfo> TrigAddressTable = new();
        /// <summary>
        /// 上电初始化
        /// </summary>
        public virtual void Init() { }
        public virtual void InitAcq() { }
        public virtual bool IsFull() => false;

        public virtual void SwitchPingpong() { }

        public virtual bool ReadTrigPosTable() => false;

        /// <summary>
        /// 从DDR中读取数据，读取后必须重新调用一次InitAcq
        /// </summary>
        /// <returns></returns>
        public virtual bool ReadAcqedData() => false;

        /// <summary>
        /// 获取波形数据，需要内部判断是从软件缓存区读取，还是重新从DDR里读取
        /// </summary>
        /// <returns></returns>
        public virtual bool TakeWave() => false;

        public virtual bool ReadSourceData(int channelID, double startTimeBySecond, double endTimeBySecond, out List<ushort> outData)
        {
            outData = new();

            return false;
        }
        public virtual bool TryTakeDdrSourceWave(Int32 channel, double startTimeBySecond, double totalTime, [NotNullWhen(true)] out List<ushort> waveData, [NotNullWhen(true)] out WfmSampleInfo wfmSampleInfo)
        {
            waveData = new List<ushort>();
            wfmSampleInfo = new WfmSampleInfo() { SampleIntervalByus = 0.5 };
            return false;
        }
        public virtual bool TryTakeSegmentWave(Int32 channelIndex, Int32 segmentIndex, out List<ushort> waveData)
        {
            waveData = new List<ushort>();
            return false;
        }
        public virtual void PostProcess() { }

        public class TrigAddrInfo
        {
            public TrigAddrInfo(UInt32 trigAddrRough, UInt32 trigAddrDetail, Double frameTimeByns/*, UInt32 trigAddrOffset*/)
            {
                TrigAddrRough = trigAddrRough;
                TrigAddrDetail = trigAddrDetail;
                FrameTimeByns = frameTimeByns;
                //TrigAddrOffset = trigAddrOffset;
            }

            public UInt32 TrigAddrRough;
            public UInt32 TrigAddrDetail;
            //public UInt32 TrigAddrOffset;
            public Double FrameTimeByns;
        }
    }
}
