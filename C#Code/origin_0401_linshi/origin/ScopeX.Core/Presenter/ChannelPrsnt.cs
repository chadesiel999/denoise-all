using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using System.ComponentModel;

namespace ScopeX.Core
{
    public abstract class ChannelPrsnt : MulticastPrsnt<IChnlView>, IChnlPrsnt
    {
        private protected abstract override ChannelModel Model
        {
            get;
        }

        public ChannelPrsnt(IDsoPrsnt idp, IChnlView? view) : base(idp)
        {
            if (view != null)
            {
                //建立起View和Presenter的关联
                //这时候View中能使用它所对应的Presenter进行业务逻辑的操作
                view.Presenter = this;

                //通过构造函数将View注入到Presenter中
                TryAddView(view);
            }
        }

        public ChannelType Type => Model.Type;

        public ChannelId Id => Model.Id;

        public String Name => Model.Name;

        public Color DrawColor
        {
            get => Model.DrawColor;
            set
            {
                Model.DrawColor = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(DrawColor)));
            }
        }

        public virtual Boolean Active
        {
            get => Model.Active;
            set => Model.Active = value;
        }

        public String Label
        {
            get => Model.Label;
            set => Model.Label = value;
        }
        public Boolean LabelVisibility
        {
            get => Model.LabelVisibility;
            set => Model.LabelVisibility = value;
        }
        public abstract ISampling Sampling
        {
            get;
        }

        //Default stride is 1000/div, so its name suffix "BymDiv", but the corresponding model name does not.
        public virtual Double PosIndexBymDiv
        {
            get => Model.Conditioning.PosIndex;
            set
            {
                var oldposindex = Model.Conditioning.PosIndex;
                Model.Conditioning.PosIndex = value;
                Dispatcher.SoftReset();
                UpdateHCursorPosByPosindex(oldposindex);
            }
        }

        public Double PosMaxIndex => Model.Conditioning.PosMaxIndex;

        public Double PosMinIndex => Model.Conditioning.PosMinIndex;

        public virtual void ResetPosIndex()
        {
            var oldposindex = Model.Conditioning.PosIndex;
            Model.Conditioning.PosIndex = Model.Conditioning.PosDefIndex;
            Dispatcher.SoftReset();
            UpdateHCursorPosByPosindex(oldposindex);
        }

        public Double PosIdxPerDiv => Model.Conditioning.PosIdxPerDiv;

        /// <summary>
        /// Adjust 'ScaleIndex' to roughly change vertical scale.
        /// </summary>
        public virtual Int32 ScaleIndex
        {
            get => Model.Conditioning.ScaleIndex;
            set
            {
                var oldscale = Model.Conditioning.ScaleBymV;
                var oldposindex = Model.Conditioning.PosIndex;
                Model.Conditioning.ScaleIndex = value;
                DsoModel.Default.GetTrigger().LeapPosIndex();
                Dispatcher.SoftReset();
                if (!FineStatusChanged)
                {
                    UpdateHCursorPosByScale(oldscale, oldposindex, Model.Conditioning.ScaleBymVAdd != 0);
                }
            }
        }

        internal Boolean FineStatusChanged { get; set; } = false;

        /// <summary>
        /// Vertical scale value, its unit is usually 'mV'.
        /// </summary>
        public Double Scale
        {
            get => Model.Conditioning.Scale;

            set
            {
                var oldscale = Model.Conditioning.ScaleBymV;
                var oldposindex = Model.Conditioning.PosIndex;
                Model.Conditioning.Scale = value;
                UpdateHCursorPosByScale(oldscale, oldposindex);
            }
        }

        //public Double ScaleMaxIndex => Model.Conditioning.ScaleMaxIndex;

        //public Double ScaleMinIndex => Model.Conditioning.ScaleMinIndex;

        /// <summary>
        /// Adjust 'ScaleTick' to finely change vertical scale.
        /// </summary>
        public virtual Int32 ScaleTick
        {
            get => Model.Conditioning.ScaleTick;
            set
            {
                Model.Conditioning.ScaleTick = value;
                Dispatcher.SoftReset();
            }
        }



        public virtual String Unit
        {
            get => Model.Conditioning.Unit;
            set
            {
                if(value.Length>=3)
                {
                    value = value.Substring(0, 3);
                }
                Model.Conditioning.Unit = value;
            }
        }

        public Prefix Prefix => Model.Conditioning.Prefix;

        public WfmPack? Pack => Model.Pack;

        public WfmVuDatabase VuDatabase => Model.VuDatabase;
        public WfmVuDatabase ZoomVuDatabase => Model.ZoomVuDatabase;
        public Int64? WindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public static Int64 GetNewWindowId()
        {
            return IdFactory.NextId;
        }

        public static Color GetDrawColor(ChannelId id)
        {
            return ColorLookup.Default[id.ToString()];
        }

        internal virtual void UpdateHCursorPosByScale(Double oldScale, Double oldPosIndex, Boolean isAmplitudeFineTuning = false)
        {
            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.HCursor.Source == Id && DsoModel.Default.Cursors.Type != CursorType.Vertical)
            {
                var cuurentscale = isAmplitudeFineTuning ? Model.Conditioning.ScaleBymV : Model.Conditioning.Scale;
                DsoModel.Default.Cursors.HCursor.SetPoxIndex(0, (Double)(DsoModel.Default.Cursors.HCursor[0] - oldPosIndex) * oldScale / cuurentscale + Model.Conditioning.PosIndex);
                DsoModel.Default.Cursors.HCursor.SetPoxIndex(1, (Double)(DsoModel.Default.Cursors.HCursor[1] - oldPosIndex) * oldScale / cuurentscale + Model.Conditioning.PosIndex);
            }
        }

        internal virtual void UpdateHCursorPosByPosindex(Double oldPosIndex)
        {
            if (!DsoModel.Default.Cursors.TraceWave)
            {
                return;
            }
            if (DsoModel.Default.Cursors.Active && DsoModel.Default.Cursors.HCursor.Source == Id && DsoModel.Default.Cursors.Type != CursorType.Vertical)
            {
                DsoModel.Default.Cursors.HCursor.SetPoxIndex(0, (Double)DsoModel.Default.Cursors.HCursor[0] + PosIndexBymDiv - oldPosIndex);
                DsoModel.Default.Cursors.HCursor.SetPoxIndex(1, (Double)DsoModel.Default.Cursors.HCursor[1] + PosIndexBymDiv - oldPosIndex);

            }
        }
    }
}
