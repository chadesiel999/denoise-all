using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class MathForm : FloatForm, IChnlView
    {
        private readonly MathPage _MathPage;

        public MathForm()
        {
            InitializeComponent();

            _MathPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };

            Size = new(_MathPage.Size.Width + 4, _MathPage.Size.Height + HeadHeight + 4);

            Controls.Add(_MathPage);
            Controls.SetChildIndex(_MathPage, 0);

            this.HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(MathForm)));
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

        public MathPrsnt Presenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (MathPrsnt)value;
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

            _MathPage.UpdateView(prsnt, propertyName);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                TitleColor = AppStyleConfig.DefaultTitleForeColor;
                String title= Presenter.Name;
                //抖动分析
                if (Presenter.Id.IsJitterMath() && Presenter.Args is MathCustomArg arg && arg.Occupier != null)
                {
                    var type = arg.Occupier.ToString();
                    var temp = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(type).Trim();
                    if (title != temp)
                    {
                        title = temp;
                    }
                }
                Title = title;
                IndicatorColor = Presenter.DrawColor;
                ActiveBorderColor = Presenter.DrawColor;
                IsIndicatorShow = true;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_MathPage.NeedPrsnt == true)
            {
                e.Cancel = true;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        //protected override void OnLeave(EventArgs e)
        //{
        //    base.OnLeave(e);

        //    if (!Presenter.Active)
        //        (Program.Oscilloscope.View as DsoForm).RemoveBadge(Presenter);
        //    //if (CanClose)
        //    Close();
        //}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            //this.IsShowHelp = Presenter.Id.IsBaseMath();
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        private void Stylize()
        {
            _MathPage.StylizeFlag = true;
            IsShowHelp = false; 
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
    }
}
