namespace ScopeX.Core.PowerAnalysis
{
    using MathNet.Numerics;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    internal class PwrQualityModel : INotifyPropertyChanged
    {
        public class QualityItems
        {
            private Double _Current;

            public Double Current
            {
                get => _Current;

                set
                {
                    _Current = value;
                    if (Double.IsFinite(value))
                    {
                        StaBuffer.Insert(value);
                    }
                }
            }

            public readonly StatisticBuffer StaBuffer;

            public QualityItems(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrQualityModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _Qualities = new ConcurrentDictionary<String, QualityItems>();

            foreach (var item in _Items)
            {
                _Qualities.TryAdd(item, new QualityItems());
            }

            Count = _Items.Count;
        }

        private readonly List<String> _Items = new List<String>()
        {
            "Vrms",//0
            "VCrestFactor",//1
            "Frequency",//2
            "Irms",//3
            "ICrestFactor", //4
            "TruePower",//5
            "ApparantPower",//6
            "ReactivePower",//7
            "PwrFactor",//8
            "Phase"//9
        };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, QualityItems> _Qualities;

        public readonly String Titles = "Value,Average,Maxumum,Minimum";
        public QualityItems this[String key] => _Qualities[key];

        public readonly Int32 Count;

        private ChannelId GetRefSource()
        {
            return (RefFreq == VIType.V) ? Analysis.VoltageSrc1 : Analysis.CurrentSrc1;
        }

        private Double? CalcBaseFreq()
        {
            Double? freq = null;
            var freqsrc = GetRefSource();
            var mi = new MeasureItemModel($"{nameof(MeasParameter.Freq)}@lv", freqsrc);
            mi.RefLevel.RefStandard = MeasureTopBaseRef.ZeroMax;
            var rst = _Meas.Calc.ForceGetResultOrCalc(mi);
            if (rst != null)
            {
                freq = rst.Value;
            }

            return freq;
        }

        private void CalcPwrQuality()
        {
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Analysis.VoltageSrc1, out var _Vprsnt);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Analysis.CurrentSrc1, out var _Iprsnt);
            if (_Vprsnt == null || _Iprsnt == null)
            {
                return;
            }
            if (!_Vprsnt.Active || !_Iprsnt.Active)
            {
                foreach (var item in _Qualities.Values)
                {
                    item.Current = Double.NaN;
                    item.StaBuffer.Clear();
                }
                return;
            }

            var vrms = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.RMS), Analysis.VoltageSrc1) ?? null;
            _Qualities[_Items[0]].Current = Quantity.ConvertByPrefix(vrms, MeasureProc.GetPfxUnitString(nameof(MeasParameter.RMS), Analysis.VoltageSrc1).Prefix);

            var irms = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.RMS), Analysis.CurrentSrc1) ?? null;
            _Qualities[_Items[3]].Current = Quantity.ConvertByPrefix(irms, MeasureProc.GetPfxUnitString(nameof(MeasParameter.RMS), Analysis.CurrentSrc1).Prefix);

            //频率：在电压源上进行测量，单位Hz
            var freq = CalcBaseFreq();
            _Qualities[_Items[2]].Current = Quantity.ConvertByPrefix(freq, Prefix.Empty/*MeasureProc.GetPfxUnitString("RMS", GetRefSource()).Prefix*/);

            //电压波峰因素=电压信号最大值/电压均方根，无单位
            var vmax = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.Max), Analysis.VoltageSrc1) ?? null;
            if (vmax != null && Double.IsFinite(vmax!.Value) && vrms != null && (!Double.IsNaN(vrms!.Value)))
            {
                _Qualities[_Items[1]].Current = vmax!.Value / vrms!.Value;
            }
            else
            {
                _Qualities[_Items[1]].Current = Double.NaN;
            }

            //电流波峰因素=电流峰值/电流有效值，无单位
            var imax = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.Max), Analysis.CurrentSrc1) ?? null;
            if (imax != null && Double.IsFinite(imax!.Value) && irms != null && Double.IsFinite(irms!.Value))
            {
                _Qualities[_Items[4]].Current = imax!.Value / irms!.Value;
            }
            else
            {
                _Qualities[_Items[4]].Current = Double.NaN;
            }

            //视在功率=电流有效值与电压有效值的乘积，S=U*I，反映了最大可利用容量，单位VA
            //反映了为确保网络能正常工作，外电路需传给网络的能量或该网络的容量；
            //视在功率不表示交流电路实际消耗的功率，只表示电路可能提供的最大功率或电路可能消耗的最大有功功率
            if (Double.IsFinite(_Qualities[_Items[0]].Current) && Double.IsFinite(_Qualities[_Items[3]].Current))
            {
                _Qualities[_Items[6]].Current = _Qualities[_Items[0]].Current * _Qualities[_Items[3]].Current;
            }
            else
            {
                _Qualities[_Items[6]].Current = Double.NaN;
            }

            //瞬时功率：P(t)=u(t)i(t)；
            //一周期内传输至负载的能量为：𝑊cycle = ∫ u(t)i(t)dt；
            //𝑇
            //0
            //瞬时功率的一周期平均值，即有功功率（IVE-131-03-18）：P = 𝑊cycle
            //𝑇 = 1
            //𝑇 ∫ u(t)i(t)dt；，单位W；有效功率=功率波形的平均值
            //有效功率表示网络中实际吸收的功率
            var pdata = GetPowerData(_Vprsnt, _Iprsnt);//????
            if (pdata != null && pdata.Length != 0)
            {
                _Qualities[_Items[5]].Current = pdata.Sum() / pdata.Length;//????
            }
            else
            {
                _Qualities[_Items[5]].Current = Double.NaN;
            }


            // 表示单位时间内电路中能量振荡或往返的规模，Q =√S2 − P2，无功功率
            //没有方向，符号由负载决定。负载为感性时 Q>0，负载为容性是 Q<0。，单位VAR
            //无功功率表示没有消耗掉的能量
            if (Double.IsFinite(_Qualities[_Items[5]].Current) && Double.IsFinite(_Qualities[_Items[6]].Current))
            {
                _Qualities[_Items[7]].Current = Math.Sqrt(_Qualities[_Items[6]].Current * _Qualities[_Items[6]].Current - _Qualities[_Items[5]].Current * _Qualities[_Items[5]].Current);
            }
            else
            {
                _Qualities[_Items[7]].Current = Double.NaN;
            }


            //描述电路中某点的实际传递或消耗的平均功率与视在功率之间的关
            //系，功率因素数越高，电路的利用率就越高。PF=P/S，正负符号由有功功率的方向决定
            if (Double.IsFinite(_Qualities[_Items[5]].Current) && Double.IsFinite(_Qualities[_Items[6]].Current))
            {
                _Qualities[_Items[8]].Current = _Qualities[_Items[5]].Current / _Qualities[_Items[6]].Current;
            }
            else
            {
                _Qualities[_Items[8]].Current = Double.NaN;
            }
            //相位：Ф = ACOS(PF))，单位°
            if (Double.IsFinite(_Qualities[_Items[8]].Current))
            {
                _Qualities[_Items[9]].Current = Math.Acos(_Qualities[_Items[8]].Current) * 180 / Math.PI;
            }
            else
            {
                _Qualities[_Items[9]].Current = Double.NaN;
            }
        }

        private Double[]? GetPowerData(IChnlPrsnt _Vprsnt, IChnlPrsnt _Iprsnt)
        {
            if (CheckChannelPrsnt(_Vprsnt) && CheckChannelPrsnt(_Iprsnt))
            {
                var _Idata = _Iprsnt!.Pack!.Buffer.Cast<Double>().Select(x => x * 1E-3);
                var _Vdata = _Vprsnt!.Pack!.Buffer.Cast<Double>().Select(x => x * 1E-3);
                Int32 start_index = 0;
                Int32 end_index = _Idata.Count();
                var cycles = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.Period), Analysis.VoltageSrc1);
                if (cycles != null && cycles.Count > 0)
                {
                    start_index = (Int32)Math.Round(cycles.First().Start);
                    end_index = (Int32)Math.Round(cycles.Last().End);
                    _Vdata = _Vdata.Skip(start_index).Take(end_index - start_index + 1);
                    _Idata = _Idata.Skip(start_index).Take(end_index - start_index + 1);
                }


                return _Idata.Zip(_Vdata, (a, b) => a * b).ToArray();
            }
            else
            {
                return null;
            }
        }

        private Boolean CheckChannelPrsnt(IChnlPrsnt? prsnt)
        {
            if (prsnt != null && prsnt.Active && prsnt.Pack != null && prsnt.Pack.Buffer.GetLength(1) > 0)
            {
                return true;
            }

            return false;
        }

        private VIType _RefFreq = VIType.V;
        public VIType RefFreq
        {
            get => _RefFreq;
            set
            {
                if (_RefFreq != value)
                {
                    _RefFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Statistics;
        public Boolean Statistics
        {
            get => _Statistics;
            set
            {
                if (_Statistics != value)
                {
                    _Statistics = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _CompletionFlag = true;
        public Boolean CompletionFlag
        {
            get => _CompletionFlag;
            set
            {
                if (_CompletionFlag != value)
                {
                    _CompletionFlag = value;
                }
            }
        }

        //Run方法，执行计算的操作
        public void Run()
        {
            if (_CompletionFlag)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => QualityAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }


        private void QualityAnalysis()
        {
            _CompletionFlag = false;
            CalcPwrQuality();
            _CompletionFlag = true;
            Thread.Sleep(10);
        }

        public void Reset()
        {
            foreach (var p in _Qualities)
            {
                p.Value.StaBuffer.Clear();
            }
        }
        public ChannelId BoundMathId
        {
            get;
            set;
        } = ChannelId.M1;

        public String Formula => $"{MathType.Custom}:{Analysis.VoltageSrc1}*{Analysis.CurrentSrc1}";

        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);

            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
