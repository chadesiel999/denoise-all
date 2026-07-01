using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using NPOI.OpenXmlFormats.Spreadsheet;
using ScopeX.ComModel;
using ScopeX.Core.Presenter.RadioFrequency;
using ScopeX.Core.Tools;
using static ScopeX.Core.MathFftArg;

namespace ScopeX.Core
{
    public class MarkerPrsnt : MulticastPrsnt<IMarkerView>, IMarkerPrsnt
    {
        public ConcurrentDictionary<ChannelId, MarkerItemPrsnt> Items = new ConcurrentDictionary<ChannelId, MarkerItemPrsnt>();

        private Semaphore @lock = new Semaphore(1, 1);



        private protected override MarkerModel Model
        {
            get;
        }

        public MarkerPrsnt(IDsoPrsnt idp, IMarkerView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = DsoModel.Default.Markers;
            Model.PropertyChanged += OnPropertyChanged;

            //Attach View
            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }



        public Action<ChannelId> SwtichFocusItem;
        public void SwtichFocusItemMethod(ChannelId id)
        {
            SwtichFocusItem?.Invoke(id);
        }


        public void Run()
        {
            if (@lock.WaitOne())
            {
                Model.Run();
                @lock.Release();
            }
        }

        public MarkerItemPrsnt this[ChannelId id]
        {
            get
            {
                TryGetorAddItem(id, out var item);
                return item;
            }
        }

        public Boolean TryGetorAddItem(ChannelId source, out MarkerItemPrsnt prsnt)
        {
            var item = Items.Values.Where(x => x.Source == source);
            if (item == null || item.Count() == 0)
            {
                return AddItem(source, out prsnt);
            }
            prsnt = item.First();
            return true;
        }

        private Boolean AddItem(ChannelId source, out MarkerItemPrsnt prsnt)
        {
            @lock.WaitOne();
            ChannelId? markerid = ChannelId.MAKER1;
            var markers = ChannelIdExt.GetMarkers();
            if (Items.Count == markers.Count())
            {
                markerid = Items.Values.Where(x => !x.ManualMarkerActive).First().Id;
            }
            else
            {
                markerid = ChannelIdExt.GetMarkers().Where(o => !Items.ContainsKey(o)).First();
            }
            if (Model.AddMarker(source, out var model, markerid.Value))
            {
                model.MaunalMarker.GetPosAxisInfo = GetHPosAxisInfo;
                model.MaunalMarker.GetValueAxisInfo = GetVValueAxisInfo;
                model.AutoMarker.GetPosAxisInfo = GetHPosAxisInfo;
                model.AutoMarker.GetValueAxisInfo = GetVValueAxisInfo;
                prsnt = new MarkerItemPrsnt(Dso, model)
                {
                    MaunalMarkerBar = new RFMarkerBarPrsnt(model.MaunalMarker, c => DsoModel.Default.GetChannel(c).DrawColor),
                    AutoMarkerBar = new RFMarkerBarPrsnt(model.AutoMarker, c => DsoModel.Default.GetChannel(c).DrawColor)
                };
                Items.TryAdd(markerid.Value, prsnt);
                @lock.Release();
                return true;
            }
            @lock.Release();
            Items.TryGetValue(markerid.Value, out prsnt);
            return false;
        }

        public Boolean TryRemoveItem(ChannelId source)
        {
            @lock.WaitOne();
            var key = Items.Values.Where(x => x.Source == source).FirstOrDefault();
            if (key!=null)
            {
                Items.TryGetValue(key.Id, out var prsnt);
                if (prsnt != null)
                {
                    prsnt.MarkerResultsTableEnable = false;
                    Items.TryRemove(key.Id, out _);
                    Model.TryRemoveItem(key.Id);
                    @lock.Release();
                    return true;
                }
            }
            @lock.Release();
            return false;
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
