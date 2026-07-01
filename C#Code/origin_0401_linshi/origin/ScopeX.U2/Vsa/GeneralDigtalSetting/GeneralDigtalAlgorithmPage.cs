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
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class GeneralDigtalAlgorithmPage : UserControl, IStylize, IVsaGenerateDigtalView
    {
        private Boolean _ArgToCtrl;
        public GeneralDigtalAlgorithmPage()
        {
            InitializeComponent();
        }
        public GenerateDigtalPrsnt Presenter
        {
            get;
            set;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        IVsaVsaGenerateDigtalPrsnt IView<IVsaVsaGenerateDigtalPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (GenerateDigtalPrsnt)value;
        }

        public void UpdateView(object presenter, string propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.TimeSyncType):
                    CbxTimeSync.SelectedIndex = (Int32)Presenter.TimeSyncType;
                    break;
                case nameof(Presenter.CarrySyncType):
                    CbxCarrierSync.SelectedIndex = (Int32)Presenter.CarrySyncType;
                    break;
            }
            _ArgToCtrl = false;
        }

        private void CbxTimeSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TimeSyncType = (VsaTimeSyncType)CbxTimeSync.SelectedIndex;
            }
                
        }

        private void CbxCarrierSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CarrySyncType = (VsaCarrySyncType)CbxCarrierSync.SelectedIndex;
            }
        }
    }
}
