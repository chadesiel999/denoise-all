using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    public partial class BasicCfgPage : UserControl, IStylize
    {
        public BasicCfgPage()
        {
            InitializeComponent();
            ChkHorizontalSetting.Font = ChkTriggerSetting.Font = ChkAcquisitionSeeting.Font = ChkVerticalSetting.Font = DefaultStyleConfig.DefaultLabelFont;
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

        private void BtnExtClkRefresh_Click(Object sender, EventArgs e)
        {
            LblExtClkLockState.Text = WidgetPrsnt.HardwareMiscFunc("Ext10MHzLocked", "") == "true" ?
                "ON" : "OFF";
        }
    }
}
