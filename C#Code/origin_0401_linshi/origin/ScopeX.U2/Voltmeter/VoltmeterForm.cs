using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class VoltmeterForm : FloatForm, IVoltmeterView
    {
        private readonly VoltmeterPage _DVMPage;

        public VoltmeterForm()
        {
            InitializeComponent();

            _DVMPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };
            Size = new(_DVMPage.Size.Width, _DVMPage.Size.Height + HeadHeight);

            Controls.Add(_DVMPage);
            Controls.SetChildIndex(_DVMPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(VoltmeterForm)));
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

        public VoltmeterPrsnt Presenter
        {
            get;
            set;
        }

        //IVoltmeterPrsnt IVoltmeterView.Presenter
        //{
        //    get => Presenter;
        //    set => Presenter = (VoltmeterPrsnt)value;
        //}

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (VoltmeterPrsnt)value;
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
                return;
            }
            
            switch(propertyName)
            {
                case nameof(Presenter.IsStatActive):
                    break;
            }


            _DVMPage.UpdateView(prsnt, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void Stylize()
        {
            _DVMPage.StylizeFlag = true;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Presenter.DrawColor;
            HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }
    }
}
