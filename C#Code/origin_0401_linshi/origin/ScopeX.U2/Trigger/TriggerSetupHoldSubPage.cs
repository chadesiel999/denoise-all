// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
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
    
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class TriggerSetupHoldSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public TriggerSetupHoldSubPage()
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


        public TrigSetupHoldPrsnt Presenter
        {
            get => (TrigSetupHoldPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigSetupHoldPrsnt)value;
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
                case nameof(Presenter.DataSource):
                    if (Presenter.DataSource != (ChannelId)CbxDataSource.SelectValue)
                    {
                        CbxDataSource.SelectValue = Presenter.DataSource;
                        NebUpperDataPos.UpdateValueString();
                    }
                    break;
                case "PosIndex":
                    NebUpperDataPos.UpdateValueString();
                    //NebBelowDataPos.UpdateValueString();
                    break;

                case nameof(Presenter.ClkSource):
                    if (Presenter.ClkSource != (ChannelId)CbxClkSource.SelectValue)
                    {
                        CbxClkSource.SelectValue = Presenter.ClkSource;
                        NebClkThreshold.UpdateValueString();
                    }
                    break;
                case "CompPosIndex":
                    NebClkThreshold.UpdateValueString();
                    break;

                case nameof(Presenter.Violation):
                    RdoViolation.ChoosedButtonIndex = (Int32)Presenter.Violation;
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
                case nameof(Presenter.DataPosPolarity):
                    RdoPatSetting.ChoosedButtonIndex = (Int32)Presenter.DataPosPolarity;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                LoadDataSource();
                LoadClkSource();
                RdoViolation.ChoosedButtonIndex = (Int32)Presenter.Violation;
                RdoPolarity.ChoosedButtonIndex = (Int32)Presenter.ClkPolarity;
                NebUpperDataPos.UpdateValueString();
                //NebBelowDataPos.UpdateValueString();
                NebClkThreshold.UpdateValueString();
                NebTsu.UpdateValueString();
                NebThd.UpdateValueString();
                RdoPatSetting.ChoosedButtonIndex = (Int32)Presenter.DataPosPolarity;
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
            NebClkThreshold.StringFormatFunc = (_) => CompPosToString(Presenter.VuClkCompPosition, Presenter.ClkPrefix, Presenter.ClkUnit);
            NebClkThreshold.AddClicked = (_, e) => Presenter.ClkCompPosIndex += e.Step;
            NebClkThreshold.SubClicked = (_, e) => Presenter.ClkCompPosIndex += e.Step;
            NebClkThreshold.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebClkThreshold);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuClkCompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.ClkPrefix));

                nkf.SetKeyBoardValue(LblClkPos.Text, Presenter.ClkUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuClkCompPosition, Presenter.ClkPrefix),
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
            NebUpperDataPos.StringFormatFunc = (_) => CompPosToString(Presenter.VuUpperDataPosition, Presenter.DataPrefix, Presenter.DataUnit);
            NebUpperDataPos.AddClicked = (_, e) => Presenter.UpperDataPosIndex += e.Step;
            NebUpperDataPos.SubClicked = (_, e) => Presenter.UpperDataPosIndex += e.Step;
            NebUpperDataPos.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebUpperDataPos);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.VuUpperDataPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.DataPrefix));

                nkf.SetKeyBoardValue(LblDataUpperPos.Text, Presenter.DataUnit, 4, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.VuUpperDataPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxDataCompPosition, Presenter.DataPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinDataCompPosition, Presenter.DataPrefix));

                nkf.ShowDialogByPosition();
            };

            ////NebBelowDataPos
            //NebBelowDataPos.StringFormatFunc = (value) => CompPosToString(Presenter.LowerDataPosition, Presenter.DataPrefix, Presenter.DataUnit);
            //NebBelowDataPos.AddClicked = (_, e) => Presenter.LowerDataPosIndex += e.Step;
            //NebBelowDataPos.SubClicked = (_, e) => Presenter.LowerDataPosIndex += e.Step;
            //NebBelowDataPos.EditValueChicked = (a, b) =>
            //{
            //    var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //    var onokclickeventaction = new Action<Double>((data) =>
            //        Presenter.LowerDataPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.DataPrefix));

            //    nkf.SetKeyBoardValue(LblDataLowerPos.Text, Presenter.DataUnit, 3, onokclickeventaction,
            //        Quantity.ConvertByPrefix(Presenter.LowerDataPosIndex, Presenter.DataPrefix),
            //        Quantity.ConvertByPrefix(Presenter.MaxDataCompPosition, Presenter.DataPrefix),
            //        Quantity.ConvertByPrefix(Presenter.MinDataCompPosition, Presenter.DataPrefix));

            //    nkf.ShowDialogByPosition();
            //};

            //NebTsu
            ControlsHotKnob.Default.InitHotKnob(NebTsu);
            NebTsu.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebTsu);
            };
            NebTsu.AddClicked = (o, e) => Presenter.AdjTsu(1/*e.Step*/);
            NebTsu.SubClicked = (o, e) => Presenter.AdjTsu(-1/*e.Step*/);
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
            NebThd.AddClicked = (o, e) => Presenter.AdjThd(1/*e.Step*/);
            NebThd.SubClicked = (o, e) => Presenter.AdjThd(-1/*e.Step*/);
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
            var temp = ChannelIdExt.GetAnalogs().Where(o => o != Presenter.DataSource).Select(o => new ComboBoxItem(o.ToString(), o)).ToList();
            CbxClkSource.DataSource = temp;
            /*if (Presenter.ClkSource != (ChannelId)CbxClkSource.SelectValue)
            {
                CbxClkSource.SelectValue = Presenter.ClkSource;
            }*/
            var selectedItem = temp.FirstOrDefault(c => c.Key == Presenter.ClkSource.ToString());
            if (selectedItem != null)
            {
                CbxClkSource.SelectIndex = temp.IndexOf(selectedItem);
            }
        }

        private void LoadDataSource()
        {
            var temp = ChannelIdExt.GetAnalogs().Where(o => o != Presenter.ClkSource).Select(o => new ComboBoxItem(o.ToString(), o)).ToList();
            CbxDataSource.DataSource = temp;
            /*if (Presenter.DataSource != (ChannelId)CbxDataSource.SelectValue)
            {
                CbxDataSource.SelectValue = Presenter.DataSource;
            }*/
            var selectedItem = temp.FirstOrDefault(c => c.Key == Presenter.DataSource.ToString());
            if (selectedItem != null)
            {
                CbxDataSource.SelectIndex = temp.IndexOf(selectedItem);
            }
        }

        //private void CbxClkSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {

        //        Presenter.ClkSource = Enum.Parse<ChannelId>(CbxClkSource.SelectedItem.ToString());
        //        _ArgToCtrl = true;
        //        LoadDataSource();
        //        DsoPrsnt.FocusId = Presenter.ClkSource;
        //        _ArgToCtrl = false;
        //    }
        //}

        //private void CbxDataSource_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        Presenter.DataSource = Enum.Parse<ChannelId>(CbxDataSource.SelectedItem.ToString());
        //        _ArgToCtrl = true;
        //        LoadClkSource();
        //        DsoPrsnt.FocusId = Presenter.DataSource;
        //        _ArgToCtrl = false;
        //    }
        //}

        private void RdoPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClkPolarity = (EdgeSlope)RdoPolarity.ChoosedButtonIndex;
            }
        }

        private void RdoViolation_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Violation = (SetupHoldViolation)RdoViolation.ChoosedButtonIndex;
            }
        }

        private static String CompPosToString(Double position, Prefix pfx, String unit)
        {
            //return new Quantity(position, pfx, unit).ToString("##0.###", true, 7);
            if (unit == "V")
            {
                return new Quantity(position, pfx, unit).ToString("##0.####;-##0.####;0", true, 7);
            }
            else
            {
                return new Quantity(position, pfx, unit).ToString("##0.####;-##0.####;0", true, 7);
            }
        }

        private static String TimeToString(Int64 time)
        {
            return new Quantity(time, Prefix.Pico, "s").ToString("##0.###", true, 14);
        }

        private void RdoPatSetting_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.DataPosPolarity = (EdgeSlope)RdoPatSetting.ChoosedButtonIndex;
            }
        }

        private void CbxClkSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClkSource = Enum.Parse<ChannelId>(CbxClkSource.SelectKey.ToString());
                _ArgToCtrl = true;
                LoadDataSource();
                // DsoPrsnt.FocusId = Presenter.ClkSource;
                _ArgToCtrl = false;
            }

        }

        private void CbxDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.DataSource = Enum.Parse<ChannelId>(CbxDataSource.SelectKey.ToString());
                _ArgToCtrl = true;
                LoadClkSource();
                //DsoPrsnt.FocusId = Presenter.DataSource;
                _ArgToCtrl = false;
            }

        }


    }
}
