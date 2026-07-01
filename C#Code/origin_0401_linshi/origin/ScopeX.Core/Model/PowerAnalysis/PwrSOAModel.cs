using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.MathExt;

namespace ScopeX.Core.PowerAnalysis
{
    internal class PwrSOAModel : INotifyPropertyChanged
    {
        public PwrSOAModel(PowerAnalysisModel pam, MeasureModel _)
        {
            Analysis = pam;
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        //测试是否成功
        public Boolean TestSucceed
        {
            get;
            private set;
        }

        //获取的波形总数
        public Int32 AcqWfms
        {
            get;
            private set;
        }

        //测试失败的波形总数
        public Int32 FailWfms
        {
            get;
            private set;
        }

        //各分段的数据区域
        //public List<PointF> _MaskPolygon;

        //物理坐标下的模板
        private readonly List<(Double x, Double y)> _MaskPolygon = new();

        internal IReadOnlyList<(Double x, Double y)> MaskRegion => _MaskPolygon.AsReadOnly();

        //当前测试失败波形中的违例点
        /// <summary>
        /// Defines the Hits.
        /// </summary>
        internal List<List<(Double x, Double y)>> HitsBuffer = new List<List<(Double x, Double y)>>();

        public IReadOnlyList<IReadOnlyList<(Double x, Double y)>> Hits => HitsBuffer.AsReadOnly();
        private List<(Double x, Double y)> _MaskData = new();
        public List<(Double x, Double y)> MaskData
        {
            get => _MaskData;
            set
            {
                if (_MaskData != value)
                {
                    _MaskData = value;
                    MaskDataChanged = true;
                    OnPropertyChanged();
                }
            }
        }
        public Boolean MaskDataChanged = false;
        private Boolean _StopOnFault = false;
        public Boolean StopOnFault
        {
            get => _StopOnFault;
            set
            {
                if (_StopOnFault != value)
                {
                    _StopOnFault = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int64? _WindowId;
        public Int64? WindowId
        {
            get => _WindowId;
            set
            {
                if (_WindowId != value)
                {
                    _WindowId = value;
                    OnPropertyChanged();
                }
            }
        }
        private Double _MaxPower = Constants.DEF_SOA_POWER;
        public Double MaxPower
        {
            get => _MaxPower;
            set
            {
                value = ValidateMaxPower(value);
                if (_MaxPower != value)
                {
                    _MaxPower = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxPower = Constants.MAX_SOA_POWER;

        public readonly Double MinMaxPower = Constants.MIN_SOA_POWER;

        private static Double GetStep(Double value, Int32 b)
        {
            value = Math.Abs(value);
            if (value > 0)
            {
                var n = (Int32)Math.Floor(Math.Log10(value));
                n += b + 1;
                if (n < b)
                {
                    n = b;
                }
                return Math.Pow(10, n);
            }
            return 1;
        }

        private Double ValidateMaxPower(Double value)
        {
            if (value > MaxMaxPower)
            {
                value = MaxMaxPower;
            }
            else if (value < MinMaxPower)
            {
                value = MinMaxPower;
            }

            var step = GetStep(value, -3);

            value = Math.Round(value / step) * step;

            return value;
        }

        private Double _MaxCurrent = 6;
        public Double MaxCurrent
        {
            get => _MaxCurrent;
            set
            {
                value = ValidateMaxCurrent(value);

                if (_MaxCurrent != value)
                {
                    _MaxCurrent = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxCurrent = Constants.MAX_SOA_CUR;

        public readonly Double MinMaxCurrent = Constants.MIN_SOA_CUR;

        private Double ValidateMaxCurrent(Double value)
        {
            if (value > MaxMaxCurrent)
            {
                value = MaxMaxCurrent;
            }
            else if (value < MinMaxCurrent)
            {
                value = MinMaxCurrent;
            }

            var step = GetStep(value, -3);

            value = Math.Round(value / step) * step;

            return value;
        }


        private Double _MaxVoltage = 8;
        public Double MaxVoltage
        {
            get => _MaxVoltage;
            set
            {
                value = ValidateMaxVoltage(value);

                if (_MaxVoltage != value)
                {
                    _MaxVoltage = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxVoltage = Constants.MAX_SOA_VOL;

        public readonly Double MinMaxVoltage = Constants.MIN_SOA_VOL;

        private Double ValidateMaxVoltage(Double value)
        {
            if (value > MaxMaxVoltage)
            {
                value = MaxMaxVoltage;
            }
            else if (value < MinMaxVoltage)
            {
                value = MinMaxVoltage;
            }

            var step = GetStep(value, -3);

            value = Math.Round(value / step) * step;

            return value;
        }

        private AxisType _AxisType = AxisType.Linear;
        public AxisType AxisType
        {
            get => _AxisType;
            set
            {
                if (_AxisType != value)
                {
                    _AxisType = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }


        private Double _MaxLogY = Constants.DEF_SOA_LOGY_MAX;
        public Double MaxLogY
        {
            get => _MaxLogY;
            set
            {
                if (_MaxLogY != value)
                {
                    _MaxLogY = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxLogY = Constants.MAX_SOA_LOGY_MAX;

        public Double MinMaxLogY => MinLogY * Constants.DELTA_SOA_LOG;

        private Double _MinLogY = Constants.DEF_SOA_LOGY_MIN;
        public Double MinLogY
        {
            get => _MinLogY;
            set
            {
                if (_MinLogY != value)
                {
                    _MinLogY = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxMinLogY => MaxLogY / Constants.DELTA_SOA_LOG;

        public readonly Double MinMinLogY = Constants.MIN_SOA_LOGY_MIN;

        private Double _MaxLinY = 6;
        public Double MaxLinY
        {
            get => _MaxLinY;
            set
            {
                if (_MaxLinY != value)
                {
                    _MaxLinY = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxLinY = Constants.MAX_SOA_LINY_MAX;

        public Double MinMaxLinY => MinLinY + Constants.DELTA_SOA_LIN;


        private Double _MinLinY = Constants.DEF_SOA_LINY_MIN;
        public Double MinLinY
        {
            get => _MinLinY;
            set
            {
                if (_MinLinY != value)
                {
                    _MinLinY = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxMinLinY => MaxLinY - Constants.DELTA_SOA_LIN;

        public readonly Double MinMinLinY = Constants.MIN_SOA_LINY_MIN;

        private Double _MaxLogX = Constants.DEF_SOA_LOGX_MAX;
        public Double MaxLogX
        {
            get => _MaxLogX;
            set
            {
                if (_MaxLogX != value)
                {
                    _MaxLogX = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxLogX = Constants.MAX_SOA_LOGX_MAX;

        public Double MinMaxLogX => MinLogX * Constants.DELTA_SOA_LOG;


        private Double _MinLogX = Constants.DEF_SOA_LOGX_MIN;
        public Double MinLogX
        {
            get => _MinLogX;
            set
            {
                if (_MinLogX != value)
                {
                    _MinLogX = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxMinLogX => MaxLogX / Constants.DELTA_SOA_LOG;

        public readonly Double MinMinLogX = Constants.MIN_SOA_LOGX_MIN;

        private Double _MaxLinX = 8;
        public Double MaxLinX
        {
            get => _MaxLinX;
            set
            {
                if (_MaxLinX != value)
                {
                    _MaxLinX = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxMaxLinX = Constants.MAX_SOA_LINX_MAX;

        public Double MinMaxLinX => MinLinX + Constants.DELTA_SOA_LIN;

        private Double _MinLinX = -4;
        public Double MinLinX
        {
            get => _MinLinX;
            set
            {
                if (_MinLinX != value)
                {
                    _MinLinX = value;
                    MakeLinearMask();
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxMinLinX => MaxLinX - Constants.DELTA_SOA_LIN;

        public readonly Double MinMinLinX = Constants.MIN_SOA_LINX_MIN;

        private Boolean _RunCompleted = false;
        public Boolean RunCompleted
        {
            get { return _RunCompleted; }
            set
            {
                if (_RunCompleted != value)
                {
                    _RunCompleted = value;
                }
            }
        }

        private void MakeLinearMask()
        {
            lock ((_MaskPolygon as IList).SyncRoot)
            {
                var xmax = MaxLinX;
                var xmin = MinLinX;
                var ymax = MaxLinY;
                var ymin = MinLinY;
                if (AxisType == AxisType.Log)
                {
                    xmax = MaxLogX;
                    xmin = MinLogX;
                    ymax = MaxLogY;
                    ymin = MinLogY;
                }

                _MaskPolygon.Clear();

                _MaskPolygon.Add((xmin, MaxCurrent));

                var umin = MaxPower / MaxCurrent;

                if (umin > 0 && umin < MaxVoltage)
                {
                    var du = (MaxVoltage - umin) / 1000;
                    var u = umin;
                    for (Int32 j = 1; j < 1000; j++)
                    {
                        _MaskPolygon.Add((u, MaxPower / u));
                        u += du;
                    }
                }
                else
                {
                    _MaskPolygon.Add((MaxVoltage, MaxCurrent));
                }

                _MaskPolygon.Add((MaxVoltage, ymin));

                _MaskPolygon.Add((xmax, ymin));
                _MaskPolygon.Add((xmax, ymax));
                _MaskPolygon.Add((xmin, Math.Max(ymax, MaxCurrent)));
            }
        }

        private Boolean TestByLinear()
        {
            var xch = DsoModel.Default.GetChannel(Analysis.VoltageSrc1);
            if (xch?.Pack is null)
            {
                return false;
            }
            var ych = DsoModel.Default.GetChannel(Analysis.CurrentSrc1);
            if (ych?.Pack is null)
            {
                return false;
            }

            var xbuffer = xch.Pack.Buffer.ToJagged()[0];
            var ybuffer = ych.Pack.Buffer.ToJagged()[0];

            if (AcqWfms < Int32.MaxValue)
            {
                AcqWfms++;
            }

            TestSucceed = true;

            if (_MaskPolygon.Count < 3)
            {
                MakeLinearMask();
            }
            else
            {
                lock ((_MaskPolygon as ICollection).SyncRoot)
                {

                    for (Int32 i = (Int32)ych.Pack.Properties.VuStartIndex; i < xbuffer.Length; i++)
                    {
                        var pt = (Convert.ToSingle(xbuffer[i] / 1000), Convert.ToSingle(ybuffer[i] / 1000));//todo mV->V

                        if (_MaskPolygon.InPolygon(pt))
                        {
                            TestSucceed = false;
                        }

                    }
                }
            }

            if (!TestSucceed)
            {
                if (FailWfms < Int32.MaxValue)
                {
                    FailWfms++;
                }

                if (StopOnFault)
                {
                    Dispatcher.Stop();
                }
            }

            return true;
        }
        private void MaskTesting()
        {
            var xch = DsoModel.Default.GetChannel(Analysis.VoltageSrc1);
            if (xch?.Pack is null)
            {
                return;
            }
            var ych = DsoModel.Default.GetChannel(Analysis.CurrentSrc1);
            if (ych?.Pack is null)
            {
                return;
            }
            var xbuffer = xch.Pack.Buffer.ToJagged()[0];
            var ybuffer = ych.Pack.Buffer.ToJagged()[0];

            if (AcqWfms < Int32.MaxValue)
            {
                AcqWfms++;
            }

            Boolean testsucceed = true;

            Boolean hitstate = false;
            var hits = new List<List<(Double x, Double y)>>();
            var hit = new List<(Double x, Double y)>();

            for (Int32 i = (Int32)ych.Pack.Properties.VuStartIndex; i < xbuffer.Length; i++)
            {
                var pt = (Convert.ToSingle(xbuffer[i] / 1000), Convert.ToSingle(ybuffer[i] / 1000));//todo mV->V

                if (_MaskData.InPolygon(pt))
                {
                    testsucceed = false;
                    if (hitstate == false && hit.Count > 0)
                    {
                        hits.Add(hit);
                        hit = new List<(Double x, Double y)>();
                    }
                    hitstate = true;
                    hit.Add(pt);
                }
                else
                {
                    hitstate = false;
                }

            }
            if (hit.Count > 0)
            {
                hits.Add(hit);
            }
            TestSucceed = testsucceed;
            HitsBuffer = hits;

            if (!TestSucceed)
            {
                if (FailWfms < Int32.MaxValue)
                {
                    FailWfms++;
                }

                if (StopOnFault)
                {
                    Dispatcher.Stop();
                }
            }
        }
        public void Run()
        {
            _RunCompleted = false;
            MaskTesting();
            _RunCompleted = true;
        }

        public void Reset()
        {
            TestSucceed = false;
            AcqWfms = 0;
            FailWfms = 0;
        }

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
