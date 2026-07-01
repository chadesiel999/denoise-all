using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class SettingForm : FloatForm, ISettingView
    {
        /// <summary>
        /// Defines the _OtherPage.
        /// </summary>
        private readonly OtherSettingPage _OtherSettingPage;

        /// <summary>
        /// Defines the _AuxOutputPage.
        /// </summary>
        private readonly AuxOutputPage _AuxOutputPage;

        /// <summary>
        /// Defines the DisplaySettingPage.
        /// </summary>
        private readonly DisplaySettingPage _DisplaySettingPage;

        /// <summary>
        /// Defines the AutoSettingPage.
        /// </summary>
        private readonly AutoSettingPage _AutoSettingPage;

        /// <summary>
        /// Defines the NetworkSettingPage.
        /// </summary>
        private readonly NetWorkPage _NetWorkPage;

        private readonly ComponentSettingPage? _ComponentSettingPage;

        private readonly ManufacturerAdatperPage _AdatperPage;

        /// <summary>
        /// Defines the NetworkSettingPage.
        /// <author>Zhang XuLin</author>
        /// </summary>
        private readonly ScreenDetectionPage _ScreenDetectionPage;

        public Int32 GroupIndex { get; set; } = -1;

        private String _RecordKey = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingForm"/> class.
        /// </summary>
        public SettingForm()
        {
            InitializeComponent();
            //_AuxOutputPage = new()
            //{
            //    BackColor = Color.Transparent,
            //    Dock = DockStyle.Fill,
            //    Presenter = Program.Oscilloscope.Setting
            //};
            //Program.Oscilloscope.Setting.TryAddView(_AuxOutputPage);
            //_OtherSettingPage = new()
            //{
            //    BackColor = Color.Transparent,
            //    Dock = DockStyle.Fill,
            //    DispPresenter = Program.Oscilloscope.Display
            //};
            //Program.Oscilloscope.Display.TryAddView(_OtherSettingPage);

            _DisplaySettingPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                Presenter = Program.Oscilloscope.Display
            };
            Program.Oscilloscope.Display.TryAddView(_DisplaySettingPage);

            _AutoSettingPage = new(Program.Oscilloscope)
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            _NetWorkPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                Presenter = Program.Oscilloscope.LAN
            };
            Program.Oscilloscope.LAN.TryAddView(_NetWorkPage);
            //_ScreenDetectionPage = new()
            //{
            //    BackColor = Color.Transparent,
            //    Dock = DockStyle.Fill,
            //    Presenter = Program.Oscilloscope.SystemCheck
            //};
            //Program.Oscilloscope.SystemCheck.TryAddView(_ScreenDetectionPage);

            //if (PlatformUIManager.Default.Platform.Attribute.FunctionCropping)
            //{
            //    _ComponentSettingPage = new ComponentSettingPage(Program.Oscilloscope)
            //    {
            //        BackColor = Color.Transparent,
            //        Dock = DockStyle.Fill,
            //    };
            //}

            //_AdatperPage = new()
            //{
            //    BackColor = Color.Transparent,
            //    Dock = DockStyle.Fill,
            //};

            InitNbgSetting();

            _RecordKey = $"{this.Name}_{NbgSetting.Name}";
            this.HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(SettingForm)));
            };
            //NbgSetting.CurrentGroupIndexChanged += (_, _) =>
            //{
            //    switch (NbgSetting.CurrentGroupIndex)
            //    {
            //        case 0:
            //            _DisplaySettingPage.Refresh();
            //            break;
            //        case 1:
            //            _AutoSettingPage.Refresh();
            //            break;
            //        case 2:
            //            _NetWorkPage.Refresh();
            //            break;
            //        case 3:
            //            _AuxOutputPage.Refresh();
            //            break;
            //        case 4:
            //            _OtherSettingPage.Refresh();
            //            break;
            //        default:
            //            break;
            //    }
            //};
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgSetting.CurrentGroupIndex;
            base.OnHandleDestroyed(e);
        }

        public void InitNbgSetting()
        {
            if (PlatformUIManager.Default.Platform.Attribute.FunctionCropping)
            {
                Size = new(_NetWorkPage.Size.Width, _NetWorkPage.Size.Height + HeadHeight + NbgSetting.CurrentGroupNum * NbgSetting.NavBarHeight);

                //NbgSetting.SetGroupContent(0, _DisplaySettingPage);
                //NbgSetting.SetGroupContent(1, _AutoSettingPage);
                NbgSetting.SetGroupContent(0, _NetWorkPage);
                //NbgSetting.SetGroupContent(3, _AuxOutputPage);
                //NbgSetting.SetGroupContent(4, _ComponentSettingPage);
                //NbgSetting.SetGroupContent(5, _AdatperPage);
                //NbgSetting.SetGroupContent(6, _OtherSettingPage);

                return;
            }

            Controls.Remove(NbgSetting);

            //GroupItem groupItem1 = new GroupItem();
            //GroupItem groupItem2 = new GroupItem();
            GroupItem groupItem3 = new GroupItem();
            //GroupItem groupItem4 = new GroupItem();
            //GroupItem groupItem5 = new GroupItem();
            //GroupItem groupItem6 = new GroupItem();

            //GroupItem groupItem7 = new GroupItem();
            NbgSetting = new NavBarGroup();
            NbgSetting.AssignHelper = "";
            NbgSetting.CurrentGroupIndex = 0;
            NbgSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            //groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            //groupItem1.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //groupItem1.Font = null;
            //groupItem1.FontColor = System.Drawing.Color.White;
            //groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group0"); // "显示";
            //groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            //groupItem2.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //groupItem2.Font = null;
            //groupItem2.FontColor = System.Drawing.Color.White;
            //groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group1"); // "自动设置与校正";
            groupItem3.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            groupItem3.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            groupItem3.Font = null;
            groupItem3.FontColor = System.Drawing.Color.White;
            groupItem3.GroupSize = new System.Drawing.Size(0, 0);
            groupItem3.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group2"); // "通信";
            //groupItem4.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            //groupItem4.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //groupItem4.Font = null;
            //groupItem4.FontColor = System.Drawing.Color.White;
            //groupItem4.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem4.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group3"); // "辅助输入与输出";

            //groupItem5.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            //groupItem5.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //groupItem5.Font = null;
            //groupItem5.FontColor = System.Drawing.Color.White;
            //groupItem5.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem5.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group4"); // "指令适配器";

            //groupItem6.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            //groupItem6.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //groupItem6.Font = null;
            //groupItem6.FontColor = System.Drawing.Color.White;
            //groupItem6.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem6.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group5"); // "自检";

            //groupItem7.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 46);
            //groupItem7.ButtonColor = System.Drawing.Color.FromArgb(50, 50, 50);
            //groupItem7.Font = null;
            //groupItem7.FontColor = System.Drawing.Color.White;
            //groupItem7.GroupSize = new System.Drawing.Size(0, 0);
            //groupItem7.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.Group6"); // "其他";

            NbgSetting.GroupItems = new UserControls.GroupItem[] { /*groupItem1,*//* groupItem2,*/ groupItem3/*, groupItem4, groupItem5, groupItem6, groupItem7*/ };
            NbgSetting.LanguagePattern = ScopeX.Controls.LanguageDefinition.LanguagePattern.Default;
            NbgSetting.Location = new System.Drawing.Point(2, 45);
            NbgSetting.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            NbgSetting.Name = "NbgSetting";
            NbgSetting.NavBarHeight = 40;
            NbgSetting.NavForeColor = System.Drawing.Color.White;
            NbgSetting.NavGroupColor = System.Drawing.Color.FromArgb(41, 42, 46);
            NbgSetting.NavGroupHeight = 363;
            NbgSetting.Size = new System.Drawing.Size(456, 603);
            NbgSetting.SplitColor = System.Drawing.Color.Empty;
            NbgSetting.StyleFlags = UserControls.Style.StyleFlag.None;
            NbgSetting.StylizeFlag = true;
            NbgSetting.TabIndex = 0;

            Controls.Add(NbgSetting);
            Controls.SetChildIndex(NbgSetting, 0);
            Size = new(_NetWorkPage.Size.Width, _NetWorkPage.Size.Height + HeadHeight + NbgSetting.CurrentGroupNum * NbgSetting.NavBarHeight);

            //NbgSetting.SetGroupContent(0, _DisplaySettingPage);
            //NbgSetting.SetGroupContent(1, _AutoSettingPage);
            NbgSetting.SetGroupContent(0, _NetWorkPage);
            //NbgSetting.SetGroupContent(3, _AuxOutputPage);
            //NbgSetting.SetGroupContent(4, _AdatperPage);
            //NbgSetting.SetGroupContent(5, _ScreenDetectionPage);
            //NbgSetting.SetGroupContent(6, _OtherSettingPage);
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public SettingPrsnt Presenter
        {
            get; //=> (SettingPrsnt)(ParentForm as ISettingView).Presenter;
            set; //=> (ParentForm as ISettingView).Presenter = value;
        }

        ISettingPrsnt IView<ISettingPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (SettingPrsnt)value;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                //_ArgToCtrl = true;

                //_ArgToCtrl = false;
            }
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            SetGroupIndex(GroupIndex);
            LanguageFactory.CacheFormLanguageControls(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Program.Oscilloscope.Display.TryRemoveView(_DisplaySettingPage);
            //Program.Oscilloscope.Display.TryRemoveView(_OtherSettingPage);
            //Program.Oscilloscope.Setting.TryRemoveView(_AuxOutputPage);
            Program.Oscilloscope.LAN.TryRemoveView(_NetWorkPage);
            base.OnClosing(e);
        }

        private void SetGroupIndex(Int32 index = 0)
        {
            if (index < 0)
            {
                if (!DsoPrsnt.NavBarGroupRecords.ContainsKey(_RecordKey))
                {
                    DsoPrsnt.NavBarGroupRecords.AddOrUpdate(_RecordKey, 0, (k, v) => 0);
                }
                index = DsoPrsnt.NavBarGroupRecords[_RecordKey];
            }
            if (index > NbgSetting.CurrentGroupNum)
            {
                index = NbgSetting.CurrentGroupNum - 1;
            }
            NbgSetting.CurrentGroupIndex = index;
        }

        public override void Refresh()
        {
            base.Refresh();
            switch (NbgSetting.CurrentGroupIndex)
            {
                //case 0:
                //    _DisplaySettingPage.Refresh();
                //    break;
                //case 1:
                //    _AutoSettingPage.Refresh();
                //    break;
                case 0:
                    _NetWorkPage.Refresh();
                    break;
                //case 3:
                //    _AuxOutputPage.Refresh();
                //    break;
                //case 4:
                //    _OtherSettingPage.Refresh();
                //    break;
                default:
                    break;
            }
        }
        private void Stylize()
        {
            IsShowHelp = false;
            _DisplaySettingPage.StylizeFlag = true;
            //_AutoSettingPage.StylizeFlag = true;
            _NetWorkPage.StylizeFlag = true;
            //_AuxOutputPage.StylizeFlag = true;
            //_OtherSettingPage.StylizeFlag = true;
            //_AdatperPage.StylizeFlag = true;
            //_ScreenDetectionPage.StylizeFlag = true;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, ScopeX.UserControls.Style.StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }
    }
}
