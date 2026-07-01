namespace ScopeX.U2.Search
{
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using System;
    using System.Windows.Forms;

    public partial class SearchItemForm : FloatForm, ISearchItemView
    {
        private Control _OptionPage;

        private Boolean _ArgToCtrl;

        public SearchItemForm()
        {
            InitializeComponent();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(SearchItemForm)));
            };
        }

        public void Reload(SearchItemPrsnt prsnt)
        {
            if (prsnt != null)
            {
                try
                {
                    this.Presenter.TryRemoveView(this);
                    this.Presenter = null;
                    this.Presenter = prsnt;
                    prsnt.TryAddView(this);
                    Load();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                this.Close();
            }
        }

        private Control GetOptionPage(SearchType st)
        {
            return st switch
            {
                SearchType.Edge => MakeEdgePage(),
                SearchType.Pulse => MakePulsePage(),
                SearchType.Timeout => MakeTimeoutPage(),
                SearchType.Runt => MakeRuntPage(),
                SearchType.Window => MakeWindowPage(),
                SearchType.Transition => MakeTransitionPage(),
                SearchType.Pattern => throw new NotImplementedException(),
                SearchType.SetupHold => MakeSetupHoldPage(),
                SearchType.Auto => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            Control MakeEdgePage()
            {
                var qp = new SearchEdgeSubPage(Presenter)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }

            Control MakePulsePage()
            {
                var qp = new SearchPulseSubPage(Presenter)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }
            Control MakeTransitionPage()
            {
                var qp = new SearchTransitionSubPage((SearchTransitionPrsnt)Presenter.SearchTypePrsnt)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }
            Control MakeRuntPage()
            {
                var qp = new SearchRuntSubPage((SearchRuntPrsnt)Presenter.SearchTypePrsnt)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }
            Control MakeSetupHoldPage()
            {
                var qp = new SearchSetupHoldSubPage((SearchSetupHoldPrsnt)Presenter.SearchTypePrsnt)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }
            Control MakeTimeoutPage()
            {
                var qp = new SearchTimeoutSubPage((SearchTimeoutPrsnt)Presenter.SearchTypePrsnt)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }
            Control MakeWindowPage()
            {
                var qp = new SearchWindowSubPage((SearchWindowPrsnt)Presenter.SearchTypePrsnt)
                {
                    Dock = DockStyle.Top,
                };
                qp.Presenter.TryAddView(qp);
                return qp;
            }

        }

        private void ChangeOptionPage(SearchType st)
        {
            TlpOptions.Controls.Remove(_OptionPage);
            _OptionPage.Dispose();
            if (_OptionPage is ITriggerView tv)
            {
                tv.Presenter?.TryRemoveView(tv);
            }
            _OptionPage = GetOptionPage(st);
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            TlpOptions.Controls.Add(_OptionPage, 0, 1);
        }

        private void LoadOptionPage()
        {
            Title = Presenter.Name.ToString();
            if (_OptionPage != null)
            {
                if (TlpOptions.Controls.Contains(_OptionPage))
                {
                    TlpOptions.Controls.Remove(_OptionPage);
                }
                _OptionPage.Dispose();
            }
            _OptionPage = GetOptionPage(Presenter.Type);
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            TlpOptions.Controls.Add(_OptionPage, 0, 1);
        }

        public SearchItemPrsnt Presenter { get; set; }

        private static void ShowOrHideInfoForm(SearchItemPrsnt prsnt)
        {
            if (prsnt.EventEnable)
            {
                SearchApp.Default.ShowInfoForm(prsnt);
            }
            else
            {
                SearchApp.Default.HideInfoForm();
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

        ISearchItemPrsnt IView<ISearchItemPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchItemPrsnt)value; }

        public SearchType Type => Presenter.Type;

        public long ID => Presenter.ID;

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Load();
            Stylize();
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        private void Load()
        {
            LoadOptionPage();
            UpdateView();
        }

        private void Stylize()
        {
            if (_OptionPage is IStylize stylepage)
            {
                stylepage.StylizeFlag = true;
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            String[] property = propertyName.Split(":");
            if (property.Length == 2)
            {
                if (property[0] != Presenter.Name)
                {
                    UpdateView();
                    return;
                }
                else
                {
                    propertyName = property[1];
                }
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    ChkActive.Checked = Presenter.Active;
                    break;
                case nameof(Presenter.Visible):
                    ChkMarkerDisplay.Checked = Presenter.Visible;
                    break;
                case nameof(Presenter.Type):
                    CbxType.SelectIndex = (Int32)Presenter.Type;
                    ChangeOptionPage(Presenter.Type);
                    break;
                case nameof(Presenter.EventEnable):
                    if (Presenter?.ID == SearchApp.Default.FoucsItem?.ID)
                    {
                        ChkShowEvent.Checked = Presenter.EventEnable;
                        ShowOrHideInfoForm(Presenter);
                    }
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            //Just need to update its own directly all controls
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = Presenter.Active;
                ChkMarkerDisplay.Checked = Presenter.Visible;
                ChkShowEvent.Checked = Presenter.EventEnable;
                CbxType.SelectIndex = (Int32)Presenter.Type;
                _ArgToCtrl = false;
                JudgeCopyFromTriggerEnable();
                //ShowOrHideInfoForm(Presenter);
            }
        }


        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Active = ChkActive.Checked;
            }
        }

        private void ChkMarkerDisplay_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Visible = ChkMarkerDisplay.Checked;
            }
        }

        private void ChkShowEvent_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EventEnable = ChkShowEvent.Checked;
            }
        }

        private void CbxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Type = (SearchType)CbxType.SelectIndex;
            }
        }

        private void BtnCopyToTrigger_Click(object sender, EventArgs e)
        {
            Presenter.SetToTrigger();
        }

        public void JudgeCopyFromTriggerEnable()
        {
            var tigch = TriggerPrsnt.GetTriggerSource();
            BtnCopyFromTrigger.Visible = (TriggerPrsnt.Type == TriggerType.Edge && tigch != null && tigch.Value.IsAnalog()) || (TriggerPrsnt.Type == TriggerType.PulseWidth && tigch != null && tigch.Value.IsAnalog());
        }

        private void BtnCopyFromTrigger_Click(object sender, EventArgs e)
        {
            Presenter.ReadFromTrigger();
        }

        private void BtnCloseCurrentItem_Click(object sender, EventArgs e)
        {
            Presenter.Active = false;
        }

        private void BtnCloseAll_Click(object sender, EventArgs e)
        {
            SearchApp.Default.Presenter.RemoveAll();
            this.Close();
        }
    }
}
