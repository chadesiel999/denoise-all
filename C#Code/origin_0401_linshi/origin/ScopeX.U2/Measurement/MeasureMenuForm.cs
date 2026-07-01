using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class MeasureMenuForm :FloatForm, IView
    {
        private readonly MeasureMenuPage _MeasureMenuPage;

        public MeasureMenuForm()
        {
            InitializeComponent();
            _MeasureMenuPage = new MeasureMenuPage();
            Controls.Add(_MeasureMenuPage);
            Controls.SetChildIndex(_MeasureMenuPage, 0);
            _MeasureMenuPage.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _MeasureMenuPage.Location = new Point(2, HeadHeight);
            Size = new(_MeasureMenuPage.Size.Width, _MeasureMenuPage.Size.Height + HeadHeight);
            this.HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(MeasureMenuForm)));
            };
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

        public MeasPrsnt Presenter
        {
            get;
            set;
        }

        //IMeasPrsnt IView<IMeasPrsnt>.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (MeasPrsnt)value;
        //}

        public CymometerPrsnt CymometerPresenter
        {
            get;
            set;
        }

        //ICymometerPrsnt IView<ICymometerPrsnt>.Presenter
        //{
        //    get => CymometerPresenter;
        //    set => CymometerPresenter = (CymometerPrsnt)value;
        //}

        public VoltmeterPrsnt VoltmeterPresenter
        {
            get;
            set;
        }

        //IBadge IView<IBadge>.Presenter
        //{
        //    get => VoltmeterPresenter;
        //    set => VoltmeterPresenter = (VoltmeterPrsnt)value;
        //}

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
            _MeasureMenuPage.UpdateView(prsnt, propertyName);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            CymometerPresenter.TryRemoveView(this);
            VoltmeterPresenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            //this.AdjustSize(new(_MeasureMenuPage.Size.Width, _MeasureMenuPage.Size.Height + HeadHeight));
            Size = new Size(_MeasureMenuPage.Size.Width + 4, _MeasureMenuPage.Size.Height + HeadHeight + 2);
            Style();
            base.OnLoad(e);
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        private void Style()
        {
            _MeasureMenuPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            IsShowPin = false;
            IsShowClose = false;
            IsShowHelp = false;
            HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        }
    }
}
