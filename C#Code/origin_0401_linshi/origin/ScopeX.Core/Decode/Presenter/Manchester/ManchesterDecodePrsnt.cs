using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolCommon;

namespace ScopeX.Core.Decode
{
    public class ManchesterDecodePrsnt : ProtocolPrsnt
    {
        public ManchesterDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (ManchesterDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.Manchester);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }

        private protected override ManchesterDecodeModel Model { get; }
        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;
        public Int64 MaxBaudRate => Model.MaxBaudrate;
        public Int64 MinBaudRate => Model.MinBaudrate;
        public Byte MaxSyncSize => Model.MaxSyncSize;
        public Byte MinSyncSize => Model.MinSyncSize;
        public Byte MaxHeaderSize => Model.MaxHeaderSize;
        public Byte MinHeaderSize => Model.MinHeaderSize;
        public Byte MaxDataSize => Model.MaxDataSize;
        public Byte MinDataSize => Model.MinDataSize;
        public Byte MaxDataNum => Model.MaxDataNum;
        public Byte MinDataNum => Model.MinDataNum;
        public Byte MaxTrailerSize => Model.MaxTrailerSize;
        public Byte MinTrailerSize => Model.MinTrailerSize;
        public Double MaxIdleBitSize => Model.MaxIdleBitSize;
        public Double MinIdleBitSize => Model.MinIdleBitSize;
        public Double MaxTolerance => Model.MaxTolerance;
        public Double MinTolerance => Model.MinTolerance;
        public Byte MaxStartEdge => Model.MaxStartEdge;
        public Byte MinStartEdge => Model.MinStartEdge;
        public String Unit => Model.Unit;
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value.Clamp(ActivedChannels);
        }

        /*门限阈值输入*/
        public Double Threshold
        {
            get => Model.Threshold;
            set => Model.Threshold = Math.Clamp(value, MinThreshold, MaxThreshold);
        }

        /*数据速率输入*/
        public Int64 Baudrate
        {
            get => Model.Baudrate;
            set => Model.Baudrate = Math.Clamp(value, MinBaudRate, MaxBaudRate);
        }

        /*边沿选择*/
        public ProtocolManchester.Polarity Polarity
        {
            get => Model.Polarity;
            set => Model.Polarity = value.Clamp();
        }

        /*位顺序选择*/
        public ProtocolManchester.MSB_LSB MSB_LSB
        {
            get => Model.MSB_LSB;
            set => Model.MSB_LSB = value.Clamp();
        }

        /*奇偶性选择*/
        public ProtocolManchester.OddEvenCheck OddEvenCheck
        {
            get => Model.OddEvenCheck;
            set => Model.OddEvenCheck = value.Clamp();
        }

        /*数据包视图选择*/
        public ProtocolManchester.DataView DataFlag
        {
            get => Model.DataFlag;
            set => Model.DataFlag = value;
        }
        
        /*Sync_size输入*/
        public Byte SyncSize
        {
            get => Model.SyncSize;
            set => Model.SyncSize = Math.Clamp(value, MinSyncSize, MaxSyncSize);
        }

        /*Header_Size输入*/
        public Byte HeaderSize
        {
            get => Model.HeaderSize;
            set => Model.HeaderSize = Math.Clamp(value, MinHeaderSize, MaxHeaderSize);
        }

        /*Data_size输入*/
        public Byte DataSize
        {
            get => Model.DataSize;
            set => Model.DataSize = Math.Clamp(value, MinDataSize, MaxDataSize);
        }

        /*Data_num输入*/
        public Byte DataNum
        {
            get => Model.DataNum;
            set => Model.DataNum = Math.Clamp(value, MinDataNum, MaxDataNum);
        }

        /*Trailer_Size输入*/
        public Byte TrailerSize
        {
            get => Model.TrailerSize;
            set => Model.TrailerSize = Math.Clamp(value, MinTrailerSize, MaxTrailerSize);
        }

        /*IdBit_Size输入*/
        public Double IdleBitSize
        {
            get => Model.IdleBitSize;
            set => Model.IdleBitSize = Math.Clamp(value, MinIdleBitSize, MaxIdleBitSize);
        }

        /*容限输入*/
        public Double Tolerance
        {
            get => Model.Tolerance;
            set => Model.Tolerance = Math.Clamp(value, MinTolerance, MaxTolerance);
        }

        /*起始边沿输入*/
        public Byte StartEdge
        {
            get => Model.StartEdge;
            set => Model.StartEdge = Math.Clamp(value, MinStartEdge, MaxStartEdge);
        }
    }
}
