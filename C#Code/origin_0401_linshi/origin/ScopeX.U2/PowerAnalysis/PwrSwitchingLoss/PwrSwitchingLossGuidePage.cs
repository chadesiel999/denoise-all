namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;

    public partial class PwrSwitchingLossGuidePage : UserControl, IStylize
    {
        public PwrSwitchingLossGuidePage()
        {
            InitializeComponent();
            LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KaiGuanSunHao_KaiGuanSunHaoFenXiCeLiangKaiGuanQiJianZaiJingTiGuanDeKaiGuanJieDuanHeChuanDaoJieDuanDeGongLvHeNengLiangSunShi__n_nCeLiangBuZou_JiangDianYaTanTouHuoDianLiuTanTouAnTuShiJinXingLianJie_");
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

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
