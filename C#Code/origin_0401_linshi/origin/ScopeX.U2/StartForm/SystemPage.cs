using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    [ToolboxItem(true)]
    public partial class SystemPage : StartPageBase,IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public SystemPage()
        {
            InitializeComponent();
        }

        public override void SetElementStyle()
        {
            base.StylizeTlp(this.TlpBody);
            TlpBody.RowStyles[1] = new RowStyle(SizeType.Absolute, 30);
            TlpBody.Controls.Cast<Control>().Where(ctl => ctl.GetType() == typeof(ScopeXLabel)).ToList().ForEach(lbl =>
            {
                ScopeXLabel label = lbl as ScopeXLabel;
                label.ForeColor = AppStyleConfig.DefaultTitleForeColor;
                label.BackColor = Color.FromArgb(60, 65, 75);//AppStyleConfig.DefaultTitleBackColor;
                label.Height = AppStyleConfig.DefaultLabelHeight;
            });

            TlpBody.Controls.Cast<Control>().Where(ctl => ctl.GetType() == typeof(ScopeXIconButton)).ToList().ForEach(btn =>
            {
                ScopeXIconButton button = btn as ScopeXIconButton;
                button.ForeColor = AppStyleConfig.DefaultTitleForeColor;
                button.BackColor = Color.FromArgb(60, 65, 75);//AppStyleConfig.DefaultTitleBackColor;
                button.MouseinBackColor = AppStyleConfig.DefaultTitleBackColor.GetBrightnessColor(0.1);
            });
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_MINIMIZE);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_CLOSE);
        }

        private void BtnShutDown_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SHUTDOWN);
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_RESTART);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_LOGOUT);
        }
    }
}
