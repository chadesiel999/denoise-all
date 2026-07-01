using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Tools;
using ScopeX.ComModel;
using System.Runtime.CompilerServices;
using ScopeX.MathExt;
using ScopeX.Core.Decode;
using System.IO;
using SixLabors.ImageSharp.Memory;
using System.ComponentModel;

namespace ScopeX.Core
{
    internal class ReferenceModel : ChannelModel
    {
        internal sealed class ConditioningModel : VertAxisModel
        {
            public ConditioningModel() : base("Conditioning")
            {
                GetScaleValue = GetAnalogScaleFromIndexTick;
                GetScaleIndex = GetIndexTickFromAnalogScale;
            }

            private Double GetAnalogScaleFromIndexTick(Int32 index, Int32 tick)
            {
                var (initindex, _) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
                // modify by lhj 根据档位索引获取Scale，需要乘上 探头倍率增益和单位比率
                return ScaleFactory.Default.GetScale(index + initindex, InitialScale.Value) * _ProbeGain * _ProbeUnitRatio;
            }

            private (Int32 Index, Int32 Tick) GetIndexTickFromAnalogScale(Double scaleValue)
            {
                //var (index, tick) = ScaleFactory.Default.TrySetScale(scaleValue / (InitialScale.Value * _ProbeGain));
                //return (index + InitialScale.Index, tick);
                var (initindex, inittick) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
                var (index, tick) = ScaleFactory.Default.TryGetScaleIndex(scaleValue, InitialScale.Value);

                index -= initindex;

                return (index, tick);
            }

