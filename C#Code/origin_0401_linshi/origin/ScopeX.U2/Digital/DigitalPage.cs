using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Collections.Generic;
using static ScopeX.UserControls.SelectComboBox;


namespace ScopeX.U2
{
    public partial class DigitalPage : UserControl, IChnlView, IStylize
    {
        private readonly CheckBox[] _ActiveBits;

        private Boolean _ArgToCtrl;

        private List<ComboBoxItem> _ActBits = new List<ComboBoxItem>();

        private List<ComboBoxItem> _BitsGroup = new List<ComboBoxItem>();
        //private Int32 _Group = 0;

        public DigitalPage()
        {
            InitializeComponent();
            Init();

            _ActiveBits = new[]
            {
                ChkD1, ChkD2, ChkD3, ChkD4,
                ChkD5, ChkD6, ChkD7, ChkD8,
                ChkD9, ChkD10,ChkD11,ChkD12,
                ChkD13,ChkD14,ChkD15,ChkD16
            };
            foreach (var bit in _ActiveBits)
            {
            DefaultStyleManager.Instance.RegisterControlRecursion(bit, StyleFlag.FontSize);
            }
            int index = 0;
            foreach (var id in ChannelIdExt.GetDigitals())
            {
                //CbxBits.Items.Add("D" + (id - ChannelId.D0).ToString());

                var bits = new ComboBoxItem("D" + (id - ChannelId.D0).ToString(), (object)index,null);
                if (bits.Item1 != null && bits.Item2 != null)
                {
                    _ActBits.Add(bits);
                }
                index++;
            }

            for (Int32 i = 0; i < ChannelIdExt.DigiChnlNum / 16; i++)
            {
                CbxGroup.Items.Add("D" + i * 16 + " ... " + "D" + (15 + i * 16));
                var bits = new ComboBoxItem("D" + i * 16 + " ... " + "D" + (15 + i * 16), (object)i, null);
                if (bits.Item1 != null && bits.Item2 != null)
                {
                    _BitsGroup.Add(bits);
                }
            }
        }

