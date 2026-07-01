// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using EventBus;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class TriggerSingleSrcModel : TriggerModel
    {
        public TriggerSingleSrcModel()
        {
            _Positions = new (Double, Double)[ChannelIdExt.AnaChnlNum + 2];
            for (Int32 i = 0; i < _Positions.Length; i++)
            {
                _Positions[i] = new(0, 0);
            };
        }

        private readonly Int32 _ANA_COMP_SITE = ChannelIdExt.AnaChnlNum;

        /*private readonly MagnetManager _magnetManager = new MagnetManager()
        {
            Threshold = 50,
        };*/

        //相对通道坐标，缺省值为0，单位1000/div
        private readonly (Double Index, Double Value)[] _Positions;
        //private readonly (Double Index, Double Value)[] _Positions = new (Double, Double)[ChannelIdExt.AnaChnlNum + 1]
        //{
        //    //Corresponding to the DIGITAL comparator of analog channel C1
        //    new(0, 0),  
        //    //C2
        //    new(0, 0),
        //    //C3
        //    new(0, 0),
        //    //C4
        //    new(0, 0),
        //    //Corresponding to the ANALOG comparator of Trigger channel
        //    new(0, 0),
        //};

        private Int32 GetSite(ChannelId ts)
        {
            if (IsAnalogFirst)
            {
                return _ANA_COMP_SITE;
            }
            else if (ts.IsExtTrigger())
            {
                return _Positions.Length - 3;
            }
            else if (ts == ChannelId.AC)
            {
                return _Positions.Length - 2;
            }
            else if (ts == ChannelId.AuxIn)
            {
                return _Positions.Length - 1;
            }
            else
            {
                return (Int32)ts;
            }
            //return IsAnalogFirst ? _ANA_COMP_SITE : (ts.IsExtTrigger() ? _Positions.Length - 2 : (ts.IsExtTrigger() ? _Positions.Length - 1 : (Int32)ts));
        }

        private ChannelId _Source = ChannelId.C1;

        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 电平值虚拟坐标 -- 最大值 （相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标）
        /// </summary>
        public override Double MaxPosIndex
        {
            get
            {
                if (Source.IsAnalog())
                {
                    var chmodel = DsoModel.Default.AnalogChnls.First(x => x.Id == Source);
                    return Constants.MAX_TRIGGER_IDX - chmodel.Conditioning.PosIndex;
                }
                else return Constants.MAX_TRIGGER_IDX;
            }
        }

        /// <summary>
        /// 电平值虚拟坐标 -- 最小值 （相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标）
        /// </summary>
        public override Double MinPosIndex
        {
            get
            {
                if (Source.IsAnalog())
                {
                    var chmodel = DsoModel.Default.AnalogChnls.First(x => x.Id == Source);
                    return Constants.MIN_TRIGGER_IDX - chmodel.Conditioning.PosIndex;
                }
                else return Constants.MIN_TRIGGER_IDX;
            }
        }

        /// <summary>
        /// 电平值虚拟坐标 （相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标）
        /// </summary>
        public Double CompPosIndex
        {
            get => GetPosIndex(Source);
            set => SetPosIndex(Source, value);
        }

        /// <summary>
        /// 电平值
        /// </summary>
        public Double CompPosition
        {
            get => GetPosition(Source);
            set => SetPosition(Source, value);
        }

        /// <summary>
        /// 最大电平值
        /// </summary>
        public Double MaxCompPosition => GetMaxPosition(Source);

        /// <summary>
        /// 最小电平值
        /// </summary>
        public Double MinCompPosition => GetMinPosition(Source);

        public IEnumerable<(Double Index, Double Value)> Positions => _Positions;

        public Prefix PosPrefix => GetPosPrefix(Source);

        public String PosUnit => GetPosUnit(Source);

        public Double GetPosIndex(ChannelId ts)
        {
            try
            {
                return ts.IsDigital() ? GetDigUserThroldIndex(ts) : _Positions[GetSite(ts)].Index;
            }
            catch (Exception e) 
            {
                return 0;
            }
            
        }

        public void SetPosIndex(ChannelId ts, Double posIndex/*, Double mingap = Constants.MIN_TRIGGER_GAP*/)
        {
            if (ts.IsDigital())
            {
                SetDigUserThroldIndex(ts, (Int32)posIndex);
                OnPropertyChanged(nameof(CompPosIndex));
            }
            else
            {
                var site = GetSite(ts);
                posIndex = ValidatePosIndex(posIndex);
                // 电平不需要磁吸效果，先去掉
                /*var canmagnet = _magnetManager.Determine(posIndex, MinPosIndex, MaxPosIndex);
                if (canmagnet != null)
                    posIndex = canmagnet!.Value;*/

                if (posIndex != _Positions[site].Index)
                {
                    _Positions[site].Index = posIndex;
                    _Positions[site].Value = PosIndexToValue(ts, posIndex);
                }
                OnPropertyChanged(nameof(CompPosIndex));
            }
        }

        //!!!Notice: The following function name 'GetPosition' does not append postfix 'BymV',
        //because it depends on the function 'GetAnalogAxisInfo' and 'GetDigiModel'.
        public Double GetPosition(ChannelId ts)
        {
            return ts.IsDigital() ? GetDigiUserThroldBymV(ts) : PosIndexToValue(ts, GetPosIndex(ts));
        }

        public void SetPosition(ChannelId ts, Double value)
        {
            if (ts.IsDigital())
            {
                SetDigiUserThroldBymV(ts, value);
                OnPropertyChanged(nameof(CompPosIndex));
            }
            else
            {
                SetPosIndex(ts, ValueToPosIndex(ts, value));
            }
        }

        //获得基于通道垂直零点的触发位置
        public Double GetRelPosIndex(ChannelId ts)
        {
            return ts.IsDigital() ? GetDigUserThroldIndex(ts) : _Positions[GetSite(ts)].Index + GetAnalogAxisInfo(ts).Pos0;
        }

        public void SetRelPosIndex(ChannelId ts, Double value)
        {
            if (ts.IsDigital())
            {
                throw new ArgumentException("Not Support!");
            }

            // 电平不需要磁吸效果，先去掉
            /*var canmagnet = _magnetManager.Determine(value, MinPosIndex, MaxPosIndex);
            if (canmagnet != null)
                value = canmagnet!.Value;*/

            value -= GetAnalogAxisInfo(ts).Pos0;
            if (_Positions[GetSite(ts)].Index != value)
            {
                _Positions[GetSite(ts)].Index = value;
                _Positions[GetSite(ts)].Value = PosIndexToValue(ts,value);
            }
            OnPropertyChanged(nameof(CompPosIndex));
        }

        //!!!Notice: When 'IsAnalogFirst' is true, trigger position corresponds to ANALOG comparator of Trigger channel;
        //Otherwise, to DIGITAL comparator.
        public Boolean IsAnalogFirst { get; protected set; } = true;

        public Double ScreenPosIndex { get; set; }

        public Double RelPosIndex { get => GetRelPosIndex(Source); set => SetRelPosIndex(Source, value); }

        protected static Double MaxDigiThrold => DigitalBitCtrlGrpModel.MaxUserThrold;

        protected static Double MinDigiThrold => DigitalBitCtrlGrpModel.MinUserThrold;

        public Double GetMaxPosition(ChannelId ts)
        {
            return ts.IsDigital() ? MaxDigiThrold : PosIndexToValue(ts, MaxPosIndex);
        }

        public Double GetMinPosition(ChannelId ts)
        {
            return ts.IsDigital() ? MinDigiThrold : PosIndexToValue(ts, MinPosIndex);
        }

        public override void LeapPosIndex()
        {
            LeapPosIndex(Source);
        }

        public void LeapPosIndex(ChannelId ts)
        {
            if (ts.IsDigital())
            {
                return;
            }

            var site = GetSite(ts);
            var oldindex = _Positions[site].Index;// ValueToPosIndex(ts, _Positions[site].Value);

            _Positions[site].Index = ValidatePosIndex(ValueToPosIndex(ts, _Positions[site].Value),false);
            if (_Positions[site].Index != oldindex)
            {
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
            OnPropertyChanged(nameof(CompPosIndex));
        }

        public void ResetCompPosIndex()
        {
            SetPosIndex(Source, 0);
        }

        public void SetCompPosIndexCenter()
        {
            try
            {
                if (Source.IsDigital())
                    return;

                if (Source == ChannelId.Ext5 || Source == ChannelId.Ext)
                {
                    SetPosIndex(Source, 0);
                    return;
                }

                //var pkg = DsoModel.Default.GetWfmPack(Source);
                var ach = (AnalogModel)DsoModel.Default.GetChannel(Source);
                if (ach == null || ach.Pack == null || ach.Pack.Buffer == null)
                {
                    SetPosIndex(Source, 0);
                    return;
                }

                Double[] buffer = ach.Pack.Buffer.Cast<Double>().ToArray();
                Double pos = (buffer.Max() + buffer.Min()) / 2;
                #region 不同幅度档判断
                switch (ach.Conditioning.Scale)
                {
                    case 20:
                        if (Math.Abs(pos) < 2)
                            pos = 0;
                        break;
                    case 50:
                        if (Math.Abs(pos) < 5)
                            pos = 0;
                        break;
                    case 100:
                        if (Math.Abs(pos) < 10)
                            pos = 0;
                        break;
                    case 200:
                        if (Math.Abs(pos) < 20)
                            pos = 0;
                        break;
                    case 500:
                        if (Math.Abs(pos) < 50)
                            pos = 0;
                        break;
                    case 1000:
                    case 2000:
                    case 5000:
                    case 10000:
                        if (Math.Abs(pos) < 100)
                            pos = 0;
                        break;
                    default: break;

                }
                #endregion
                Double centerpos = pos - ach.Conditioning.BiasByuV / 1000;
                SetPosition(Source, centerpos);
            }
            catch (Exception ex)
            {
                SetPosIndex(Source, 0);
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Error));
            }
        }

        protected static Double GetDigiUserThroldBymV(ChannelId ts)
        {
            return GetDigiModel?.Invoke(ts)?.UserThroldBymV ?? 0;
        }

        protected static void SetDigiUserThroldBymV(ChannelId ts, Double value)
        {
            var dm = GetDigiModel?.Invoke(ts);
            if (dm != null)
            {
                dm.UserThroldBymV = value;
            }
        }

        //protected static (Double Throld, Prefix Pfx, String Unit) GetDigiInfo(ChannelId ts)
        //{
        //    return (GetDigiModel?.Invoke(ts)?.UserThroldBymV ?? 0, Prefix.Milli, "V");
        //}

        protected static Int32 GetDigUserThroldIndex(ChannelId ts)
        {
            return GetDigiModel?.Invoke(ts)?.UserThroldIndex ?? 0;
        }

        protected static void SetDigUserThroldIndex(ChannelId ts, Int32 value)
        {
            var dm = GetDigiModel?.Invoke(ts);
            if (dm != null)
            {
                dm.UserThroldIndex = value;
            }
        }

    }
}
