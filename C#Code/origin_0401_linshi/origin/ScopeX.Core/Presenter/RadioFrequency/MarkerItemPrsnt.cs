using System;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Core.Model.RadioFrequency;
using ScopeX.Core.Tools;

namespace ScopeX.Core.Presenter.RadioFrequency
{
    public class MarkerItemPrsnt : MulticastPrsnt<IMarkerItemView>, Core.IMarkerItemPrsnt
    {
        internal MarkerItemPrsnt(IDsoPrsnt idp, MarkerItemModel m, IMarkerItemView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        private protected MarkerItemModel ItemModel { get; }

        private protected override MarkerItemModel Model => ItemModel;


        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }
        public Boolean ManualMarkerActive
        {
            get => Model.ManualMarkerActive;
            set => Model.ManualMarkerActive = value;
        }

        public Boolean AtuoMarkerActive
        {
            get => Model.AtuoMarkerActive;
            set => Model.AtuoMarkerActive = value;
        }

        public Boolean MarkerResultsTableEnable
        {
            get => Model.MarkerResultsTableEnable;
            set => Model.MarkerResultsTableEnable = value;
        }
        public ChannelId Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }
        public Int32 FocusId
        {
            get => Model.FocusId;
            set => Model.FocusId = value;
        }

        public MarkerReadMode ReadMode
        {
            get => Model.ReadMode;
            set => Model.ReadMode = value;
        }

        public SortOption SortMode
        {
            get => Model.SortMode;
            set => Model.SortMode = value;
        }

        public Double RFThreshold
        {
            get => Model.RFThreshold;
            set => Model.RFThreshold = value;
        }

        public Double RFExcursion
        {
            get => Model.RFExcursion;
            set => Model.RFExcursion = value;
        }

        public Int32 MaxMarkerCount
        {
            get => Model.MaxMarkerCount;
            set => Model.MaxMarkerCount = value;
        }

        public void MoveCursor(Int32 index, Int32 step)
        {
            MaunalMarkerBar[index] += step;
        }

        public String Unit => Model.Unit;

        public Prefix Prefix => Model.Prefix;

        //Horizontal Cursor For Amplitude Measurment
        public RFMarkerBarPrsnt MaunalMarkerBar { get; init; }

        //Vertical Cursor For Time and Amplitude Measurement
        public RFMarkerBarPrsnt AutoMarkerBar { get; init; }


        public Boolean IsSync
        {
            get;
            set;
        }

        public (String X_Unit, String Y_Unit) GetMarkerUnit()
        {
            if (ChannelIdExt.IsMath(Source))
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(Source);

                return (mch.Sampling.Unit, mch.Conditioning.Unit);

            }

