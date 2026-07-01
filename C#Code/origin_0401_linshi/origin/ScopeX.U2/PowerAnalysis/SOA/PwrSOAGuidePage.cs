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

    public partial class PwrSOAGuidePage : UserControl, IStylize
    {
        public PwrSOAGuidePage()
        {
            InitializeComponent();
            LblGuide.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnQuanGongZuoQu_CeLiangShuRuDianYaHeDianLiuYiJiChanShengDeGongLv_CeShiJieGuoFanYingLeShuRuJiaoLiuXianLuDeZhiLiang__n_nCeLiangBuZou_JiangDianYaTanTouHeDianLiuTanTouAnTuShiLianJie_");
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

        protected override void Dispose(bool disposing)
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
