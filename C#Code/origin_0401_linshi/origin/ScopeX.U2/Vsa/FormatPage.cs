using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class FormatPage : UserControl, IVsaView
    {
        private Boolean _ArgToCtrl;

        public FormatPage(VectorAnalysisPrsnt vap)
        {
            InitializeComponent();
            InitSourceList();
            InitFormateList();
            Presenter = vap;
        }

        public VectorAnalysisPrsnt Presenter
        {
            //get => (VectorAnalysisPrsnt)(ParentForm as IVsaView).Presenter; 
            //set => (ParentForm as IVsaView).Presenter = value;
            get;
            set;
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

        IVsaPrsnt IView<IVsaPrsnt>.Presenter { get => Presenter; set => Presenter = (VectorAnalysisPrsnt)value; }

        public void UpdateView(Object prsnt, String propertyName)
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DefaultStyleManager.Instance.RegisterControl(this);
            UpdateView();
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Enabled):
                    ChkEnabled.Checked = Presenter.Enabled;
                    break;
                case nameof(Presenter.Source):
                    CbxSource.SelectedIndex = (Int32)Presenter.Source;
                    break;
                case nameof(Presenter.Template):
                    CbxTemplate.SelectedIndex = (Int32)Presenter.Template;
                    //LoadOptionPage();
                    break;
                case nameof(Presenter.Format):
                    CbxFormat.SelectedIndex = (Int32)Presenter.Format;
                    break;
                case nameof(Presenter.SymbolRate):
                    BtnSymbolRate.Text = SymbolRateToString();
                    break;
                case nameof(Presenter.BitsPerSym):
                    BtnBitsPerSym.Text = BitsPerSymToString();
                    break;
                case nameof(Presenter.TimingEst):
                    CbxTimingEst.SelectedIndex = (Int32)Presenter.TimingEst;
                    break;
                case nameof(Presenter.Interpolation):
                    RdoInterpolation.ChoosedButtonIndex = (Int32)Presenter.Interpolation;
                    break;
                case nameof(Presenter.SampPerBaud):
                    BtnSampPerBaud.Text = SampsPerBaudToString();
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkEnabled.Checked = Presenter.Enabled;
                CbxSource.SelectedIndex = (Int32)Presenter.Source;
                CbxTemplate.SelectedIndex = (Int32)Presenter.Template;
                CbxFormat.SelectedIndex = (Int32)Presenter.Format;
                BtnSymbolRate.Text = SymbolRateToString();
                BtnBitsPerSym.Text = BitsPerSymToString();

                CbxTimingEst.SelectedIndex = (Int32)Presenter.TimingEst;
                RdoInterpolation.ChoosedButtonIndex = (Int32)Presenter.Interpolation;
                BtnSampPerBaud.Text = SampsPerBaudToString();

                _ArgToCtrl = false;
            }
        }

        //private Control GetOptionPage(VsaTemplateOpt tt)
        //{
        //    return tt switch
        //    {
        //        VsaTemplateOpt.RF => new RFDiagramPage(Presenter),
        //        VsaTemplateOpt.IQ => new IQDiagramPage(Presenter),
        //        _ => new CustomDiagramPage(Presenter),
        //    };
        //}

        //private void LoadOptionPage()
        //{
        //    if (TlpVector.Controls.Contains(_OptionSubPage))
        //    {
        //        TlpVector.Controls.Remove(_OptionSubPage);
        //        _OptionSubPage.Dispose();
        //    }

        //    _OptionSubPage = GetOptionPage(Presenter.Template);
        //    _OptionSubPage.Dock = DockStyle.Fill;
        //    TlpVector.Controls.Add(_OptionSubPage, 0, 3);
        //}

        private void InitSourceList()
        {
            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());

            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = (ChannelId)CbxSource.SelectedIndex;
                }
            };
        }

        private void InitFormateList()
        {
            CbxFormat.Items.Clear();
            CbxFormat.Items.AddRange(Enum.GetNames<VsaFormatOpt>());

            CbxFormat.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Format = (VsaFormatOpt)CbxFormat.SelectedIndex;
                }
            };
        }

        private String SymbolRateToString() => new Quantity(Presenter.SymbolRate, Prefix.Empty, QuantityUnit.Hertz).ToString("#0.###", true);

        private String BitsPerSymToString() => new Quantity(Presenter.BitsPerSym, Prefix.Empty, QuantityUnit.Constant).ToString("##0", false);

        private String SampsPerBaudToString() => new Quantity(Presenter.SampPerBaud, Prefix.Empty, QuantityUnit.Constant).ToString("##0", false);

        private void CbxTemplate_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Template = (VsaTemplateOpt)CbxTemplate.SelectedIndex;
            }
        }

        private void BtnSymbolRate_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.SymbolRate = data);

            nkf.SetKeyBoardValue(LblSymbolRate.Text, QuantityUnit.Hertz.ToUnitString(), 3, oncomfirm,
                Presenter.SymbolRate, Presenter.MaxSymbolRate, Presenter.MinSymbolRate);

            nkf.ShowDialogByPosition();
        }

        private void BtnBitsPerSym_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.BitsPerSym = (Int32)data);

            nkf.SetKeyBoardValue(LblBitPerSym.Text, "", 0, oncomfirm,
                Presenter.BitsPerSym, Presenter.MaxBitsPerSym, Presenter.MinBitsPerSym);

            nkf.ShowDialogByPosition();
        }

        private void CbxTimingEst_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TimingEst = (VsaTimingEstOpt)CbxTimingEst.SelectedIndex;
            }
        }

        private void RdoInterpolation_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Interpolation = (VsaItplOpt)RdoInterpolation.ChoosedButtonIndex;
            }
        }

        private void BtnSampPerBaud_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.SampPerBaud = (Int32)data);

            nkf.SetKeyBoardValue(LblSampPerBaud.Text, "", 0, oncomfirm,
                Presenter.SampPerBaud, Presenter.MaxSampPerBaud, Presenter.MinSampPerBaud);

            nkf.ShowDialogByPosition();
        }

        private void ChkEnabled_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Enabled = ChkEnabled.Checked;
            }
        }
    }
}
