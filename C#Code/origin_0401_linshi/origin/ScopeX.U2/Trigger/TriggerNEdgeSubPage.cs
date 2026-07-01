// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using System.Collections.Generic;
    using static ScopeX.UserControls.SelectComboBox;


    public partial class TriggerNEdgeSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public TriggerNEdgeSubPage()
        {
            InitializeComponent();
            Init();
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

        public TrigNEdgePrsnt Presenter
        {
            get => (TrigNEdgePrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigNEdgePrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
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
                    //CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                    CbxSource.SelectValue = (Int32)Presenter.Source;
                    LblPosition.Visible = NebPositon.Visible = BtnResetPosition.Visible = !((ChannelId)Presenter.Source).IsDigital();
                    NebPositon.UpdateValueString();
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
                case nameof(Presenter.EdgeNumber):
                    NebEdgeNumber.UpdateValueString();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                InitSourceList();
                //CbxSource.SelectedIndex = CbxSource.FindStringExact(Presenter.Source.ToString());
                CbxSource.SelectValue = (Int32)Presenter.Source;
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Polarity;

                NebPositon.UpdateValueString();
                NebDuration.UpdateValueString();
                NebEdgeNumber.UpdateValueString();
                _ArgToCtrl = false;
            }
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
                    Presenter.VuCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix));

                nkf.SetKeyBoardValue(LblPosition.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuCompPosition, Presenter.PosPrefix),
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

            //NebEdgeNumber
            ControlsHotKnob.Default.InitHotKnob(NebEdgeNumber);
            NebEdgeNumber.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebEdgeNumber);
            };
            NebEdgeNumber.AddClicked = (o, e) => Presenter.EdgeNumber++;
            NebEdgeNumber.SubClicked = (o, e) => Presenter.EdgeNumber--;
            NebEdgeNumber.StringFormatFunc = (_) => Presenter.EdgeNumber.ToString();
            NebEdgeNumber.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebEdgeNumber);
                var onokclickeventaction = new Action<Double>((date) => Presenter.EdgeNumber = Convert.ToInt32(date));
                nkf.SetKeyBoardValue(LblEdgeNumber.Text, "#", 3, onokclickeventaction, Presenter.EdgeNumber, Presenter.MaxEdgeNumber, Presenter.MinEdgeNumber);

                nkf.ShowDialogByPosition();
            };

        }

        private void InitSourceList()
        {
            //只有模拟通道和数字通道
            var sources = PlatformUIManager.Default.Platform.GetTriggerSource(true);
            CbxSource.DataSource = sources.Select(o => new ComboBoxItem(o.ToString(), (Int32)o)).ToList();
        }

        private void BtnResetPosition_Click(object sender, EventArgs e)
        {
            Presenter.SetPosIndexCenter();
        }

        //private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.Source = Enum.Parse<ChannelId>(CbxSource.SelectedItem.ToString());
        //    }
        //}

        private void RdoPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Polarity = (EdgeSlope)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private String CompPosToString()
        {
            //return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            if (Presenter.PosUnit == "V")
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.######", true, 7);
            }
            else
            {
                return new Quantity(Presenter.VuCompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString("##0.###", true, 7);
            }
        }

        private String DurationToString()
        {
            return new Quantity(Presenter.DurationByps, Prefix.Pico, "s").ToString("##0.###", true, 14);
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = (ChannelId)CbxSource.SelectValue;
            }
        }
    }
}
