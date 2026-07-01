using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.U2.Tools;

namespace ScopeX.U2
{

    class CustomPrintController :StandardPrintController
    {
        private bool SoftKeyboardIsCLosed = false;
        public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
        {
            SoftKeyboardIsCLosed = false;
            base.OnStartPrint(document, e);
        }

        public override Graphics OnStartPage(PrintDocument document, PrintPageEventArgs e)
        {
            if (Program.Oscilloscope.View is DsoForm df)
            {
                df.Activate();
            }
            SoftKeyboardIsCLosed = true;
            SystemSoftKeyboard.Close();

            return base.OnStartPage(document, e);
        }
        public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
        {
            base.OnEndPage(document, e);
        }
        public override void OnEndPrint(PrintDocument document, PrintEventArgs e)
        {
            base.OnEndPrint(document, e);
            if (!SoftKeyboardIsCLosed)
            {//取消打印需要关闭弹出的软键盘
                SystemSoftKeyboard.Close();
            }
        }
    }

    class PrintApp :IDisposable
    {
        public static PrintApp Default
        {
            get;
            internal set;
        }

        public PrintPrsnt Presenter
        {
            get;
        }

        public PrintApp(PrintPrsnt prsnt)
        {
            Presenter = prsnt;

            _PrintDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            _PrintDoc.EndPrint += PrintDoc_EndPrint;
        }

        private void PrintDoc_EndPrint(object sender, PrintEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }
            WeakTip.Default.Write("Print", MsgTipId.PrintPageSucceeded);
        }

