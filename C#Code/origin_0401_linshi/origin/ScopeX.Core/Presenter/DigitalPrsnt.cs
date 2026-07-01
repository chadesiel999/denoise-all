using System;
using System.Drawing;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class DigitalPrsnt : ChannelPrsnt
    {
        private protected override DigitalModel Model
        {
            get;
        }

        public DigitalPrsnt(ChannelId id, IDsoPrsnt idp, ITimebasePrsnt tmbprsnt) : base(idp, null)
        {
            Model = (DigitalModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;

            Sampling = (TimebasePrsnt)tmbprsnt;
        }

        public override TimebasePrsnt Sampling
        {
            get;
        }

        public Int32 FocusBitId
        {
            get => Model.FocusBitId;
            set => Model.FocusBitId = value;
        }

        //public new DigiWfmPack? Pack => Model.Pack;

        public Double BitHeight => Model.BitHeight;

        public new Double PosMaxIndex => base.PosMaxIndex - BitHeight;

        public DigiHeightOpt BitHeightOpt
        {
            get => Model.BitHeightOpt;
            set
            {
                var old = Model.BitHeightOpt;
                Model.BitHeightOpt = value;
                Arrange(old, value);
                Dispatcher.SoftReset();
            }
        }

        public Int32 BitLength => Model.Conditioning.Bits.Count;

        public override Boolean Active
        {
            //get => Model.Conditioning.Bits.Any(o => o.Active);
            //set
            //{
            //    for (Int32 i = 0; i < BitLength; i++)
            //        SetActiveAt(i, value);
            //    Dispatcher.SoftReset();
            //}

            get => Model.Active;
            set
            {
                if (!Constants.ENABLE_LA && value)
                {
                    WeakTip.Default.Write("Digital", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }

                for (Int32 i = 0; i < BitLength; i++)
                {
                    SetActiveAt(i, value);
                }

                if (TriggerModel.State != SysState.Stop)
                {
                    Dispatcher.SoftReset();
                    KeyLed.Default.SetOtherChannelState(ControlChannelType.Digital, new[] { value });
                }
            }
        }

        public void AdjPosIndex(Double step)
        {
            Model.Conditioning.PosIndex += step * BitHeight;
            Dispatcher.SoftReset();
        }

        public Boolean GetActiveAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].ActiveBit;
        }

        public void SetActiveAt(Int32 idx, Boolean value)
        {
            if(value && Model.Active == false)
            {
                if(!PlatformManager.Default.Platform.DoLAMutex())
                {
                    return;
                }
            }
           
            Model.Conditioning.Bits[idx].ActiveBit = value;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
            Dispatcher.SoftReset();
        }

        /// <summary>
        /// 打开所有LA通道
        /// </summary>
        public void SetCalibrationDigital()
        {
            OpenAllDigital();
            BitHeightOpt = DigiHeightOpt.Large;
            AutoLocate();
        }

        /// <summary>
        /// 关闭所有LA通道
        /// </summary>
        public void CloseAllDigital()
        {
            for (Int32 i = 0; i < BitLength; i++)
            {
                SetActiveAt(i, false);
            }
            Active = false;
        }

        public void OpenAllDigital()
        {
            for (Int32 i = 0; i < BitLength; i++)
            {
                SetActiveAt(i, true);
            }
        }


        public Color GetColorAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].DrawColor;
        }
        public Boolean GetLabelVisibltyAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].LabelVisiblity;
        }
        public Color SetColorAt(Int32 idx, Color color)
        {
            return Model.Conditioning.Bits[idx].DrawColor = color;
        }

        public Double GetPosIndexAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].PosIndex;
        }

        public void SetPosIndexAt(Int32 idx, Double value)
        {
            Model.Conditioning.Bits[idx].PosIndex = value;
            Dispatcher.SoftReset();
        }

        public String GetLabelAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].Label;
        }

        public void SetLabelAt(Int32 idx, String value)
        {
            Model.Conditioning.Bits[idx].Label = value;
        }
        public void SetLabelVisiblity(Int32 idx, Boolean value)
        {
            Model.Conditioning.Bits[idx].LabelVisiblity = value;
        }
        //public String GetLogicGroup(Int32 idx) => Model.Conditioning.Bits[idx].LogicalGroup;

        //public void SetLogicGroup(Int32 idx, Int32 group) => Model.Conditioning.Bits[idx].LogicalGroup = group;

        public DigiTholdFamily GetFamilyAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].CtrlGroup.Family;
        }

        public void SetFamilyAt(Int32 idx, DigiTholdFamily family)
        {
            Model.Conditioning.Bits[idx].CtrlGroup.Family = family;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        public Double GetUserThroldAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].CtrlGroup.UserThroldBymV;
        }

        public void SetUserThroldAt(Int32 idx, Double threshold)
        {
            Model.Conditioning.Bits[idx].CtrlGroup.UserThroldBymV = threshold;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Double MaxUserThrold => DigitalBitCtrlGrpModel.MaxUserThrold;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Double MinUserThrold => DigitalBitCtrlGrpModel.MinUserThrold;

        public Int32 FamilyGrpCount => Model.Conditioning.Count;

        public DigiTholdFamily GetFamilyAtGrp(Int32 idx)
        {
            return Model.Conditioning.Groups[idx].Family;
        }

        public void SetFamilyAtGrp(Int32 idx, DigiTholdFamily family)
        {
            Model.Conditioning.Groups[idx].Family = family;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        public Double GetUserThroldAtGrp(Int32 idx)
        {
            return Model.Conditioning.Groups[idx].UserThroldBymV;
        }

        public void SetUserThroldAtGrp(Int32 idx, Double threshold)
        {
            Model.Conditioning.Groups[idx].UserThroldBymV = threshold;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        public Int32 GetUserThroldIdxAtGrp(Int32 idx)
        {
            return Model.Conditioning.Groups[idx].UserThroldIndex;
        }

        public void SetUserThroldIdxAtGrp(Int32 idx, Int32 throldIndex)
        {
            Model.Conditioning.Groups[idx].UserThroldIndex = throldIndex;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }
        public void AdjUserThroldIdxAtGrp(Int32 idx, Int32 step)
        {
            Model.Conditioning.Groups[idx].UserThroldIndex += step;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }
        public Double GetUserHystAt(Int32 idx)
        {
            return Model.Conditioning.Bits[idx].CtrlGroup.UserHystBymV;
        }

        public void SetUserHystAt(Int32 idx, Double hyst)
        {
            Model.Conditioning.Bits[idx].CtrlGroup.UserHystBymV = hyst;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Double MaxUserHyst => DigitalBitCtrlGrpModel.MaxUserHyst;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Double MinUserHyst => DigitalBitCtrlGrpModel.MinUserHyst;

        public Double GetUserHystAtGrp(Int32 idx)
        {
            return Model.Conditioning.Groups[idx].UserHystBymV;
        }

        public void SetUserHystAtGrp(Int32 idx, Double hyst)
        {
            Model.Conditioning.Groups[idx].UserHystBymV = hyst;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        public Int32 GetUserHystIdxAtGrp(Int32 idx)
        {
            return Model.Conditioning.Groups[idx].UserHystIndex;
        }

        public void SetUserHystIdxAtGrp(Int32 idx, Int32 hystIndex)
        {
            Model.Conditioning.Groups[idx].UserHystIndex = hystIndex;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        public void AdjUserHystIdxAtGrp(Int32 idx, Int32 step)
        {
            Model.Conditioning.Groups[idx].UserHystIndex += step;
            Hardware.HdCmdFactory.Push(HdCmd.Digital);
        }

        public void AutoLocate()
        {
            Double pos = PosMinIndex;
            for (Int32 i = 0; i < BitLength; i++)
            {
                if (Model.Conditioning.Bits[i].ActiveBit)
                {
                    SetPosIndexAt(i, pos);
                    pos += BitHeight;
                }
            }
        }

        public void Arrange(DigiHeightOpt old, DigiHeightOpt current)
        {
            if (old == current)
            {
                return;
            }

            var h = DigitalBitModel.GetBitHeight((Int32)current);
            for (Int32 i = 0; i < BitLength; i++)
            {
                if (Model.Conditioning.Bits[i].ActiveBit)
                {
                    var pos = Math.Round((GetPosIndexAt(i) - PosMinIndex) / h, MidpointRounding.AwayFromZero) * h + PosMinIndex;
                    SetPosIndexAt(i, pos);
                }
            }

        }

        //public override Double PosIndexBymDiv
        //{
        //    get => DsoPrsnt.FocusId.IsDigital() ? GetPosIndexAt(DsoPrsnt.FocusId - ChannelId.D1) : GetPosIndexAt(0);
        //    set
        //    {
        //        if (DsoPrsnt.FocusId.IsDigital())
        //            SetPosIndexAt(DsoPrsnt.FocusId - ChannelId.D1, value);
        //        else
        //            SetPosIndexAt(0, value);
        //    }
        //}

        //public override Int32 ScaleIndex
        //{
        //    get => (Int32)BitHeightOpt;
        //    set
        //    {
        //        if (value > (Int32)DigiHeightOpt.Large)
        //            value = (Int32)DigiHeightOpt.Large;
        //        else if (value < (Int32)DigiHeightOpt.Small)
        //            value = (Int32)DigiHeightOpt.Small;
        //        BitHeightOpt = (DigiHeightOpt)value;
        //    }
        //}

        //public override Int32 ScaleTick
        //{
        //    get => 0;
        //    set => _ = value;
        //}

        //public List<String> GetBitGroups() => Model.GetBitGroups();

        //public Boolean IsGroupAlreadyIncluded(String groupName) => Model.IsGroupAlreadyIncluded(groupName);

        //public Boolean AddGroup(String name, Color color) => Model.AddGroup(name, color);

        //public Boolean RemoveGroup(String name) => Model.RemoveGroup(name);

        //public Boolean MakeBitIndependent(ChannelId bit) => Model.MakeBitIndependent(bit);

        //public Boolean DistributeBit(String groupName, ChannelId bit) => Model.DistributeBit(groupName, bit);

        //public Double GetGroupPosition(String groupName) => Model.GetGroupPosition(groupName);

        //public List<ChannelId> GetBits(String groupName) => Model.GetBits(groupName);

        //public void AdjGroupPosition(String groupName,Double step) => Model.AdjGroupPosition(groupName, step);

        //public String GetGroupLabel(String? groupName) => Model.GetGroupLabel(groupName);

        //public void SetGroupLabel(String? groupName, String label) => Model.SetGroupLabel(groupName, label);
    }
}
