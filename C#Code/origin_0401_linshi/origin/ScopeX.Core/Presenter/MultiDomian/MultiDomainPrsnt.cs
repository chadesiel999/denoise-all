using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class MultiDomainPrsnt : MulticastPrsnt<IMultiDomainView>, IMultiDomainPrsnt
    {
        public MultiDomainPrsnt(IDsoPrsnt idp, IMultiDomainView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.MultiDomain,
                ModelCreateOptions.Standalone => new MultiDomainModel(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        private protected override MultiDomainModel Model { get; }

        public Int64 MaxSampleRate
        {
            get => Model.MaxSampleRate;
        }

        public Boolean Active
        { 
            get => Model.Active;
            set
            {
                Model.Active = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            } 
        }

        public Boolean ThreeDimensionalEnable
        {
            get => Model.ThreeDimensionalEnable;
            set
            {
                Model.ThreeDimensionalEnable = value;
            }
        }

        public Int64? ThreeDimensionalWindowsId
        { 
            get => Model.ThreeDimensionalWindowsId;
            set => Model.ThreeDimensionalWindowsId = value;
        }

        public Int32 ThreeDimensionalBuildCnt
        {
            get => Model.ThreeDimensionalBuildCnt;
            set
            {
                Model.ThreeDimensionalBuildCnt++;
            }
        }

        //public List<Double[]> ThreeDimensionalData => Model.ThreeDimensionalBuffer;
        public List<List<Double>> ThreeDimensionalData => Model.WaterFallsBuffer;

        public Boolean SynchronizationEnable
        {
            get => Model.SynchronizationEnable;
            set
            {
                Model.SynchronizationEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        //private Double _ZoomStart = 0;
        public Double ZoomStart
        {
            get => Model.ZoomStart;
            set
            {
                Model.ZoomStart = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
                //if (_ZoomStart != value)
                //{
                //    _ZoomStart = value;
                //    Model.ZoomStart = _ZoomStart;
                //    Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
                //}
            }
        }

        //private Double _ZoomLength = 0;
        public Double ZoomLength
        {
            get => Model.ZoomLength;
            set
            {
                Model.ZoomLength = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
                //if (_ZoomLength != value)
                //{
                //    _ZoomLength = value;
                //    Model.ZoomLength = _ZoomLength;
                //    Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
                //}
            }
        }

        public RectangleF ValidArea(RectangleF rect)
        {
            return Model.ValidArea(rect);
        }

        public Boolean ParameterTuningEnable
        {
            get => Model.ParameterTuningEnable;
            set
            {
                Model.ParameterTuningEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public LogarithmUnit PUnit
        { 
            get => Model.PUnit;
            set => Model.PUnit = value;
        }

        public Boolean NormalLineActive
        { 
            get => Model.NormalLineActive;
            set => Model.NormalLineActive = value;
        }

        public Boolean MaxHoldLineActive
        {
            get => Model.MaxHoldLineActive;
            set => Model.MaxHoldLineActive = value;
        }

        public Boolean MinHoldLineActive
        {
            get => Model.MinHoldLineActive;
            set => Model.MinHoldLineActive = value;
        }

        public Boolean AverageLineActive
        {
            get => Model.AverageLineActive;
            set => Model.AverageLineActive = value;
        }

        public Int32 AverageCount
        { 
            get => Model.AverageCount;
            set => Model.AverageCount = value;
        }

        public RFWindowType WindowType
        { 
            get => Model.WindowType;
            set
            {
                Model.WindowType = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Double RBWByHz
        {
            get => Model.RBWByHz;
            set
            {
                Model.RBWByHz = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int64 SpanFreqByHz
        {
            get => Model.SpanFreqByHz;
            set
            {
                Model.SpanFreqByHz = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int32 SpanOptForTimeFreq
        {
            get => Model.SpanOptForTimeFreq;
            set
            {
                Model.SpanOptForTimeFreq = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public List<Int64> SpanListForTimeFreq
        {
            get => Model.SpanListForTimeFreq;
        }

        public Double TimeScaleForTimeFreq
        {
            get => Model.TimeScaleForTimeFreq;
            set
            {
                Model.TimeScaleForTimeFreq = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Double MinTimeScale => Model.MinTimeScale;

        public Double MaxTimeScale => Model.MaxTimeScale;


        public Int64 CenterFreqByHz
        {
            get => Model.CenterFreqByHz;
            set
            {
                Model.CenterFreqByHz = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int64 StartFreqByHz
        {
            get => Model.StartFreqByHz;
            set
            {
                Model.StartFreqByHz = value;
            }
        }

        public Int64 MaxSpanFreq => Model.MaxSpanFreq;
        public Int64 MinSpanFreq => Model.MinSpanFreq;

        public Int64 EndFreqByHz
        {
            get => Model.EndFreqByHz;
            set
            {
                Model.EndFreqByHz = value;
            }
        }

        public Int64 FFTLength
        {
            get => Model.FFTLength;
            set
            {
                Model.FFTLength = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Int64 STFTLength
        {
            get => Model.STFTLength;
            set
            {
                Model.STFTLength = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Double TimeStep
        {
            get => Model.TimeStep;
            set
            {
                Model.TimeStep = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public void ExcuteRoughSpec()
        {
            Model.RoughSpecCnt++;
            Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
        }

        public Int64 STFTStep
        {
            get => Model.STFTStep;
            set
            {
                Model.STFTStep = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public MultiDomainFigureEnum CurFigureType
        { 
            get => Model.CurFigureType;
            set => Model.CurFigureType = value;
        }

        public Boolean CurFigureEnable
        {
            get => Model.CurFigureEnable;
            set
            {
                Model.CurFigureEnable = value;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }
        }

        public Boolean TryGetTimeFreqData(out Double[,] data)
        {
            data = (Double[,])Model.SpectrogramBufferArray.Clone();
            return true;
        }
    }
}
