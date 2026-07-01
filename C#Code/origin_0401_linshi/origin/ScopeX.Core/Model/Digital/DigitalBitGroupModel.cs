using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Tools;
using ScopeX.ComModel;
using System.Drawing;


namespace ScopeX.Core
{
    //class DigitalBitGroupModel : IBitModel
    //{
    //    public ChannelType Type
    //    {
    //        get;
    //    }

    //    //public ChannelId Id
    //    //{
    //    //    get;
    //    //}

    //    public String Name
    //    {
    //        get;
    //    }

    //    public Color DrawColor
    //    {
    //        get;
    //        set;
    //    }

    //    private Boolean _Active = false;
    //    public Boolean Active
    //    {
    //        get => _Active;
    //        set
    //        {
    //            if (_Active != value)
    //            {
    //                _Active = value;
    //                OnPropertyChanged?.Invoke(nameof(Active));
    //            }
    //        }

    //    }

    //    private String _Label = String.Empty;
    //    public String Label
    //    {
    //        get => _Label;
    //        set
    //        {
    //            if (_Label != value)
    //            {
    //                _Label = value;
    //                OnPropertyChanged?.Invoke(nameof(Label));
    //            }
    //        }
    //    }

    //    private static Int32 _ScaleIndex;
    //    public Int32 ScaleIndex
    //    {
    //        get => _ScaleIndex;
    //        set
    //        {
    //            value = ValidateScaleIndex(value);
    //            if (_ScaleIndex != value)
    //            {
    //                _ScaleIndex = value;
    //                OnPropertyChanged?.Invoke(nameof(ScaleIndex));
    //            }
    //        }
    //    }

    //    public static readonly Int32 ScaleMaxIndex = 2;

    //    public static readonly Int32 ScaleMinIndex = 0;

    //    protected static Int32 ValidateScaleIndex(Int32 scaleIndex)
    //    {
    //        if (scaleIndex > ScaleMaxIndex)
    //            return ScaleMaxIndex;
    //        else if (scaleIndex < ScaleMinIndex)
    //            return ScaleMinIndex;
    //        return scaleIndex;
    //    }

    //    public Double BitHeight => GetBitHeight(ScaleIndex);

    //    public static Double GetBitHeight(Int32 scaleIndex) => Constants.IDX_PER_YDIV / 8 * (1 << scaleIndex);

    //    private Double _PosIndex = PosDefIndex;
    //    public Double PosIndex
    //    {
    //        get => _PosIndex;
    //        set
    //        {
    //            value = ValidatePosIndex(value);
    //            if (_PosIndex != value)
    //            {
    //                _PosIndex = value;
    //                MoveGroupPosition();
    //                OnPropertyChanged?.Invoke(nameof(PosIndex));
    //            }
    //        }
    //    }

    //    public static readonly Double PosMaxIndex = Constants.DEF_YPOS_IDX + Constants.VIS_YDIVS_NUM / 2.0 * Constants.IDX_PER_YDIV;

    //    public static readonly Double PosMinIndex = Constants.DEF_YPOS_IDX - Constants.VIS_YDIVS_NUM / 2.0 * Constants.IDX_PER_YDIV;

    //    public static readonly Double PosDefIndex = PosMinIndex;

    //    protected Double ValidatePosIndex(Double posIndex)
    //    {
    //        var value = Math.Round(posIndex / BitHeight, MidpointRounding.AwayFromZero) * BitHeight;
    //        if (value + BitHeight > PosMaxIndex)
    //            value = PosMaxIndex - BitHeight;
    //        else if (value < PosMinIndex)
    //            value = PosMinIndex;
    //        return value;
    //    }

    //    private Dictionary<ChannelId, Double> _PositionToGroup = new Dictionary<ChannelId, Double>();

    //    public DigitalBitGroupModel(/*ChannelId id,*/String name, Color color, Action<String>? onPropertyChanged = null)
    //    {
    //        Type = ChannelType.Logic;
    //        //Id = id;
    //        Name = name;
    //        DrawColor = color;

    //        OnPropertyChanged = onPropertyChanged;
    //        DigitalBitModels = new();

    //    }

    //    protected Action<String>? OnPropertyChanged
    //    {
    //        get;
    //    }

    //    public List<DigitalBitModel> DigitalBitModels;

    //    public DigitalBitModel? GetBitGroup(String name)
    //    {
    //        foreach (var item in DigitalBitModels)
    //        {
    //            if (item.Name == name)
    //            {
    //                return item;
    //            }
    //        }
    //        return null;
    //    }

    //    public Boolean AddBit(DigitalBitModel digitalBitModel)
    //    {
    //        if (!DigitalBitModels.Contains(digitalBitModel))
    //        {
    //            DigitalBitModels.Add(digitalBitModel);
    //            _PositionToGroup.Add(digitalBitModel.Id, digitalBitModel.PosIndex - _PosIndex);
    //            digitalBitModel.LogicalGroup = Name;
    //            return true;
    //        }
    //        return false;
    //    }

    //    public Boolean RemoveBit(DigitalBitModel digitalBitModel)
    //    {
    //        if (DigitalBitModels.Contains(digitalBitModel))
    //        {
    //            digitalBitModel.LogicalGroup = null;
    //            DigitalBitModels.Remove(digitalBitModel);
    //            _PositionToGroup.Remove(digitalBitModel.Id);
    //            return true;
    //        }
    //        return false;
    //    }

    //    private void MoveGroupPosition()
    //    {
    //        foreach (var item in DigitalBitModels)
    //        {
    //            item.PosIndex = _PosIndex + _PositionToGroup[item.Id];
    //        }
    //    }
    //}
}
