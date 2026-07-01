using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class FrequencyVSTimePage : UserControl, IChnlView, IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        #region Field&Property

        private Boolean _ArgToCtrl;

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

        public RadioFrequencyPrsnt Presenter
        {
            get => (RadioFrequencyPrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (RadioFrequencyPrsnt)value;
        }
        #endregion Field&Property

        public FrequencyVSTimePage()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
    
            CbxSTFTDataLength.Text = (Presenter.STFTLength.ToString());
            CbxSTFTStep.Text = (Presenter.STFTStep.ToString());

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
           
                CbxSTFTDataLength.Text = (Presenter.STFTLength.ToString());
                CbxSTFTStep.Text = (Presenter.STFTStep.ToString());

                _ArgToCtrl = false;
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void CbxSTFTDataLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                Presenter.STFTLength = Convert.ToInt32(CbxSTFTDataLength.Items[CbxSTFTDataLength.SelectedIndex]);
        }

        private void CbxSTFTStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                Presenter.STFTStep = Convert.ToInt32(CbxSTFTStep.Items[CbxSTFTStep.SelectedIndex]);
        }
    }
}
