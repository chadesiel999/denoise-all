using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class ExceptionCapturePage : UserControl, IExceptionCaptureView, IStylize
    {
        private Boolean _ArgToCtrl = false;

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        public ExceptionCapturePage()
        {
            InitializeComponent();
            InitSourceList();
            InitNebAnorml();
            InitNebFrameLengtht();
        }

        #region Init

        private void InitSourceList()
        {
            CbxSource.DataSource = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() && c.Active).OrderBy(x => x).Select(o => new ComboBoxItem(o.ToString(), o, null)).ToList();
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if ((ChannelId)CbxSource.SelectValue != Presenter.ExceptionCaptureChnlId)
                    {
                        Presenter.ExceptionCaptureChnlId = (ChannelId)CbxSource.SelectValue;
                    }
                }
            };

            NebFrameLength.DataSource = new List<int> { 25, 50, 100, 200 }
                .Select(o => new ComboBoxItem(o.ToString(), o, null))
                .ToList();
            NebFrameLength.SelectIndex = 0;
            NebFrameLength.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if (NebFrameLength.SelectValue is int frameLength && frameLength != Presenter.CaptureExceptionFrameLength)
                    {
                        Presenter.CaptureExceptionFrameLength = frameLength;
                    }
                }
            };
        }

        private void InitNebAnorml()
        {
            NebFrameSelect.AddClicked = (a, b) => Presenter.AnormalFrameID++;
            NebFrameSelect.SubClicked = (a, b) => Presenter.AnormalFrameID--;
            NebFrameSelect.StringFormatFunc = (value) => Presenter.AnormalFrameID.ToString();
            NebFrameSelect.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.AnormalFrameID = (Int32)data;

                nkf.SetKeyBoardValue(LblWfmSelectId.Text, "", 2, onokclickeventaction,
                    Presenter.AnormalFrameID, Presenter.AnormalFrameCount - 1, Presenter.MinAnormlFrameId);

                nkf.ShowDialogByPosition();
            };
        }

        private void InitNebFrameLengtht()
        {
            //NebFrameLength.AddClicked = (a, b) => Presenter.CaptureExceptionFrameLength++;
            //NebFrameLength.SubClicked = (a, b) => Presenter.CaptureExceptionFrameLength--;
            //NebFrameLength.StringFormatFunc = (value) => Presenter.CaptureExceptionFrameLength.ToString();
            //NebFrameLength.EditValueChicked = (a, b) =>
            //{
            //    NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            //    Action<Double> onokclickeventaction = (data) =>
            //        Presenter.CaptureExceptionFrameLength = (Int32)data;

            //    nkf.SetKeyBoardValue(LblDelay.Text, "", 2, onokclickeventaction,
            //        Presenter.CaptureExceptionFrameLength, Int32.MaxValue, Int32.MinValue);

            //    nkf.ShowDialogByPosition();
            //};
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
            Presenter?.TryRemoveView(this);
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

        public ExceptionCapturePrsnt Presenter
        {
            get;
            set;
        }


        IExceptionCapturePrsnt IView<IExceptionCapturePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ExceptionCapturePrsnt)value;
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

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    ChkDisplay.Checked = Presenter.Active;
                    break;
                case nameof(Presenter.AnormalFrameCount):
                    BtnFrameCount.Text = GetFrameCountOrFrameID(Presenter.AnormalFrameCount);
                    break;
                case nameof(Presenter.AnormalFrameID):
                    NebFrameSelect.UpdateValueString();
                    break;
                case nameof(Presenter.CaptureExceptionFrameLength):
                    NebFrameLength.SelectValue = Presenter.CaptureExceptionFrameLength;
                    break;
                case nameof(Presenter.ExceptionViewMode):
                    RdoViewMode.ChoosedButtonIndex = (Int32)Presenter.ExceptionViewMode;
                    ChangeCtrlStatus();
                    break;
                case nameof(Presenter.TemplateTriggerSource):
                    RdoTemplateMode.ChoosedButtonIndex = (Int32)Presenter.TemplateTriggerSource;
                    break;
                case nameof(Presenter.CaptureExceptionEnable):
                    ChkEnable.Checked = Presenter.CaptureExceptionEnable;
                    break;
                case nameof(Presenter.ExceptionCaptureChnlId):
                    CbxSource.SelectValue = Presenter.ExceptionCaptureChnlId;
                    RdoTemplateMode.ChoosedButtonIndex = (Int32)Presenter.TemplateTriggerSource;
                    RdoViewMode.ChoosedButtonIndex = (Int32)Presenter.ExceptionViewMode;
                    ChkEnable.Checked = Presenter.CaptureExceptionEnable;

                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkEnable.Checked = Presenter.CaptureExceptionEnable;
                CbxSource.SelectValue = Presenter.ExceptionCaptureChnlId;
                RdoViewMode.ChoosedButtonIndex = (Int32)Presenter.ExceptionViewMode;
                RdoTemplateMode.ChoosedButtonIndex = (Int32)Presenter.TemplateTriggerSource;
                NebFrameLength.SelectValue = Presenter.CaptureExceptionFrameLength;
                NebFrameSelect.UpdateValueString();
                BtnFrameCount.Text = GetFrameCountOrFrameID(Presenter.AnormalFrameCount);
                ChkDisplay.Checked = Presenter.Active;
                ChangeCtrlStatus();
                _ArgToCtrl = false;
            }
        }

        private String GetFrameCountOrFrameID(UInt32 countOrId)
        {
            return $"{countOrId}{QuantityUnit.Count.ToUnitString()}";
            //return new Quantity(countOrId, Prefix.Empty, QuantityUnit.Count).ToString("#0.###", true);
        }

        private void BtnSendTemplate_Click(object sender, EventArgs e)
        {
            Presenter.BuildTemplate();
        }

        private void BtnExceptionDataExport_Click(object sender, System.EventArgs e)
        {
            Presenter.ExportCaptureWave2File();
        }
        private void RdoViewMode_IndexChanged(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ExceptionViewMode = (ExceptionViewMode)RdoViewMode.ChoosedButtonIndex;
                ChangeCtrlStatus();
            }
        }

        private void ChangeCtrlStatus()
        {
            var visible = false;
            switch (Presenter.ExceptionViewMode)
            {
                case ExceptionViewMode.None:
                case ExceptionViewMode.Single:
                    visible = false;
                    break;
                case ExceptionViewMode.All:
                    visible = true;
                    break;
                default:
                    break;
            }
            LblStartId.Visible = LblEndId.Visible = NebStartFrameId.Visible = NebEndFrameId.Visible = visible;
            LblWfmSelectId.Visible = NebFrameSelect.Visible = !visible;
        }

        private void RdoTemplateMode_IndexChanged(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.TemplateTriggerSource = (TemplateTriggerSourceEnum)RdoTemplateMode.ChoosedButtonIndex;
            }
        }

        private void ChkEnable_CheckedChangedEvent(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CaptureExceptionEnable = ChkEnable.Checked;
            }
        }

        private void ChkDisplay_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = ChkDisplay.Checked;
            }
        }

        private static Boolean StudyStatus = false;
        private void BtnWfmStudy_Click(object sender, EventArgs e)
        {
            if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.ExCaptureWfmStudy, MessageType.Asking))
            {
                Presenter.AnormalFrameCount = 0;
                Presenter.ClearFlag = true;
                Presenter.WfmStudy();

                if (Presenter.StudyStatus)
                {
                    ExceptionCapturePage.StudyStatus = true;
                    WeakTip.Default.Write($"ExceptionCapture", MsgTipId.ExCaptureWfmStudySuccess, duration: 5);
                }
                else
                    WeakTip.Default.Write($"ExceptionCapture", MsgTipId.ExCaptureWfmStudyFail, duration: 5);
            }
        }

        private void BtnExceptionCapture_Click(object sender, EventArgs e)
        {
            if (ExceptionCapturePage.StudyStatus == false)
            {
                WeakTip.Default.Write($"ExceptionCapture", MsgTipId.ExCaptureWfmStudyExcute, duration: 5);
                return;
            }
            Presenter.ClearFlag = false;
            Presenter.ExcuteExCapture();
        }

        private void NebFrameLength_Load(object sender, EventArgs e)
        {

        }

        private void LblDelay_Click(object sender, EventArgs e)
        {

        }
    }
}

