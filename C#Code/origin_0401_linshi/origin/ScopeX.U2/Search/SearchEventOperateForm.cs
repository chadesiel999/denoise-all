// Copyright (c) ScopeX. All Rights Reserved
// <author></author>
// <date>2022/3/23</date>

namespace ScopeX.U2
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.U2.Search;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    /// <summary>
    /// Defines the <see cref="SearchEventOperateForm" />.
    /// </summary>
    public partial class SearchEventOperateForm : FloatForm, ISearchView
    {

        private readonly SearchOperatePage _OperatePage;

        private readonly SearchSaveConfigPage _SaveConfigPage;

        public SearchEventOperateForm()
        {
            InitializeComponent();

            _OperatePage = new();
            _SaveConfigPage = new();

            _OperatePage.BackColor = Color.Transparent;
            _SaveConfigPage.BackColor = Color.Transparent;

            this.NbgSearchEvent.SetGroupContent(0, _OperatePage);
            this.NbgSearchEvent.SetGroupContent(1, _SaveConfigPage);


            Size = new(_OperatePage.Size.Width, _OperatePage.Size.Height + HeadHeight + NbgSearchEvent.CurrentGroupNum * NbgSearchEvent.NavBarHeight);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(SearchEventOperateForm)));
            };
        }

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        public SearchPrsnt Presenter { get; set; }

        /// <summary>
        /// Gets the CreateParams.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        ISearchPrsnt IView<ISearchPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchPrsnt)value; }

        /// <summary>
        /// The Refresh.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        /// <summary>
        /// The UpdateView.
        /// </summary>
        /// <param name="presenter">The presenter<see cref="Object"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="String"/>.</param>
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

        /// <summary>
        /// The OnFormClosed.
        /// </summary>
        /// <param name="e">The e<see cref="FormClosedEventArgs"/>.</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Presenter.TryRemoveView((ISearchView)this);
        }

        /// <summary>
        /// The OnLoad.
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        /// <summary>
        /// 窗体风格化
        /// </summary>
        private void Stylize()
        {
            _OperatePage.StylizeFlag = true;
            _SaveConfigPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        /// <summary>
        /// The Update.
        /// </summary>
        /// <param name="prsnt">The presenter<see cref="Object"/>.</param>
        /// <param name="propertyName">The propertyName<see cref="String"/>.</param>
        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _OperatePage.UpdateView(prsnt, propertyName);
            _SaveConfigPage.UpdateView(prsnt, propertyName);

        }

        /// <summary>
        /// The UpdateView.
        /// </summary>
        protected void UpdateView()
        {
            //Just need to update its own directly all controls
            if (!DesignMode)
            {
                this.TitleColor = AppStyleConfig.DefaultTitleForeColor;
                // this.Title = Presenter.Name;
                //this.HeadBackColor = Presenter.DrawColor.GetBrightnessColor(-0.2);
            }
        }











    }
}
