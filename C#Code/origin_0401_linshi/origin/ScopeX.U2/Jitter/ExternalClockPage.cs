using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using System.Drawing;
using static ScopeX.UserControls.SelectComboBox;
using ScopeX.Core.Jitter;

namespace ScopeX.U2
{
    public partial class ExternalClockPage : UserControl, IJitterView, IStylize
    {
        private Boolean _ArgToCtrl;
        public ExternalClockPage()
        {
            InitializeComponent();
            InitClkSourceList();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Presenter?.TryRemoveView(this);
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public JitterPrsnt Presenter
        {
            get;//=> (JitterPrsnt)(ParentForm as IJitterView).Presenter;
            set;// => (ParentForm as IJitterView).Presenter = value;
        }
        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
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
                case nameof(Presenter.RefClkSource):
                    //CbxRefSource.SelectedIndex = (Int32)Presenter.RefClkSource;
                    CbxRefSource.SelectValue = (Int32)Presenter.RefClkSource;
                    break;
                case nameof(Presenter.RefClkThreshold):
                    BtnRefClkThrold.Text = ThresholdToString();
                    break;
                case nameof(Presenter.RefClkDeskew):
                    BtnRefClkDeskew.Text = DeskewToString();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                BtnRefClkThrold.Text = ThresholdToString();
                BtnRefClkDeskew.Text = DeskewToString();
                //CbxRefSource.SelectedIndex = (Int32)Presenter.RefClkSource;
                CbxRefSource.SelectValue = (Int32)Presenter.RefClkSource;
                _ArgToCtrl = false;
            }
        }
        private void InitClkSourceList()
        {
            CbxRefSource.DataSource= ChannelIdExt.GetAnalogs().Select(o => new ComboBoxItem(o.ToString(), o, null)).ToList();
            CbxRefSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.RefClkSource = (ChannelId)CbxRefSource.SelectValue;
                }
            };
        }

        private void BtnRefClkThrold_Click(object sender, System.EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnRefClkThrold);
            var oncomfirm = new Action<Double>((data) => Presenter.RefClkThreshold = data);

            nkf.SetKeyBoardValue(LblRefClkThrold.Text, QuantityUnit.Percent.ToUnitString(), 3, oncomfirm,
                Presenter.RefClkThreshold, Presenter.MaxThreshold, Presenter.MinThreshold);

            nkf.ShowDialogByPosition();
        }

        private void BtnRefClkDeskew_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnRefClkDeskew);
            var oncomfirm = new Action<Double>((data) => Presenter.RefClkDeskew = data);

            nkf.SetKeyBoardValue(LblRefClkDeskew.Text, QuantityUnit.Second.ToUnitString(), 3, oncomfirm,
                Presenter.RefClkDeskew, Presenter.MaxRefClkDeskew, Presenter.MinRefClkDeskew);

            nkf.ShowDialogByPosition();
        }

        private String DeskewToString() => new Quantity(Presenter.RefClkDeskew, Prefix.Empty, QuantityUnit.Second).ToString("#0.###", true);

        private String ThresholdToString() => new Quantity(Presenter.RefClkThreshold, Prefix.Empty, QuantityUnit.Percent).ToString("#0.0", true);
    }
}
