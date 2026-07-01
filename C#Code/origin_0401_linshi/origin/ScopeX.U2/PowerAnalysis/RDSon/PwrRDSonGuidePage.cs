using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ScopeX.U2
{
      public partial class PwrRDSonGuidePage : UserControl, IStylize
    {
        public PwrRDSonGuidePage()
        {
            InitializeComponent();
            LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Rdson_RdsonFenXiCeLiang");
            DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

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

        protected override void Dispose(Boolean disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