            private Double _ProbeGain = 1;
            public Double ProbeGain
            {
                get => _ProbeGain;
                set
                {
                    if (_ProbeGain != value)
                    {
                        _ProbeGain = value;
                        OnPropertyChanged();
                    }
                }
            }
            private Double _ProbeUnitRatio = 1;
            /// <summary>
            /// 探头单位比率 x**/V
            /// </summary>
            public Double ProbeUnitRatio
            {
                get => _ProbeUnitRatio;
                set
                {
                    if (_ProbeUnitRatio != value)
                    {
                        _ProbeUnitRatio = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal sealed class SamplingModelEx : SamplingModel
        {
            public override Int32 ScaleIndex
            {
                get => base.ScaleIndex;
                set
                {
                    base.ScaleIndex = (Int32)value;

                    ReCalcMaxMinPosIdx(InitialScale.Value);

                    base.PosIndex = PosDefIndex - TempPosition * PosIdxPerDiv / Scale;
                }
            }
        }

        public ReferenceModel(ChannelId id, Color color, Boolean active)
            : base(ChannelType.File, id, color)
        {
            Active = active;

            Conditioning = new ConditioningModel();
            Sampling = new SamplingModelEx();

            //PrepareSamples, ReadSamples, ProcessSamples are not called, so they do not need to be initialized
        }

        public String FullFileName
        {
            get;
            set;
        } = "";

        public override ConditioningModel Conditioning
        {
            get;
        }

        public override SamplingModelEx Sampling
        {
            get;
        }

        public static Boolean TryReadCSV(ChannelId id, String file, ref ReferenceModel? rm)
        {
            WfmPack? pkg = null;
            try
            {
                Double sampinterval = 0;
                Int32 index = 0;

                (Prefix Prefix, String Name) chnlunit = (Prefix.Milli, "V");
                (Int32 Index, Double Value) chnlscale = ((Int32)AnaChnlScaleIndex.Lv2, 1);
                (Double Index, Double Value) chnlposition = ((Int32)AnaChnlScaleIndex.Lv2, 2);
                (Prefix Prefix, String Name) tmbunit = (Prefix.Micro, "s");
                (Int32 Index, Double Value) tmbscale = ((Int32)AnaChnlTimebaseIndex.Lv10, 1E3);
                (Double Index, Double Value) tmbposition = (5000, 0);
                (Double Gain, Double UnitRatio) probeinfo = (1, 1);
                var result = System.IO.File.ReadAllLines(file).Select(x =>
                {
                    if (String.IsNullOrWhiteSpace(x))
                    {
                        index++;
                        return (false, 0d, 0d);
                    }
                    if ((index == 0) || (index == 1) || (index == 2) || (index == 3) || (index == 4))
                    {
                        index++;
                        return (false, 0d, 0d);
                    }

                    if ((index >= 5) && (index <= 11))
                    {
                        if (index == 11)
                        {
                            var cells = x.Split(',');
                            if (cells.Count() == 2 && cells[1].Trim() == "Y-axis(V)")
                            {
                                chnlunit = (Prefix.Empty, "V");
                            }
                            index++;
                            return (false, 0d, 0d);
                        }
                        var strs = x.Split(new char[] { ':', ',' });
                        if (strs.Length <= 1)
                        {
                            return (false, 0d, 0d);
                        }
                        if (index == 5)
                        {
                            index++;
                            if (Int32.TryParse(strs[2].TrimEnd(','), out var tmbval1) && Double.TryParse(strs[1], out var tmbval2))
                            {
                                tmbscale = (tmbval1, tmbval2);
                            }
                            return (false, 0d, 0d);
                        }
                        if (index == 6)
                        {
                            index++;
                            if (Double.TryParse(strs[2].TrimEnd(','), out var tmbval1) && Double.TryParse(strs[1], out var tmbval2))
                            {
                                tmbposition = (tmbval1, tmbval2);
                            }
                            return (false, 0d, 0d);
                        }
                        if (index == 7)
                        {
                            index++;
                            if (Int32.TryParse(strs[2].TrimEnd(','), out var chlval1) && Double.TryParse(strs[1], out var chlval2))
                            {
                                chnlscale = (chlval1, chlval2);
                            }
                            return (false, 0d, 0d);
                        }

                        if (index == 8)
                        {
                            index++;
                            if (Double.TryParse(strs[2].TrimEnd(','), out var chlval1) && Double.TryParse(strs[1], out var chlval2))
                            {
                                chnlposition = (chlval1, chlval2);
                            }
                            return (false, 0d, 0d);
                        }
                        if (index == 9)
                        {
                            index++;
                            if (Double.TryParse(strs[1].TrimEnd(','), out var sampleval1))
                            {
                                sampinterval = sampleval1;
                            }
                            return (false, 0d, 0d);
                        }
                        if (index == 10)
                        {
                            index++;
                            if (Double.TryParse(strs[2].TrimEnd(','), out var probegain) && Double.TryParse(strs[1], out var probeunitratio))
                            {
                                probeinfo = (probegain, probeunitratio);
                            }
                            return (false, 0d, 0d);
                        }
                    }
                    else
                    {
                        var temparraystrs = x.Split(',');
                        if (temparraystrs.Length <= 1)
                        {
                            return (false, 0d, 0d);
                        }
                        if (Double.TryParse(temparraystrs[0], out var val1) && Double.TryParse(temparraystrs[1], out var val2))
                        {

                            return (true, val1, val2);
                        }
                    }
                    return (false, 0d, 0d);
                }).Where(x => x.Item1).Select(x => (x.Item2, x.Item3)).ToArray();

                //if (result.Length <= 1 || result[0].Item1 == result[1].Item1) return false;
                if (result.Length <= 1) return false;

                Boolean repairconditions1 = result[1].Item1 == result[0].Item1;
                Boolean repairconditions2 = (result[1].Item1 - result[0].Item1 != sampinterval) && sampinterval > 0;

                //   Need To Repair Accurate Time --- ljw 24.6
                if (repairconditions1 || repairconditions2)
                {
                    Double startTime = result[0].Item1;
                    for (Int32 i = 1; i < result.Count(); i++)
                    {
                        result[i].Item1 = startTime + i * sampinterval;
                    }
                }
                List<Double> tempdata = result.Select(x => x.Item2).ToList();

                Double[,] tempchdata = new Double[1, tempdata.Count];
                Buffer.BlockCopy(tempdata.ToArray(), 0, tempchdata, 0, tempchdata.Length * Unsafe.SizeOf<Double>());

                pkg = new WfmPack(tempchdata, 0, tempchdata.Length, new WfmProperties(id.ToString())
                {
                    SampInterval = sampinterval,
                    ChnlUnit = chnlunit,
                    DrawMethod = DrawMethod.Plot,
                    ChnlScale = chnlscale,
                    TmbPosition = tmbposition,
                    ChnlPosition = chnlposition,
                    TmbScale = tmbscale,
                    TmbUnit = tmbunit,
                    ProbeInfo = probeinfo,
                });
                var wp = pkg.Properties;
                if (rm == null)
                {
                    rm = new ReferenceModel(id, ColorLookup.Default[id.ToString()], true);
                }

                rm.Conditioning.PosMaxIndex = Constants.MAX_YPOS_IDX;
                rm.Conditioning.PosMinIndex = Constants.MIN_YPOS_IDX;
                rm.Conditioning.PosIndex = wp.ChnlPosition.Index;

                rm.Conditioning.InitialScale = (0, 0.5);
                rm.Conditioning.ScaleMaxIndex = wp.ChnlScale.Index + 6;
                rm.Conditioning.ScaleMaxIndex = rm.Conditioning.ScaleMaxIndex > (Int32)AnaChnlScaleIndex.Lv20 ? (Int32)AnaChnlScaleIndex.Lv20 : rm.Conditioning.ScaleMaxIndex;
                rm.Conditioning.ScaleMinIndex = wp.ChnlScale.Index - 6;
                rm.Conditioning.ScaleMinIndex = rm.Conditioning.ScaleMinIndex < (Int32)AnaChnlScaleIndex.Lv1m ? (Int32)AnaChnlScaleIndex.Lv1m : rm.Conditioning.ScaleMinIndex;
                rm.Conditioning.ScaleIndex = wp.ChnlScale.Index;
                rm.Conditioning.ProbeGain = wp.ProbeInfo?.Gain ?? 1;
                rm.Conditioning.ProbeUnitRatio = wp.ProbeInfo?.UnitRatio ?? 1;
                //rm.Conditioning.Scale = rm.Conditioning.GetScaleValue(rm.Conditioning.ScaleIndex, 0);
                if (rm.Conditioning.Scale != wp.ChnlScale.Value && wp.ChnlScale.Value / rm.Conditioning.Scale < 3)
                {
                    rm.Ylevel_SelectStatus = true;
                    rm.Conditioning.ScaleBymVAdd = wp.ChnlScale.Value - rm.Conditioning.GetScaleValue(rm.Conditioning.ScaleIndex, 0);
                }
                else
                {
                    rm.Conditioning.ScaleBymVAdd = 0;
                }


                rm.Conditioning.Unit = wp.ChnlUnit.Name;
                rm.Conditioning.Prefix = wp.ChnlUnit.Prefix;

                rm.Sampling.InitialScale = wp.TmbScale;
                rm.Sampling.ScaleMaxIndex = wp.TmbScale.Index + 5;
                rm.Sampling.ScaleMinIndex = wp.TmbScale.Index - 5;
                rm.Sampling.ScaleIndex = wp.TmbScale.Index;

                rm.Sampling.PosMaxIndex = Constants.MAX_XPOS_IDX;
                rm.Sampling.PosMinIndex = -500000;
                rm.Sampling.PosIndex = wp.TmbPosition.Index;

                rm.Sampling.Unit = wp.TmbUnit.Name;
                rm.Sampling.Prefix = wp.TmbUnit.Prefix;

                rm.Pack = pkg;
                rm.PackForVu = pkg;

                //!!!Notice: Initialize the function making waveforms' view buffer based on waveform
                rm.MakeVuSamples = WfmVuDatabase.Rescale;
                rm.VuDatabase.Add(rm.MakeVuSamples(rm, 0, null).Value.Item1);

                rm.FullFileName = file;
                return true;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
            }
            if (pkg == null)
            {
                return false;
            }
            return false;
        }

        public static Boolean TryRead(ChannelId id, String file, ref ReferenceModel? rm)
        {
            //Deserialize to take WfmPack Object
            WfmPack? pkg;
            try
            {
                if (!CheckWfmFileSize(file))
                {
                    WeakTip.Default.Write("Ref", MsgTipId.RefFileError2);
                    return false;
                }

                using System.IO.MemoryStream memorystream = new(System.IO.File.ReadAllBytes(file));
                pkg = BinaryConvert.Deserialize<WfmPack>(memorystream);
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else
                return false;
#endif
            }

            if (pkg is null || pkg.Properties is null)
            {
                return false;
            }
            pkg.Properties.Name = id.ToString();

            var wp = pkg.Properties;
            if (wp.IsCompatible)
            {
                if (rm is null)
                {
                    //Create a new reference channel model
                    rm = new ReferenceModel(id, ColorLookup.Default[id.ToString()], false);
                }

                rm.Conditioning.PosMaxIndex = Constants.MAX_YPOS_IDX;
                rm.Conditioning.PosMinIndex = Constants.MIN_YPOS_IDX;
                rm.Conditioning.PosIndex = wp.ChnlPosition.Index;

                rm.Conditioning.ProbeGain = wp.ProbeInfo?.Gain ?? 1;
                rm.Conditioning.ProbeUnitRatio = wp.ProbeInfo?.UnitRatio ?? 1;
                rm.Conditioning.InitialScale = (0, 0.5);
                rm.Conditioning.ScaleMaxIndex = wp.ChnlScale.Index + 6;
                rm.Conditioning.ScaleMaxIndex = rm.Conditioning.ScaleMaxIndex > (Int32)AnaChnlScaleIndex.Lv20 ? (Int32)AnaChnlScaleIndex.Lv20 : rm.Conditioning.ScaleMaxIndex;
                rm.Conditioning.ScaleMinIndex = wp.ChnlScale.Index - 6;
                rm.Conditioning.ScaleMinIndex = rm.Conditioning.ScaleMinIndex < (Int32)AnaChnlScaleIndex.Lv1m ? (Int32)AnaChnlScaleIndex.Lv1m : rm.Conditioning.ScaleMinIndex;
                rm.Conditioning.ScaleIndex = wp.ChnlScale.Index;

                rm.Conditioning.Unit = wp.ChnlUnit.Name;
                rm.Conditioning.Prefix = wp.ChnlUnit.Prefix;

                rm.Sampling.InitialScale = wp.TmbScale;
                rm.Sampling.ScaleMaxIndex = wp.TmbScale.Index + 5;
                rm.Sampling.ScaleMinIndex = wp.TmbScale.Index - 5;
                rm.Sampling.ScaleIndex = wp.TmbScale.Index;
                ///rm.Conditioning.Scale = rm.Conditioning.GetScaleValue(rm.Conditioning.ScaleIndex, 0);
                if (rm.Conditioning.Scale != wp.ChnlScale.Value && wp.ChnlScale.Value / rm.Conditioning.Scale < 3)
                {
                    rm.Ylevel_SelectStatus = true;
                    rm.Conditioning.ScaleBymVAdd = wp.ChnlScale.Value - rm.Conditioning.GetScaleValue(rm.Conditioning.ScaleIndex, 0);
                }
                else
                {
                    rm.Conditioning.ScaleBymVAdd = 0;
                }
                rm.Sampling.PosMaxIndex = Constants.MAX_XPOS_IDX;
                rm.Sampling.PosMinIndex = -500000;
                rm.Sampling.PosIndex = wp.TmbPosition.Index;


                rm.Sampling.Unit = wp.TmbUnit.Name;
                rm.Sampling.Prefix = wp.TmbUnit.Prefix;

                rm.Pack = pkg;
                rm.PackForVu = pkg;
                // rm.Pack = new WfmPack(pkg.Buffer.Select(o => Quantity.ConvertByPrefix(o, pkg.Properties.ChnlUnit.Prefix)), pkg.Offset, pkg.Length, pkg.Properties);
                //!!!Notice: Initialize the function making waveforms' view buffer based on waveform
                rm.MakeVuSamples = WfmVuDatabase.Rescale;
                rm.VuDatabase.Add(rm.MakeVuSamples(rm, 0, null).Value.Item1);

                rm.FullFileName = file;
                return true;
            }

            return false;
        }

        private const Int64 MAX_WFMFILE_SIZE = 1_000_000L;
        /// <summary>
        /// 检测波形文件大小是否小于1M
        /// </summary>
        /// <param name="fullpath">文件路径</param>
        /// <returns></returns>
        private static Boolean CheckWfmFileSize(String fullpath)
        {
            FileInfo fileInfo = new FileInfo(fullpath);
            return fileInfo.Length < MAX_WFMFILE_SIZE + 100;
        }

        #region 幅度细调

        public Double GetScale(AnaChnlScaleIndex index)
        {
            return Conditioning.GetScaleValue((Int32)index, 0);
        }

        internal Int32 GetScaleIndex(Double value)
        {
            return Conditioning.GetScaleIndex(value).Item1;
        }

        public Double ScaleBymV
        {
            get => Conditioning.ScaleBymV;
            set
            {
                Double oldvalue = Conditioning.Scale / (Conditioning.ProbeGain * Conditioning.ProbeUnitRatio);
                var oldposindex = Conditioning.PosIndex;
                if (Ylevel_SelectStatus)
                {
                    //最小值判断
                    if (value < GetScale((AnaChnlScaleIndex)Conditioning.ScaleMinIndex))
                    {
                        WeakTip.Default.Write("Scale", MsgTipId.LessthanMin, false, "", 1);
                        return;
                    }
                    Double maxscale = GetScale((AnaChnlScaleIndex)Conditioning.ScaleMaxIndex);
                    //最大值判断
                    if (value > maxscale)
                    {
                        WeakTip.Default.Write("Scale", MsgTipId.GreatethanMax, false, "", 1);
                        return;
                    }

                    //获取细调幅度值
                    Conditioning.ScaleBymVAdd = Math.Round(value, 9) - oldvalue;

                    //更新档位
                    UpdateScaleIndex(value);
                    //整数逻辑判断
                    if (Conditioning.Scale / 100 >= 1)
                    {
                        //当所设置的幅度，取值不为所设置的档位/100的整数倍，需减去这部分
                        if (Math.Abs(Conditioning.ScaleBymVAdd) > Conditioning.Scale / 100 || Math.Abs(Conditioning.ScaleBymVAdd) % (Conditioning.Scale / 100) != 0)
                        {
                            Conditioning.ScaleBymVAdd -= (Conditioning.ScaleBymVAdd % (Conditioning.Scale / 100));
                        }
                    }
                    //小数逻辑判断
                    else
                    {
                        //当所设置的幅度，取值不为所设置的档位/100的整数倍，需减去这部分
                        Conditioning.ScaleBymVAdd -= ((Math.Round(Conditioning.ScaleBymVAdd * 100 * 100e3, 4)) % Conditioning.Scale * 1e3) / 100e3;
                    }
                }
                else
                {
                    Conditioning.Scale = value;
                    Conditioning.ScaleBymVAdd = 0;
                }
            }
        }

        /// <summary>
        /// 设置垂直挡位幅度
        /// </summary>
        /// <param name="saleValue"></param>
        public void SetScaleValueBymV(Int32 saleValue)
        {
            // 获取设置前的幅度
            Double oldvalue = Conditioning.ScaleBymV / (Conditioning.ProbeGain * Conditioning.ProbeUnitRatio);
            var oldposindex = Conditioning.PosIndex;
            if ((oldvalue + Conditioning.Scale * saleValue / 100D) == Conditioning.Scale)
            {
                Conditioning.ScaleBymVAdd = 0;
                return;
            }
            //最小值判断
            if (Math.Round(oldvalue, 4) <= GetScale((AnaChnlScaleIndex)Conditioning.ScaleMinIndex) && saleValue < 0)
            {
                WeakTip.Default.Write("Scale", MsgTipId.LessthanMin, false, "", 1);
                return;
            }
            //最大值判断
            var maxscale = GetScale((AnaChnlScaleIndex)Conditioning.ScaleMaxIndex);
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
        }

        /// <summary>
        /// 更新计算下一垂直挡位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="IsAdd"></param>
        public void UpdateScaleNextIndex(Double value, Boolean IsAdd, out Int32 scaleindex, out Double scalebymvadd)
        {
            scaleindex = Conditioning.ScaleIndex;
            scalebymvadd = Conditioning.ScaleBymVAdd;
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
            if (value == Conditioning.Scale)
            {
                return;
            }
            else if (value < Conditioning.Scale)
            {
                Double data = GetScale((AnaChnlScaleIndex)(Conditioning.ScaleIndex));
                Int32 tempscaleindex = Conditioning.ScaleIndex;
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
                Conditioning.IsUpdateScale = true;
                if (Conditioning.ScaleIndex != tempscaleindex)
                {
                    Conditioning.ScaleIndex = tempscaleindex;
                }
                Conditioning.ScaleBymVAdd = value - Conditioning.Scale;
                Conditioning.IsUpdateScale = false;
            }
            else
            {
                Double data = GetScale((AnaChnlScaleIndex)(Conditioning.ScaleIndex));
                Int32 anachnlscaleindex = (Int32)AnaChnlScaleIndex.Lv10;
                Int32 tempscaleindex = Conditioning.ScaleIndex;
                ///归档操作
                while (value > data && anachnlscaleindex > tempscaleindex)
                {
                    //判断是否夸档
                    tempscaleindex++;
                    data = GetScale((AnaChnlScaleIndex)(tempscaleindex));
                }
                Conditioning.IsUpdateScale = true;
                if (Conditioning.ScaleIndex != tempscaleindex)
                {
                    Conditioning.ScaleIndex = tempscaleindex;
                }
                Conditioning.ScaleBymVAdd = value - Conditioning.Scale;
                Conditioning.IsUpdateScale = false;
            }
            OnPropertyChanged(nameof(ScaleBymV));
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
                    //Model.Conditioning.Scale = Model.Conditioning.ScaleBymV;
                    //设置当前原本的档位
                    Double oldScale = Conditioning.ScaleBymV / (Conditioning.ProbeGain * Conditioning.ProbeUnitRatio);
                    var oldposindex = Conditioning.PosIndex;
                    Int32 newScaleIndex = GetScaleIndex(oldScale);
                    Conditioning.ScaleBymVAdd = 0;
                    Conditioning.ScaleIndex = newScaleIndex;
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
                    OnPropertyChanged(nameof(Ylevel_SelectStatus));
                }
            }
        }

        #endregion

    }
}
