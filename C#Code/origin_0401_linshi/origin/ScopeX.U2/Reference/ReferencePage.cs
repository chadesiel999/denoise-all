using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    public partial class ReferencePage : UserControl, IChnlView, IStylize
    {
        private Boolean _ArgToCtrl;

        public ReferencePage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            LblAmplitudeSelection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.FuDuXiTiao"); // "幅度细调";
            ChkAmplitude.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive", "CheckedText");
            ChkAmplitude.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnalogForm.NbgAnalog.TlpNavBarContainer.VerticalPage.ChkActive");
            //NebVScale
			 ControlsHotKnob.Default.InitHotKnob(NebVScale);
            NebVScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebVScale);
            };
            NebVScale.AddClicked += (_, _) =>
            {
                if (Presenter.Ylevel_SelectStatus)
                {
                    Presenter.SetScaleValueBymV(1);
                }
                else
                {
                    Presenter.ScaleIndex++;
                }
            };
            NebVScale.SubClicked += (_, _) =>
            {
                if (Presenter.Ylevel_SelectStatus)
                {
                    Presenter.SetScaleValueBymV(-1);
                }
                else
                {
                    Presenter.ScaleIndex--;
                }
            };
            NebVScale.StringFormatFunc = (value) => VScaleToString();
            NebVScale.EditValueChicked = (a, b) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebVScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.ScaleBymV = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.Prefix);

                nkf.SetKeyBoardValue(LblVScale.Text, Presenter.Unit, 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.ScaleBymV, Presenter.Prefix),
                    Quantity.ConvertByPrefix(Presenter.MaxScale, Presenter.Prefix),
                    Quantity.ConvertByPrefix(Presenter.MinScale, Presenter.Prefix));

                nkf.ShowDialogByPosition();
            };

            //NebVPos
            ControlsHotKnob.Default.InitHotKnob(NebVPos);
            NebVPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebVPos);
            };
            NebVPos.AddClicked += (_, e) => Presenter.PosIndexBymDiv += e.Step;
            NebVPos.SubClicked += (_, e) => Presenter.PosIndexBymDiv += e.Step;
            NebVPos.StringFormatFunc = (value) => VPosToString();
            NebVPos.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebVPos);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.PosMaxIndex, Prefix.Milli);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.PosMinIndex, Prefix.Milli);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.PosIndexBymDiv, Prefix.Milli);
                nkf.NumberKeyboard.Unit = QuantityUnit.Division.ToUnitString();
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.Title = LblVPos.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.PosIndexBymDiv = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Prefix.Milli);

                    nkf.Close();
                };

                nkf.ShowDialogByPosition();
            };

            //NebHScale
            ControlsHotKnob.Default.InitHotKnob(NebHScale);
            NebHScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHScale);
            };
            NebHScale.AddClicked += (_, _) => Presenter.Sampling.ScaleIndex++;
            NebHScale.SubClicked += (_, _) => Presenter.Sampling.ScaleIndex--;
            NebHScale.StringFormatFunc = (value) => HScaleToString();
            NebHScale.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHScale);
                Action<Double> onokclickeventaction = (data) =>
                    Presenter.Sampling.Scale = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.Sampling.Prefix);

                nkf.SetKeyBoardValue(LblHScale.Text, Presenter.Sampling.Unit, 2, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.Sampling.Scale, Presenter.Sampling.Prefix),
                    Quantity.ConvertByPrefix(Presenter.Sampling.MaxScale, Presenter.Sampling.Prefix),
                    Quantity.ConvertByPrefix(Presenter.Sampling.MinScale, Presenter.Sampling.Prefix));

                nkf.ShowDialogByPosition();
            };

            //NebHPos
            ControlsHotKnob.Default.InitHotKnob(NebHPos);
            NebHPos.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebHPos);
            };
            NebHPos.AddClicked += (_, e) => Presenter.Sampling.PosIndexBymDiv += e.Step;
            NebHPos.SubClicked += (_, e) => Presenter.Sampling.PosIndexBymDiv += e.Step;
            NebHPos.StringFormatFunc = (value) => HPosToString();
            NebHPos.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebHPos);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.Sampling.PosMaxIndex, Prefix.Milli);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.Sampling.PosMinIndex, Prefix.Milli);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.Sampling.PosIndexBymDiv, Prefix.Milli);
                nkf.NumberKeyboard.Unit = QuantityUnit.Division.ToUnitString();
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.Title = LblHPos.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.Sampling.PosIndexBymDiv = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Prefix.Milli);

                    nkf.Close();
                };

                nkf.ShowDialogByPosition();
            };
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
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

        public ReferencePrsnt Presenter
        {
            get => (ReferencePrsnt)(ParentForm as IChnlView).Presenter;
            set => (ParentForm as IChnlView).Presenter = value;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (ReferencePrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });

            }
            else
                Update(prsnt, propertyName);
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            ChkAmplitude.Checked = Presenter.Ylevel_SelectStatus;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    break;
                case "ConditioningScale":
                case "ConditioningScaleUnit":
                case nameof(Presenter.ScaleBymV):
                    NebVScale.UpdateValueString();
                    TbxUnit.Text = Presenter.Unit;
                    ReDecode(Presenter.Id);
                    break;
                case "ConditioningPosition":
                    NebVPos.UpdateValueString();
                    ReDecode(Presenter.Id);
                    break;
                case "SamplingScale":
                    NebHScale.UpdateValueString();
                    ReDecode(Presenter.Id);
                    break;
                case "SamplingPosition":
                    NebHPos.UpdateValueString();
                    ReDecode(Presenter.Id);
                    break;
                case nameof(Presenter.Label):
                    TbxLabel.Text = Presenter.Label;
                    break;
                case nameof(Presenter.LabelVisibility):
                    ChkLabelVisiblity.Checked= Presenter.LabelVisibility;
                    break;
                case nameof(Presenter.Ylevel_SelectStatus):
                    ChkAmplitude.Checked = Presenter.Ylevel_SelectStatus;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                NebVScale.UpdateValueString();
                NebVPos.UpdateValueString();
                NebHScale.UpdateValueString();
                NebHPos.UpdateValueString();

                TbxLabel.Text = Presenter.Label;
                ChkLabelVisiblity.Checked = Presenter.LabelVisibility;
                TbxUnit.Text = Presenter.Unit;
                BtnFullFileName.Text = GetFileName();

                _ArgToCtrl = false;
            }
        }

        private String VScaleToString()
        {
            return new Quantity(Presenter.ScaleBymV, Presenter.Prefix, Presenter.Unit).ToString();
        }

        private String VPosToString()
        {
            return new Quantity(Presenter.PosIndexBymDiv, Prefix.Milli, QuantityUnit.Division).ToString("#0.###", true);
        }

        private String HScaleToString()
        {
            return new Quantity(Presenter.Sampling.Scale, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString();
        }

        private String HPosToString()
        {
            //return new Quantity(Presenter.Sampling.PositionByus, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString(5, true);
            return new Quantity(Presenter.Sampling.PosIndexBymDiv, Prefix.Milli, QuantityUnit.Division).ToString("#0.###", true);
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();

            _ArgToCtrl = true;
            //BaseDisplayForm form = (ParentForm.Owner as DsoForm).MultiWindowManager.GetWindow(Presenter.WindowId);
            //if (form?.IsMainForm == false)
            //{
            //    ChkIndependentWindow.Checked = true;
            //}
            //else
            //{
            //    ChkIndependentWindow.Checked = false;
            //}
            ChkIndependentWindow.Checked = Presenter.WindowId != (ParentForm.Owner as DsoForm).MultiWindowManager.MainFigure.WindowId;
            _ArgToCtrl = false;
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = false;

                //(Program.Oscilloscope.View as DsoForm).RemoveBadge(Presenter);
            }
        }
        private void ChkLabelVisiblity_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.LabelVisibility = ChkLabelVisiblity.Checked;

            }
        }
        private void BtnFullFileName_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //FileBrowserForm rdform = FileBrowserForm.Instance;
                //rdform.SetFileFilter(Enum.GetValues<WfmFormat>().Where(x => x == WfmFormat.Binary));
                //rdform.SetPath(System.IO.Path.GetDirectoryName(Presenter.FullFileName));
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Binary(*.bin)|*.bin| CSV(*.csv) |*.csv";
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(Presenter.FullFileName);
                if (Path.GetExtension(BtnFullFileName.Text) == ".csv")
                {
                    dialog.FilterIndex = 2;
                }

                //if (rdform.ShowDialogByEvent() == DialogResult.Yes)
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ReferencePrsnt rprsnt = Presenter;
                    FileInfo fileInfo = new FileInfo(dialog.FileName);
                    if (fileInfo.Extension == "." + WfmFormat.Binary.GetAlias())
                    {
                        if (ReferencePrsnt.TryRead(Presenter.Id, Presenter.Dso, dialog.FileName/*rdform.FullFileName*/, ref rprsnt))
                        {
                            BtnFullFileName.Text = GetFileName(rprsnt);
                            rprsnt.Active = true;
                            ReDecode(rprsnt.Id);
                            return;
                        }
                    }
                    if (fileInfo.Extension == "." + WfmFormat.CSV.GetAlias())
                    {
                        if (ReferencePrsnt.TryReadSVG(Presenter.Id, Presenter.Dso, dialog.FileName/*rdform.FullFileName*/, ref rprsnt))
                        {
                            BtnFullFileName.Text = GetFileName(rprsnt);
                            rprsnt.Active = true;
                            ReDecode(rprsnt.Id);
                            return;
                        }
                    }
                    WeakTip.Default.Write("REF", MsgTipId.RefFileError1);
                }

            }
        }

        private string GetFileName(ReferencePrsnt prsnt = null)
        {
            String name = String.Empty;
            if (prsnt == null)
                name = Presenter?.FullFileName;
            else
                name = prsnt?.FullFileName;
            if (!string.IsNullOrWhiteSpace(name))
            {
                Size size = TextRenderer.MeasureText(name, BtnFullFileName.Font, BtnFullFileName.Size, TextFormatFlags.NoPadding);
                if (size.Width >= BtnFullFileName.Size.Width * 0.9)
                {
                    /// <Remark>创建人：彭博 创建日期：2024/2/22 20:00:00  原因：显示文本框显示不全，需省略信息 </Remark>
                    //name = System.IO.Path.GetPathRoot(name) + "...\\" + System.IO.Path.GetFileName(name);
                    name = GetFileNameEx(name);
                }
            }

            return name;
        }

        /// <summary>
        /// 获取文件名省略信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="IsFirst"></param>
        /// <returns></returns>
        /// <Remark>创建人：彭博 创建日期：2024/2/22 20:00:00  原因：显示文本框显示不全，需省略信息 </Remark>
        public string GetFileNameEx(string name, bool IsFirst = true)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Size size = TextRenderer.MeasureText(name + "...", BtnFullFileName.Font, BtnFullFileName.Size, TextFormatFlags.NoPadding);
                if (size.Width >= BtnFullFileName.Size.Width * 0.9)
                {
                    name = name.Remove(name.Length - 1);
                    return GetFileNameEx(name, false);
                }
                else
                {
                    if (IsFirst)
                    {
                        return name;
                    }
                    else
                    {
                        return name + "...";
                    }
                }
            }
            return name;
        }

        private void BtnResetVPos_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ResetPosIndex();
            }
        }

        private void BtnResetHPos_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Sampling.ResetPosIndex();
            }
        }

        private void ChkIndependentWindow_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (ChkIndependentWindow.Checked)
                {
                    Presenter.WindowId = ChannelPrsnt.GetNewWindowId();
                }
                else
                {
                    IWaveformFigure form = (ParentForm.Owner as DsoForm).MultiWindowManager.MainFigure;
                    if (form != null)
                    {
                        Presenter.WindowId = form.WindowId;
                    }
                }
            }
        }

        private void TbxLabel_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Label = TbxLabel.Text;
            }
        }

        private void TbxUnit_TextChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Unit = TbxUnit.Text;
            }
        }

        private void ReDecode(ChannelId id)
        {
            if (Core.Decode.DecodeDataHelper.Instance.ReferenceDataSource != null && Core.Decode.DecodeDataHelper.Instance.ReferenceDataSource.Length > id - ChannelIdExt.MinRChId)
            {
                Core.Decode.DecodeDataHelper.Instance.ReferenceDataSource[id - ChannelIdExt.MinRChId].HasData = false;
            }
        }


        /// <summary>
        /// 幅度细调开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkAmplitude_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (ChkAmplitude.Checked)
                {
                    Presenter.Ylevel_SelectStatus = true;
                }
                else
                {
                    Presenter.Ylevel_SelectStatus = false;
                }
            }
        }

        public void UpdateAmplitude_Checked()
        {
            ChkAmplitude.Checked = Presenter.Ylevel_SelectStatus;
        }

    }
}