            return (String.Empty, String.Empty);
        }

        public void FrequencyToCenter()
        {
            if (AtuoMarkerActive)
            {
                if (DsoModel.Default.TryGetChannel(Source, out var channel) && channel is MathModel mp && mp != null && mp.Args != null && mp.Args is MathFftArg fftarg)
                {
                    if (channel.Active)
                    {
                        var peaks = AutoMarkerBar.LastResult;

                        peaks = peaks?.OrderByDescending(o => o.Amp).ToList();
                        if (peaks != null && channel.VuDatabase.Current != null && peaks.Count > 1)
                        {
                            var sampleRate = channel.VuDatabase.Current.ZoomRatio;
                            var offsetX = channel.VuDatabase.Current.Start;
                            var (ax, axp, axu) = AutoMarkerBar.GetPosInfo(offsetX + 1 / sampleRate * peaks[0].PositionIndex);
                            mp.FrequencyAdapter.ValueCenter = ax;
                        }
                    }
                }
            }
        }

        private (Double Scale, Double Pos0, Prefix Pfx, String Unit) GetHPosAxisInfo(ChannelId source)
        {
            if (ChannelIdExt.IsMath(source))
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(source);
                var freqadapter = mch.FrequencyAdapter;
                return (freqadapter.ValueSpan / Constants.VIS_XDIVS_NUM / mch.Sampling.PosIdxPerDiv, mch.Sampling.PosIndex, mch.Sampling.Prefix, mch.Sampling.Unit);
            }
            var chn = (RadioFrequencyModel)DsoModel.Default.GetChannel(source);
            var samplerate = ((Double)chn.Sampling.FrequencyScale / Constants.IDX_PER_XDIV) / chn.Sampling.RBW;
            var offset = ((Double)(chn.Sampling.StartFrequency - chn.Sampling.FigureStartFrequency) / chn.Sampling.FrequencyScale) * Constants.IDX_PER_XDIV;
            return (chn.Sampling.FrequencyScale / chn.Sampling.PosIdxPerDiv, chn.Sampling.PosIndex, chn.Sampling.Prefix, chn.Sampling.Unit);
        }

        private (Double[,]? Buffer, Double Scale, Double Pos0, Prefix Pfx, String Unit, Double SampleRate, Double Offset) GetVValueAxisInfo(ChannelId source)
        {
            if (ChannelIdExt.IsMath(source))
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(source);
                var freqadapter = mch.FrequencyAdapter;
                //var sampleRatem = ((Double)mch.FrequencyAdapter.ValueSpan / Constants.IDX_PER_XDIV) / (samplefrequency/ (Int32)(((MathFftArg)(mch.Args!)).Number));
                //var offsetm = ((Double)(0 - (mch.FrequencyAdapter.ValueCenter - mch.FrequencyAdapter.ValueSpan / 2)) / ((Double)mch.FrequencyAdapter.ValueSpan / Constants.IDX_PER_XDIV)) * Constants.IDX_PER_XDIV;
                var sampleratem = (Double)(mch.VuDatabase?.Current?.ZoomRatio <= 0 ? 1 : mch.VuDatabase?.Current?.ZoomRatio);
                var offsetm = (Double)(mch.VuDatabase?.Current?.Start);
                return (mch.VuDatabase?.Current?.Buffer, mch.Conditioning.Scale / mch.Conditioning.PosIdxPerDiv, mch.Conditioning.PosIndex, mch.Conditioning.Prefix, mch.Conditioning.Unit, sampleratem, offsetm);
            }
            var chn = (RadioFrequencyModel)DsoModel.Default.GetChannel(source);
            var samplerate = ((Double)chn.Sampling.FrequencyScale / Constants.IDX_PER_XDIV) / chn.Sampling.RBW;
            var offset = ((Double)(chn.Sampling.StartFrequency - chn.Sampling.FigureStartFrequency) / chn.Sampling.FrequencyScale) * Constants.IDX_PER_XDIV;
            //return (chn.VuDatabase?.All.First().Buffer, chn.Conditioning.ScaleBymV / chn.Conditioning.PosIdxPerDiv, chn.Conditioning.PosIndex, chn.Conditioning.Prefix, chn.Conditioning.Unit);
            return (chn.VuDatabase/*Normal*/?.Current?.Buffer, chn.Conditioning.AmpScale / chn.Conditioning.PosIdxPerDiv, chn.Conditioning.PosIndex, chn.Conditioning.Prefix, chn.Conditioning.Unit, samplerate, offset);
        }

        private (Double Scale, Double Pos0, Prefix Pfx, String Unit) GetVPosAxisInfo(ChannelId source)
        {
            if (ChannelIdExt.IsMath(source))
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(source);
                var freqadapter = mch.FrequencyAdapter;
                return (mch.Conditioning.Scale / mch.Conditioning.PosIdxPerDiv, mch.Conditioning.PosIndex, mch.Conditioning.Prefix, mch.Conditioning.Unit);
            }
            var chn = (RadioFrequencyModel)DsoModel.Default.GetChannel(source);

            return (chn.Conditioning.AmpScale / chn.Conditioning.PosIdxPerDiv, chn.Conditioning.PosIndex, chn.Conditioning.Prefix, chn.Conditioning.Unit);
        }

        private (Double[,]? Buffer, Double Scale, Double Pos0, Prefix Pfx, String Unit) GetHValueAxisInfo(ChannelId source)
        {
            if (ChannelIdExt.IsMath(source))
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(source);
                var freqadapter = mch.FrequencyAdapter;
                return (mch.VuDatabase?.Current?.Buffer, freqadapter.ValueSpan / Constants.VIS_XDIVS_NUM / mch.Sampling.PosIdxPerDiv, mch.Sampling.PosIndex, mch.Sampling.Prefix, mch.Sampling.Unit);
            }
            var chn = (RadioFrequencyModel)DsoModel.Default.GetChannel(source);

            //return (chn.VuDatabase?.All.First().Buffer, chn.Sampling.ScaleByus / chn.Sampling.PosIdxPerDiv, chn.Sampling.PosIndex, chn.Sampling.Prefix, chn.Sampling.Unit);
            return (chn.VuDatabaseNormal?.Current?.Buffer, chn.Sampling.FrequencyScale / chn.Sampling.PosIdxPerDiv, chn.Sampling.PosIndex, chn.Sampling.Prefix, chn.Sampling.Unit);
        }
    }
}
