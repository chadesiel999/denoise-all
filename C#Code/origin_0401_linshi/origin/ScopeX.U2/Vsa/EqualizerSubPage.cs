using System;
using System.Windows.Forms;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class EqualizerSubPage : UserControl, IVsaView
    {
        public EqualizerSubPage(VectorAnalysisPrsnt vap, EqualizerNodePrsnt enp)
        {
            InitializeComponent();
            Presenter = vap;
            EqualizerPrsnt = enp;
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        public EqualizerNodePrsnt EqualizerPrsnt
        {
            get;
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

        IVsaPrsnt IView<IVsaPrsnt>.Presenter { get => Presenter; set => Presenter = (VectorAnalysisPrsnt)value; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            switch (propertyName)
            {
                case nameof(EqualizerPrsnt.Gradient):
                    BtnGradient.Text = GradientToString();
                    break;
                case nameof(EqualizerPrsnt.TapLength):
                    BtnTapLength.Text = TapLengthToString();
                    break;
                case nameof(EqualizerPrsnt.SymLength):
                    BtnSymLength.Text = SymLengthToString();
                    break;
                default:
                    UpdateView();
                    break;
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                BtnGradient.Text = GradientToString();
                BtnTapLength.Text = TapLengthToString();
                BtnSymLength.Text = SymLengthToString();
            }
        }

        private String GradientToString() => EqualizerPrsnt.Gradient.ToString("G3");

        private String TapLengthToString() => EqualizerPrsnt.TapLength.ToString("D1");

        private String SymLengthToString() => EqualizerPrsnt.SymLength.ToString("D1");

        private void BtnGradient_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => EqualizerPrsnt.Gradient = data);

            nkf.SetKeyBoardValue(LblGradient.Text, "", 3, oncomfirm,
                EqualizerPrsnt.Gradient, EqualizerPrsnt.MaxGradient, EqualizerPrsnt.MinGradient);

            nkf.ShowDialogByPosition();
        }

        private void BtnSymLength_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => EqualizerPrsnt.SymLength = (Int32)data);

            nkf.SetKeyBoardValue(LblSymLength.Text, "", 0, oncomfirm,
                EqualizerPrsnt.SymLength, EqualizerPrsnt.MaxSymLength, EqualizerPrsnt.MinSymLength);

            nkf.ShowDialogByPosition();
        }

        private void BtnTapLength_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => EqualizerPrsnt.TapLength = (Int32)data);

            nkf.SetKeyBoardValue(LblTapLength.Text, "", 0, oncomfirm,
                EqualizerPrsnt.TapLength, EqualizerPrsnt.MaxTapLength, EqualizerPrsnt.MinTapLength);

            nkf.ShowDialogByPosition();
        }
    }
}
