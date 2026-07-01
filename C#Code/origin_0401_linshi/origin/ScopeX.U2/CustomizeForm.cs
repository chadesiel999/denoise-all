using EventBus;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{

    public partial class CustomizeForm : FloatForm, IChnlView
    {
        public CustomizeForm()
        {
            InitializeComponent();
            FixedToolIconInfos[2].IsShow = false;
            FixedToolIconInfos[3].IsShow = false;
            InitCbxSetting();
            Int32 lx = (Program.Oscilloscope.View as DsoForm).Width;
            Int32 ly = (Program.Oscilloscope.View as DsoForm).Height;
            Int32 locationx = (lx - Width) / 2;
            locationx = locationx > 0 ? locationx : 0;
            Int32 locationy = (ly - Height) / 2;
            locationy = locationy > 0 ? locationy : 0;
            Location = new Point(locationx, locationy);
            //StartPosition = FormStartPosition.CenterScreen;
            //EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new() { Current = this,Type= FormType.SettingForm });
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

        public IBadge Presenter { get; set; }

        protected void UpdateView()
        {
            //Just need to update its own directly all controls
            if (!DesignMode)
            {
                CbxSetting.SelectValue = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm;
            }
        }
        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        private void InitCbxSetting()
        {
            CbxSetting.SelectedIndexChanged -= CbxSetting_SelectedIndexChanged;
            var cbxlist = Enum.GetValues<Forms>().Select(x => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(EnumEx.GetDescription(x))/*EnumEx.GetDescription(x)*/, x, null)).ToList();
            CbxSetting.DataSource = cbxlist;
            CbxSetting.SelectedIndexChanged += CbxSetting_SelectedIndexChanged;

            BtnRun.Visible = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm != Forms.None;
        }

        private void CbxSetting_SelectedIndexChanged(Object sender, EventArgs e)
        {
            var form = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm;
            if (form != (Forms)CbxSetting.SelectIndex)
            {
                DsoPrsnt.DefaultDsoPrsnt.UserSettingForm = (Forms)CbxSetting.SelectValue;
            }
            BtnRun.Visible = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm != Forms.None;
        }
        private void Stylize()
        {

            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Font != null)
            {
                Font = null;
            }
            base.OnFormClosed(e);
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (propertyName == nameof(DsoPrsnt.DefaultDsoPrsnt.UserSettingForm))
            {
                CbxSetting.SelectValue = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm;
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var form = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm;
            if (form != Forms.None)
            {
                _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, (Int32)form);
            }
        }
    }
}
