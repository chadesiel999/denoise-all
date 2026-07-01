using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    /// <summary>
    /// 通用数字解调的参数集合
    /// </summary>
    public class GenerateDigtalPrsnt : MulticastPrsnt<IVsaGenerateDigtalView>, IVsaVsaGenerateDigtalPrsnt
    {
        public GenerateDigtalPrsnt(IDsoPrsnt idp) : base(idp)
        {
            Model = DsoModel.Default.VectorAnalysisModel.GenerateDigtalModel;
            Model.PropertyChanged += OnPropertyChanged;

            _GraphPrsntTable.Add(VsaGraphType.ITime, new VsaGenerateDigtalGraphPrsnt(Model.InphaseTimeGraph));
            _GraphPrsntTable.Add(VsaGraphType.QTime, new VsaGenerateDigtalGraphPrsnt(Model.QuadratureTimeGraph));
            _GraphPrsntTable.Add(VsaGraphType.Constellation, new VsaGenerateDigtalGraphPrsnt(Model.ConstellationGraph));
            _GraphPrsntTable.Add(VsaGraphType.IEye, new VsaGenerateDigtalGraphPrsnt(Model.IEyeGraph));
            _GraphPrsntTable.Add(VsaGraphType.QEye, new VsaGenerateDigtalGraphPrsnt(Model.QEyeGraph));
            _GraphPrsntTable.Add(VsaGraphType.Vector, new VsaGenerateDigtalGraphPrsnt(Model.VectorGraph));
            _GraphPrsntTable.Add(VsaGraphType.EVM, new VsaGenerateDigtalGraphPrsnt(Model.EvmGraph));
            _GraphPrsntTable.Add(VsaGraphType.PhaseDeviation, new VsaGenerateDigtalGraphPrsnt(Model.PhaseErrorTimeGraph));
            _GraphPrsntTable.Add(VsaGraphType.AmplitudeDeviation, new VsaGenerateDigtalGraphPrsnt(Model.AmplitudeErrorTimeGraph));
        }

        private protected override GenerateDigtalModel Model { get; }

        private Dictionary<VsaGraphType, AdvancedMathPrsnt> _GraphPrsntTable = new();
        public AdvancedMathPrsnt? GetVsaGraphPrsnt(VsaGraphType graphType)
        {
            if (!_GraphPrsntTable.TryGetValue(graphType, value: out AdvancedMathPrsnt? graphprsnt))
            {
                return null;
            }
            return graphprsnt;
        }

        public VsaGraphType CurGraphType { get; set; } = VsaGraphType.ITime;

        public AdvancedMathPrsnt? GetCurVsaGraphPrsnt
        {
            get
            {
                if (!_GraphPrsntTable.TryGetValue(CurGraphType, out AdvancedMathPrsnt? graphprsnt))
                {
                    return null;
                }
                return graphprsnt;
            }
        }

        public VsaFormatOpt FormatOpt
        {
            get => Model.FormatOpt;
            set => Model.FormatOpt = value;
        }

        public Double SymbolRateMax
        {
            get => Model.SymbolRateMax;
        }

        public Double SymbolRateMin
        {
            get => Model.SymbolRateMin;
        }

        public Double SymbolRate
        {
            get => Model.SymbolRate;
            set => Model.SymbolRate = value;
        }

        public Double FilterPara
        {
            get => Model.FilterPara;
            set => Model.FilterPara = value;
        }

        public Double FilterParaMax
        {
            get => Model.FilterParaMax;
        }

        public Double FilterParaMin
        {
            get => Model.FilterParaMin;
        }

        public Dictionary<String, ParamStatistics> ErrParamTable
        {
            get { return Model.ErrParamTable; }
        }

        public VsaGraphType GraphType
        {
            get;
            set;
        }

        public VsaMeasureFilterTypeOpt MeasureFilterType
        {
            get => Model.MeasureFilterType;
            set => Model.MeasureFilterType = value;
        }

        public VsaRefFilterTypeOpt RefFilterType
        {
            get => Model.RefFilterType;
            set => Model.RefFilterType = value;
        }

        public VsaTimeSyncType TimeSyncType
        {
            get => Model.TimeSyncType;
            set => Model.TimeSyncType = value;
        }

        public VsaCarrySyncType CarrySyncType
        {
            get => Model.CarrySyncType;
            set => Model.CarrySyncType = value;
        }

        public Double CarryFreq
        {
            get => Model.CarryFreq;
            set => Model.CarryFreq = value;
        }

        public Double CarryFreqMax
        {
            get => Model.CarryFreqMax;
        }

        public Double CarryFreqMin
        {
            get => Model.CarryFreqMin;
        }


        public Double BandWidth
        {
            get => Model.BandWidth;
            set => Model.BandWidth = value;
        }

        public Double BandWidthMax
        {
            get => Model.BandWidthMax;
        }

        public Double BandWidthMin
        {
            get => Model.BandWidthMin;
        }

        public Double CarryFreqError
        {
            get => Model.CarryFreqError;
            set => Model.CarryFreqError = value;
        }

        public Double CarryFreqErrorMax
        {
            get => Model.CarryFreqErrorMax;
        }

        public Double CarryFreqErrorMin
        {
            get => Model.CarryFreqErrorMin;
        }

        public Boolean FreqDetect
        {
            get => Model.FreqDetect;
            set => Model.FreqDetect = value;
        }

        public Boolean EqualizerEnabled
        {
            get => Model.EqualizerEnabled;
            set => Model.EqualizerEnabled = value;
        }

        public VsaEqualizeMode EqualizeMode
        {
            get => Model.EqualizeMode;
            set => Model.EqualizeMode = value;
        }

        public Double ConvergenceCoefficient
        {
            get => Model.ConvergenceCoefficient;
            set => Model.ConvergenceCoefficient = value;
        }

        public Double ConvergenceCoefficientMax
        {
            get => Model.ConvergenceCoefficientMax;
        }
        public Double ConvergenceCoefficientMin
        {
            get => Model.ConvergenceCoefficientMin;
        }

        public Boolean EqualizerReset
        {
            get => Model.EqualizerReset;
            set => Model.EqualizerReset = value;
        }

        public EqualizeOverSampling OverSample
        {
            get => Model.OverSample;
            set => Model.OverSample = value;
        }

        public Int32 FilterCofficientCnt
        {
            get => Model.FilterCofficientCnt;
            set => Model.FilterCofficientCnt = value;
        }

        public Int32 FilterCofficientCntMax
        {
            get => Model.FilterCofficientCntMax;
        }
        public Int32 FilterCofficientCntMin
        {
            get => Model.FilterCofficientCntMin;
        }

        public Int32 SymbolLength
        {
            get => Model.SymbolLength;
            set => Model.SymbolLength = value;
        }

        public Int32 SymbolLengthMax
        {
            get => Model.SymbolLengthMax;
        }
        public Int32 SymbolLengthMin
        {
            get => Model.SymbolLengthMin;
        }

        public Double RollOffFactorMax
        {
            get => Model.RollOffFactorMax;
        }

        public Double RollOffFactorMin
        {
            get => Model.RollOffFactorMin;
        }

        public Double RollOffFactor
        {
            get => Model.RollOffFactor;
            set => Model.RollOffFactor = value;
        }


    }
}
