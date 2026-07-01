using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Decode;

public class CANDecodePrsnt : ProtocolPrsnt
{
    public CANDecodePrsnt(ChannelId id, IDsoPrsnt idp, IProtocolView? view, Boolean isTrigDecode = false) : base(idp, view)
    {
        Model = (CANDecodeModelCPP)DecodeTools.GetChannelDecodeModel(id, SerialProtocolType.CAN);
        Model.PropertyChanged -= OnPropertyChanged;
        Model.PropertyChanged += OnPropertyChanged;
    }

    public override List<ChannelId> GetDecodeSource()
    {
        return new List<ChannelId>() { Source1 };
    }
    public Dictionary<ProtocolCAN.SignalRate, Int64> SignalRateMap
    {
        get => Model.SignalRateMap;
    }

    public Int64 MaxSignalRate => Model.MaxSignalRate;
    public Int64 MinSignalRate => Model.MinSignalRate;

    //自定义的信号速率（当SignalRate == TriggerCANSignalRate.CANSignalRate_custom时使用）
    public Int64 CustomSignalRate
    {
        get => Model.CustomSignalRate;
        set => Model.CustomSignalRate = Math.Clamp(value, MinSignalRate, MaxSignalRate);
    }
    public Int32 MaxSamplePoint => Model.MaxSamplePoint;


    public Int32 MinSamplePoint => Model.MinSamplePoint;
    //采样点(%)
    public Int32 SamplePoint
    {
        get => Model.SamplePoint;
        set => Model.SamplePoint = Math.Clamp(value, MinSamplePoint, MaxSamplePoint);
    }
    public Double MaxThreshold => Model.MaxThreshold;
    public Double MinThreshold => Model.MinThreshold;

    public Double SDAThreshold
    {
        get => Model.SDAThreshold;
        set => Model.SDAThreshold = Math.Clamp(value, MinThreshold, MaxThreshold);
    }
    public String SDAUnit => Model.SDAUnit;

    public Double SDAThresholdBymV
    {
        get => SDAThreshold * 1_000D;
        set => SDAThreshold = value / 1000D;
    }

    //输入1
    public ChannelId Source1
    {
        get => Model.Source1;
        set => Model.Source1 = value.Clamp(ActivedChannels);
    }

    //输入2(信号类型选择"差分"时使用)
    public ChannelId Source2
    {
        get => Model.Source2;
        set => Model.Source2 = value.Clamp(ActivedChannels);
    }

    //信号速率
    public ProtocolCAN.SignalRate SignalRate
    {
        get => Model.SignalRate;
        set => Model.SignalRate = value.Clamp();
    }

    //信号类型
    public ProtocolCAN.SignalType SignalType
    {
        get => Model.SignalType;
        set => Model.SignalType = value.Clamp();
    }


    /// <summary>
    /// Model
    /// </summary>
    private override protected CANDecodeModelCPP Model { get; }
}
