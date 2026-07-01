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
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class FocusedCapturePage : UserControl, IArtificialIntelligenceView, IStylize
    {
        public FocusedCapturePage()
        {
            InitializeComponent();
            InitCbxSource();
            InitNebAnorml();
            InitNebTemplateOffset();
            InitNebUserDefinePosStart();
            InitNebFrameTrigDataLen();
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
                    Presenter.TemplateTriggerChnlId = (ChannelId)CbxSource.SelectedIndex;
                }
            };
        }

        private void InitNebAnorml()
        {
            NebAnormSelect.AddClicked = (a, b) => Presenter.FrameIdForTrig++;
            NebAnormSelect.SubClicked = (a, b) => Presenter.FrameIdForTrig--;
            NebAnormSelect.StringFormatFunc = (value) => Presenter.FrameIdForTrig.ToString();
            NebAnormSelect.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.FrameIdForTrig = (UInt32)data;

                nkf.SetKeyBoardValue(LblAnormSelect.Text, "", 2, onokclickeventaction,
                    Presenter.FrameIdForTrig, Presenter.MaxAnormlFrameId, Presenter.MinAnormlFrameId);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebTemplateOffset()
        {
            NebTemplateOffset.AddClicked = (a, b) => Presenter.TemplateOffset++;
            NebTemplateOffset.SubClicked = (a, b) => Presenter.TemplateOffset--;
            NebTemplateOffset.StringFormatFunc = (value) => Presenter.TemplateOffset.ToString();
            NebTemplateOffset.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.TemplateOffset = (UInt32)data;

                nkf.SetKeyBoardValue(LblTemplateOffset.Text, "", 2, onokclickeventaction,
                    Presenter.TemplateOffset, 2 << Constants.ADC_BITS, 0);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebUserDefinePosStart()
        {
            NebUserDefinePosStart.AddClicked = (a, b) => Presenter.UserDefinePosStart++;
            NebUserDefinePosStart.SubClicked = (a, b) => Presenter.UserDefinePosStart--;
            NebUserDefinePosStart.StringFormatFunc = (value) => Presenter.UserDefinePosStart.ToString();
            NebUserDefinePosStart.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.UserDefinePosStart = (Int32)data;

                nkf.SetKeyBoardValue(LblUserDefineStartPos.Text, "", 2, onokclickeventaction,
                    Presenter.UserDefinePosStart, 2 << Constants.ADC_BITS, 0);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebFrameTrigDataLen()
        {
            NebFrameTrigDataLen.AddClicked = (a, b) => Presenter.FrameTrigDataLen++;
            NebFrameTrigDataLen.SubClicked = (a, b) => Presenter.FrameTrigDataLen--;
            NebFrameTrigDataLen.StringFormatFunc = (value) => Presenter.FrameTrigDataLen.ToString();
            NebFrameTrigDataLen.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.FrameTrigDataLen = (UInt32)data;

                nkf.SetKeyBoardValue(LblAnormSelect.Text, "", 2, onokclickeventaction,
                    Presenter.FrameTrigDataLen, UInt32.MaxValue, UInt32.MinValue);

                nkf.ShowDialogByPosition();
            };
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

        //[Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private Boolean _ArgToCtrl = false;

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
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
                case nameof(Presenter.TemplateTriggerChnlId):
                    UpdateView();
                    break;
                case nameof(Presenter.FramworkDetectEnable):
                    ChkFormworkEnable.Checked = Presenter.FramworkDetectEnable;
                    break;
                case nameof(Presenter.TemplateSource):
                    RdoTemplateSource.ChoosedButtonIndex = (Int32)Presenter.TemplateSource;
                    break;
                case nameof(Presenter.TemplateOffset):
                    NebTemplateOffset.UpdateValueString();
                    break;
                case nameof(Presenter.UserDefinePosStart):
                    NebUserDefinePosStart.UpdateValueString();
                    break;
                case nameof(Presenter.FrameIdForTrig):
                    NebAnormSelect.UpdateValueString();
                    break;
                case nameof(Presenter.FrameTrigDataLen):
                    NebFrameTrigDataLen.UpdateValueString();
                    break;
            }

            _ArgToCtrl = false;
        }

        private void UpdateView()
        {
            if (DesignMode)
                return;

            _ArgToCtrl = true;

            ChkFormworkEnable.Checked = Presenter.FramworkDetectEnable;
            RdoTemplateSource.ChoosedButtonIndex = (Int32)Presenter.TemplateSource;
            NebTemplateOffset.UpdateValueString();
            NebUserDefinePosStart.UpdateValueString();
            NebAnormSelect.UpdateValueString();
            NebFrameTrigDataLen.UpdateValueString();

            _ArgToCtrl = false;
        }

        private void ChkFormworkEnable_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FramworkDetectEnable = ChkFormworkEnable.Checked;
            }
        }

        private void RdoTemplateSource_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TemplateSource = (TemplateSourceEnum)RdoTemplateSource.ChoosedButtonIndex;
            }
        }

        private void BtnTemplate_Click(object sender, EventArgs e)
        {
            Presenter.SendTemplateTrigger();
        }
    }
}
