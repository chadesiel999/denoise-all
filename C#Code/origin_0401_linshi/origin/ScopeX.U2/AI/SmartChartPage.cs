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
    public partial class SmartChartPage : UserControl, IArtificialIntelligenceView, IStylize
    {
        public SmartChartPage()
        {
            InitializeComponent();
            InitCbxSource();

        }

        private Boolean _ArgToCtrl = false;

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

            CbxSource.SelectedIndex = (Int32)Presenter.AiSetChnlId;
            ChkAiSetEnable.Checked = Presenter.CurAiSetEnable;
            ChkSignalRecognition.Checked = Presenter.CurAiSignalRecognitionEnable;
            ChkAiWindows.Checked = Presenter.CurAiWindowsEnable;
            ChkAiParams.Checked = Presenter.CurAiParamsEnable;

            _ArgToCtrl = false;
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, string>(UpdateView), prsnt, propertyName);
                return;
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;

            switch (propertyName)
            {
                case nameof(Presenter.AiSetChnlId):
                    UpdateView();
                    break;

                case nameof(Presenter.CurAiSetEnable):
                    ChkAiSetEnable.Checked = Presenter.CurAiSetEnable;
                    break;

                case nameof(Presenter.CurAiSignalRecognitionEnable):
                    ChkSignalRecognition.Checked = Presenter.CurAiSignalRecognitionEnable;
                    break;

                case nameof(Presenter.CurAiWindowsEnable):
                    ChkAiWindows.Checked = Presenter.CurAiWindowsEnable;
                    break;

                case nameof(Presenter.CurAiParamsEnable):
                    ChkAiParams.Checked = Presenter.CurAiParamsEnable;
                    break;
            }

            _ArgToCtrl = false;
        }
        private void InitCbxSource()
        {
            CbxSource.DataSource = ChannelIdExt.GetAnalogs().Select(o => new KeyValuePair<ChannelId, String>(o, o.ToString())).ToList();
            CbxSource.DisplayMember = "Value";
            CbxSource.ValueMember = "Key";

            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.AiSetChnlId = (ChannelId)CbxSource.SelectedIndex;
                }
            };
        }

        private void ChkSignalRecognition_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurAiSignalRecognitionEnable = ChkSignalRecognition.Checked;
            }
        }

        private void ChkAiSetEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurAiSetEnable = ChkAiSetEnable.Checked;
            }
        }

        private void ChkAiWindows_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurAiWindowsEnable = ChkAiWindows.Checked;
            }
        }

        private void ChkAiParams_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurAiParamsEnable = ChkAiParams.Checked;
            }
        }
    }
}
