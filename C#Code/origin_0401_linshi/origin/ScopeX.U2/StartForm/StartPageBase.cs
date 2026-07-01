using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class StartPageBase : UserControl
    {
        public StartPageBase()
        {
            InitializeComponent();
        }

        protected void StylizeTlp(TableLayoutPanel tlp)
        {
            tlp.Controls.Cast<Control>().Where(ctl => ctl.GetType() == typeof(ScopeXLabel)).ToList().ForEach(lbl =>
            {
                ScopeXLabel label = lbl as ScopeXLabel;
                label.Font = AppStyleConfig.DefaultContextFont;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.ForeColor = AppStyleConfig.DefaultContextForeColor;
                label.BackColor = AppStyleConfig.DefaultContextBackColor;
            });

            tlp.Controls.Cast<Control>().Where(ctl => ctl.GetType() == typeof(ScopeXIconButton)).ToList().ForEach(btn =>
            {
                ScopeXIconButton button = btn as ScopeXIconButton;
                button.ForeColor = AppStyleConfig.DefaultContextForeColor;
                button.BackColor = AppStyleConfig.DefaultContextBackColor;
                button.MouseinBackColor = AppStyleConfig.DefaultTitleBackColor.GetBrightnessColor(0.1);
            });
        }

        /// <summary>
        /// 设置
        /// </summary>
        public virtual void SetElementStyle() 
        { }
    }
}
