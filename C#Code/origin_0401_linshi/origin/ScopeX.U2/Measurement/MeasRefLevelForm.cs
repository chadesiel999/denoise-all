using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.U2.BaseControl;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class MeasRefLevelForm : FlashBorderForm, IMeasView
    {
        private readonly MeasRefLevelPage _RefLevelPage;

        public MeasRefLevelForm(Int32 pxIndex)
        {
            InitializeComponent();

            _RefLevelPage = new(pxIndex)
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };
            Size = new(_RefLevelPage.Size.Width, _RefLevelPage.Size.Height + HeadHeight);

            Controls.Add(_RefLevelPage);
            Controls.SetChildIndex(_RefLevelPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(MeasRefLevelForm)));
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

        IMeasPrsnt IView<IMeasPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (MeasPrsnt)value;
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
            _RefLevelPage.UpdateView(prsnt, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        private void Stylize()
        {
            IsShowHelp = false;
            _RefLevelPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }
    }
}
