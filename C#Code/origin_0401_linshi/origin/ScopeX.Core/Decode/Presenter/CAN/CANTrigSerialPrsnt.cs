using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode;

public class CANTrigSerialPrsnt : TrigSerialPrsnt
{
    public CANTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.CAN, view)
    {
        Model = (CANTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.CAN);
        LoadEvent();
    }
    /// <summary>
    /// 重载参数
    /// </summary>
    public override void LoadEvent()
    {
        if (Model != null)
        {
            Model.PropertyChanged += OnPropertyChanged;
        }
    }

    /// <summary>
    /// 切换类型，注销事件
    /// </summary>
    public override void DisposeEvent()
    {
        if (Model != null)
        {
            Model.PropertyChanged -= OnPropertyChanged;
        }
    }
    public override String ConditionName => nameof(Condition);

    public Int32 MinByteCount => Model.MinByteCount;


    public Int32 MaxByteCount => Model.MaxByteCount;
    //字节数(触发条件选择"数据"或"ID和数据"时使用)
    public Int32 ByteCount
    {
        get => Model.ByteCount;
        set => Model.ByteCount = Math.Clamp(value, MinByteCount, MaxByteCount);
    }
    public Int32 MaxByteIndex => Model.MaxByteIndex;
    public Int32 MinByteIndex => Model.MinByteIndex;

    //字节号(触发条件选择"数据"或"ID和数据"时使用)
    public Int32 ByteIndex
    {
        get => Model.ByteIndex;
        set => Model.ByteIndex = Math.Clamp(value, MinByteIndex, MaxByteIndex);
    }

    public ProtocolCAN.Condition Condition
    {
        get => Model.Condition;
        set => Model.Condition = value.Clamp();
    }
    public UInt64 MaxData => Model.MaxData;


    public UInt64 MinData => Model.MinData;
    //数据(触发条件选择"数据"或"ID和数据"时使用)
    public UInt64 Data
    {
        get => Model.Data;
        set => Model.Data = (UInt64)Math.Clamp((Int64)value, (Int64)MinData, (Int64)MaxData);
    }

    //数据限定(触发条件选择"数据"或"ID和数据"时使用)
    public ProtocolCAN.DataRelation DataRelation
    {
        get => Model.DataRelation;
        set => Model.DataRelation = value.Clamp();
    }
    public Int32 MaxExtendedID => Model.MaxExtendedID;


    public Int32 MinExtendedID => Model.MinExtendedID;
    //扩展ID号(触发条件选择"ID"或"ID和数据"时使用)
    public Int32 ExtendedID
    {
        get => Model.ExtendedID;
        set => Model.ExtendedID = Math.Clamp(value, MinExtendedID, MaxExtendedID);
    }

    //帧类型(触发条件选择"帧类型"时使用
    public ProtocolCAN.FrameType FrameType
    {
        get => Model.FrameType;
        set => Model.FrameType = value.Clamp();
    }

    //ID帧类型(触发条件选择"ID"或"ID和数据"时使用)
    public ProtocolCAN.IDFrameDirection IDFrameDirection
    {
        get => Model.IDFrameDirection;
        set => Model.IDFrameDirection = value.Clamp();
    }

    //ID标准(触发条件选择"ID"或"ID和数据"时使用)
    public ProtocolCAN.IDStandard IDStandard
    {
        get => Model.IDStandard;
        set => Model.IDStandard = value.Clamp();
    }

    public Int32 MinStandardID => Model.MinStandardID;


    public Int32 MaxStandardID => Model.MaxStandardID;
    //标准ID号(触发条件选择"ID"或"ID和数据"时使用)
    public Int32 StandardID
    {
        get => Model.StandardID;
        set => Model.StandardID = Math.Clamp(value, MinStandardID, MaxStandardID);
    }
    public ProtocolCAN.ErrorPacketType ErrorPacketType
    {
        get => Model.ErrorPacketType;
        set => Model.ErrorPacketType = value.Clamp();
    }


    private override protected CANTrigSerialModel Model
    {
        get;
    }
}
