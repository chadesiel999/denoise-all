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
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PrintForm : FloatForm, IPrintView
    {
        private readonly PrintPage _SetPrinterPage;

        public PrintForm()
        {
            InitializeComponent();
            _SetPrinterPage = new()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
            };
            Size = new(_SetPrinterPage.Size.Width, _SetPrinterPage.Size.Height + HeadHeight);

            Controls.Add(_SetPrinterPage);
            Controls.SetChildIndex(_SetPrinterPage, 0);
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(PrintForm)));
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


        public PrintPrsnt Presenter
        {
            get;
            set;
        }

        IPrintPrsnt IView<IPrintPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PrintPrsnt)value;
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            _SetPrinterPage.UpdateView(presenter, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        private void Stylize()
        {
            IsShowHelp = false;
            _SetPrinterPage.StylizeFlag = true;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }
    }
}
