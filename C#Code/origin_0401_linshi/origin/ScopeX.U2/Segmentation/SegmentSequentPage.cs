using System;
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
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2
{
    public partial class SegmentSequentPage : UserControl, ITimebaseView, IStylize
    {
        #region 成员属性
        public TimebasePrsnt Presenter
        {
            get => (TimebasePrsnt)(ParentForm as ITimebaseView).Presenter;
            set => (ParentForm as ITimebaseView).Presenter = value;
        }
        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter { get => Presenter; set => Presenter = (TimebasePrsnt)value; }

        #endregion 成员属性
        public SegmentSequentPage()
        {
            InitializeComponent();
            Tag = SegmentWorkMode.Sequent;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private void UpdateView()
        {
            if (DesignMode)
                return;
            NebStartFrame.UpdateValueString();
            NebEndFrame.UpdateValueString();
            CbxViewMode.SelectedIndex = (Int32)Presenter.RenderType;
        }

        public void UpdateView(object presenter, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Presenter.SequentStartFrame):
                    NebStartFrame.UpdateValueString();
                    break;
                case nameof(Presenter.SequentEndFrame):
                    NebEndFrame.UpdateValueString();
                    break;
                case nameof(Presenter.RenderType):
                    CbxViewMode.SelectedIndex = (Int32)Presenter.RenderType;
                    break;
                default:
                    UpdateView();
                    break;
            }
        }

        private void FragmentSequentPage_Load(object sender, EventArgs e)
        {
            Init();
            UpdateView();
        }

        private void Init()
        {
            NebStartFrame.AddClicked = (o, e) => Presenter.SequentStartFrame += e.Step;
            NebStartFrame.SubClicked = (o, e) => Presenter.SequentStartFrame += e.Step;
            NebStartFrame.StringFormatFunc = (value) => $"{Presenter.SequentStartFrame} / {Presenter.FrameCount}";
            NebStartFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebStartFrame);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SequentStartFrame = (Int32)data);

                nkf.SetKeyBoardValue(LblStartFrame.Text, "", 2, onokclickeventaction,
                    Presenter.SequentStartFrame, Presenter.SequentEndFrame, 0);

                nkf.ShowDialogByPosition();
            };

            NebEndFrame.AddClicked = (o, e) => Presenter.SequentEndFrame += e.Step;
            NebEndFrame.SubClicked = (o, e) => Presenter.SequentEndFrame += e.Step;
            NebEndFrame.StringFormatFunc = (value) => $"{Presenter.SequentEndFrame} / {Presenter.FrameCount}";
            NebEndFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebEndFrame);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.SequentEndFrame = (Int32)data);

                nkf.SetKeyBoardValue(LblEndFrame.Text, "", 2, onokclickeventaction,
                    Presenter.SequentEndFrame, Presenter.FrameCount, Presenter.SequentStartFrame);

                nkf.ShowDialogByPosition();
            };
        }

        private void CbxViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Presenter.RenderType = (PlotRenderType)CbxViewMode.SelectedIndex;
        }
    }
}
