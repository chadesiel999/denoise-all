using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class FileForm : FloatForm, IFileView
    {
        private readonly WfmSavePage _WaveSavePage;
        private readonly ScreenSavePage _PicSavePage;
        private readonly SettingPage _SettingSavePage;
        private readonly DataExportPage _DataExportPage;
        private readonly LongStorageWfmSavePage _LongStorageWfmSavePage;
        private readonly AiExportPage _AiExportPage;
        private String _RecordKey = String.Empty;
        public FileForm(FilePrsnt prsnt)
        {
            InitializeComponent();
            InitLangControl();
            Presenter = prsnt;
            _PicSavePage = new()
            {
                BackColor = Color.Transparent,
            };

            _WaveSavePage = new()
            {
                Presenter = Presenter,
                BackColor = Color.Transparent,
            };

            _SettingSavePage = new()
            {
                BackColor = Color.Transparent,
            };

            _LongStorageWfmSavePage = new()
            {
                BackColor = Color.Transparent,
            };

            _DataExportPage = new DataExportPage()
            {
                BackColor = Color.Transparent,
            };

            _AiExportPage = new()
            {
                Presenter = Presenter,
                BackColor = Color.Transparent, 
            };

            NbgFile.SetGroupContent(0, _WaveSavePage);
            NbgFile.SetGroupContent(1, _PicSavePage);
            NbgFile.SetGroupContent(2, _SettingSavePage);
            NbgFile.SetGroupContent(3, _LongStorageWfmSavePage);
            NbgFile.SetGroupContent(4, _DataExportPage);
            NbgFile.SetGroupContent(5, _AiExportPage); 

            Size = new(_PicSavePage.Width, _AiExportPage.Height + HeadHeight + NbgFile.NavBarHeight * NbgFile.CurrentGroupNum);


            _RecordKey = $"{this.Name}_{NbgFile.Name}";
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(FileForm)));
            };
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgFile.CurrentGroupIndex;
            base.OnHandleDestroyed(e);
        }
        private void SetGroupIndex()
        {
            var index = -1;
            if (index < 0)
            {
                if (!DsoPrsnt.NavBarGroupRecords.ContainsKey(_RecordKey))
                {
                    DsoPrsnt.NavBarGroupRecords.AddOrUpdate(_RecordKey, 0, (k, v) => 0);
                }
                index = DsoPrsnt.NavBarGroupRecords[_RecordKey];
            }
            if (index > NbgFile.CurrentGroupNum)
            {
                index = NbgFile.CurrentGroupNum - 1;
            }
            NbgFile.CurrentGroupIndex = index;
        }
        private void InitLangControl()
        {
            UserControls.GroupItem groupItem1 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem2 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem3 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem4 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem5 = new UserControls.GroupItem();
            UserControls.GroupItem groupItem6 = new UserControls.GroupItem();
            groupItem1.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem1.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem1.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem1.GroupSize = new System.Drawing.Size(0, 0);
            groupItem1.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXing");

            groupItem2.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem2.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem2.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem2.GroupSize = new System.Drawing.Size(0, 0);
            groupItem2.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiePing");

            groupItem3.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem3.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem3.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem3.GroupSize = new System.Drawing.Size(0, 0);
            groupItem3.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiTongSheZhi");

            groupItem4.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem4.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem4.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem4.GroupSize = new System.Drawing.Size(0, 0);
            groupItem4.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShenCunChuShuJuDaoChu");

            groupItem5.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem5.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem5.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem5.GroupSize = new System.Drawing.Size(0, 0);
            groupItem5.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DataExportHeader");

            groupItem6.BackGroundColor = System.Drawing.Color.FromArgb(41, 42, 45);
            groupItem6.ButtonColor = System.Drawing.Color.FromArgb(53, 54, 58);
            groupItem6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            groupItem6.FontColor = System.Drawing.Color.FromArgb(232, 234, 237);
            groupItem6.GroupSize = new System.Drawing.Size(0, 0);
            groupItem6.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AiShuJuDaoChu");
            NbgFile.GroupItems = (new UserControls.GroupItem[] { groupItem1, groupItem2, groupItem3, groupItem4, groupItem5, groupItem6 });
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

        public FilePrsnt Presenter
        {
            get;
            set;
        }

        IFilePrsnt IView<IFilePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (FilePrsnt)value;
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

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _PicSavePage.UpdateView(presenter, propertyName);
            _SettingSavePage.UpdateView(presenter, propertyName);
            _WaveSavePage.UpdateView(presenter, propertyName);
            _LongStorageWfmSavePage.UpdateView(presenter, propertyName);
            _DataExportPage.UpdateView(presenter, propertyName);
            _AiExportPage.UpdateView(presenter, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            SetGroupIndex();
        }

        private void Stylize()
        {
            IsShowHelp = false;
            _WaveSavePage.StylizeFlag = true;
            _PicSavePage.StylizeFlag = true;
            _SettingSavePage.StylizeFlag = true;
            _LongStorageWfmSavePage.StylizeFlag = true;
            _DataExportPage.StylizeFlag = true;
            _AiExportPage.StylizeFlag = true;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);

            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void NbgFile_CurrentGroupIndexChanged(object sender, int previousIndex)
        {
            if (NbgFile.CurrentGroupIndex == 0)
            {
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.WfmPath, FilePrsnt.GetWfmFileExtName(Presenter.WfmFormat));
            }
            else if (NbgFile.CurrentGroupIndex == 1)
            {
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.PicPath, FilePrsnt.GetPicFileExtName(Presenter.PicFormat));
            }
            else if (NbgFile.CurrentGroupIndex == 2)
            {
                Presenter.FileName = Presenter.MakeDefaultFileName(Presenter.SettingSavePath,".set");
            }
        }
    }
}
