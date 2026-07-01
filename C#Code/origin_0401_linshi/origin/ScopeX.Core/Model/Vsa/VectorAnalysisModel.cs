// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/2</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Channels;
    using ScopeX.ComModel;
    using ScopeX.MathExt;

    internal partial class VectorAnalysisModel : INotifyPropertyChanged
    {
        public VectorAnalysisModel()
        {
            GenerateDigtalModel = new();
            BluetoothModel = new();
            _RFNodeList = new()
            {
                new MixerNode(OnPropertyChanged),
                new FilterNode(OnPropertyChanged),
                new CarrierEstNode(OnPropertyChanged),
                new EqualizerNode(OnPropertyChanged),
                new PhaseEstNode(OnPropertyChanged)
            };

            _IQNodeList = new()
            {
                new DCBlockNode(OnPropertyChanged),
                new FilterNode(OnPropertyChanged),
                new EqualizerNode(OnPropertyChanged),
            };

            _CustomNodeList = new(8)
             {
                new NoOpNode(OnPropertyChanged),
                new NoOpNode(OnPropertyChanged),
                new NoOpNode(OnPropertyChanged),
                new NoOpNode(OnPropertyChanged),

                new NoOpNode(OnPropertyChanged),
                new NoOpNode(OnPropertyChanged),
                new NoOpNode(OnPropertyChanged),
                new NoOpNode(OnPropertyChanged),
            };

            _DspNodeList = _RFNodeList;
        }


        private Boolean _Enabled = false;
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    Acquisition.Default.UpdateReadInfoList();
                    OnPropertyChanged();
                }
            }
        }

        public VsaSignalType SignalType { get; set; } = VsaSignalType.GeneralDigtal;


        private VsaFormatOpt _Modulation = VsaFormatOpt.QAM16;
        public VsaFormatOpt Format
        {
            get => _Modulation;
            set
            {
                if (_Modulation != value)
                {
                    _Modulation = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _SymbolRate = Constants.MIN_SYMBOL_RATE;

        public Double SymbolRate
        {
            get => _SymbolRate;
            set
            {
                value = ValidateSymbolRate(value);

                if (_SymbolRate != value)
                {
                    _SymbolRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxSymbolRate = Constants.MAX_SYMBOL_RATE;

        public readonly Double MinSymbolRate = Constants.MIN_SYMBOL_RATE;

        private Double ValidateSymbolRate(Double value)
        {
            value = Math.Round(value, 7, MidpointRounding.AwayFromZero);

            if (value > MaxSymbolRate)
            {
                value = MaxSymbolRate;
            }
            else if (value < MinSymbolRate)
            {
                value = MinSymbolRate;
            }

            return value;
        }

        private Int32 _BitsPerSym = Constants.MIN_BITS_PER_SYM;

        public Int32 BitsPerSym
        {
            get => _BitsPerSym;
            set
            {
                value = ValidateBitsPerSym(value);

                if (_BitsPerSym != value)
                {
                    _BitsPerSym = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int32 MaxBitsPerSym = Constants.MAX_BITS_PER_SYM;

        public readonly Int32 MinBitsPerSym = Constants.MIN_BITS_PER_SYM;

        private Int32 ValidateBitsPerSym(Int32 value)
        {
            if (value > MaxBitsPerSym)
            {
                value = MaxBitsPerSym;
            }
            else if (value < MinBitsPerSym)
            {
                value = MinBitsPerSym;
            }

            return value;
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

        private ChannelId _Source2nd = ChannelId.C2;

        public ChannelId Source2nd
        {
            get => _Source2nd;
            set
            {
                if (_Source2nd != value && Template == VsaTemplateOpt.IQ)
                {
                    _Source2nd = value;
                    OnPropertyChanged();
                }
            }
        }

        public UInt32 DataLength => 1024 * 1024;

        private VsaItplOpt _Interpolation = VsaItplOpt.Linear;

        public VsaItplOpt Interpolation
        {
            get => _Interpolation;
            set
            {
                if (_Interpolation != value)
                {
                    _Interpolation = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _SampPerBaud = Constants.MIN_SAMP_PER_BAUD;

        public Int32 SampPerBaud
        {
            get => _SampPerBaud;
            set
            {
                value = ValidateSampPerBaud(value);

                if (_SampPerBaud != value)
                {
                    _SampPerBaud = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int32 MaxSampPerBaud = Constants.MAX_SAMP_PER_BAUD;

        public readonly Int32 MinSampPerBaud = Constants.MIN_SAMP_PER_BAUD;

        private Int32 ValidateSampPerBaud(Int32 value)
        {
            if (value > MaxBitsPerSym)
            {
                value = MaxBitsPerSym;
            }
            else if (value < MinBitsPerSym)
            {
                value = MinBitsPerSym;
            }

            return value;
        }

        private VsaTimingEstOpt _TimingEst = VsaTimingEstOpt.Square;

        public VsaTimingEstOpt TimingEst
        {
            get => _TimingEst;
            set
            {
                if (_TimingEst != value)
                {
                    _TimingEst = value;
                    OnPropertyChanged();
                }
            }
        }

        private VsaTemplateOpt _Template = VsaTemplateOpt.RF;

        public VsaTemplateOpt Template
        {
            get => _Template;
            set
            {
                if (_Template != value)
                {
                    _Template = value;
                    SetNodeList(value);
                    OnPropertyChanged();
                }

                void SetNodeList(VsaTemplateOpt vto)
                {
                    foreach (var node in _DspNodeList)
                    {
                        node.OnPropertyChanged = null;
                    }

                    _DspNodeList = vto switch
                    {
                        VsaTemplateOpt.RF => _RFNodeList,
                        VsaTemplateOpt.IQ => _IQNodeList,
                        _ => _CustomNodeList,
                    };

                    foreach (var node in _DspNodeList)
                    {
                        node.OnPropertyChanged = OnPropertyChanged;
                    }
                }
            }
        }

        private VsaGraphType _GraphType = VsaGraphType.ITime;
        public VsaGraphType GraphType
        {
            get => _GraphType;
            set
            {
                if (_GraphType != value && SignalType == VsaSignalType.GeneralDigtal)
                {
                    _GraphType = value;
                    OnPropertyChanged();
                }
            }
        }

        public IEnumerable<Enum> GraphTypeGroup
        {
            get
            {
                switch (SignalType)
                {
                    case VsaSignalType.GeneralDigtal:
                        return Enum.GetValues(typeof(VsaGraphType)).Cast<Enum>();
                    default:
                        return Enum.GetValues(typeof(VsaGraphType)).Cast<Enum>();
                }
            }
        }

        public GenerateDigtalModel GenerateDigtalModel;

        public BluetoothModel BluetoothModel;

        private Double[] _DataReading = new Double[0];
        private Double _DataReadingSampleFreq = 0.1;

        private Double[] _DataReaded;
        private Double _DataReadedSampleFreq = 0.1;

        private volatile Boolean _IsUpdateReading = false;
        private Object _Lock = new Object();

        public void SetSampleData(UInt16[] data, Double sampleInterval)
        {
            if (sampleInterval.Equals(0))
                return;
            lock (_Lock)
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel((ChannelId)Source);
                var pos0 = ach.Conditioning.PosIndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                var ratio = ach.Conditioning.Scale / Constants.SAMPS_PER_YDIV;
                _DataReading = data.Select(o => (Double)(o - pos0) * ratio).ToArray();
                _DataReadingSampleFreq = 1 / sampleInterval;
                _IsUpdateReading = true;
            }
        }

        public (Double carrierFreq, Double symbolRate, Double evm, Double snr) EstimateCarrierAndSymbolRate()
        {
            return GenerateDigtalModel.EstimateCarrierAndSymbolRate(_DataReading, _DataReadingSampleFreq);
        }

        private void SwitchData()
        {
            if (DsoModel.DataSrcOpt == DataSourceOpt.Simulator)
            {
                foreach (var ch in DsoModel.Default.AnalogChnls)
                {
                    if (ch.Id == _Source && ch.VuDatabase.Current != null)
                    {
                        var dataMatrix = ch.VuDatabase.Current.Buffer;
                        _DataReaded = new double[dataMatrix.Length];
                        if (dataMatrix.GetLength(0) < dataMatrix.GetLength(1))
                        {
                            for (int i = 0; i < dataMatrix.Length; i++)
                            {
                                _DataReaded[i] = dataMatrix[0, i];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dataMatrix.Length; i++)
                            {
                                _DataReaded[i] = dataMatrix[i, 0];
                            }
                        }

                    }
                    else break;
                }

                //_DataReaded = ReadDataFromFile(@"modSignal.txt");
                _DataReadedSampleFreq = 2.5E9;
                return;
            }

            lock (_Lock)
            {
                if (!_IsUpdateReading)
                    return;
                _DataReaded = _DataReading.ToArray();
                _DataReadedSampleFreq = _DataReadingSampleFreq;
                _IsUpdateReading = false;
            }
        }

        private Double[] ReadDataFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new Double[0];
            }
            StreamReader sr = new StreamReader(fileName);
            List<Double> data = new List<Double>();

            while (!sr.EndOfStream)
            {
                data.Add(Double.Parse(sr.ReadLine()!));
            }
            sr.Close();
            return data.ToArray();
        }

        public void Run()
        {
            try
            {
                if (!_Enabled)
                {
                    return;
                }

                SwitchData();

                if (_DataReaded.Length == 0)
                    return;

                switch (SignalType)
                {
                    case VsaSignalType.GeneralDigtal:
                        GenerateDigtalModel.Run_New(_DataReaded, _DataReadedSampleFreq);
                        break;
                    case VsaSignalType.Bluetooth:
                        BluetoothModel.Run();
                        break;
                    default:
                        break;

                }
            }
            catch (Exception e) 
            {
            
            }
        }

        private List<DspNode> _DspNodeList;

        public readonly List<DspNode> _RFNodeList;

        public readonly List<DspNode> _IQNodeList;

        public readonly List<DspNode> _CustomNodeList;

        public DspNode? GetNode(Int32 index)
        {
            var nodes = _DspNodeList;
            if (index >= 0 && index < nodes.Count)
            {
                return nodes[index];
            }
            return null;
        }

        public MixerNode MakeMixerNode() => new(OnPropertyChanged);

        public FilterNode MakeFilterNode() => new(OnPropertyChanged);

        public EqualizerNode MakeEqualizerNode() => new(OnPropertyChanged);

        public DCBlockNode MakeDCBlockNode() => new(OnPropertyChanged);

        public CarrierEstNode MakeCarrierEstNode() => new(OnPropertyChanged);

        public PhaseEstNode MakePhaseEstNode() => new(OnPropertyChanged);

        public CustomNode MakeCustomNode() => new(OnPropertyChanged);

        public NoOpNode MakeNoOpNode() => new(OnPropertyChanged);

        public Boolean SetNode(Int32 index, DspNode node)
        {
            if (_CustomNodeList.Count >= index && index >= 0)
            {
                if (node.NodeType != _CustomNodeList[index].NodeType)
                {
                    _CustomNodeList[index] = node;
                    OnPropertyChanged($"NodeType{index}");
                    return true;
                }
            }
            return false;
        }

        public Int32 Count => _DspNodeList.Count;

        public void AddNode(DspNode node)
        {
            _CustomNodeList.Add(node);

            OnPropertyChanged($"NodeType{_CustomNodeList.Count - 1}");
        }

        public void RemoveNodeAt(Int32 index)
        {
            if (_CustomNodeList.Count >= index && index >= 0)
            {
                _CustomNodeList[index].OnPropertyChanged = null;
                _CustomNodeList.RemoveAt(index);
                OnPropertyChanged($"NodeType{index}");
            }
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

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "") => _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
