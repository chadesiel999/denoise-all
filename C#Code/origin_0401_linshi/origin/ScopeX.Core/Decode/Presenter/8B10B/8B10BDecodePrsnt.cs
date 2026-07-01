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
    public class D8B10BDecodePrsnt : ProtocolPrsnt
    {
        public D8B10BDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (D8B10BDecodeModel)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.Common_8b10b);
            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }
        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source };
        }

        private protected override D8B10BDecodeModel Model { get; }
        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;
        public Int64 MaxBaudRate => Model.MaxBaudrate;
        public Int64 MinBaudRate => Model.MinBaudrate;
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
    }
}
