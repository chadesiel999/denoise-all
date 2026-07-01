using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class MultiDomainViewPage : UserControl, IMultiDomainView, IStylize
    {
        public MultiDomainViewPage()
        {
            InitializeComponent();
            InitCbxSource();
            InitCbxFigureType();
            InitCbxPowerUnit();
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
                    MultiDomainPresenter.Source = (ChannelId)CbxSource.SelectedIndex;
                }
            };
        }

        private void InitCbxFigureType()
        {
            CbxFigureType.DataSource = Enum.GetValues<MultiDomainFigureEnum>().Select(o => new KeyValuePair<MultiDomainFigureEnum, String>(o, o.GetAlias())).ToList();
            CbxFigureType.DisplayMember = "Value";
            CbxFigureType.ValueMember = "Key";

            CbxFigureType.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    MultiDomainPresenter.CurFigureType = (MultiDomainFigureEnum)CbxFigureType.SelectedIndex;
                }
            };
        }

        private void InitCbxPowerUnit()
        {
            CbxPowerUnit.DataSource = Enum.GetValues<LogarithmUnit>().Select(o => new KeyValuePair<LogarithmUnit, String>(o, o.GetAlias())).ToList();
            CbxPowerUnit.DisplayMember = "Value";
            CbxPowerUnit.ValueMember = "Key";

            //CbxPowerUnit.SelectedValue = LogarithmUnit.dBmV;

            CbxPowerUnit.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    MultiDomainPresenter.PUnit = (LogarithmUnit)CbxPowerUnit.SelectedIndex;
                }
            };
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        //[Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public MultiDomainPrsnt MultiDomainPresenter
        {
            get => (MultiDomainPrsnt)(ParentForm as IMultiDomainView).Presenter;
            set => (ParentForm as IMultiDomainView).Presenter = value;
        }

        IMultiDomainPrsnt IView<IMultiDomainPrsnt>.Presenter
        {
            get => MultiDomainPresenter;
            set => MultiDomainPresenter = (MultiDomainPrsnt)value;
        }

        private Boolean _ArgToCtrl;

        private void InitView()
        {
            _ArgToCtrl = true;
            ChkActive.Checked = MultiDomainPresenter.Active;
            CbxFigureType.SelectedIndex = (Int32)MultiDomainPresenter.CurFigureType;
            ChkFigureEnable.Checked = MultiDomainPresenter.CurFigureEnable;
            CbxSource.SelectedIndex = (Int32)MultiDomainPresenter.Source;
            ChkSynchronizationEnable.Checked = MultiDomainPresenter.SynchronizationEnable;
            ChkParameterTuningEnable.Checked = MultiDomainPresenter.ParameterTuningEnable;
            ChkThreeDimensionalEnable.Checked = MultiDomainPresenter.ThreeDimensionalEnable;
            //if (MultiDomainPresenter.ThreeDimensionalEnable)
            //{
            //    (Program.Oscilloscope.View as DsoForm).TryAddThreeDimensionalUI(MultiDomainPresenter);
            //}
            //else
            //{
            //    (Program.Oscilloscope.View as DsoForm).TryRemoveThreeDimensionalUI(MultiDomainPresenter);
            //}
            _ArgToCtrl = false;
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (ParentForm == null)
            {
                return;
            }
            if (String.IsNullOrEmpty(propertyName))
            {
                InitView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(MultiDomainPresenter.Active):
                    ChkActive.Checked = MultiDomainPresenter.Active;
                    break;
                case nameof(MultiDomainPresenter.Source):
                    CbxSource.SelectedIndex = (Int32)MultiDomainPresenter.Source;
                    break;
                case nameof(MultiDomainPresenter.CurFigureType):
                    CbxFigureType.SelectedIndex = (Int32)MultiDomainPresenter.CurFigureType;
                    ChkFigureEnable.Checked = MultiDomainPresenter.CurFigureEnable;
                    break;
                case nameof(MultiDomainPresenter.CurFigureEnable):
                    ChkFigureEnable.Checked = MultiDomainPresenter.CurFigureEnable;
                    break;
                case nameof(MultiDomainPresenter.ThreeDimensionalEnable):
                    ChkThreeDimensionalEnable.Checked = MultiDomainPresenter.ThreeDimensionalEnable;
                    if (MultiDomainPresenter.ThreeDimensionalEnable)
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddThreeDimensionalUI(MultiDomainPresenter);
                    }
                    else
                    {
                        (Program.Oscilloscope.View as DsoForm).TryRemoveThreeDimensionalUI(MultiDomainPresenter);
                    }
                    break;
                case nameof(MultiDomainPresenter.SynchronizationEnable):
                    ChkSynchronizationEnable.Checked = MultiDomainPresenter.SynchronizationEnable;
                    break;
                case nameof(MultiDomainPresenter.ParameterTuningEnable):
                    ChkParameterTuningEnable.Checked = MultiDomainPresenter.ParameterTuningEnable;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            InitView();
            base.OnLoad(e);
            Stylize();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void ChkActive_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.Active = ChkActive.Checked;
            }
        }

        private void ChkSynchronizationEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.SynchronizationEnable = ChkSynchronizationEnable.Checked;
            }
        }

        private void ChkFigureEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.CurFigureEnable = ChkFigureEnable.Checked;
            }
        }

        private void ChkParameterTuningEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.ParameterTuningEnable = ChkParameterTuningEnable.Checked;
            }
        }

        private void ChkThreeDimensionalEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                MultiDomainPresenter.ThreeDimensionalEnable = ChkThreeDimensionalEnable.Checked;
            }
        }
    }
}
