using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using System.Drawing;
using static ScopeX.Controls.Common.APIs.APIsStructs;

namespace ScopeX.U2
{
    public partial class MeasureMenuPage : UserControl, IView, IStylize
    {
        private Boolean _ArgToCtrl;

        public MeasureMenuPage()
        {
            InitializeComponent();
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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public MeasPrsnt Presenter
        {
            get => (ParentForm as MeasureMenuForm)?.Presenter;
            //set => (ParentForm as IMeasView).Presenter = value;
        }

        //IMeasPrsnt IView<IMeasPrsnt>.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (MeasPrsnt)value;
        //}

        public CymometerPrsnt CymometePresenter
        {
            get => (ParentForm as MeasureMenuForm).CymometerPresenter;
            //set => (ParentForm as ICymometerView).Presenter = value;
        }

        //ICymometerPrsnt IView<ICymometerPrsnt>.Presenter
        //{
        //    get => CymometePresenter;
        //    set => CymometePresenter = (CymometerPrsnt)value;
        //}

        public VoltmeterPrsnt VoltmeterPresenter
        {
            get => (ParentForm as MeasureMenuForm).VoltmeterPresenter;
            //set => (ParentForm as IVoltmeterView).Presenter = value;
        }

        //IBadge IView<IBadge>.Presenter
        //{
        //    get => VoltmeterPresenter;
        //    set => VoltmeterPresenter = (VoltmeterPrsnt)value;
        //}

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
                case "Active":
                    BtnVoltmeter.Checked = VoltmeterPresenter.Active;
                    BtnCymometer.Checked = CymometePresenter.Active;
                    BtnMeasure.Checked = Presenter.Active;
                    BtnCloseAllMeasure.Enabled = Presenter.Active;
                    BtnMeasureGateScreen.Enabled = BtnMeasureGateCursor.Enabled = BtnStatistic.Enabled = Presenter.Active;
                    break;
                case nameof(Presenter.SnapshotActive):
                    BtnSnapShot.Checked = Presenter.SnapshotActive;
                    break;
                case nameof(Presenter.Strobe):
                    BtnMeasureGateCursor.Enabled = BtnMeasureGateCursor.Checked = Presenter.Strobe == MeasureGate.Cursor;
                    BtnMeasureGateCursor.Enabled = !BtnMeasureGateCursor.Checked;
                    BtnMeasureGateScreen.Checked = Presenter.Strobe == MeasureGate.Screen;
                    BtnMeasureGateScreen.Enabled = !BtnMeasureGateScreen.Checked;
                    break;
                case nameof(Presenter.IsStatActive):
                    BtnStatistic.Checked = Presenter.IsStatActive;
                    BtnResetStatistic.Enabled = Presenter.IsStatActive;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                BtnVoltmeter.Checked = VoltmeterPresenter.Active;
                BtnCymometer.Checked = CymometePresenter.Active;

                BtnSnapShot.Checked = Presenter.SnapshotActive;

                BtnMeasureGateScreen.Checked = Presenter.Strobe == MeasureGate.Screen;

                BtnResetStatistic.Enabled = Presenter.IsStatActive;

                BtnMeasure.Checked = Presenter.Active;

                BtnMeasureGateScreen.Enabled = BtnMeasureGateCursor.Enabled = BtnStatistic.Enabled = Presenter.Active;
                if (BtnMeasureGateScreen.Enabled)
                {
                    BtnStatistic.Checked = Presenter.IsStatActive && Presenter.Active;
                    BtnMeasureGateScreen.Enabled = !BtnMeasureGateScreen.Checked;
                }
                else
                {
                    BtnMeasureGateScreen.Checked = false;
                }

                if (BtnMeasureGateCursor.Enabled)
                {
                    BtnMeasureGateCursor.Checked = Presenter.Strobe == MeasureGate.Cursor;
                    BtnMeasureGateCursor.Enabled = !BtnMeasureGateCursor.Checked;
                }
                else
                {
                    BtnMeasureGateCursor.Checked = false;
                }

                var items = Presenter.SelectedItems?.Where(x => x.Active && x.MeasureType == MeasureType.Single);

                BtnMeasureOperation.Enabled = items != null && items.Count() >= 2;

                BtnCloseAllMeasure.Enabled = Presenter.Active;
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
            UpdateView();
            StylizeTlp(this.Tlp);
            base.OnLoad(e);
        }

        private void StylizeTlp(TableLayoutPanel tlp)
        {
            tlp.Controls.Cast<Control>().Where(ctl => ctl.GetType() == typeof(ScopeXCheckButton)).ToList().ForEach(btn =>
            {
                var button = btn as ScopeXCheckButton;
                button.BaseFont = AppStyleConfig.DefaultButtonFont;
            });
        }

