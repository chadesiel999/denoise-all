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
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PrecisionSetPage : UserControl, IArtificialIntelligenceView, IStylize
    {
        public PrecisionSetPage()
        {
            InitializeComponent();
            InitCbxSource();
            InitCbxSubbandTable();
            InitCbxSubbandCtrlMethodTable();
            InitNebSubbandBaseNoise();
            InitNebLoValue();
            InitBandFreqLimit();
            InitNebFreqByHz();
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
            InitCbxResolution();
            UpdateView();
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
                case nameof(Presenter.ReCfgDbiChnlId):
                    UpdateView();
                    break;

                case nameof(Presenter.AutoCfgAnaChnlBitWidthEnable):
                    ChkAutoCfgBitWidth.Checked = Presenter.AutoCfgAnaChnlBitWidthEnable;
                    break;

                case nameof(Presenter.AnaChnlBitWidth):
                    CbxResolution.SelectedItem = Presenter.AnaChnlBitWidth;
                    break;

                case nameof(Presenter.ReconfigurableDBIEnable):
                    ChkReconfigurableDBI.Checked = Presenter.ReconfigurableDBIEnable;
                    break;

                case nameof(Presenter.SubbandsEnable):
                    ChkSubbandEnable.Checked = ((0b1u << CbxSubband.SelectedIndex) & Presenter.SubbandsEnable) != 0;
                    break;

                case nameof(Presenter.CurSubbandCtrlMethod):
                    CbxSubbandCtrlMethod.SelectedItem = Presenter.CurSubbandCtrlMethod;
                    break;

                case nameof(Presenter.CurSubbandBaseNoise):
                    NebSubbandBaseNoise.UpdateValueString();
                    break;

                case nameof(Presenter.CurLocalFreq):
                    NebSubbandLocalFreq.UpdateValueString();
                    break;

                case nameof(Presenter.CurBandFreqLimit):
                    NebSubbandBandLimit.UpdateValueString();
                    break;

                case nameof(Presenter.LeftFreqByHz):
                    NebLeftFreqByHz.UpdateValueString();
                    break;

                case nameof(Presenter.RightFreqByHz):
                    NebRightFreqByHz.UpdateValueString();
                    break;

                case nameof(Presenter.PrecisionSubbandId):
                    CbxSubband.SelectedIndex = Presenter.PrecisionSubbandId;
                    NebSubbandBaseNoise.UpdateValueString();
                    NebSubbandLocalFreq.UpdateValueString();
                    NebSubbandBandLimit.UpdateValueString();
                    NebLeftFreqByHz.UpdateValueString();
                    NebRightFreqByHz.UpdateValueString();
                    break;
            }

            _ArgToCtrl = false;
        }
        private void UpdateView()
        {
            if (DesignMode)
                return;

            _ArgToCtrl = true;

            CbxSource.SelectedIndex = (Int32)Presenter.ReCfgDbiChnlId;
            ChkReconfigurableDBI.Checked = Presenter.ReconfigurableDBIEnable;
            ChkSubbandEnable.Checked = ((0b1u << CbxSubband.SelectedIndex) & Presenter.SubbandsEnable) != 0;
            CbxSubbandCtrlMethod.SelectedIndex = (Int32)Presenter.CurSubbandCtrlMethod;
            NebSubbandBaseNoise.UpdateValueString();
            NebSubbandLocalFreq.UpdateValueString();
            NebSubbandBandLimit.UpdateValueString();
            NebLeftFreqByHz.UpdateValueString();
            NebRightFreqByHz.UpdateValueString();
            CbxSubband.SelectedIndex = Presenter.PrecisionSubbandId;
            ChkAutoCfgBitWidth.Checked = Presenter.AutoCfgAnaChnlBitWidthEnable;
            CbxResolution.SelectedItem = Presenter.AnaChnlBitWidth;

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
                    Presenter.ReCfgDbiChnlId = (ChannelId)CbxSource.SelectedIndex;
                }
            };
        }

        private void InitCbxSubbandTable()
        {
            //CbxSubband.DataSource = Presenter.SubbandTable.Select(o => new KeyValuePair<Int32, String>(o, o.ToString())).ToList();
            CbxSubband.DisplayMember = "Value";
            CbxSubband.ValueMember = "Key";

            CbxSubband.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    ChkSubbandEnable.Checked = ((0b1u << CbxSubband.SelectedIndex) & Presenter.SubbandsEnable) != 0;
                    Presenter.PrecisionSubbandId = CbxSubband.SelectedIndex;
                }
            };
        }

        private void InitCbxSubbandCtrlMethodTable()
        {
            CbxSubbandCtrlMethod.DataSource = Enum.GetValues<SubbandCtrlMethod>().Select(o => new KeyValuePair<String, SubbandCtrlMethod>(o.GetAlias(), o)).ToList();
            CbxSubbandCtrlMethod.DisplayMember = "Key";
            CbxSubbandCtrlMethod.ValueMember = "Value";

            CbxSubbandCtrlMethod.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.CurSubbandCtrlMethod = (SubbandCtrlMethod)CbxSubbandCtrlMethod.SelectedIndex;
                }
            };
        }

        private void InitNebSubbandBaseNoise()
        {
            NebSubbandBaseNoise.AddClicked = (a, b) => Presenter.CurSubbandBaseNoise++;
            NebSubbandBaseNoise.SubClicked = (a, b) => Presenter.CurSubbandBaseNoise--;
            NebSubbandBaseNoise.StringFormatFunc = (value) => new Quantity(Presenter.CurSubbandBaseNoise, Prefix.Empty, "dB").ToString();
            NebSubbandBaseNoise.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.CurSubbandBaseNoise = (UInt32)data;

                nkf.SetKeyBoardValue(LblSubbandBaseNoise.Text, "dB", 2, onokclickeventaction,
                    Presenter.CurSubbandBaseNoise, UInt32.MaxValue, UInt32.MinValue);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebLoValue()
        {
            NebSubbandLocalFreq.AddClicked = (a, b) => Presenter.CurLocalFreq++;
            NebSubbandLocalFreq.SubClicked = (a, b) => Presenter.CurLocalFreq--;
            NebSubbandLocalFreq.StringFormatFunc = (value) => new Quantity(Presenter.CurLocalFreq, Prefix.Empty, "Hz").ToString();
            NebSubbandLocalFreq.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.CurLocalFreq = (UInt64)data;

                nkf.SetKeyBoardValue(LblSubbandLocalFreq.Text, "Hz", 4, onokclickeventaction,
                    Presenter.CurLocalFreq, UInt64.MaxValue, UInt64.MinValue);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitBandFreqLimit()
        {
            NebSubbandBandLimit.AddClicked = (a, b) => Presenter.CurBandFreqLimit++;
            NebSubbandBandLimit.SubClicked = (a, b) => Presenter.CurBandFreqLimit--;
            NebSubbandBandLimit.StringFormatFunc = (value) => new Quantity(Presenter.CurBandFreqLimit, Prefix.Empty, "Hz").ToString();
            NebSubbandBandLimit.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.CurBandFreqLimit = (UInt64)data;

                nkf.SetKeyBoardValue(LblSubbandBandLimit.Text, "Hz", 2, onokclickeventaction,
                    Presenter.CurBandFreqLimit, UInt64.MaxValue, UInt64.MinValue);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebFreqByHz()
        {
            NebLeftFreqByHz.AddClicked = (a, b) => Presenter.LeftFreqByHz++;
            NebLeftFreqByHz.SubClicked = (a, b) => Presenter.LeftFreqByHz--;
            NebLeftFreqByHz.StringFormatFunc = (value) => new Quantity(Presenter.LeftFreqByHz, Prefix.Empty, "Hz").ToString();
            NebLeftFreqByHz.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.LeftFreqByHz = (UInt64)data;

                nkf.SetKeyBoardValue(LblLeftFreqByHz.Text, "Hz", 2, onokclickeventaction,
                    Presenter.LeftFreqByHz, UInt64.MaxValue, UInt64.MinValue);

                nkf.ShowDialogByPosition();
            };

            NebRightFreqByHz.AddClicked = (a, b) => Presenter.RightFreqByHz++;
            NebRightFreqByHz.SubClicked = (a, b) => Presenter.RightFreqByHz--;
            NebRightFreqByHz.StringFormatFunc = (value) => new Quantity(Presenter.RightFreqByHz, Prefix.Empty, "Hz").ToString();
            NebRightFreqByHz.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.RightFreqByHz = (UInt64)data;

                nkf.SetKeyBoardValue(LblRightFreqByHz.Text, "Hz", 2, onokclickeventaction,
                    Presenter.RightFreqByHz, UInt64.MaxValue, UInt64.MinValue);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitCbxResolution()
        {
            CbxResolution.DataSource = Presenter.AnaChnlBitWidthDefine;
            CbxResolution.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.AnaChnlBitWidth = (Int32)CbxResolution.SelectedValue;
                }
            };
        }


        private void CbxSource_Click(object sender, EventArgs e)
        {

        }


        private void ChkReconfigurableDBI_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ReconfigurableDBIEnable = ChkReconfigurableDBI.Checked;
            }
        }

        private void ChkSubbandEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                UInt32 bitvalue = 0b1u << CbxSubband.SelectedIndex;
                if (ChkSubbandEnable.Checked)
                {
                    Presenter.SubbandsEnable = Presenter.SubbandsEnable | bitvalue;
                }
                else
                {
                    Presenter.SubbandsEnable = Presenter.SubbandsEnable & ~bitvalue;
                }
            }
        }

        private void ChkAutoCfgBitWidth_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoCfgAnaChnlBitWidthEnable = ChkAutoCfgBitWidth.Checked;
            }
        }
    }
}
