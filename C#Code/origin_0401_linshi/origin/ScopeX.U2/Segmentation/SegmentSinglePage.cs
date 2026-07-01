using ScottPlot;
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
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2
{
    public partial class SegmentSinglePage : UserControl, ITimebaseView, IStylize
    {
        public SegmentSinglePage()
        {
            InitializeComponent();
            Tag = SegmentWorkMode.Single;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
                
        public TimebasePrsnt Presenter
        {
            get => (TimebasePrsnt)(ParentForm as ITimebaseView).Presenter;
            set => (ParentForm as ITimebaseView).Presenter = value;
        }
        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter { get => Presenter; set => Presenter = (TimebasePrsnt)value; }

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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Init();
            UpdateView();
        }

        private void Init()
        {
            NebSelectedFrame.AddClicked = (o, e) => Presenter.CurFrameId += e.Step;
            NebSelectedFrame.SubClicked = (o, e) => Presenter.CurFrameId += e.Step;
            NebSelectedFrame.StringFormatFunc = (value) => $"{Presenter.CurFrameId} / {Presenter.FrameCount}";
            NebSelectedFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebSelectedFrame);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.CurFrameId = (Int32)data);

                nkf.SetKeyBoardValue(LblSelectedFrame.Text, "", 2, onokclickeventaction,
                    Presenter.CurFrameId, Presenter.FrameCount, 1);

                nkf.ShowDialogByPosition();
            };

            NebRefFrame.AddClicked = (o, e) => Presenter.ReferFrameIds += e.Step;
            NebRefFrame.SubClicked = (o, e) => Presenter.ReferFrameIds += e.Step;
            NebRefFrame.StringFormatFunc = (value) => $"{Presenter.ReferFrameIds} / {Presenter.FrameCount}";
            NebRefFrame.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebRefFrame);
                var onokclickeventaction = new Action<Double>((data) =>
                    Presenter.ReferFrameIds = (Int32)data);

                nkf.SetKeyBoardValue(LblRefFrame.Text, "", 2, onokclickeventaction,
                    Presenter.ReferFrameIds, Presenter.FrameCount, 1);

                nkf.ShowDialogByPosition();
            };
        }

        private void UpdateView()
        {
            if (DesignMode)
                return;
            NebSelectedFrame.UpdateValueString();
            NebRefFrame.UpdateValueString();
            ChkRefActive.Checked = Presenter.RefActive;
            LblSelectedSeconds.Text = CurFrameSecondString();
        }

        private String CurFrameSecondString()
        {
            return new Quantity(Presenter.CurFrameSecond, Prefix.Nano, MeasUnit.Second).ToString();
        }

        public void UpdateView(object presenter, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Presenter.CurFrameId):
                    NebSelectedFrame.UpdateValueString();
                    break;
                case nameof(Presenter.ReferFrameIds):
                    NebRefFrame.UpdateValueString();
                    break;
                case nameof(Presenter.RefActive):
                    ChkRefActive.Checked = Presenter.RefActive;
                    break;
                case nameof(Presenter.CurFrameSecond):
                    LblSelectedSeconds.Text = CurFrameSecondString();
                    break;
                default:
                    UpdateView();
                    break;
            }
        }

        private void ChkRefActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            Presenter.RefActive = ChkRefActive.Checked;
        }
    }
}
