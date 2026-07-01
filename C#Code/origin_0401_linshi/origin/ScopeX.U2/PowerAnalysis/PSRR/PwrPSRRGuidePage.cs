// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/12</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;

    public partial class PwrPSRRGuidePage : UserControl, IStylize
    {
        public PwrPSRRGuidePage()
        {
            InitializeComponent();
            LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PSRRDescription");
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
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
