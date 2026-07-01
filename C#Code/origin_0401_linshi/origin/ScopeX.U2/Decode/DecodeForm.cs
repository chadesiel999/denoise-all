using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.U2.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

using static ScopeX.UserControls.ScopeXNumericEditBox;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class DecodeForm : FloatForm, IChnlView, IProtocolView
    {
        #region 字段及属性定义
        private Boolean _Inited = false;
        private List<SerialProtocolType> _SerialTypes = new List<SerialProtocolType>();
        private IProtocolView _DecodeView;
        /// <summary>
        /// Defines the _ArgToCtrl.
        /// </summary>
        private Boolean _ArgToCtrl;
        private DecodePrsnt _DecodePresenter;

        public DecodePrsnt DecodePresenter
        {
            get => _DecodePresenter;
            set
            {
                _DecodePresenter = value;
                if (value != null && !_Inited)
                {

                    _SerialTypes.Add(SerialProtocolType.Close);
                    _SerialTypes.AddRange(DecodeApp.Default.ChannelsView[DecodePresenter.Id].Keys.OrderBy(x => x));
                    //Init();
                    //this.CbxBusType.DropDownHeight = 500;
                }
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                Init();
            }
        }
        public IProtocolPrsnt Presenter { get => DecodePresenter.DecodeChPrsnt; set => _ = value; }
        IBadge IView<IBadge>.Presenter { get => DecodePresenter; set => DecodePresenter = value as DecodePrsnt; }

        #endregion 字段及属性定义

        public ChannelId Id { get; set; }

        public DecodeForm()
        {
            InitializeComponent();
            Stylize();
            //HelpClick += DecodeForm_HelpClick;
            //ChkEvent.Checked = DecodeApp.Default.IsEventOpen;
            DecodeApp.Default.UpdateEventState += UpdateEventState;
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(DecodeForm)));
            };
        }

        private void DecodeForm_HelpClick(object sender, EventArgs e)
        {
            HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(DecodeForm)));
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            //HelpClick -= DecodeForm_HelpClick;
            DecodeApp.Default.UpdateEventState -= UpdateEventState;
            base.OnHandleDestroyed(e);
        }

        public void ReLoadSource()
        {
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.ActiveBorderColor = DecodePresenter.DrawColor;
            base.OnPaint(e);
        }
        private void ChangeControlState(SerialProtocolType type)
        {
            if (type == SerialProtocolType.Close)
            {
                LblDisplay.Visible = false;
                BtnDisplay.Visible = true;

                LblMark.Visible = false;
                TbxMark.Visible = false;
                LblPosition.Visible = false;
                EbxPosition.Visible = false;
                BtnResetPos.Visible = false;
                LblDecodeType.Visible = false;
                CbxDecodeType.Visible = false;
                //PnlHeader.Height = CbxBusType.Bottom + 20;
            }
            else
            {
                LblDisplay.Visible = true;
                BtnDisplay.Visible = true;

                LblMark.Visible = true;
                TbxMark.Visible = true;
                //TbxMark.Left = LblMark.Left;
                //TbxMark.Top = BtnDisplay.Top;
                TbxMark.Width = 110;
                TbxMark.Height = BtnDisplay.Height;
                LblPosition.Visible = true;
                EbxPosition.Visible = true;
                BtnResetPos.Visible = true;
                LblDecodeType.Visible = true;
                CbxDecodeType.Visible = true;

                //PnlHeader.Height = CbxDecodeType.Bottom + 20;
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

        private void Init()
        {
            if (!DesignMode)
            {
                this.TitleColor = AppStyleConfig.DefaultTitleForeColor;
                this.Title = DecodePresenter.Name;
            }
            //this.CbxDecodeType.DataSource = DecodeDisplayMode.ASCII.GetEnumList();
            //this.CbxDecodeType.DisplayMember = "Key";
            //this.CbxDecodeType.ValueMember = "Value";

            //this.CbxDecodeType.SelectedValueChanged += (_, _) => DecodePresenter.Format = (DecodeDisplayMode)this.CbxDecodeType.SelectedValue;

            //this.CbxDecodeType.SelectedValue = DecodePresenter.Format;
            CbxDecodeType.SelectedIndexChanged -= CbxDecodeType_SelectedIndexChanged;
            CbxDecodeType.DataSource = Enum.GetValues<DecodeDisplayMode>().Select(o => new ComboBoxItem(o.ToString(), o, null)).ToList();
            CbxDecodeType.SelectedIndexChanged += CbxDecodeType_SelectedIndexChanged;
            CbxDecodeType.SelectValue = DecodePresenter.Format;
            CbxBusType.SelectedIndexChanged -= CbxBusType_SelectedIndexChanged;
            //this.CbxBusType.DataSource = _SerialTypes.GetEnumList();
            CbxBusType.DataSource = Enum.GetValues<SerialProtocolType>().
               Where(o => _SerialTypes.Contains(o)).
                  Select(o => new ComboBoxItem(o.GetAlias(), o, null)).ToList();
            CbxBusType.SelectValue = Presenter.ProtocolType;
            CbxBusType.SelectedIndexChanged += CbxBusType_SelectedIndexChanged;

            this.TbxMark.Text = DecodePresenter.Label;
            ChkLabelVisiblity.Checked = DecodePresenter.LabelVisibility;
            // 请避免事件反复注册。
            this.TbxMark.TextChanged -= TbxMark_TextChanged1;
            this.TbxMark.TextChanged += TbxMark_TextChanged1;

            this.ChkEvent.CheckedChangedEvent -= ChkEvent_CheckedChangedEvent;
            this.ChkEvent.CheckedChangedEvent += ChkEvent_CheckedChangedEvent;
            ChkLabelVisiblity.CheckedChangedEvent += ChkLabelVisiblity_CheckedChangedEvent;
            ChkLabelVisiblity.CheckedChangedEvent -= ChkLabelVisiblity_CheckedChangedEvent;
            ControlsHotKnob.Default.InitHotKnob(EbxPosition);
            EbxPosition.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(DecodePresenter, EbxPosition, nameof(DecodePresenter.PosIndexBymDiv));
            };
            this.EbxPosition.StringFormatFunc = (val) =>
            {
                return SIHelper.ValueChangeToSI(val, 3, "div", 1000, false);
            };
            this.EbxPosition.Value = SIHelper.SIChangeToValue(PosToString(), "div");
            this.EbxPosition.MaxValue = SIHelper.SIChangeToValue(MaxPosToString(), "div");
            this.EbxPosition.MinValue = SIHelper.SIChangeToValue(MinPosToString(), "div");
            this.EbxPosition.UpdateValueString();

            this.EbxPosition.EditValueChicked = (sender, args) => EditPosition();

            this.EbxPosition.AddClicked = (sender, args) =>
            {
                DecodePresenter.PosIndexBymDiv++;
                EbxPosition.Value = SIHelper.SIChangeToValue(PosToString(), "div");
                EbxPosition.UpdateValueString();
                args.Handle = true;

            };

            this.EbxPosition.SubClicked = (sender, args) =>
            {
                DecodePresenter.PosIndexBymDiv--;
                EbxPosition.Value = SIHelper.SIChangeToValue(PosToString(), "div");
                EbxPosition.UpdateValueString();
                args.Handle = true;

            };
            //在程序初始化时取消动画，防止在界面刚呈现时滑块在滑动
            this.BtnDisplay.UseAnimation = false;
            this.BtnDisplay.Checked = DecodePresenter.Active;

            // 请避免事件反复注册。
            this.BtnDisplay.CheckedChangedEvent -= BtnDisplay_CheckedChangedEvent;
            this.BtnDisplay.CheckedChangedEvent += BtnDisplay_CheckedChangedEvent;

            this.BtnDisplay.UseAnimation = true;
            LoadDecodeView((SerialProtocolType)this.CbxBusType.SelectValue);

            // 请避免事件反复注册。
            this.BtnResetPos.Click -= BtnResetPos_Click;
            this.BtnResetPos.Click += BtnResetPos_Click;

            

            _ArgToCtrl = true;
            //var form = (Owner as DsoForm).MultiWindowManager.GetWindow(DecodePresenter.WindowId);
            //if (form?.IsMainForm == false)
            //{
            //    ChkIndependentWindow.Checked = true;
            //}
            //else
            //{
            //    ChkIndependentWindow.Checked = false;
            //}
            ChkIndependentWindow.Checked = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.MainFigure.WindowId != DecodePresenter.WindowId;
            _ArgToCtrl = false;
        }

        private void ChkEvent_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (ChkEvent.Checked)
            {
                DecodeApp.Default.ShowEventLists(this.DecodePresenter.Id);

            }
            else
            {
                DecodeApp.Default.HideEventLists();
            }
            DecodePresenter.EventEnable = ChkEvent.Checked;
        }

        private void UpdateEventState(Boolean isopen)
        {
            ChkEvent.Checked = isopen;
        }

        private void BtnResetPos_Click(object sender, EventArgs e)
        {
            DecodePresenter.PosIndexBymDiv = 0;
            EbxPosition.Value = 0;
            EbxPosition.UpdateValueString();
        }

        private void BtnDisplay_CheckedChangedEvent(object sender, EventArgs e) => DecodePresenter.Active = BtnDisplay.Checked;

        private void TbxMark_TextChanged1(object sender, EventArgs e) => DecodePresenter.Label = TbxMark.Text;

        private void CbxDecodeType_SelectedIndexChanged(object sender, EventArgs e) => DecodePresenter.Format = (DecodeDisplayMode)CbxDecodeType.SelectValue;

        private void CbxBusType_SelectedIndexChanged(object sender, EventArgs e) => SelectedValueChanged();

        private void SelectedValueChanged()
        {
            if (!_ArgToCtrl)
            {
                SelectValueChangeUpdate((SerialProtocolType)this.CbxBusType.SelectValue);
            }
        }

        public void SelectValueChangeUpdate(SerialProtocolType serialtype)
        {
            if (!_ArgToCtrl)
            {
                _ArgToCtrl = true;
                if (_DecodeView != null && _DecodeView.Presenter != null && _DecodeView.Presenter.ProtocolType == serialtype)
                    return;

                Debug.WriteLine(serialtype);
                if (Program.Oscilloscope.OptionProtocols.ContainsKey(serialtype))
                {
                    serialtype = CheckOptionActive(Program.Oscilloscope.OptionProtocols[serialtype]) ? serialtype : SerialProtocolType.Close;
                }

                IEnumerable<IView> views = Presenter.GetViewList();

                if (DbcDecode.Content is IProtocolView view)
                {
                    if (view.Presenter.ProtocolType != serialtype)
                    {
                        Presenter.TryRemoveView(view);
                    }
                    if (view is UserControl uctl)
                    {
                        foreach (var ctl in uctl.Controls)
                        {
                            if (ctl is Panel p && p.Controls != null && p.Controls.Count > 0)
                            {
                                foreach (var pctl in p.Controls)
                                {
                                    if (pctl is IProtocolView pv)
                                    {
                                        Presenter.TryRemoveView(pv);
                                    }
                                }
                            }
                        }
                    }

                }

                ChangeControlState(serialtype);

                DecodePresenter.ProtocolType = serialtype;

                DecodeInfo info = null;
                foreach (IProtocolView v in views)
                {
                    if (v.Presenter == null || v.Presenter.ProtocolType != serialtype)
                        continue;
                    Presenter.TryAddView(v);
                }

                LoadDecodeView(serialtype);
                _ArgToCtrl = false;
            }
        }

        private void LoadDecodeView(SerialProtocolType serialtype)
        {
            switch (serialtype)
            {
                case SerialProtocolType.Close:
                    _DecodeView = null;
                    break;
                default:
                    _DecodeView = DecodeApp.Default.ChannelsView[DecodePresenter.Id][serialtype];
                    break;
            }

            if (_DecodeView != null)
            {
                _DecodeView.Presenter = Presenter;
                _DecodeView.Presenter.IsTrigger = false;
            }
            if (DbcDecode.Content is IProtocolView view && view != null && _DecodeView != null && _DecodeView.Presenter != null)
                _DecodeView.Presenter.TryRemoveView(view);
            DbcDecode.Content = (Control)_DecodeView;
            if (_DecodeView != null && _DecodeView.Presenter != null)
            {
                _DecodeView.Presenter.TryAddView(_DecodeView);
            }
            this.DbcDecode.Size = new System.Drawing.Size(600, 410);
            Reload();
        }



        private Boolean CheckOptionActive(OptionType optiontype)
        {
            if (OptionManager.Default.Checked(optiontype) == false)
            {
                _ArgToCtrl = true;
                //DecodePresenter.DecodeChPrsnt.ProtocolType = SerialProtocolType.Close;
                //this.CbxBusType.SelectedValue = DecodePresenter.DecodeChPrsnt.ProtocolType;
                CbxBusType.SelectValue = SerialProtocolType.Close;
                _ArgToCtrl = false;
                return false;
            }
            return true;
        }

        private String PosToString()
        {
            return new Quantity(DecodePresenter.PosIndexBymDiv, Prefix.Milli, "div").ToString("#0.###", true);
        }

        private String MaxPosToString() => new Quantity(DecodePresenter.PosMaxIndex, Prefix.Milli, "div").ToString("#0.###", true);
        private String MinPosToString() => new Quantity(DecodePresenter.PosMinIndex, Prefix.Milli, "div").ToString("#0.###", true);

        private void EditPosition()
        {
            Boolean laststatus = CanClose;
            NumberKeybordForm keyBoardForm = new NumberKeybordForm();
            keyBoardForm.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SheZhiWeiZhi");
            keyBoardForm.NumberKeyboard.DefaultValue = EbxPosition.Value;
            keyBoardForm.NumberKeyboard.MaxValue = EbxPosition.MaxValue;
            keyBoardForm.NumberKeyboard.MinValue = EbxPosition.MinValue;
            keyBoardForm.NumberKeyboard.Unit = "div";

            keyBoardForm.NumberKeyboard.OkClickEvent += (_, args) =>
            {
                DecodePresenter.PosIndexBymDiv = SIHelper.SIUnitConversion(args.Data, (Int32)Prefix.Empty, (Int32)Prefix.Milli);
                EbxPosition.Value = SIHelper.SIChangeToValue(PosToString(), "div");
                this.Activate();
                keyBoardForm?.Close();
            };

            keyBoardForm.NumberKeyboard.CancelEvent += (_, _) =>
            {
                this.Activate();
                keyBoardForm?.Close();
            };
            //this.CanClose = false;
            keyBoardForm.Disposed += (_, _) => CanClose = laststatus;

            keyBoardForm.StartPosition = FormStartPosition.Manual;
            keyBoardForm.Location = keyBoardForm.CalculateWindowPosition();

            DialogResult dialogresult = keyBoardForm.ShowDialogByEvent();
            EbxPosition.UpdateValueString();
        }

        private void UpdatePosition()
        {
            BeginInvoke(() => EbxPosition.Value = SIHelper.SIChangeToValue(PosToString(), "div"));
            BeginInvoke(() => EbxPosition.UpdateValueString());
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            switch (propertyName)
            {
                case "ConditioningPosition":
                    UpdatePosition();
                    break;
                case nameof(DecodePrsnt.Active):
                    UpdataDisplay();
                    break;
                case nameof(DecodePrsnt.EventEnable):
                    UpdateListOpen();
                    break;
                case nameof(DecodePrsnt.Label):
                    UpdateLabel();
                    break;
                case "LabelVisibility":
                    ChkLabelVisiblity.Checked = DecodePresenter.LabelVisibility;
                    break;
                case nameof(ProtocolPrsnt.ProtocolType):
                    UpdateSerialType();
                    break;
                case nameof(DecodePrsnt.Format):
                    BeginInvoke(() => CbxDecodeType.SelectIndex = (Int32)DecodePresenter.Format);
                    break;
            }
        }
        private void UpdateView()
        {
            UpdatePosition();
            UpdateLabel();
            UpdateSerialType();
            UpdataDisplay();
            ChkEvent.Checked = DecodePresenter.EventEnable = DecodeApp.Default.IsEventOpen;



        }

        private void UpdateListOpen()
        {
            BeginInvoke(() => ChkEvent.Checked = DecodePresenter.EventEnable);
        }
        private void UpdateSerialType()
        {
            Invoke(() =>
            CbxBusType.SelectValue = DecodePresenter.DecodeChPrsnt.ProtocolType);
            //this.CbxBusType.SelectedValue = DecodePresenter.DecodeChPrsnt.ProtocolType;
        }
        private void UpdateLabel()
        {
            BeginInvoke(() => this.TbxMark.Text = DecodePresenter.Label);
        }
        private void UpdataDisplay()
        {
            this.BtnDisplay.Checked = DecodePresenter.Active;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_DecodeView != null)
                Presenter.TryRemoveView(_DecodeView);

            Presenter.TryRemoveView(this);
            DecodePresenter.TryRemoveView(this);
            DecodeApp.Default.UpdateEventState -= UpdateEventState;
            base.OnClosing(e);
        }

        public void Reload()
        {
            _DecodeView?.Reload();
            this.ActiveBorderColor = DecodePresenter.DrawColor;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            this.IndicatorColor = DecodePresenter.DrawColor;
            this.ActiveBorderColor = DecodePresenter.DrawColor;
            this.IsIndicatorShow = true;
        }
        /// <summary>
        /// 界面风格化
        /// </summary>
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        }

        private void ChkIndependentWindow_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                if (ChkIndependentWindow.Checked)
                {
                    //SetHVisible(true);

                    DecodePresenter.WindowId = ChannelPrsnt.GetNewWindowId();
                }
                else
                {
                    var form = (Owner as DsoForm).MultiWindowManager.MainFigure;
                    if (form != null)
                    {
                        //SetHVisible(false);

                        DecodePresenter.WindowId = form.WindowId;
                    }
                }
            }
        }

        private void TbxMark_TextChanged(Object sender, EventArgs e)
        {
            DecodePresenter.Label = TbxMark.Text;
        }
        private void ChkLabelVisiblity_CheckedChangedEvent(Object sender, EventArgs e)
        {
            DecodePresenter.LabelVisibility = ChkLabelVisiblity.Checked;
        }
        public void UpdateThresholdUnit()
        {
        }
    }

}