        private void BtnCymometer_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                CymometePresenter.Active = !CymometePresenter.Active;
                ParentForm.Close();
            }
        }

        private void BtnVoltmeter_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                VoltmeterPresenter.Active = !VoltmeterPresenter.Active;
                ParentForm.Close();
            }
        }

        private void BtnSnapShot_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!Constants.ENABLE_Measure)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.FunctionDisabled);
                    BtnSnapShot.Checked = Presenter.SnapshotActive;//重新刷新
                    ParentForm?.Close();
                    return;
                }
                Presenter.SnapshotActive = !Presenter.SnapshotActive;
                //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SNAPSHOT);
                ParentForm?.Close();
            }

        }

        private void BtnMeasureGateScreen_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (Presenter.Strobe != MeasureGate.Screen)
                {
                    Presenter.Strobe = MeasureGate.Screen;
                    //清空测试数据
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag = true;
                    Presenter.ResetAllStats();
                    ParentForm.Close();
                }
                BtnMeasureGateScreen.Enabled = !BtnMeasureGateScreen.Checked;
            }
        }

        private void BtnMeasureGateCursor_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (Presenter.Strobe != MeasureGate.Cursor)
                {
                    Presenter.Strobe = MeasureGate.Cursor;
                    //清空测试数据
                    DsoPrsnt.DefaultDsoPrsnt.Measure.ClearFlag = true;
                    Presenter.ResetAllStats();
                    ParentForm.Close();
                }
                BtnMeasureGateCursor.Enabled = !BtnMeasureGateCursor.Checked;
            }
        }

        private void BtnOpenOrCloseStatistic_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.IsStatActive = !Presenter.IsStatActive;
                Presenter.ResetAllStats();
                ParentForm.Close();
            }
        }

        private void BtnAddItem_Click(Object sender, EventArgs e)
        {
            if (Presenter.IsAllActive())
            {
                WeakTip.Default.Write("Measure", MsgTipId.NoMoreMeasuerLabel);
                return;
            }

            var appenditem = Presenter.SelectedItems.First(o => !o.Active);
            MeasSelectionForm msf = new(appenditem.Name, DsoPrsnt.FocusId, (n, s) =>
            {
                if (Presenter.IsAllActive())
                {
                    WeakTip.Default.Write("Measure", MsgTipId.NoMoreMeasuerLabel);
                    return false;
                }
                var mi = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == n && o.Source == s);
                if (mi != null)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.MeasuerLabelExisted);
                    return false;
                }

                var ai = Presenter.SelectedItems.First(o => !o.Active);
                ai.Name = n;
                ai.Source = s;
                ai.Active = true;

                return true;
            })
            {
                StartPosition = FormStartPosition.CenterScreen,
            };

            (ParentForm as FloatForm).CanClose = false;
            msf.ShowDialogByEvent();
            //if (msf.DialogResult == DialogResult.Yes)
            //{
            //    appenditem.Name = msf.SelectedItemName;
            //    appenditem.Source = msf.Source;
            //    appenditem.Active = true;
            //}
            (ParentForm as FloatForm).CanClose = true;

            ParentForm.Close();
        }

        private void BtnMeasure_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!Constants.ENABLE_Measure)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.FunctionDisabled);
                    BtnMeasure.Checked = Presenter.Active;//重新刷新
                    ParentForm?.Close();
                    return;
                }

                Presenter.Active = !Presenter.Active;
                if (Presenter != null && !Presenter.Active)
                {
                    Presenter.Indicator = 0;
                }
                ParentForm?.Close();
            }
        }
        private void BtnMeasureOperation_Click(object sender, System.EventArgs e)
        {
            BtnMeasureOperation.Checked = false;
            var item = Presenter.SelectedItems.FirstOrDefault(x => !x.Active);
            if (item == null)
                return;
            item.MeasureType = MeasureType.Composite;
            var mif = new MeasItemCfgForm(item.Id - ChannelId.P1)
            {
                Presenter = Presenter,
                Anchor = AnchorStyles.Top,
                StartPosition = FormStartPosition.CenterScreen,
            };
            mif.Text = item.Id.ToString();
            mif.Presenter.TryAddView(mif);
            (ParentForm as FloatForm).CanClose = false;
            if (mif.ShowDialog() == DialogResult.OK)
            {
                (ParentForm as FloatForm).CanClose = true;
                ParentForm?.Close();
            }
            else
            {
                (ParentForm as FloatForm).CanClose = true;
            }
        }

        private void BtnCloseAllMeasure_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (!Constants.ENABLE_Measure)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.FunctionDisabled);
                    ParentForm?.Close();
                    return;
                }
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DeleteMeasureItems, MessageType.Asking))
                {
                    foreach (var item in Presenter.SelectedItems)
                    {
                        if (item.Active)
                        {
                            item.Active = false;
                        }
                    }

                }
                ParentForm?.Close();
            }
        }

        private void BtnResetStat_Click(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClearFlag = true;
                Presenter.ClearHisFlag = true;
                Presenter.ClearStrongFlag = true;
                Presenter.ResetAllStats();
                ParentForm.Close();
            }
        }



        //public override void SetElementStyle()
        //{
        //    StylizeTlp(this.Tlp);

        //    SplitLine.DarkColor = Color.FromArgb(60, 60, 60);

        //    SplitLine1.DarkColor = Color.FromArgb(60, 60, 60);

        //    SplitLine2.DarkColor = Color.FromArgb(60, 60, 60);
        //}

    }
}
