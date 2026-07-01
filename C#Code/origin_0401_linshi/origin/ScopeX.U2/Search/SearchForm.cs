namespace ScopeX.U2.Search
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class SearchForm : FloatForm, ISearchView
    {
        private Boolean _ArgToCtrl;

        public SearchForm()
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
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(SearchForm)));
            };
        }

        public SearchPrsnt Presenter { get; set; }

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

        ISearchPrsnt IView<ISearchPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchPrsnt)value; }

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
            Stylize();
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControl(this, StyleFlag.FontSize);
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
                case nameof(Presenter.Enabled):
                    ChkActive.Checked = Presenter.Enabled;
                    ChangeVisiableState(Presenter.Enabled);
                    BtnAddSearch.Enabled = Presenter.Enabled;
                    break;
                case nameof(Presenter.Running):
                    break;
                case nameof(Presenter.SearchCount):
                    break;
                case nameof(Presenter.SoftSearch):
                    ChkSoftSearch.Checked = Presenter.SoftSearch;
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
                ChkActive.Checked = Presenter.Enabled;
                ChkSoftSearch.Checked = Presenter.SoftSearch;
                BtnAddSearch.Enabled = Presenter.Enabled;
                ChangeVisiableState(Presenter.Enabled);
                _ArgToCtrl = false;
            }
        }

        private void BtnAddSearch_Click(object sender, EventArgs e)
        {
            //Boolean addSuccess = SearchApp.Default.AddSearch(out SearchItemPrsnt searchItemPrsnt);
            //if (addSuccess)
            //{
            //    var info = new SearchInfo(searchItemPrsnt);
            //    SearchApp.Default.InfoControls.TryAdd(searchItemPrsnt.Name, info);
            //    ChangeVisiableState(Presenter.Enabled);
            //    SearchApp.Default.SwtichSearchInfo(searchItemPrsnt);
            //}
        }

        private void ChkSoftSearch_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SoftSearch = ChkSoftSearch.Checked;
            }
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Enabled = ChkActive.Checked;
            }
        }

        private void ChkResultTable_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (ChkResultTable.Checked)
            {
                SearchItemPrsnt prsnt = null;
                if (Presenter.ItemPrsntMap !=null)
                {
                    var item = Presenter.ItemPrsntMap.FirstOrDefault();
                    prsnt = item.Value;
                }

                SearchApp.Default.ShowInfoForm(prsnt);
            }
            else
            {
                SearchApp.Default.HideInfoForm();
            }
        }

        private void ChangeVisiableState(Boolean visiable)
        {
            foreach (var item in Presenter.ItemPrsntMap)
            {
                if (SearchApp.Default.InfoControls.ContainsKey(item.Key))
                {
                    SearchApp.Default.InfoControls[item.Key].Visible = visiable;
                }
                else
                {
                    SearchApp.Default.InfoControls.TryAdd(item.Key, new SearchInfo(item.Value));
                }
            }
        }
    }
}
