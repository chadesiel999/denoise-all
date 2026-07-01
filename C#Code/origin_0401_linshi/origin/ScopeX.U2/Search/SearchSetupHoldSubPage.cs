using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System.Windows.Forms;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2.Search
{
    public partial class SearchSetupHoldSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public SearchSetupHoldSubPage(SearchSetupHoldPrsnt prsnt)
        {
            InitializeComponent();
            Presenter = prsnt;
            Init();
        }

        public SearchSetupHoldPrsnt Presenter
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

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchSetupHoldPrsnt)value; }

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
                case nameof(Presenter.DataSource):
                    CbxDataSource.Text = Presenter.DataSource.ToString();
                    break;
                case nameof(Presenter.ClkSource):
                    CbxClkSource.Text = Presenter.ClkSource.ToString();
                    break;
                case nameof(Presenter.Violation):
                    RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Violation;
                    break;
                case "CompPosIndex":
                    NebClkThreshold.UpdateValueString();
                    break;
                case "PosIndex":
                    NebUpperDataPos.UpdateValueString();
                    NebBelowDataPos.UpdateValueString();
                    break;
                case nameof(Presenter.ClkPolarity):
                    RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.ClkPolarity;
                    break;
                case "SetupByps":
                    NebTsu.UpdateValueString();
                    break;
                case "WidthByps":
                    NebThd.UpdateValueString();
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
                LoadDataSource();
                LoadClkSource();
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.Violation;
                NebUpperDataPos.UpdateValueString();
                NebBelowDataPos.UpdateValueString();
                NebClkThreshold.UpdateValueString();
                NebTsu.UpdateValueString();
                NebThd.UpdateValueString();
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.ClkPolarity;
                _ArgToCtrl = false;
            }
        }

        private static String CompPosToString(Double position, Prefix pfx, String unit)
        {
            return new Quantity(position, pfx, unit).ToString(5, true);
        }

        private static String TimeToString(Int64 time)
        {
            return new Quantity(time, Prefix.Pico, "s").ToString("##0.######", true);
        }

        private void CbxClkSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClkSource = Enum.Parse<ChannelId>(CbxClkSource.SelectedItem.ToString());
                _ArgToCtrl = true;
                LoadDataSource();
                _ArgToCtrl = false;
            }
        }

        private void CbxDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.DataSource = Enum.Parse<ChannelId>(CbxDataSource.SelectedItem.ToString());
                _ArgToCtrl = true;
                LoadClkSource();
                _ArgToCtrl = false;
            }
        }

        private void Init()
        {
            //NebClkThreshold
            ControlsHotKnob.Default.InitHotKnob(NebClkThreshold);
            NebClkThreshold.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebClkThreshold);
            };
            NebClkThreshold.StringFormatFunc = (_) => CompPosToString(Presenter.ClkCompPosition, Presenter.ClkPrefix, Presenter.ClkUnit);
            NebClkThreshold.AddClicked = (_, e) => Presenter.ClkCompPosIndex += e.Step;
            NebClkThreshold.SubClicked = (_, e) => Presenter.ClkCompPosIndex += e.Step;
            NebClkThreshold.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebClkThreshold);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.ClkCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.ClkPrefix));

                nkf.SetKeyBoardValue(LblClkPos.Text, Presenter.ClkUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.ClkCompPosition, Presenter.ClkPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxClkCompPosition, Presenter.ClkPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinClkCompPosition, Presenter.ClkPrefix));

                nkf.ShowDialogByPosition();
            };

            //NebUpperDataPos
            ControlsHotKnob.Default.InitHotKnob(NebUpperDataPos);
            NebUpperDataPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebUpperDataPos);
            };
            NebUpperDataPos.StringFormatFunc = (_) => CompPosToString(Presenter.UpperDataPosition, Presenter.DataPrefix, Presenter.DataUnit);
            NebUpperDataPos.AddClicked = (_, e) => Presenter.UpperDataPosIndex += e.Step;
            NebUpperDataPos.SubClicked = (_, e) => Presenter.UpperDataPosIndex += e.Step;
            NebUpperDataPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebUpperDataPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.UpperDataPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.DataPrefix));

                nkf.SetKeyBoardValue(LblDataUpperPos.Text, Presenter.DataUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.UpperDataPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxDataCompPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinDataCompPosition, Presenter.DataPrefix));

                nkf.ShowDialogByPosition();
            };

            //NebBelowDataPos
            ControlsHotKnob.Default.InitHotKnob(NebBelowDataPos);
            NebBelowDataPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebBelowDataPos);
            };
            NebBelowDataPos.StringFormatFunc = (value) => CompPosToString(Presenter.LowerDataPosition, Presenter.DataPrefix, Presenter.DataUnit);
            NebBelowDataPos.AddClicked = (_, e) => Presenter.LowerDataPosIndex += e.Step;
            NebBelowDataPos.SubClicked = (_, e) => Presenter.LowerDataPosIndex += e.Step;
            NebBelowDataPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebBelowDataPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.LowerDataPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.DataPrefix));

                nkf.SetKeyBoardValue(LblDataLowerPos.Text, Presenter.DataUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.LowerDataPosIndex, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxDataCompPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinDataCompPosition, Presenter.DataPrefix));

                nkf.ShowDialogByPosition();
            };

            //NebTsu
            ControlsHotKnob.Default.InitHotKnob(NebTsu);
            NebTsu.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebTsu);
            };
            NebTsu.AddClicked = (o, e) => Presenter.AdjTsu(e.Step);
            NebTsu.SubClicked = (o, e) => Presenter.AdjTsu(e.Step);
            NebTsu.StringFormatFunc = (_) => TimeToString(Presenter.TsuByps);
            NebTsu.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebTsu);
                var onokclickeventaction = new Action<double>((data) =>
                    Presenter.TsuByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                nkf.SetKeyBoardValue(LblTsu.Text, QuantityUnit.Second.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.TsuByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MaxTsu, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MinTsu, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };

            //NebThd
            ControlsHotKnob.Default.InitHotKnob(NebThd);
            NebThd.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebThd);
            };
            NebThd.AddClicked = (o, e) => Presenter.AdjThd(e.Step);
            NebThd.SubClicked = (o, e) => Presenter.AdjThd(e.Step);
            NebThd.StringFormatFunc = (_) => TimeToString(Presenter.ThdByps);
            NebThd.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebThd);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.ThdByps = Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Pico)));

                nkf.SetKeyBoardValue(LblThd.Text, QuantityUnit.Second.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.ThdByps, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MaxThd, Prefix.Pico),
                    Quantity.ConvertByPrefix(Presenter.MinThd, Prefix.Pico));

                nkf.ShowDialogByPosition();
            };
        }

        private void LoadClkSource()
        {
            CbxClkSource.Items.Clear();
            CbxClkSource.Items.AddRange(ChannelIdExt.GetAnalogs().Where(o => o != Presenter.DataSource).Select(o => o.ToString()).ToArray());
            CbxClkSource.Items.Add(ChannelId.Ext);
            CbxClkSource.SelectedIndex = CbxClkSource.FindStringExact(Presenter.ClkSource.ToString());
        }

        private void LoadDataSource()
        {
            CbxDataSource.Items.Clear();
            CbxDataSource.Items.AddRange(ChannelIdExt.GetAnalogs().Where(o => o != Presenter.ClkSource).Select(o => o.ToString()).ToArray());
            CbxDataSource.SelectedIndex = CbxDataSource.FindStringExact(Presenter.DataSource.ToString());
        }

        private void RdoPolarity_ButtonSelect(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClkPolarity = (EdgeSlope)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private void RdoViolation_ButtonSelect(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Violation = (SetupHoldViolation)RdoPolarity.ChoosedButtonIndex;
            }
        }

    }
}
