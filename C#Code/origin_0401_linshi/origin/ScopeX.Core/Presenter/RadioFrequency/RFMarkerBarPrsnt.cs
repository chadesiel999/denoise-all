using System;
using System.Collections.Generic;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class RFMarkerBarPrsnt
    {
        private protected RFMarkerBarModel Model
        {
            get;
        }
        public Boolean Active
        {
            get => Model.Active;
            set => Model.Active = value;
        }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }

        public Color DrawColor
        {
            get => Model.DrawColor;
            set => Model.DrawColor = value;
        }


        private readonly Func<ChannelId, Color> _GetColor;

        public Color Color => _GetColor(Source);

        public CursorPosFormat PosFormat
        {
            get => Model.PosFormat;
            set => Model.PosFormat = value;
        }

        public Double InitialRefPos
        {
            get => Model.InitialRefPos;
            set => Model.InitialRefPos = value;
        }

        public Double FinalRefPos
        {
            get => Model.FinalRefPos;
            set => Model.FinalRefPos = value;
        }

        public Double? this[Int32 index]
        {
            get => Model[index];
            set => Model[index] = value;
        }

        public Double MaxPosIndex => Model.MaxPosIndex;

        public Double MinPosIndex => Model.MinPosIndex;

        //public Double MaxPosition => Model.MaxPosition;

        //public Double MinPosition => Model.MinPosition;

        public IEnumerable<(Int32 Index, Double Position)> PosIndexes => Model.PosIndexes;

        public List<(Int32 PositionIndex, Double Amp/*, Double Frequency*/)>? LastResult
        {
            get { return Model.LastResult; }
        }

        public Int32 Capacity => Model.Capacity;

        public void MoveAll(Double dist) => Model.MoveAll(dist);

        public (Double Value, Prefix Pfx, String Unit) GetPosInfo(Double position, Func<Double, Object?, (Double Value, Prefix Pfx, String Unit)>? converter = null, Object? arg = null)
        {
            return Model.GetPosInfo(position, converter, arg);
        }
        public (Double Value, Prefix Pfx, String Unit) GetValueInfo(Int32 position) => Model.GetValueInfo(position);

        public (List<Double>? Values, Prefix Pfx, String Unit) GetFirstSamples(Double position) => Model.GetFirstSamples(position);

        public (List<Double>? Values, Prefix Pfx, String Unit) GetFirstSamples(Double position, ChannelId source) => Model.GetFirstSamples(position, source);

        public (Double[,]? Buffer, Range? Index, Prefix Pfx, String Unit) GetSamples(Double position) => Model.GetSamples(position);

        public Double GetFirstSamplesCoordinateY(Double position) => Model.GetFirstSamplesCoordinateY(position);

        public (List<Double>? Values, Prefix Pfx, String Unit) GetSamples(Double position, Func<Double[,], Range, List<Double>> calculator) => Model.GetSamples(position, calculator);

        public List<(Int32 PositionIndex, Double Amp/*, Double Frequency*/)>? FindMarkablePeaks() => Model.GetLargestValueIndexs();
        //public (Double samplerate,Double offset) FindPeaksSampleInfo() => Model.FindPeaksSampleInfo();

        internal RFMarkerBarPrsnt(RFMarkerBarModel m, Func<ChannelId, Color> getColor)
        {
            Model = m;
            _GetColor = getColor;
        }
    }
}
