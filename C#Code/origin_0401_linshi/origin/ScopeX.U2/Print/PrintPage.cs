using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PrintPage :UserControl, IPrintView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PrintPage()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public PrintPrsnt Presenter
        {
            get => (PrintPrsnt)(ParentForm as IPrintView).Presenter;
            set => (ParentForm as IPrintView).Presenter = value;
        }

        IPrintPrsnt IView<IPrintPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PrintPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Orient):
                    RdoPrintOrient.ChoosedButtonIndex = (Int32)Presenter.Orient;
                    break;
                case nameof(Presenter.PrintColor):
                    RdoColor.ChoosedButtonIndex = (Int32)Presenter.PrintColor;
                    break;
                case nameof(Presenter.PrintArea):
                    RdoPrintArea.ChoosedButtonIndex = (Int32)Presenter.PrintArea;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected async void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                var cts = new CancellationTokenSource();
                BtnPrint.Enabled = false;
                cts.CancelAfter(TimeSpan.FromSeconds(2));
                try
                {
                    bool isPrinterAvailable = await PrintApp.CheckPrinterAsync(cts.Token);
                    if (isPrinterAvailable)
                    {
                        foreach (String printer in PrinterSettings.InstalledPrinters)
                        {
                            CbxChoosePrinter.Items.Add(printer);
                        }
                        BtnPrint.Enabled = true;
                    }
                    else
                    {
                        CbxChoosePrinter.DataSource = "";
                    }
                }
                catch (OperationCanceledException e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e, EventBus.LogLevel.Error));
                }

                CbxChoosePrinter.SelectedItem = PrintApp.PrinterName;
                RdoColor.ChoosedButtonIndex = (Int32)Presenter.PrintColor;
                RdoPrintOrient.ChoosedButtonIndex = (Int32)Presenter.Orient;
                RdoPrintArea.ChoosedButtonIndex = (Int32)Presenter.PrintArea;
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                (Program.Oscilloscope.View as DsoForm).Activate();
                PrintApp.Print();
            }
            catch (Exception ex)
            {
                WeakTip.Default.Write("Print", MsgTipId.PrintPageFailed);
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex.ToString(), EventBus.LogLevel.Warn));
            }
        }

        private void CbxChoosePrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxChoosePrinter.SelectedItem != null)
                PrintApp.PrinterName = CbxChoosePrinter.SelectedItem.ToString();
            else
                PrintApp.PrinterName = "";
        }
        private void RdoPrintOrient_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Orient = (PrintOrient)RdoPrintOrient.ChoosedButtonIndex;
            }
        }

        private void RdoColor_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PrintColor = (PrintColor)RdoColor.ChoosedButtonIndex;
            }
        }

        private void RdoPrintArea_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PrintArea = (PrintSaveArea)RdoPrintArea.ChoosedButtonIndex;
            }
        }
    }
}
