using PdfSharpCore.Pdf.Content.Objects;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;

using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class DigiThroldPage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;
        private List<ComboBoxItem> _BitsGroup = new List<ComboBoxItem>();
        private const int AllGroupIndex = 4; //通道全选的序号
        private static int _CurrentGroupIndex = 0; //当前选中的组号
        public DigiThroldPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            for (Int32 i = 0; i < ChannelIdExt.DigiChnlNum / 4; i++)
            {
                //CbxGroup.Items.Add("D" + i * 4 + "~" + "D" + (3 + i * 4));

                var bits = new ComboBoxItem("D" + i * 4 + "~" + "D" + (3 + i * 4), i, null);
                if (bits.Item1 != null && bits.Item2 != null)
                    _BitsGroup.Add(bits);
            }
            _BitsGroup.Add(new ComboBoxItem($"D0~D{ChannelIdExt.DigiChnlNum - 1}", 4, null));
            CbxGroup.DataSource = _BitsGroup;
            CbxFamily.DataSource = Enum.GetValues<DigiTholdFamily>().Select(o => new ComboBoxItem(EnumEx.GetDescription(o), o, null)).ToList();

            ControlsHotKnob.Default.InitHotKnob(NebThreshold);
            NebThreshold.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebThreshold);
            };
            NebThreshold.StringFormatFunc = (value) => ThroldToString();
            NebThreshold.AddClicked = (a, b) =>
            {
                if ((Int32)CbxGroup.SelectValue == AllGroupIndex)
                {
                    for (int i = 0; i < Presenter.FamilyGrpCount; i++)
                        Presenter.AdjUserThroldIdxAtGrp(i, 1);
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp(0);
                }
                else
                {
                    Presenter.AdjUserThroldIdxAtGrp((Int32)CbxGroup.SelectValue, 1);
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp((Int32)CbxGroup.SelectValue);
                }

                NebThreshold.UpdateValueString();
                NebHysteresis.UpdateValueString();
            };
            NebThreshold.SubClicked = (a, b) =>
            {
                if ((Int32)CbxGroup.SelectValue == AllGroupIndex)
                {
                    for (int i = 0; i < Presenter.FamilyGrpCount; i++)
                        Presenter.AdjUserThroldIdxAtGrp(i, -1);
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp(0);
                }
                else
                {
                    Presenter.AdjUserThroldIdxAtGrp((Int32)CbxGroup.SelectValue, -1);
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp((Int32)CbxGroup.SelectValue);
                }
                   
                NebThreshold.UpdateValueString();
                NebHysteresis.UpdateValueString();
            };
            NebThreshold.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this,NebThreshold);
                var onokclickeventaction = new Action<Double>((data) =>
                {
                    if ((Int32)CbxGroup.SelectValue == AllGroupIndex)
                    {
                        for (int i = 0; i < Presenter.FamilyGrpCount; i++)
                            Presenter.SetUserThroldAtGrp(i, Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli)));
                    }
                    else
                        Presenter.SetUserThroldAtGrp((Int32)CbxGroup.SelectValue, Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli)));
                    
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp((Int32)CbxGroup.SelectValue < AllGroupIndex ? (Int32)CbxGroup.SelectValue : 0);
                    NebThreshold.UpdateValueString();
                    NebHysteresis.UpdateValueString();
                });

                nkf.SetKeyBoardValue(LblThrold.Text, QuantityUnit.Voltage.ToUnitString(), 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.GetUserThroldAtGrp((Int32)CbxGroup.SelectValue < 4 ? (Int32)CbxGroup.SelectValue : 0), Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MaxUserThrold, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MinUserThrold, Prefix.Milli));

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
            ControlsHotKnob.Default.InitHotKnob(NebHysteresis);
            NebHysteresis.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHysteresis);
            };
            NebHysteresis.StringFormatFunc = (value) => HystToString();
            NebHysteresis.AddClicked = (a, b) => Presenter.AdjUserHystIdxAtGrp((Int32)CbxGroup.SelectValue, 1);
            NebHysteresis.SubClicked = (a, b) => Presenter.AdjUserHystIdxAtGrp((Int32)CbxGroup.SelectValue, -1);
            NebHysteresis.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHysteresis);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SetUserHystAtGrp((Int32)CbxGroup.SelectValue < 4 ? (Int32)CbxGroup.SelectValue : 0, Convert.ToInt64(Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli))));

                nkf.SetKeyBoardValue(LblHyst.Text, QuantityUnit.Voltage.ToUnitString(), 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.GetUserHystAtGrp((Int32)CbxGroup.SelectValue < 4 ? (Int32)CbxGroup.SelectValue : 0), Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MaxUserHyst, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.MinUserHyst, Prefix.Milli));

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };
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

        public DigitalPrsnt Presenter
        {
            get => (DigitalPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (DigitalPrsnt)value;
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
                case "UserThroldBymV":
                    NebThreshold.UpdateValueString();
                    break;
                case "Family":
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp((Int32)CbxGroup.SelectValue < AllGroupIndex ? (Int32)CbxGroup.SelectValue : 0);
                    NebThreshold.UpdateValueString();
                    NebHysteresis.UpdateValueString();
                    break;
                case "UserHystBymV":
                    NebHysteresis.UpdateValueString();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxGroup.SelectValue = _CurrentGroupIndex;
                CbxFamily.SelectValue = Presenter.GetFamilyAtGrp((Int32)CbxGroup.SelectValue < AllGroupIndex ? (Int32)CbxGroup.SelectValue : 0);

                NebThreshold.UpdateValueString();
                NebHysteresis.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            //!!!Notice:base.OnLoad invoke CbxGroup_SelectedIndexChanged，but CbxGroup.SelectedIndex is -1
            UpdateView();
            base.OnLoad(e);
            DefaultStyleManager.Instance.RegisterControlRecursion(this.LblGroup, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(this.LblFamily, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(this.LblThrold, StyleFlag.FontSize);
        }

        private String ThroldToString()
        {
            return new Quantity(Presenter.GetUserThroldAtGrp((Int32)CbxGroup.SelectValue < 4 ? (Int32)CbxGroup.SelectValue : 0), Prefix.Milli, "V").ToString("#0.###", true);
        }

        private String HystToString()
        {
            return new Quantity(Presenter.GetUserHystAtGrp((Int32)CbxGroup.SelectValue < 4 ? (Int32)CbxGroup.SelectValue : 0), Prefix.Milli, "V").ToString("#0.###", true);
        }

        // private void CbxGroup_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //   if (!_ArgToCtrl)
        //   {
        //      CbxFamily.SelectedIndex = (Int32)Presenter.GetFamilyAtGrp(CbxGroup.SelectedIndex);
        //      NebThreshold.UpdateValueString();
        //      NebHysteresis.UpdateValueString();
        //  }
        // }

        //private void CbxFamily_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //   if (!_ArgToCtrl)
        //  {
        //      Presenter.SetFamilyAtGrp((Int32)CbxGroup.SelectValue, (DigiTholdFamily)CbxFamily.SelectedIndex);
        //  }
        // }

        private void CbxFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {

                if ((Int32)CbxGroup.SelectValue == AllGroupIndex)
                {
                    for (int i = 0; i < Presenter.FamilyGrpCount; i++)
                        Presenter.SetFamilyAtGrp(i, (DigiTholdFamily)CbxFamily.SelectValue);
                }
                else
                    Presenter.SetFamilyAtGrp((Int32)CbxGroup.SelectValue, (DigiTholdFamily)CbxFamily.SelectValue);

                NebThreshold.UpdateValueString();
                NebHysteresis.UpdateValueString();
            }
        }

        private void CbxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if ((Int32)CbxGroup.SelectValue != AllGroupIndex)
                {
                    CbxFamily.SelectValue = Presenter.GetFamilyAtGrp((Int32)CbxGroup.SelectValue);
                    NebThreshold.UpdateValueString();
                    NebHysteresis.UpdateValueString();
                }
            }
            _CurrentGroupIndex = (Int32)CbxGroup.SelectValue;
        }
    }
}
