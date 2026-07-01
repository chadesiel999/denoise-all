using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;

namespace ScopeX.U2.Search
{
    public partial class SearchSaveConfigPage : UserControl
    {
        #region Field&Property

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
        #endregion Field&Property

        public SearchSaveConfigPage()
        {
            InitializeComponent();
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
        }

    }
}