        public PrintForm MakeForm()
        {
            var pf = new PrintForm()
            {
                Presenter = Presenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            pf.Presenter.TryAddView(pf);

            return pf;
        }

        private static CustomPrintController customPrintController = null;
        //设置打印页面格式
        private static readonly PrintDocument _PrintDoc = new()
        {
            PrintController = customPrintController = new CustomPrintController()
        };

        private static readonly PageSettings _PageSettings = new();
        //LocalPrintServer localPrintServer = new LocalPrintServer();

        private static PrinterSettings _PrinterSettings = new()
        {
            PrinterName = "",
        };

        //打印机名称
        public static String PrinterName
        {
            get => _PrinterSettings.PrinterName;
            set
            {
                if (value != null)
                {
                    _PrinterSettings.PrinterName = value;

                }
            }
        }

        public static void Print()
        {
            try
            {
                SystemSoftKeyboard.Show();
                if (Default.Presenter.Orient == PrintOrient.Hor)
                    _PageSettings.Landscape = true;
                else
                    _PageSettings.Landscape = false;
                //var cts = new CancellationTokenSource();
                //cts.CancelAfter(TimeSpan.FromSeconds(1));
                //try
                //{
                //    bool isPrinterAvailable = await PrintApp.CheckPrinterAsync(cts.Token);
                //    if (isPrinterAvailable)
                //    {
                //        PrinterName = PrinterSettings.InstalledPrinters[0];
                //    }
                //    else
                //    {
                //        PrinterName = "";
                //    }

                //}
                //catch (OperationCanceledException e)
                //{
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e, EventBus.LogLevel.Error));
                //}
                _PrinterSettings = new() { PrinterName = PrinterName };
                _PrintDoc.PrinterSettings = _PrinterSettings;
                _PrintDoc.DefaultPageSettings = _PageSettings;

                _PrintDoc.Print();

            }
            catch (Exception e)
            {
                WeakTip.Default.Write(nameof(Print), MsgTipId.Undefined);
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e, EventBus.LogLevel.Error));
            }
        }

        public static Task<bool> CheckPrinterAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(500);

                return PrinterSettings.InstalledPrinters.Count > 0;
            }, cancellationToken);
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            //SystemSoftKeyboard.Close();
            Thread.Sleep(200);
            Image screenimage;
            var form = Program.Oscilloscope.View as DsoForm;

            //if (Presenter.Function == PrintFunction.Screen)
            //{
            if (Presenter.PrintArea == PrintSaveArea.Screen)
            {
                //screenimage = new Bitmap(form.ClientRectangle.Width, form.ClientRectangle.Height);
                //form.DrawToBitmap((Bitmap)screenimage, form.ClientRectangle);

                screenimage = new Bitmap(form.Bounds.Width, form.Bounds.Height);
                using Graphics g = Graphics.FromImage(screenimage);
                g.CopyFromScreen(form.Bounds.Location, new Point(0, 0), form.Bounds.Size);
            }
            else
            {
                //Control wfmctrl = form.Controls["TableLayoutLv1"].Controls["TlpWaveFormRoot"];
                //screenimage = new Bitmap(wfmctrl.Width, wfmctrl.Height);
                //wfmctrl.DrawToBitmap((Bitmap)screenimage, wfmctrl.Bounds);

                var wfmctrl = form.Controls["TlpMain"].Controls["WindowDockPanel"];
                screenimage = new Bitmap(wfmctrl.Bounds.Width, wfmctrl.Bounds.Height);
                using Graphics g = Graphics.FromImage(screenimage);
                g.CopyFromScreen(wfmctrl.Bounds.Location, new Point(0, 0), wfmctrl.Bounds.Size);
            }

            //}
            //else
            //{
            //    using var imagestream = FileToStream(Presenter.FilePath);
            //    screenimage = Image.FromStream(imagestream);
            //}

            switch (Presenter.PrintColor)
            {
                case PrintColor.BlackWhite:
                    screenimage = RgbToGray(screenimage, false);
                    break;

                case PrintColor.Reverse:
                    screenimage = RgbToGray(screenimage, true);
                    break;
            }

            Int32 height = screenimage.Height;
            Int32 width = screenimage.Width;
            //约束图像的最大情况
            if (Default.Presenter.Orient == PrintOrient.Ver)//纵向
            {
                if (height > _PageSettings.PrintableArea.Height)
                    height = (Int32)_PageSettings.PrintableArea.Height;
                if (width > _PageSettings.PrintableArea.Width)
                    width = (Int32)_PageSettings.PrintableArea.Width;
                Graphics g = ev.Graphics;
                g.DrawImage(screenimage, new Rectangle((Int32)((_PageSettings.PrintableArea.Width - width) / 2), (Int32)((_PageSettings.PrintableArea.Height - height) / 2), width, height));

            }
            else//横向
            {
                if (height > _PageSettings.PrintableArea.Width)
                    height = (Int32)_PageSettings.PrintableArea.Width;
                if (width > _PageSettings.PrintableArea.Height)
                    width = (Int32)_PageSettings.PrintableArea.Height;
                Graphics g = ev.Graphics;
                g.DrawImage(screenimage, new Rectangle((Int32)((_PageSettings.PrintableArea.Height - width) / 2), (Int32)((_PageSettings.PrintableArea.Width - height) / 2), width, height));

            }

            screenimage.Dispose();
            ev.HasMorePages = false;
        }

        private static Image RgbToGray(Image img, Boolean inverted)
        {
            try
            {
                Bitmap newbmp = new(img.Width, img.Height);
                Bitmap oldbmp = (Bitmap)img;
                Color pixel;
                for (Int32 x = 0; x < img.Width; x++)
                    for (Int32 y = 0; y < img.Height; y++)
                    {
                        pixel = oldbmp.GetPixel(x, y);
                        Int32 rr, gg, bb, result = 0;
                        rr = pixel.R;
                        gg = pixel.G;
                        bb = pixel.B;
                        result = ((Int32)(0.7 * rr) + (Int32)(0.2 * gg) + (Int32)(0.1 * bb));
                        if (inverted)
                            result = 255 - result;
                        newbmp.SetPixel(x, y, Color.FromArgb(result, result, result));
                    }
                img = newbmp;
                oldbmp.Dispose();
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Debug));
            }

            return img;
        }

        private static Stream FileToStream(string fileName)
        {
            FileStream filestream = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[filestream.Length];
            filestream.Read(bytes, 0, bytes.Length);
            filestream.Close();
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        public void Dispose()
        {
            _PrintDoc.Dispose();
        }
    }
}
