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
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class VectorAnalysisForm : FloatForm, IVsaView
    {
        private Control _OptionSubPage;

        public VectorAnalysisForm()
        {
            InitializeComponent();
        }

        private static Control GetOptionSubPage(VsaSignalType vsaType)
        {
            UserControl subpage = vsaType switch
            {
                VsaSignalType.GeneralDigtal => new GeneralDigtalSettingPage(),
                _ => new BlueToothSettingPage(),
            };
            subpage.Dock = DockStyle.Fill;
            subpage.TabIndex = 1;
            subpage.BackColor = Color.Transparent;
            if (subpage is IStylize stylepage)
            {
                stylepage.StylizeFlag = true;
                DefaultStyleManager.Instance.RegisterControlRecursion(subpage);
            }
            return subpage;
        }

        private void ChangeOptionSubPage(VsaSignalType vsaType)
        {
            //if ((_OptionSubPage as IMathView).Mode == vsaType)
            //{
            //    return;
            //}

            TlpMain.Controls.Remove(_OptionSubPage);
            _OptionSubPage.Dispose();
            //if (vsaType != VsaSignalType.GeneralDigtal)
            //{
            //    return;
            //}
            _OptionSubPage = GetOptionSubPage(vsaType);
            TlpMain.Controls.Add(_OptionSubPage, 0, 1);

            //var form = (Program.Oscilloscope.View as DsoForm).GetSingleWindow(Presenter.WindowId);
            //if (form?.IsMainForm == false)
            //{
            //    SetHVisible(true);
            //}
            //else
            //{
            //    SetHVisible(false);
            //}
        }

        private void LoadOptionPage()
        {
            _OptionSubPage = GetOptionSubPage(Presenter.SignalType);
            ChangeOptionSubPage(Presenter.SignalType);
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        private AdvancedMathPrsnt? _GraphPrsnt = null;

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            //VectorAnalysisApp.Default?.CloseVsaErrParamInfoForm();
            base.OnFormClosed(e);
            Presenter.TryRemoveView(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadOptionPage();
            InitSourceList();
            InitDemodGraphList();
            InitDestMathList();
            _GraphPrsnt = Presenter.GenerateDigtalPrsnt.GetCurVsaGraphPrsnt;
            CbxDemod.SelectedIndex = (Int32)Presenter.GenerateDigtalPrsnt.CurGraphType;
            ChkEnabled.Checked = Presenter.Enabled;
            ChkGraphEnable.Checked = _GraphPrsnt.Enabled;
            if (_GraphPrsnt.Enabled)
                CbxDestMath.Text = _GraphPrsnt.DestMathChannel.ToString();
            else
                CbxDestMath.SelectedIndex = 0;
            SyncVsaErrParamInfoFormWithPresenterEnabled();
            Stylize();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControl(this);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        private void InitSourceList()
        {
            cbxSource1.Items.Clear();
            cbxSource1.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            cbxSource1.SelectedIndexChanged += (_, _) =>
            {
                Presenter.Source = (ChannelId)cbxSource1.SelectedIndex;
            };

            cbxSource2.Items.Clear();
            cbxSource2.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            cbxSource2.SelectedIndexChanged += (_, _) =>
            {
                Presenter.Source2nd = (ChannelId)cbxSource2.SelectedIndex;
            };
        }

        private void InitDestMathList()
        {
            CbxDestMath.Items.Clear();
            foreach (var channel in ChannelIdExt.GetMaths())
            {
                if (Program.Oscilloscope.TryGetChannel(channel, out var mathprsnt))
                {
                    if (mathprsnt is MathPrsnt && (mathprsnt as MathPrsnt).Args.Occupier == null)
                    {
                        CbxDestMath.Items.Add(channel);
                    }
                }
            }
        }

        private String[] _GeneralDigtalGraph = new string[]
        {
            "I 路时域图",
            "Q 路时域图",
            "星座图",
            "I 路眼图",
            "Q 路眼图",
            "矢量图",
            "EVM",
            "相位误差时间图",
            "幅度误差时间图"
        };

        private void InitDemodGraphList()
        {
            CbxDemod.Items.Clear();
            if (Presenter.SignalType == VsaSignalType.GeneralDigtal)
            {
                CbxDemod.Items.AddRange(_GeneralDigtalGraph);
            }
        }

        protected void UpdateView()
        {
            cbxSource1.SelectedIndex = (Int32)Presenter.Source;
            cbxSource2.SelectedIndex = (Int32)Presenter.Source2nd;
            CbxTemplate.SelectedIndex = (Int32)Presenter.Template;
            CbxSignalType.SelectedIndex = (Int32)Presenter.SignalType;
        }

        protected void Update(Object prsnt, String propertyName)
        {
            ChangeOptionSubPage((prsnt as VectorAnalysisPrsnt).SignalType);
            UpdateView();
            if (String.Equals(propertyName, nameof(VectorAnalysisPrsnt.Enabled), StringComparison.Ordinal))
            {
                ChkEnabled.Checked = Presenter.Enabled;
                SyncVsaErrParamInfoFormWithPresenterEnabled();
            }
        }

        private void CbxTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Presenter.Template = (VsaTemplateOpt)CbxTemplate.SelectedIndex;
            if (Presenter.Template == VsaTemplateOpt.RF)
            {
                cbxSource2.Enabled = false;
                lblSource2.Enabled = false;
            }
            else
            {
                cbxSource2.Enabled = true;
                lblSource2.Enabled = true;
            }
        }

        private void CbxSignalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Presenter.SignalType = (VsaSignalType)CbxSignalType.SelectedIndex;
            InitDemodGraphList();
            ChangeOptionSubPage(Presenter.SignalType);
        }

        private void cbxDemod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Presenter.SignalType == VsaSignalType.GeneralDigtal)
            {
                Presenter.GenerateDigtalPrsnt.CurGraphType = (VsaGraphType)CbxDemod.SelectedIndex;
                _GraphPrsnt = Presenter.GenerateDigtalPrsnt.GetCurVsaGraphPrsnt;
                ChkGraphEnable.Checked = _GraphPrsnt.Enabled;
                InitDestMathList();
                LblDestMath.Enabled = !_GraphPrsnt.Enabled;
                CbxDestMath.Enabled = !_GraphPrsnt.Enabled;
                if (_GraphPrsnt.Enabled)
                    CbxDestMath.Text = _GraphPrsnt.DestMathChannel.ToString();
                else
                    CbxDestMath.SelectedIndex = 0;
            }            
        }

        private void BtnGraphEnable_Click(object sender, EventArgs e)
        {
            _GraphPrsnt.DestMathChannel = _GraphPrsnt.Enabled ? _GraphPrsnt.DestMathChannel : (ChannelId)(CbxDestMath.SelectedItem);
            if (ChkGraphEnable.Checked && Program.Oscilloscope.TryGetChannel(_GraphPrsnt.DestMathChannel, out var mathprsnt))
            {
                if (mathprsnt is MathPrsnt)
                {
                    (mathprsnt as MathPrsnt)!.GetOrMakeArg(MathType.Custom);
                }
            }
            _GraphPrsnt.Enabled = ChkGraphEnable.Checked;
            LblDestMath.Enabled = !_GraphPrsnt.Enabled;
            CbxDestMath.Enabled = !_GraphPrsnt.Enabled;
        }

        private void SyncVsaErrParamInfoFormWithPresenterEnabled()
        {
            VectorAnalysisApp.Default?.SyncVsaErrParamInfoFormWithPresenterEnabled();
        }

        private void ChkEnabled_Click(object sender, EventArgs e)
        {
            Presenter.Enabled = ChkEnabled.Checked;
        }
    }
}
