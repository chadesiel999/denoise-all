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
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class UnionSetPage : UserControl, IArtificialIntelligenceView, IStylize
    {
        public UnionSetPage()
        {
            InitializeComponent();
            InitCbxSource();

        }

        private Boolean _ArgToCtrl = false;

        private void InitCbxSource()
        {
            CbxSource.DataSource = ChannelIdExt.GetAnalogs().Select(o => new KeyValuePair<ChannelId, String>(o, o.ToString())).ToList();
            CbxSource.DisplayMember = "Value";
            CbxSource.ValueMember = "Key";

            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.AIUnionChnlId = (ChannelId)CbxSource.SelectedIndex;
                }
            };
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

        //protected override void OnFormClosed(FormClosedEventArgs e)
        //{
        //    Presenter.TryRemoveView(this);
        //    base.OnFormClosed(e);
        //}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
            Stylize();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public ArtificialIntelligencePrsnt Presenter
        {
            get => (ArtificialIntelligencePrsnt)(ParentForm as IArtificialIntelligenceView).Presenter;
            set => (ParentForm as IArtificialIntelligenceView).Presenter = value;
        }

        IArtificialIntelligencePrsnt IView<IArtificialIntelligencePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArtificialIntelligencePrsnt)value;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private void UpdateView()
        {
            if (DesignMode)
                return;

            _ArgToCtrl = true;
            ChkCaptureExceptionUnion.Checked = Presenter.CaptureExceptionUnionEnable;
            ChkReconfigDbiUnion.Checked = Presenter.ReconfigDbiUnionEnable;
            _ArgToCtrl = false;
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;

            switch (propertyName)
            {
                case nameof(Presenter.AIUnionChnlId):
                    UpdateView();
                    break;
                case nameof(Presenter.CaptureExceptionUnionEnable):
                    ChkCaptureExceptionUnion.Checked = Presenter.CaptureExceptionUnionEnable;
                    break;
                case nameof(Presenter.ReconfigDbiUnionEnable):
                    ChkReconfigDbiUnion.Checked = Presenter.ReconfigDbiUnionEnable;
                    break;

            }

            _ArgToCtrl = false;
        }

        private void UnionPage_Load(object sender, EventArgs e)
        {

        }

        private void ChkCaptureExceptionUnion_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CaptureExceptionUnionEnable = ChkCaptureExceptionUnion.Checked;
            }
        }

        private void ChkReconfigDbiUnion_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ReconfigDbiUnionEnable = ChkReconfigDbiUnion.Checked;
            }
        }

        private void CbxAverage_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AverageEnable(CbxAverage.Checked);
            }
        }

    }
}