        private void Init()
        {
            ControlsHotKnob.Default.InitHotKnob(NebPosition);
            NebPosition.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPosition, nameof(Presenter.PosIndexBymDiv));
            };
            NebPosition.StringFormatFunc = (value) => PosToString();
            NebPosition.AddClicked = (a, b) => Presenter.AdjPosIndex(1);
            NebPosition.SubClicked = (a, b) => Presenter.AdjPosIndex(-1);
            NebPosition.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this,NebPosition);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.PosIndexBymDiv = Quantity.ConvertByPrefix(data, Prefix.Empty, Prefix.Milli));

                nkf.SetKeyBoardValue(LblPosition.Text, QuantityUnit.Division.ToUnitString(), 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.PosIndexBymDiv, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.PosMaxIndex, Prefix.Milli),
                    Quantity.ConvertByPrefix(Presenter.PosMinIndex, Prefix.Milli));

                nkf.ShowDialogByPosition();
            };
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

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

        public DigitalPrsnt Presenter { get; set; }
        //{
        //    get => (DigitalPrsnt)(ParentForm as IChnlView).Presenter;
        //    set => (ParentForm as IChnlView).Presenter = value;
        //}

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (DigitalPrsnt)value;
        }

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
                case "ActiveBit":
                    for (Int32 i = 0; i < _ActiveBits.Length; i++)
                    {
                        _ActiveBits[i].Text = $"D{i + CbxGroup.SelectedIndex * 16}";
                        _ActiveBits[i].Checked = Presenter.GetActiveAt(i + CbxGroup.SelectedIndex * 16);
                    }
                    break;
                case "PosIndex":
                    NebPosition.UpdateValueString();
                    break;
                case nameof(Presenter.BitHeightOpt):
                    RdoHeight.ChoosedButtonIndex = (Int32)Presenter.BitHeightOpt;
                    break;
                case "LabelVisiblity":
                    ChkLabelVisiblity.Checked = Presenter.GetLabelVisibltyAt(Presenter.FocusBitId);
                    break;
                case nameof(Presenter.FocusBitId):
                    NebPosition.UpdateValueString();
                    TbxLabel.Text = Presenter.GetLabelAt(Presenter.FocusBitId);
                    break;
                case "Label":
                    TbxLabel.Text = Presenter.GetLabelAt(Presenter.FocusBitId);
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                for (Int32 i = 0; i < _ActiveBits.Length; i++)
                {
                    _ActiveBits[i].Checked = Presenter.GetActiveAt(i);
                }
                

                CbxBits.DataSource = _ActBits;
                CbxBits.SelectValue = Presenter.FocusBitId;
                CbxBits.SelectedIndexChanged += (_, _) =>
                {
                    if (!_ArgToCtrl)
                    {
                        Presenter.FocusBitId = (Int32)CbxBits.SelectValue;
                    }
                };
                RdoHeight.ChoosedButtonIndex = (Int32)Presenter.BitHeightOpt;
                NebPosition.UpdateValueString();
                TbxLabel.Text = Presenter.GetLabelAt((Int32)CbxBits.SelectValue);
                ChkLabelVisiblity.Checked = Presenter.GetLabelVisibltyAt(Presenter.FocusBitId);
                CbxGroup.SelectedIndex = 0;
                _ArgToCtrl = false;
            }
        }

        private String PosToString()
        {
            return new Quantity(Presenter.PosIndexBymDiv, Prefix.Milli, "div").ToString("#0.###", true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
            _ArgToCtrl = true;

            //var form = (ParentForm.Owner as DsoForm).MultiWindowManager.GetWindow(Presenter.WindowId);
            //if (form?.IsMainForm == false)
            //{
            //    ChkIndependentWindow.Checked = true;
            //}
            //else
            //{
            //    ChkIndependentWindow.Checked = false;
            //}
            ChkIndependentWindow.Checked = Presenter.WindowId != (ParentForm.Owner as DsoForm).MultiWindowManager.MainFigure.WindowId;
            _ArgToCtrl = false;
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _ActiveBits.Length; i++)
            {
                Presenter.SetActiveAt(i + CbxGroup.SelectedIndex * 16, true);
            }
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _ActiveBits.Length; i++)
            {
                Presenter.SetActiveAt(i + CbxGroup.SelectedIndex * 16, false);
            }
        }

        private void ChkActiveBit_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                var name = (sender as CheckBox).Text;
                var idx = Int32.Parse(name[1..]);
                Presenter.SetActiveAt(idx, (sender as CheckBox).Checked);
            }
        }

        //private void CbxBits_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //   if (!_ArgToCtrl)
        //   {
        //      Presenter.FocusBitId = CbxBits.SelectedIndex;
        //  }
        //}
        private void ChkLabelVisiblity_CheckedChangedEvent(Object sender, EventArgs e)
        {
            Presenter.SetLabelVisiblity(Presenter.FocusBitId, ChkLabelVisiblity.Checked);
        }
        private void TbxLabel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (OnEnterKeyPressed(sender, e))
            {
                Presenter.SetLabelAt((Int32)CbxBits.SelectValue, TbxLabel.Text);
            }
        }

        private void TbxLabel_Leave(object sender, EventArgs e)
        {
            Presenter.SetLabelAt((Int32)CbxBits.SelectValue, TbxLabel.Text);
        }

        private void TbxLabel_TextChanged(Object sender, EventArgs e)
        {
            Presenter.SetLabelAt(Presenter.FocusBitId, TbxLabel.Text);
        }

        private static Boolean OnEnterKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                return true;
            }

            return false;
        }

        private void RdoHeight_IndexChanged(object sender, EventArgs e)
        {
            Presenter.BitHeightOpt = (DigiHeightOpt)RdoHeight.ChoosedButtonIndex;
        }

        private void BtnAutoLocate_Click(object sender, EventArgs e)
        {
            Presenter.AutoLocate();
        }

        private void CbxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                for (Int32 i = 0; i < _ActiveBits.Length; i++)
                {
                    _ActiveBits[i].Text = $"D{i + CbxGroup.SelectedIndex * 16}";
                    _ActiveBits[i].Checked = Presenter.GetActiveAt(i + CbxGroup.SelectedIndex * 16);
                }
            }
        }


        private void ChkIndependentWindow_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (ChkIndependentWindow.Checked)
                {
                    Presenter.WindowId = ChannelPrsnt.GetNewWindowId();
                }
                else
                {
                    var form = (ParentForm.Owner as DsoForm).MultiWindowManager.MainFigure;
                    if (form != null)
                    {
                        Presenter.WindowId = form.WindowId;
                    }
                }
            }
        }


    }
}
