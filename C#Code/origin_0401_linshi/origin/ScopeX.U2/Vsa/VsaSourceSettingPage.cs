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
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class VsaSourceSettingPage : UserControl, IVsaView
    {
        public VsaSourceSettingPage()
        {
            InitializeComponent();
            InitSourceList();
        }

        private String[] _GeneralDigtalGraph = new string[]
        {
            "IQ时域图",
            "星座图",
            "眼图",
            "矢量图",
            "符号表",
            "符号误差表",
            "相位误差时间图",
            "幅度误差时间图"
        };

        public VectorAnalysisPrsnt Prsent
        {
            get;
            set;
        }

        public IVsaPrsnt Presenter { get => Prsent; set => Prsent = (VectorAnalysisPrsnt)value; }

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

        protected void Update(Object prsnt, String propertyName)
        {
            UpdateView();
        }

        protected void UpdateView()
        {
            cbxSource1.SelectedIndex = (Int32)Prsent.Source;
            cbxSource2.SelectedIndex = (Int32)Prsent.Source2nd;
            CbxTemplate.SelectedIndex = (Int32)Prsent.Template;
            CbxSignalType.SelectedIndex = (Int32)Prsent.SignalType;
        }

        private void InitSourceList()
        {
            cbxSource1.Items.Clear();
            cbxSource1.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            cbxSource1.SelectedIndexChanged += (_, _) =>
            {
                Prsent.Source = (ChannelId)cbxSource1.SelectedIndex;
            };

            cbxSource2.Items.Clear();
            cbxSource2.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            cbxSource2.SelectedIndexChanged += (_, _) =>
            {
                Prsent.Source2nd = (ChannelId)cbxSource2.SelectedIndex;
            };
        }

        private void InitDemodGraphList()
        {
            cbxDemod.Items.Clear();
            cbxDemod.Text = "";
            if (Prsent.SignalType == VsaSignalType.GeneralDigtal)
            {
                cbxDemod.Items.AddRange(_GeneralDigtalGraph);
                cbxDemod.SelectedIndex = 0;
            }
            //cbxDemod.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DefaultStyleManager.Instance.RegisterControl(this);
            UpdateView();
        }

        private void CbxSignalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Prsent.SignalType = (VsaSignalType)CbxSignalType.SelectedIndex;
            InitDemodGraphList();
        }

        private void CbxTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Prsent.Template = (VsaTemplateOpt)CbxTemplate.SelectedIndex;
            if (Prsent.Template == VsaTemplateOpt.RF)
            {
                cbxSource2.Visible = false;
                lblSource2.Visible = false;
            }
            else
            {
                cbxSource2.Visible = true;
                lblSource2.Visible = true;
            }
        }
    }
}
