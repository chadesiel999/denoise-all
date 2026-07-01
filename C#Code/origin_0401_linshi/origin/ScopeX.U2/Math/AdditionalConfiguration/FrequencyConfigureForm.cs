using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class FrequencyConfigureForm : FloatForm, IChnlView
    {
        private Boolean _ArgToCtrl;

        public FrequencyConfigureForm()
        {
            InitializeComponent();
            _OBPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _ACPRPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _CPPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _THDPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            Init();
        }

        private readonly OBPage _OBPage;

        private readonly ACPRPage _ACPRPage;

        private readonly CPPage _CPPage;

        private readonly THDPage _THDPage;

        private Control _OptionPage;
        private void Init()
        {
            //NebAverageTimes
            NebAverageTimes.AddClicked = (a, b) => _FftArg.AverageTimes++;
            NebAverageTimes.SubClicked = (a, b) => _FftArg.AverageTimes--;
            NebAverageTimes.StringFormatFunc = (value) => _FftArg.AverageTimes.ToString();

            NebAverageTimes.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebAverageTimes);
                nkf.NumberKeyboard.UseSI = false;
                Action<Double> onokclickeventaction = (data) =>
                    _FftArg.AverageTimes = (Int32)data;

                nkf.SetKeyBoardValue(LblAverageTimes.Text, QuantityUnit.Constant.ToUnitString(), 0, onokclickeventaction,
                    _FftArg.AverageTimes,
                    Constants.RF_AVERAGE_TIMES_MAX,
                    Constants.RF_AVERAGE_TIMES_MIN);

                //nkf.ShowDialogByPosition();
            };

        }
        private Control GetPage()
        {
            switch (_FftArg.MeasureType)
            {
                case FrequencyMeasureType.ACPR:
                    return _ACPRPage;
                case FrequencyMeasureType.OB:
                    return _OBPage;
                case FrequencyMeasureType.CP:
                    return _CPPage;
                case FrequencyMeasureType.THD:
                    return _THDPage;
            }
            return _CPPage;
        }

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

        public MathPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (MathPrsnt)value;
        }

        private MathFftArg _FftArg;

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _FftArg = (MathFftArg)Presenter.GetOrMakeArg(MathType.FFT);
            _OptionPage = GetPage();
            PnlCurrentFreqMeasType.Controls.Add(_OptionPage);
            RdoCurrentFreqMeasType.ChoosedButtonIndex = (Int32)_FftArg.MeasureType;
            Stylize();
            UpdateView();
        }

        private void Stylize()
        {
            IsShowHelp = false;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        public void UpdateView(Object prsnt, String propertyName = "")
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                switch (_FftArg.MeasureType)
                {
                    case FrequencyMeasureType.ACPR:
                        _ACPRPage.Refresh();
                        break;
                    case FrequencyMeasureType.OB:
                        _OBPage.Refresh();
                        break;
                    case FrequencyMeasureType.CP:
                        _CPPage.Refresh();
                        break;
                    case FrequencyMeasureType.THD:
                        _THDPage.Refresh();
                        break;
                }
              
                return;
            }
            if (_FftArg is null)
            {
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(_FftArg.NormalLine):
                    ChkNormal.Checked = _FftArg.NormalLine;
                    break;
                case nameof(_FftArg.MaxHoldLine):
                    ChkMaxHold.Checked = _FftArg.MaxHoldLine;
                    break;
                case nameof(_FftArg.MinHoldLine):
                    ChkMinHold.Checked = _FftArg.MinHoldLine;
                    break;
                case nameof(_FftArg.AverageLine):
                    ChkAverage.Checked = _FftArg.AverageLine;
                    break;
                case nameof(_FftArg.AverageTimes):
                    NebAverageTimes.UpdateValueString();
                    break;
            }
            NebAverageTimes.UpdateValueString();
            _OBPage.UpdateView(prsnt, propertyName);
            _ACPRPage.UpdateView(prsnt, propertyName);
            _CPPage.UpdateView(prsnt, propertyName);
            _THDPage.UpdateView(prsnt, propertyName);
            switch (_FftArg.MeasureType)
            {
                case FrequencyMeasureType.ACPR:
                    _ACPRPage.UpdateView(prsnt, propertyName);
                    break;
                case FrequencyMeasureType.OB:
                    _OBPage.UpdateView(prsnt, propertyName);
                    break;
                case FrequencyMeasureType.CP:
                    _CPPage.UpdateView(prsnt, propertyName);
                    break;
                case FrequencyMeasureType.THD:
                    _THDPage.UpdateView(prsnt, propertyName);
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (_FftArg is null)
                {
                    return;
                }
                ChkNormal.Checked = _FftArg.NormalLine;
                ChkMaxHold.Checked = _FftArg.MaxHoldLine;
                ChkMinHold.Checked = _FftArg.MinHoldLine;
                ChkAverage.Checked = _FftArg.AverageLine;

                NebAverageTimes.UpdateValueString();
            }
        }
        private void ChkAverage_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _FftArg.AverageLine = ChkAverage.Checked;
            }
        }

        private void ChkMinHold_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _FftArg.MinHoldLine = ChkMinHold.Checked;
            }
        }

        private void ChkMaxHold_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _FftArg.MaxHoldLine = ChkMaxHold.Checked;
            }
        }

        private void ChkNormal_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _FftArg.NormalLine = ChkNormal.Checked;
            }
        }

        private void RdoCurrentFreqMeasType_IndexChanged(object sender, EventArgs e)
        {
            _FftArg.MeasureType = (FrequencyMeasureType)RdoCurrentFreqMeasType.ChoosedButtonIndex;
            _OptionPage = GetPage();
            PnlCurrentFreqMeasType.Controls.RemoveAt(0);
            PnlCurrentFreqMeasType.Controls.Add(_OptionPage);
        }
    }
}