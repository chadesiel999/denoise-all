using System;
using System.Collections.Generic;
using NPOI.HSSF.Record.CF;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode
{
    public class ARINC429DecodePrsnt : ProtocolPrsnt
    {
        public ARINC429DecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
        {
            Model = (ARINC429DecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.ARINC429);

            Model.PropertyChanged -= OnPropertyChanged;
            Model.PropertyChanged += OnPropertyChanged;
        }

        private protected override ARINC429DecodeModelCPP Model { get; }


        public ProtocolARINC429.DecodeMode DecodeMode
        {
            get => Model.DecodeMode;
            set
            {
                Model.DecodeMode = value.Clamp();
                if (TriggerSerialShareParameter.Default.ProtocolType == SerialProtocolType.ARINC429 && IsTrigger)
                {
                    if (TrigSerialPrsnt.GetOrMakeTriggerSerial(Dso, TriggerSerialShareParameter.Default.ProtocolType) is ARINC429TrigSerialPrsnt arinc429)
                    {
                        arinc429.DecodeMode = Model.DecodeMode;
                    }
                }
            }
        }

        public override List<ChannelId> GetDecodeSource()
        {
            return new List<ChannelId>() { Source1 };
        }

        public ChannelId Source1
        {
            get => Model.Source1;
            set => Model.Source1 = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public ChannelId Source2
        {
            get => Model.Source2;
            set => Model.Source2 = ChannelIdExt.Clamp(value, ActivedChannels);
        }
        public ProtocolARINC429.InputMode InputMode
        {
            get => Model.InputMode;
            set => Model.InputMode = value.Clamp();
        }
        public Int32 MaxBaud => Model.MaxBaud;
        public Int32 MinBaud => Model.MinBaud;


        public Double MaxThreshold => Model.MaxThreshold;
        public Double MinThreshold => Model.MinThreshold;
        public Double ThresholdH
        {
            get => Model.ThresholdH;
            set => Model.ThresholdH = Math.Clamp(value, MinThreshold, MaxThreshold);
        }

        public String UnitH => Model.UnitH;

        public Double ThresholdHBymV
        {
            get => ThresholdH * 1_000D;
            set => ThresholdH = value / 1000D;
        }
        public Double ThresholdL
        {
            get => Model.ThresholdL;
            set => Model.ThresholdL = Math.Clamp(value, MinThreshold, MaxThreshold);
        }
        public String UnitL => Model.UnitL;

        public Double ThresholdLBymV
        {
            get => ThresholdL * 1_000D;
            set => ThresholdL = value / 1000D;
        }

        public ProtocolARINC429.SignalRate SignalRate
        {
            get => Model.SignalRate;
            set => Model.SignalRate = value.Clamp();
        }

        public Int32 CustomBaud
        {
            get => Model.CustomBaud;
            set => Model.CustomBaud = Math.Clamp(value, MinBaud, MaxBaud);
        }

        public ProtocolCommon.Polarity Polarity
        {
            get
            {
                return Model.Polarity;
            }
            set
            {
                Model.Polarity = value.Clamp();
            }
        }



    }
}
