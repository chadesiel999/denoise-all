using CyUSB;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
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

namespace ScopeX.U2.Demo
{
    public partial class PowerAnalysisPage : UserControl, IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public PowerAnalysisPage()
        {
            InitializeComponent();
            LblType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
            BtnAdd.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YanShi");
            InitComboxList();
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
        private void InitComboxList()
        {
            CbxMode.DataSource = Enum.GetValues<PowerAnalysisOpt>().
               Where(o => o == PowerAnalysisOpt.PowerQuality || o == PowerAnalysisOpt.Harmonic /*|| o == PowerAnalysisOpt.SafeOperationArea*/
              /* || o == PowerAnalysisOpt.SwitchingLoss */|| o == PowerAnalysisOpt.Ripple /*|| o == PowerAnalysisOpt.LoopAnalysis*/).
               Select(o => new ComboBoxItem(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(o.GetDescription()), o, null)).ToList();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if ((PowerAnalysisOpt)CbxMode.SelectValue == PowerAnalysisOpt.PowerQuality)
            {
                Program.Oscilloscope.AddDemo(DemoType.PWRQuality);
            }
            else if ((PowerAnalysisOpt)CbxMode.SelectValue == PowerAnalysisOpt.Harmonic)
            {
                Program.Oscilloscope.AddDemo(DemoType.PWRHarmonic);
            }
            else if ((PowerAnalysisOpt)CbxMode.SelectValue == PowerAnalysisOpt.SwitchingLoss)
            {
                Program.Oscilloscope.AddDemo(DemoType.SwitchingLoss);
            }
            else if ((PowerAnalysisOpt)CbxMode.SelectValue == PowerAnalysisOpt.Ripple)
            {
                Program.Oscilloscope.AddDemo(DemoType.Ripple);
            }
        }
    }
}
