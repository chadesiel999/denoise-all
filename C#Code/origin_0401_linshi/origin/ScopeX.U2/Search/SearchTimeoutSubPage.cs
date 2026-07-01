using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2.Search
{
    public partial class SearchTimeoutSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public SearchTimeoutSubPage(SearchTimeoutPrsnt prsnt)
        {
            InitializeComponent();
            Presenter = prsnt;
            Init();
        }

        public SearchTimeoutPrsnt Presenter
        {
            get;
            set;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchTimeoutPrsnt)value; }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Source):
                    CbxSource.Text = Presenter.Source.ToString();
                    break;
                case "CompPosIndex":
                    NebPositon.UpdateValueString();
                    break;
                case nameof(Presenter.Polarity):
                    RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                    break;
                case nameof(Presenter.DurationByps):
                    NebDuration.UpdateValueString();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxSource.Text = Presenter.Source.ToString();
                NebPositon.UpdateValueString();
                NebDuration.UpdateValueString();
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;
                _ArgToCtrl = false;
            }
        }

        private void BtnResetDuration_Click(object sender, EventArgs e)
        {
            Presenter.DurationByps = Presenter.MinDuration;
        }

        private void BtnResetPosition_Click(object sender, EventArgs e)
        {
            Presenter.ResetPosIndex();
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
            }
        }

        private String CompPosToString()
        {
            return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString(5, true);
        }

        private String DurationToString()
        {
            return new Quantity(Presenter.DurationByps, Prefix.Pico, "s").ToString("##0.######", true);
        }

        private void Init()
        {
            //NebPositon
            ControlsHotKnob.Default.InitHotKnob(NebPositon);
            NebPositon.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPositon);
            };
            NebPositon.StringFormatFunc = (_) => CompPosToString();
            NebPositon.AddClicked = (_, e) => Presenter.PosIndex += e.Step;
            NebPositon.SubClicked = (_, e) => Presenter.PosIndex += e.Step;
            NebPositon.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPositon);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.CompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblPosition.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.CompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };

            //NebDuration
            ControlsHotKnob.Default.InitHotKnob(NebDuration);
            NebDuration.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebDuration);
            };
            NebDuration.AddClicked = (o, e) => Presenter.AdjDuration(e.Step);
            NebDuration.SubClicked = (o, e) => Presenter.AdjDuration(e.Step);
            NebDuration.StringFormatFunc = (_) => DurationToString();
            NebDuration.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebDuration);
                var onokclickeventaction = new Action<double>((data) =>
                    Presenter.DurationByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                nkf.SetKeyBoardValue(LblDurationps.Text, "s", 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.DurationByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MaxDuration, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MinDuration, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };

            InitSourceList();
        }

        private void InitSourceList()
        {
            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
        }

        private void RdoPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (LevelPolarity)RdoPolarity.ChoosedButtonIndex;
            }
        }
    }
}
