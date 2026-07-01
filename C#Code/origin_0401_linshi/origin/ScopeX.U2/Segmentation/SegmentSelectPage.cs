using System;
// <author>LJW</author>
// <date>2022/6/29</date>
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    public partial class SegmentSelectPage : UserControl, ITimebaseView, IStylize
    {
        #region 成员属性
        public TimebasePrsnt Presenter
        {
            get => (TimebasePrsnt)(ParentForm as ITimebaseView).Presenter;
            set => (ParentForm as ITimebaseView).Presenter = value;
        }
        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter { get => Presenter; set => Presenter = (TimebasePrsnt)value; }
        #endregion 成员属性

        public SegmentSelectPage()
        {
            InitializeComponent();
            Tag = SegmentWorkMode.Select;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;


        private void UpdateView()
        {
            if (DesignMode)
                return;
            CbxViewMode.SelectedIndex = (Int32)Presenter.RenderType;
        }

        public void UpdateView(object presenter, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Presenter.RenderType):
                    CbxViewMode.SelectedIndex = (Int32)Presenter.RenderType;
                    break;
            }
        }

        private void FragmentSelectPage_Load(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void CbxViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Presenter.RenderType = (PlotRenderType)CbxViewMode.SelectedIndex;
        }

        private void BtnFramesSelected_Click(object sender, EventArgs e)
        {
            SegmentChooseForm form = new(Presenter);
            form.ShowDialog();
        }
    }
}
