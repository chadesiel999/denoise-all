// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.Core
{
    using ScopeX.ComModel;
    using ScopeX.Core.Model;
    using ScopeX.Core.Tools;
    using ScopeX.Hardware.Driver;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class AnalogPrsnt : ChannelPrsnt
    {
        /// <summary>
        /// 为了避免高压警告状态不同步，所以使用事件及时通知
        /// </summary>
        public static void InitAnalogHighVoltageWarningEvent()
        {
            Hd.HighVoltageWarningEvent -= Hd_HighVoltageWarningEvent;
            Hd.HighVoltageWarningEvent += Hd_HighVoltageWarningEvent;
        }

        private static void Hd_HighVoltageWarningEvent(Int32 data)
        {
            var annalogchannel = data >> 24;
            var exttrigger = (data >> 16) & 0xFF;
            Boolean channel1 = false, channel2 = false, channel3 = false, channel4 = false, exttri = false;
            if ((annalogchannel & 0b00000001) == 0b00000001)
            {
                // 模拟通道1 高压警告
                channel1 = true;
            }

            if ((annalogchannel & 0b00000010) == 0b00000010)
            {
                // 模拟通道2 高压警告
                channel2 = true;
            }

            if ((annalogchannel & 0b00000100) == 0b00000100)
            {
                // 模拟通道3 高压警告
                channel3 = true;
            }

            if ((annalogchannel & 0b00001000) == 0b00001000)
            {
                // 模拟通道4 高压警告
                channel4 = true;
            }

            // 目前就只有一个外触发
            if (exttrigger == 1)
            {
                // 外触发高压警告
                exttri = true;
            }
            EventBus.EventBroker.Instance.GetEvent<HardwareWarningEventMessageArgs>().Publish(null, new HardwareWarningEventMessageArgs(channel1, channel2, channel3, channel4, exttri));
        }

        /// <summary>
        /// 高温警告反更新界面标致。防止嵌套标识
        /// </summary>
        public Boolean IsUpdateCouplingBack { get; set; } = false;
        private protected override AnalogModel Model { get; }

        /// <summary>
        /// 通道上是否接入有源探头
        /// AutoSet使用
        /// </summary>
        public Boolean HasActiveProbe
        {
            get => !String.IsNullOrEmpty(SerailNumber);
        }

        public AnalogPrsnt(ChannelId id, IDsoPrsnt idp, IChnlView? view, ITimebasePrsnt tmbprsnt) : base(idp, view)
        {
            Model = (AnalogModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;

            Sampling = (TimebasePrsnt)tmbprsnt;

            Model.Conditioning.Prompter = WeakTip.Default;
            Model.Sampling.Prompter = WeakTip.Default;
            IsCoarse = true;
            Model.Conditioning.ScaleBymVAdd = 0;
        }

        public override TimebasePrsnt Sampling { get; }

        public override Boolean Active
        {
            get => base.Active;
            set
            {
                if (value && (Id == ChannelId.C3 || Id == ChannelId.C4) && !DsoPrsnt.DefaultDsoPrsnt.CheckLAMutex(false))
                    return;
                base.Active = value;
            }
        }



        public AnaChnlScaleIndex AnaScaleIndex
        {
            get => (AnaChnlScaleIndex)ScaleIndex;
            set => ScaleIndex = (Int32)value;
        }

        internal AnaChnlScaleIndex ScaleMaxIndex => Model.Conditioning.ScaleMaxIndex;

        internal AnaChnlScaleIndex ScaleMinIndex => Model.Conditioning.ScaleMinIndex;

        #region 幅度细调

        public Double ScaleBymV
        {
            get => Model.Conditioning.ScaleBymV;
            set
            {
                var oldscale = Model.Conditioning.ScaleBymV;
                Double oldvalue = Model.Conditioning.Scale;
                var oldposindex = Model.Conditioning.PosIndex;
                Int32 oldbandwidth = Bandwidth;
                if (Ylevel_SelectStatus)
                {
                    //最小值判断
                    if (value < GetScale(AnaChnlScaleIndex.Lv5m))
                    {
                        WeakTip.Default.Write("Scale", MsgTipId.LessthanMin, false, "", 1);
                        return;
                    }
                    Double maxscale = Coupling != AnaChnlCoupling.DC50 ? GetScale(AnaChnlScaleIndex.Lv10) : GetScale(AnaChnlScaleIndex.Lv1);
                    //最大值判断
                    if (value > maxscale)
                    {
                        WeakTip.Default.Write("Scale", MsgTipId.GreatethanMax, false, "", 1);
                        return;
                    }

                    //获取细调幅度值
                    Model.Conditioning.ScaleBymVAdd = Math.Round(value, 9) - oldvalue;

                    //更新档位
                    UpdateScaleIndex(value);
                    //整数逻辑判断
                    if (Model.Conditioning.Scale / 100 >= 1)
                    {
                        //当所设置的幅度，取值不为所设置的档位/100的整数倍，需减去这部分
                        if (Math.Abs(Model.Conditioning.ScaleBymVAdd) > Model.Conditioning.Scale / 100 || Math.Abs(Model.Conditioning.ScaleBymVAdd) % (Model.Conditioning.Scale / 100) != 0)
                        {
                            Model.Conditioning.ScaleBymVAdd -= (Model.Conditioning.ScaleBymVAdd % (Model.Conditioning.Scale / 100));
                        }
                    }
                    //小数逻辑判断
                    else
                    {
                        //当所设置的幅度，取值不为所设置的档位/100的整数倍，需减去这部分
                        Model.Conditioning.ScaleBymVAdd -= ((Math.Round(Model.Conditioning.ScaleBymVAdd * 100 * 100e3, 4)) % Model.Conditioning.Scale * 1e3) / 100e3;
                    }
                    UpdateHCursorPosByScale(oldscale, oldposindex, Ylevel_SelectStatus);
                    FineStatusChanged = false;
                }
                else
                {
                    Model.Conditioning.Scale = value / (ProbeGain * ProbeUnitRatio);
                    Model.Conditioning.ScaleBymVAdd = 0;
                    UpdateHCursorPosByScale(oldvalue, oldposindex, Ylevel_SelectStatus);
                }

                LimitBandwidth(oldvalue, oldbandwidth);

                Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
                if (TriggerPrsnt.Type == TriggerType.Serial)
                {
                    if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                }
            }
        }

        /// <summary>
        /// 设置垂直挡位幅度
        /// </summary>
        /// <param name="saleValue">垂直幅度</param>
        public void SetScaleValueBymV(Int32 saleValue)
        {
            //获取设置前的幅度
            Double oldvalue = Model.Conditioning.ScaleBymV;
            var oldposindex = Model.Conditioning.PosIndex;
            if ((oldvalue + Model.Conditioning.Scale * saleValue / 100D) == Model.Conditioning.Scale)
            {
                Model.Conditioning.ScaleBymVAdd = 0;
                return;
            }
            //最小值判断
            if (Math.Round(oldvalue, 4) <= GetScale(AnaChnlScaleIndex.Lv5m) && saleValue < 0)
            {
                WeakTip.Default.Write("Scale", MsgTipId.LessthanMin, false, "", 1);
                return;
            }
            //最大值判断
            var maxscale = Coupling != AnaChnlCoupling.DC50 ? GetScale(AnaChnlScaleIndex.Lv10) : GetScale(AnaChnlScaleIndex.Lv1);
            if (Math.Round(oldvalue, 4) >= maxscale && saleValue > 0)
            {
                WeakTip.Default.Write("Scale", MsgTipId.GreatethanMax, false, "", 1);
                return;
            }
            //更新档位
            UpdateScaleNextIndex(oldvalue, saleValue > 0, out Int32 temscaleindex, out Double temscalebymvadd);
            //整数逻辑判断
            if (GetScale((AnaChnlScaleIndex)temscaleindex) / 100 >= 1)
            {
                //当所设置的幅度，取值不为所设置的档位/100的整数倍，需减去这部分
                if (Math.Abs(temscalebymvadd) > GetScale((AnaChnlScaleIndex)temscaleindex) / 100 || Math.Abs(temscalebymvadd) % (GetScale((AnaChnlScaleIndex)temscaleindex) / 100) != 0)
                {
                    temscalebymvadd -= (temscalebymvadd % (GetScale((AnaChnlScaleIndex)temscaleindex) / 100));
                }
            }
            //小数逻辑判断
            else
            {
                var param = Math.Round((Math.Round(temscalebymvadd * 100e3, 4)) % (GetScale((AnaChnlScaleIndex)temscaleindex) * 1e3), 4) * 1e-3;
                //当所设置的幅度，取值不为所设置的档位/100的整数倍，需减去这部分
                temscalebymvadd -= param / 100;
            }
            //计算得出需要增加的步进
            temscalebymvadd += GetScale((AnaChnlScaleIndex)temscaleindex) / 100 * saleValue;
            var newvalue = temscalebymvadd + GetScale((AnaChnlScaleIndex)temscaleindex);
            //再次更新档位
            UpdateScaleIndex(newvalue);
            Int32 oldbandwidth = Bandwidth;
            LimitBandwidth(oldvalue, oldbandwidth);
            Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
            if (TriggerPrsnt.Type == TriggerType.Serial)
            {
                if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                {
                    Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                }
            }
            UpdateHCursorPosByScale(oldvalue, oldposindex, Ylevel_SelectStatus);
        }

        /// <summary>
        /// 更新计算下一垂直挡位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="IsAdd"></param>
        public void UpdateScaleNextIndex(Double value, Boolean IsAdd, out Int32 scaleindex, out Double scalebymvadd)
        {
            scaleindex = ScaleIndex;
            scalebymvadd = Model.Conditioning.ScaleBymVAdd;
            if (value == GetScale((AnaChnlScaleIndex)scaleindex))
            {
                ///如果当前设置值和档位的值一致，1.当需要增加操作时，加档，2.减小时，使用当前档位
                scaleindex += scaleindex < (Int32)AnaChnlScaleIndex.Lv10 && IsAdd ? 1 : 0;
                scalebymvadd = value - GetScale((AnaChnlScaleIndex)scaleindex);
            }
            else if (value > GetScale((AnaChnlScaleIndex)scaleindex))
            {
                if (scaleindex < (Int32)AnaChnlScaleIndex.Lv10)
                {
                    scaleindex++;
                    scalebymvadd = value - GetScale((AnaChnlScaleIndex)scaleindex);
                }
            }
        }

        /// <summary>
        /// 更新垂直档位
        /// </summary>
        /// <param name="value"></param>
        public void UpdateScaleIndex(Double value)
        {
            if (value == Model.Conditioning.Scale)
            {
                return;
            }
            else if (value < Model.Conditioning.Scale)
            {
                Double data = GetScale((AnaChnlScaleIndex)(Model.Conditioning.ScaleIndex));
                Int32 tempscaleindex = ScaleIndex;
                ///归档操作
                while (value <= data && (Int32)AnaChnlScaleIndex.Lv1m < tempscaleindex)
                {
                    data = GetScale((AnaChnlScaleIndex)(tempscaleindex - 1));
                    if (value <= data)
                    {
                        //判断是否夸档
                        tempscaleindex--;
                    }
                    else
                        break;
                }
                Model.Conditioning.IsUpdateScale = true;
                if (ScaleIndex != tempscaleindex)
                {
                    FineStatusChanged = true;
                    ScaleIndex = tempscaleindex;
                }
                Model.Conditioning.ScaleBymVAdd = Math.Round(value - Model.Conditioning.Scale, 4);
                Model.Conditioning.IsUpdateScale = false;
            }
            else
            {
                Double data = GetScale((AnaChnlScaleIndex)(Model.Conditioning.ScaleIndex));
                Int32 anachnlscaleindex = (Int32)AnaChnlScaleIndex.Lv10;
                Int32 tempscaleindex = ScaleIndex;
                ///归档操作
                while (value > data && anachnlscaleindex > tempscaleindex)
                {
                    //判断是否夸档
                    tempscaleindex++;
                    data = GetScale((AnaChnlScaleIndex)(tempscaleindex));
                }
                Model.Conditioning.IsUpdateScale = true;
                if (ScaleIndex != tempscaleindex)
                {
                    FineStatusChanged = true;
                    ScaleIndex = tempscaleindex;
                }
                Model.Conditioning.ScaleBymVAdd = Math.Round(value - Model.Conditioning.Scale, 4);
                Model.Conditioning.IsUpdateScale = false;
            }
            OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(ScaleBymV)));
        }

        private Boolean _Ylevel_SelectStatus = false;

        /// <summary>
        /// 幅度细调选中状态
        /// </summary>
        public Boolean Ylevel_SelectStatus
        {
            get { return _Ylevel_SelectStatus; }
            set
            {
                if (_Ylevel_SelectStatus != value)
                {
                    FineStatusChanged = true;
                    //设置当前原本的档位
                    var oldscale = Model.Conditioning.ScaleBymV;
                    var oldposindex = Model.Conditioning.PosIndex;
                    var newscaleindex = GetScaleIndex(oldscale / (ProbeGain * ProbeUnitRatio));
                    Model.Conditioning.ScaleBymVAdd = 0;
                    ScaleIndex = newscaleindex;
                    //刷新界面
                    //Model.Conditioning.Scale = (Model.Conditioning.Scale / （ProbeGain * ProbeGainCaliRatio）);
                    _Ylevel_SelectStatus = value;
                    if (_Ylevel_SelectStatus)
                    {
                        WeakTip.Default.Write("Analog", MsgTipId.FinetuningofamplitudeON);
                    }
                    else
                    {
                        WeakTip.Default.Write("Analog", MsgTipId.FinetuningofamplitudeOFF);
                    }

                    KeyLed.Default.SetAnalogChannelConfig(Id, Active, Coupling, Bandwidth, _Ylevel_SelectStatus);
                    OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(Ylevel_SelectStatus)));
                    if (FineStatusChanged)
                    {
                        UpdateHCursorPosByScale(oldscale, oldposindex, Ylevel_SelectStatus);
                        FineStatusChanged = false;
                    }
                }
            }
        }

        #endregion

        internal override void UpdateHCursorPosByScale(Double oldScale, Double oldPosIndex, Boolean isAmplitudeFineTuning = false)
        {
            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.HCursor.Source == Id && DsoModel.Default.Cursors.Type != CursorType.Vertical)
            {
                var cuurentscale = isAmplitudeFineTuning ? ScaleBymV : Model.Conditioning.Scale;
                DsoModel.Default.Cursors.HCursor.SetPoxIndex(0, (Double)(DsoModel.Default.Cursors.HCursor[0] - oldPosIndex) * oldScale / cuurentscale + Model.Conditioning.PosIndex);
                DsoModel.Default.Cursors.HCursor.SetPoxIndex(1, (Double)(DsoModel.Default.Cursors.HCursor[1] - oldPosIndex) * oldScale / cuurentscale + Model.Conditioning.PosIndex);
            }
        }

        public override Int32 ScaleIndex
        {
            get => base.ScaleIndex;
            set
            {
                var @oldbias = this.Bias;
                Double oldvalue = Model.Conditioning.Scale;
                Int32 oldbandwidth = Bandwidth;
                Int32 old = base.ScaleIndex;
                base.ScaleIndex = value;
                this.Bias = @oldbias;
                if (old != base.ScaleIndex)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag = true;
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag = true;
                }
                DsoPrsnt.DefaultDsoPrsnt.PwrModulationClear();
                LimitBandwidth(oldvalue, oldbandwidth);
                Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
                if (TriggerPrsnt.Type == TriggerType.Serial)
                {
                    if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                }
            }
        }

        private void LimitBandwidth(Double oldValue, Int32 oldbandwidth)
        {
            if (!PlatformManager.Default.Platform.LimitBandwidth)
                return;
            var value = (Int32)Math.Ceiling(oldValue / (ProbeGain * ProbeUnitRatio));
            var scale = (Int32)Math.Ceiling(Model.Conditioning.Scale / (ProbeGain * ProbeUnitRatio));
            if (oldValue != Model.Conditioning.Scale)
            {
                if (value <= 2 && scale > 2)
                {
                    if (Coupling == AnaChnlCoupling.DC50)
                    {
                        if (DsoModel.Default.Timebase.InterleaveMode != AdcInterleaveMode.Mode4To1)
                        {
                            if (_TempLBandwidth == 0)
                            {
                                Int32 index = OptionsManager.Default.Is2GHz ? 0 : 1;
                                _TempLBandwidth = BandWidthNames.ElementAt(index).Index;
                                //TempLBandwidth = PlatformManager.Default.Platform.LZBANDWIDTHNAMES.ElementAt(index).Index;
                            }
                        }
                        Bandwidth = _TempLBandwidth != -1 ? _TempLBandwidth : Bandwidth;
                    }
                    else
                    {
                        Bandwidth = _TempHBandwidth != -1 ? _TempHBandwidth : Bandwidth;
                    }
                }
                if (value > 2 && scale <= 2)
                {
                    if (Coupling == AnaChnlCoupling.DC50)
                    {
                        _TempLBandwidth = oldbandwidth;
                    }
                    else
                    {
                        _TempHBandwidth = oldbandwidth;
                    }
                }
            }

            if (scale <= 2)
            {
                Bandwidth = BandWidthNames.Last().Index;
                //if (Coupling == AnaChnlCoupling.DC50)
                //{
                //    Bandwidth = PlatformManager.Default.Platform.LZBANDWIDTHNAMES.Last().Index;
                //}
                //else
                //{
                //    Bandwidth = PlatformManager.Default.Platform.HZBANDWIDTHNAMES.Last().Index;
                //}
            }
        }

        public override Int32 ScaleTick
        {
            get => base.ScaleTick;
            set
            {
                Double oldvalue = Model.Conditioning.Scale;
                Int32 oldbandwidth = Bandwidth;

                base.ScaleTick = value;

                LimitBandwidth(oldvalue, oldbandwidth);

                DsoModel.Default.GetTrigger().LeapPosIndex();

                Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
            }
        }

        public Double MaxScale => Model.Conditioning.MaxScale;

        public Double MinScale => Model.Conditioning.MinScale;
        private Int32 _TempHBandwidth = -1;
        private Int32 _TempLBandwidth = -1;
        public Int32 Bandwidth
        {
            get => Model.Conditioning.Bandwidth;
            set
            {
                if (PlatformManager.Default.Platform.NeedCheckBandwidth)
                {
                    //fix bug 3222 ,ljw 25.4
                    if (value != 3 && (UInt32)ScaleIndex <= (UInt32)AnaChnlScaleIndex.Lv2m)
                    {
                        WeakTip.Default.Write("Scale", MsgTipId.VerticalLvNotSupportThisSetBandwidth, false, "", 1);
                        return;
                    }
                    if (Coupling == AnaChnlCoupling.DC50 && Constants.ENABLE_BANDWIDTH)
                    {
                        if (Sampling.InterleaveMode == AdcInterleaveMode.Mode1To1 || Sampling.InterleaveMode == AdcInterleaveMode.Mode2To1)
                        {
                            if (value != 0)
                            {
                                Model.Conditioning.Bandwidth = value;
                            }
                        }
                        else
                        {
                            Model.Conditioning.Bandwidth = value;
                        }
                    }
                    else
                    {
                        Model.Conditioning.Bandwidth = value;
                    }
                }
                else
                {
                    Model.Conditioning.Bandwidth = value;
                }

                Hardware.HdCmdFactory.Push(HdCmd.ChnlBandwidth);
                KeyLed.Default.SetAnalogChannelConfig(Id, Active, Coupling, Bandwidth, Ylevel_SelectStatus);
            }
        }
        public IReadOnlyList<(Int32 Index, String Name)> BandWidthNames
        {
            get
            {
                return Model.Conditioning.BandWidthNames;
            }
        }

        /// <summary>
        /// 当前通道支持的耦合方式
        /// </summary>
        public IEnumerable<AnaChnlCoupling> SupportedCouplings
        {
            get
            {
                return Model.Conditioning.SupportedCouplings;
            }
        }

        public Double Bias
        {
            get => Model.Conditioning.BiasByuV;
            set
            {
                if (value != Model.Conditioning.BiasByuV)
                {
                    var oldbias = Model.Conditioning.BiasByuV;
                    Model.Conditioning.BiasByuV = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ChnlBias);

                    var trig = TriggerPrsnt.GetOrMakeTrigger(Dso, TriggerPrsnt.Type);
                    var trigsource = TriggerPrsnt.GetTriggerSource();
                    var bias = (Model.Conditioning.BiasByuV - oldbias) / 1E3 / ScaleBymV * Constants.IDX_PER_YDIV;
                    if (trig is TrigSingleSrcPrsnt)
                    {
                        if (trigsource == Id)
                        {
                            var tripos = trig.PosIndex;
                            trig.PosIndex = tripos - bias;
                        }
                    }
                    if (trig is TrigMultiLevelPrsnt tmp)
                    {
                        if (tmp is TrigDelayPrsnt tdp)
                        {
                            if (Id == tdp.SourceOne)
                            {
                                var tripos = tdp.PosUpperIndex;
                                tdp.PosUpperIndex = tripos - bias;
                            }
                            if (Id == tdp.SourceTwo)
                            {
                                var tripos = tdp.DataCompPosIndex;
                                tdp.DataCompPosIndex = tripos - bias;
                            }
                        }
                        else if (tmp is TrigSustainTimePrsnt tstp)
                        {
                            var tripos = tstp.GetPosIndex(Id);
                            tstp.SetPosIndex(Id, tripos - bias);
                        }
                        else
                        {
                            var upperpos = tmp.PosUpperIndex;
                            var lowerpos = tmp.PosLowerIndex;
                            tmp.PosUpperIndex = upperpos - bias;
                            tmp.PosLowerIndex = lowerpos - bias;
                        }
                    }
                    if (trig is TrigPatPrsnt tpp)
                    {
                        var tripos = tpp.GetPosIndex(Id);
                        tpp.SetPosIndex(Id, tripos - bias);
                    }
                    if (trig is TrigSetupHoldPrsnt sp)
                    {
                        if (sp.ClkSource == Id)
                        {
                            var tripos = sp.ClkCompPosIndex;
                            sp.ClkCompPosIndex = tripos - bias;
                        }
                        if (sp.DataSource == Id)
                        {
                            if (sp.DataPosPolarity == EdgeSlope.Rise)
                            {
                                sp.UpperDataPosIndex -= bias;
                            }
                            else
                                sp.LowerDataPosIndex -= bias;
                        }
                    }
                }
            }
        }

        public Double BiasBymV
        {
            get => Bias / 1_000;
            set => Bias = value * 1_000;
        }

        /// <summary>
        /// 通道延迟
        /// </summary>
        public Double Delay
        {
            get => Model.Conditioning.Delay;
            set
            {
                if (value != Model.Conditioning.Delay)
                {
                    Model.Conditioning.Delay = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ChnlDelay);
                }
            }
        }

        /// <summary>
        /// 通道延迟功能是否启用
        /// </summary>
        public Boolean IsDelayable => Model.Conditioning.IsDelayable;

        /// <summary>
        /// 通道延迟的最小步进值，单位为：s
        /// </summary>
        public Double DelayStep => Model.Conditioning.DelayStep;


        public Double MaxBias => Model.Conditioning.MaxBias;
        public Double MinBias => Model.Conditioning.MinBias;

        public PlotRenderType RenderType
        {
            get => Model.RenderType;
            set => Model.RenderType = value;
        }

        public AnaChnlCoupling Coupling
        {
            get => Model.Conditioning.Coupling;
            set
            {
                Model.Conditioning.Coupling = value;
                if (Model.Conditioning.Coupling == AnaChnlCoupling.DC50 && Bandwidth == 0 && Constants.ENABLE_BANDWIDTH)
                {
                    Int32 index = OptionsManager.Default.Is2GHz ? 0 : 1;
                    Bandwidth = BandWidthNames.ElementAt(index).Index;
                }
                Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
                KeyLed.Default.SetAnalogChannelConfig(Id, Active, Coupling, Bandwidth, Ylevel_SelectStatus);
            }
        }

        public ProbeType GetProbeType(ChannelId id)
        {
            var probe = Hd.ProbeManager.ProbeStatus();
            if (probe == null)
            {
                return ProbeType.Null;
            }
            if (probe[id].SerailNumber.Contains("PA"))
            {
                return ProbeType.Singled;
            }
            else if (probe[id].SerailNumber.Contains("DA"))
            {
                return ProbeType.Diff;
            }
            else
            {
                return ProbeType.Null;
            }
        }

        /// <summary>
        /// add by lihuijun
        /// 通道探头Gain校正
        /// 
        /// 在FPGA数据GAIN修正的基础上 增加探头的增益修正
        /// 界面不可见
        /// </summary>
        public Double ProbeGainCaliRatio
        {
            get => Model.Conditioning.ProbeGainCaliRatio;
            set
            {
                Model.Conditioning.ProbeGainCaliRatio = value;
            }
        }

        /// <summary>
        /// add by lihuijun
        /// 通道探头Offset校正
        /// 
        /// 在通道偏置校正 和 用户设置偏置 的基础上加上探头的偏置校正
        /// 界面不可见
        /// </summary>
        public Double ProbeOffsetCaliBias
        {
            get => Model.Conditioning.ProbeOffsetCaliBias;
            set
            {
                Model.Conditioning.ProbeOffsetCaliBias = value;
            }
        }

        /// <summary>
        /// 保存校正数据
        /// </summary>
        public Boolean SaveProbeUserCalibDataToLocal()
        {
            return Model.SaveProbeUserCalibDataToLocal();
        }

        public Boolean ProbeUserCalibDefault()
        {
            return Model.ProbeUserCalibDefault();
        }

        public Boolean IsInverted
        {
            get => Model.Conditioning.IsInverted;
            set
            {
                if (value != Model.Conditioning.IsInverted)
                {
                    Model.Conditioning.IsInverted = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ChnlInverted);

                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag = true;
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearHisFlag = true;
                    DsoModel.Default.Meas.Calc.ClearAllStat();
                    Dispatcher.SoftReset();
                }
            }
        }

        public override Double PosIndexBymDiv
        {
            get => base.PosIndexBymDiv;
            set
            {
                base.PosIndexBymDiv = value;

                DsoModel.Default.AnalogChPositionUpdateTime = TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue);// DateTime.Now;
                Hardware.HdCmdFactory.Push(HdCmd.ChnlPosition);
                if (TriggerPrsnt.Type == TriggerType.Serial)
                {
                    if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    }
                }
            }
        }

        public Double PositionBymV => Model.Conditioning.Position;

        public Int64 Deskew
        {
            get => Model.Deskew;
            set => Model.Deskew = value;
        }

        public Int64 MaxDeskew => Model.MaxDeskew;

        public Int64 MinDeskew => Model.MinDeskew;

        public void AdjDeskew(Int64 delta) => Model.AdjDeskew(delta);

        public AnaChnlProbe ProbeIndex
        {
            get => Model.Conditioning.ProbeIndex;
            set
            {
                if (ProbeLocked)
                    return;
                Model.Conditioning.ProbeIndex = value;
            }
        }
        public Double MaxProbeGain => Model.Conditioning.MaxProbeGain;
        public Double MinProbeGain => Model.Conditioning.MinProbeGain;
        public Double ProbeGain
        {
            get => Model.Conditioning.ProbeGain;
            set => Model.Conditioning.ProbeGain = value;
        }

        /// <summary>
        /// 探头单位 移除 统一用Unit
        /// </summary>
        public override String Unit
        {
            get => base.Unit;
            set
            {
                if (!String.IsNullOrEmpty(value) && value != "V")
                {
                    if (ProbeUnitIsCustomized)
                    {
                        base.Unit = value;
                    }

                }
                else
                {
                    ProbeUnitIsCustomized = false;
                    base.Unit = value;
                }
            }
        }

        /// <summary>
        /// 探头单位-比率
        /// </summary>
        public Double ProbeUnitRatio
        {
            get => Model.Conditioning.ProbeUnitRatio;
            set => Model.Conditioning.ProbeUnitRatio = value;
        }

        public Double MaxProbeUnitRatio => Model.Conditioning.MaxProbeUnitRatio;
        public Double MinProbeUnitRatio => Model.Conditioning.MinProbeUnitRatio;

        /// <summary>
        /// 单位比率是否可定制（当不为V是可定制）
        /// </summary>
        public Boolean ProbeUnitIsCustomized
        {
            get => Model.Conditioning.ProbeUnitIsCustomized;
            set
            {

                if (Model.Conditioning.ProbeUnitIsCustomized != value)
                {
                    Model.Conditioning.ProbeUnitIsCustomized = value;
                    if (value)
                    {
                        Unit = "A";
                    }
                }
            }
        }

        public Byte[]? WfmDpx { get => Model.WfmDpx; set => Model.WfmDpx = value; }
        public Int32 ZIndex { get => Model.ZIndex; set => Model.ZIndex = value; }

        public UInt32 MainWinMaxHitTimes { get => Model.MainWinMaxHitTimes; set => Model.MainWinMaxHitTimes = value; }
        public UInt32 MainWinMinHitTimes { get => Model.MainWinMinHitTimes; set => Model.MainWinMinHitTimes = value; }
        public Int32 DpxChOnCount { get => Model.DpxChOnCount; set => Model.DpxChOnCount = value; }
        public Int32 DpxChIndex1 { get => Model.DpxChIndex1; set => Model.DpxChIndex1 = value; }
        public Int32 DpxChIndex2 { get => Model.DpxChIndex2; set => Model.DpxChIndex2 = value; }
        public Double DpxCorrection { get => Model.DpxCorrection; set => Model.DpxCorrection = value; }
        internal Boolean IsCoarse
        {
            get => Model.Conditioning.IsCoarse;
            set => Model.Conditioning.IsCoarse = value;
        }

        public override void ResetPosIndex()
        {
            base.ResetPosIndex();
            Hardware.HdCmdFactory.Push(HdCmd.ChnlPosition);
        }

        public Double GetScale(AnaChnlScaleIndex index)
        {
            return Model.Conditioning.GetScaleValue((Int32)index, 0);
        }

        internal Int32 GetScaleIndex(Double value)
        {
            return Model.Conditioning.GetScaleIndex(value).Item1;
        }


        public Object FlagInfo
        {
            get => Model.Conditioning.FlagInfo;
            set => Model.Conditioning.FlagInfo = value;
        }

        public ProbeKeyType ProbeBtnType
        {
            get => Model.ProbeBtnType;
            set => Model.ProbeBtnType = value;
        }

        public AttenuationType AttenuationType
        {
            get => Model.AttenuationType;
            set => Model.AttenuationType = value;
        }

        public Boolean ProbeConnected
        {
            get => Model.ProbeConnected;
            set => Model.ProbeConnected = value;
        }

        public String ProbeSN
        {
            get => Model.ProbeSN;
        }

        public String SerailNumber
        {
            get => Model.SerailNumber;
        }
        public Boolean WriteProbeFactInfo(String info)
        {
            Hd.ProbeManager!.SetOneProbeFactoryInfo(Id, info);
            return true;
        }

        public String ProbeHwVerion
        {
            get => Model.ProbeHwVerion;
        }

        public Boolean ProbeLocked
        {
            get => Model.Conditioning.ProbeLocked;
            set => Model.Conditioning.ProbeLocked = value;
        }

        /// <summary>
        /// 探头直流偏
        /// </summary>
        public Double ProbeDCBiasBymV
        {
            get => Model.ProbeDCBiasBymV;
            set => Model.ProbeDCBiasBymV = value;
        }
        public Double MaxProbeDCBiasBymV => Model.MaxProbeDCBiasBymV;
        public Double MinProbeDCBiasBymV => Model.MinProbeDCBiasBymV;

        public Boolean SettingProbeInfo(String? info)
        {
            return false;
        }

        public Boolean IsInCopy = false;

        public void CopyParasFormOtherChannel(AnalogPrsnt prsnt)
        {
            IsInCopy = true;
            if (Coupling != prsnt.Coupling)
            {
                Coupling = prsnt.Coupling;
            }
            if (Ylevel_SelectStatus != prsnt.Ylevel_SelectStatus)
            {
                Ylevel_SelectStatus = prsnt.Ylevel_SelectStatus;
            }
            if (ScaleIndex != prsnt.ScaleIndex)
            {
                ScaleIndex = prsnt.ScaleIndex;
            }
            if (ScaleBymV != prsnt.ScaleBymV)
            {
                ScaleBymV = prsnt.ScaleBymV;
            }
            if (PosIndexBymDiv != prsnt.PosIndexBymDiv)
            {
                PosIndexBymDiv = prsnt.PosIndexBymDiv;
            }
            if (Bias != prsnt.Bias)
            {
                Bias = prsnt.Bias;
            }
            if (Label != prsnt.Label)
            {
                Label = prsnt.Label;
            }
            if (Delay != prsnt.Delay)
            {
                Delay = prsnt.Delay;
            }
            if (Bandwidth != prsnt.Bandwidth)
            {
                Bandwidth = prsnt.Bandwidth;
            }
            if (IsInverted != prsnt.IsInverted)
            {
                IsInverted = prsnt.IsInverted;
            }
            if (Active != prsnt.Active)
            {
                Active = prsnt.Active;
            }
            IsInCopy = false;
        }
    }
}
