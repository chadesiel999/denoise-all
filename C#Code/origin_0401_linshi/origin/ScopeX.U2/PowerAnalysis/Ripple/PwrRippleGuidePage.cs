// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/12</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls.Style;

    public partial class PwrRippleGuidePage : UserControl, IStylize
    {
        public PwrRippleGuidePage()
        {
            InitializeComponent();
            LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WenBoFenXi_CeLiangDianYuanShuChuZhiLiuXinHaoDeFengDaoFengJiZhi_KeHengLiangDianYuanDeDianYaDiaoJieNengLiHeLvBoDeZhiLiang__n_nCeLiangBuZou_JiangDianYaTanTouHuoDianLiuTanTouAnTuShiJinXingLianJie_");
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
        }
    }
}
