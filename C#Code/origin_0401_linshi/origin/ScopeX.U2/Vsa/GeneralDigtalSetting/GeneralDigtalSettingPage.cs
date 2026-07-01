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
    public partial class GeneralDigtalSettingPage : UserControl, IVsaGenerateDigtalView
    {
        public GeneralDigtalSettingPage()
        {
            InitializeComponent();
        }

        public GenerateDigtalPrsnt Presenter
        {
            get => ((ParentForm as IVsaView).Presenter as VectorAnalysisPrsnt).GenerateDigtalPrsnt;
            set => ((ParentForm as IVsaView).Presenter as VectorAnalysisPrsnt).GenerateDigtalPrsnt = value;
        }
        IVsaVsaGenerateDigtalPrsnt IView<IVsaVsaGenerateDigtalPrsnt>.Presenter { get => Presenter; set => Presenter = (GenerateDigtalPrsnt)value; }

        private GeneralDigitalFreqBandWidthPage _FreqBandWidthPage = new();
        private GeneralDigtalAlgorithmPage _AlgorithmPage = new();
        private GeneralDigtalEqualizerPage _EqualizerPage = new();
        private GenaralDigtalModulateParaPage _ModulateParaPage = new();
        private void LoadOptionPage()
        {
            NbgVsa.SetGroupContent(0, _ModulateParaPage);
            NbgVsa.SetGroupContent(1, _FreqBandWidthPage);
            NbgVsa.SetGroupContent(2, _EqualizerPage);
            NbgVsa.SetGroupContent(3, _AlgorithmPage);
        }

        protected override void OnLoad(EventArgs e)
        {
            _ModulateParaPage.StylizeFlag = true;
            _ModulateParaPage.Presenter = Presenter;
            _FreqBandWidthPage.StylizeFlag = true;
            _FreqBandWidthPage.Presenter = Presenter;
            _EqualizerPage.StylizeFlag = true;
            _EqualizerPage.Presenter = Presenter;
            _AlgorithmPage.StylizeFlag = true;
            _AlgorithmPage.Presenter = Presenter;
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
            Presenter.TryAddView(this);
            LoadOptionPage();
            base.OnLoad(e);
        }

        protected void Update(Object prsnt, String propertyName)
        {
            _ModulateParaPage.UpdateView(prsnt, propertyName);
        }

        public void UpdateView(Object prsnt, String propertyName)
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
    }
}
