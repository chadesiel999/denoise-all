using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class AiParamsSetForm : FloatForm, IArtificialIntelligenceView
    {
        public AiParamsSetForm()
        {
            InitializeComponent();

            _ReconfigDbiPage = new() { BackColor = Color.Transparent };

            //_CaptureExceptionPage = new() { BackColor = Color.Transparent };

            _SignalRecognitionPage = new() { BackColor = Color.Transparent };

            _IntelligentNoiseReductionPage = new() { BackColor = Color.Transparent };

            _TemplateTrigPage = new() { BackColor = Color.Transparent };

            _UnionSetPage = new() { BackColor = Color.Transparent };

            NbgAiSet.SetGroupContent(0, _SignalRecognitionPage);
            NbgAiSet.SetGroupContent(1, _ReconfigDbiPage);
            NbgAiSet.SetGroupContent(2, _IntelligentNoiseReductionPage);
            //NbgAiSet.SetGroupContent(3, _CaptureExceptionPage);
            NbgAiSet.SetGroupContent(3, _TemplateTrigPage);
            NbgAiSet.SetGroupContent(4, _UnionSetPage);

            Size = new(_ReconfigDbiPage.Size.Width, _ReconfigDbiPage.Size.Height + HeadHeight + NbgAiSet.CurrentGroupNum * NbgAiSet.NavBarHeight);

            NbgAiSet.CurrentGroupIndexChanged += (obj, index) =>
            {
                switch (NbgAiSet.CurrentGroupIndex)
                {
                    case 0:
                        _SignalRecognitionPage?.Refresh();
                        break;
                    case 1:
                        _ReconfigDbiPage?.Refresh();
                        break;
                    case 2:
                        _IntelligentNoiseReductionPage?.Refresh();
                        break;
                    //case 3:
                    //    _CaptureExceptionPage?.Refresh();
                        //break;
                    case 3:
                        _TemplateTrigPage?.Refresh();
                        break;
                    case 4:
                        _UnionSetPage?.Refresh();
                        break;
                    default:
                        break;
                }

            };
        }

        private readonly PrecisionSetPage _ReconfigDbiPage;
        private readonly IntelligentNoiseReductionPage _IntelligentNoiseReductionPage;
        //private readonly ExceptionCapturePage _CaptureExceptionPage;
        private readonly SmartChartPage _SignalRecognitionPage;
        private readonly FocusedCapturePage _TemplateTrigPage;
        private readonly UnionSetPage _UnionSetPage;

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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public ArtificialIntelligencePrsnt Presenter
        {
            get;
            set;
        }

        IArtificialIntelligencePrsnt IView<IArtificialIntelligencePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArtificialIntelligencePrsnt)value;
        }


        public void UpdateView(object prsnt, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _ReconfigDbiPage.UpdateView(prsnt, propertyName);
            //_CaptureExceptionPage.UpdateView(prsnt, propertyName);
            _SignalRecognitionPage.UpdateView(prsnt, propertyName);
            _IntelligentNoiseReductionPage.UpdateView(prsnt, propertyName);
            _TemplateTrigPage.UpdateView(prsnt, propertyName);
            _UnionSetPage.UpdateView(prsnt, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Presenter.TryRemoveView(this);
        }

        private void Stylize()
        {
            _ReconfigDbiPage.StylizeFlag = true;
            //_CaptureExceptionPage.StylizeFlag = true;
            _SignalRecognitionPage.StylizeFlag = true;
            _IntelligentNoiseReductionPage.StylizeFlag = true;
            _TemplateTrigPage.StylizeFlag = true;
            _UnionSetPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(50, 55, 65);
        }
    }
}
