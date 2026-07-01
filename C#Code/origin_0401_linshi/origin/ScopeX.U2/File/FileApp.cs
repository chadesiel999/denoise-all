// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/19</date>

namespace ScopeX.U2
{
    using PdfSharpCore;
    using PdfSharpCore.Drawing;
    using PdfSharpCore.Pdf;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.U2.PassFail;
    using System.Threading;

    public enum ScreenshotFormType
    {
        MainWindow = 0,
        PassFailInfo = 1
    }
    /// <summary>
    /// Defines the <see cref="FileApp" />.
    /// </summary>
    public class FileApp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileApp"/> class.
        /// </summary>
        /// <param name="prsnt">The prsnt<see cref="FilePrsnt"/>.</param>
        public FileApp(FilePrsnt prsnt)
        {
            FilePrsnt.GetImageStreamHandler = GetImageStream;
            FilePrsnt.SaveLabNoteBookHandler = SaveLabNoteBook;
            Presenter = prsnt;
        }

        /// <summary>
        /// Gets or sets the Default.
        /// </summary>
        public static FileApp Default { get; internal set; }

        /// <summary>
        /// Gets the Presenter.
        /// </summary>
        public FilePrsnt Presenter { get; }

        /// <summary>
        /// The GetImage.
        /// </summary>
        /// <param name="region">The region<see cref="PicArea"/>.</param>
        /// <returns>The <see cref="Bitmap"/>.</returns>
        public static Bitmap GetImage(PicArea region = PicArea.FullScreen, PicColor color = PicColor.Standard, Boolean needCloseWeakTip = false)
        {
            try
            {
                Bitmap img = null;
                if (Program.Oscilloscope.View is not DsoForm form)
                {
                    return null;
                }

                if (region == PicArea.FullScreen)
                {
                    //img = new(form.ClientRectangle.Width, form.ClientRectangle.Height);
                    form?.Invoke(new Action(() =>
                    {
                        img = CaptureControl(form);
                        //form.DrawToBitmap(img, form.ClientRectangle);
                    }));
                }
                else if (region == PicArea.Application)
                {
                    if (needCloseWeakTip)
                    {
                        //截屏双重保险确保截屏不显示弱提示
                        WeakTip.Default.Close();
                        form!.CloseWeakTipForm(); 
                    }
                    if (form.WindowState == FormWindowState.Minimized)
                    {
                        return null;
                    }
                    img = new Bitmap(form.Bounds.Width, form.Bounds.Height);
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.CopyFromScreen(form.Bounds.Location, new Point(0, 0), form.Bounds.Size);
                    }
                }
                else
                {
                    var ctrl = form.Controls["TlpMain"].Controls["WindowDockPanel"];

                    ctrl?.Invoke(() => ctrl?.Refresh());// 确保控件已经完成重绘
                    ctrl?.Invoke(() => ctrl?.SuspendLayout()); //暂停控件布局调整，确保稳定性
                    try
                    {
                        ctrl?.Invoke(new Action(() =>
                        {
                            img = CaptureControlEx(ctrl);
                        }));
                    }
                    catch (Exception ex)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to get screen image: " + ex.ToString(), EventBus.LogLevel.Error));
                    }
                    finally
                    {
                        ctrl?.Invoke(() => ctrl?.ResumeLayout());
                    }
                }
                if (img == null)
                    return null;
                switch (color)
                {
                    case PicColor.Standard:
                        break;
                    case PicColor.BlackWhite:
                        img = RgbToGray(img, false);
                        break;
                    case PicColor.Reverse:
                        img = RgbToGray(img, true);
                        break;
                    default:
                        break;
                }

                return img;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to get screen image: " + e.ToString(), EventBus.LogLevel.Error));
            }

            return null;
        }

        /// <summary>
        /// Add Date Time Info to Image
        /// </summary>
        /// <param name="bitmap">Image entity</param>
        /// <param name="date">date time</param>
        /// <param name="x">X-axis position</param>
        /// <param name="y">Y-axis position</param>
        public static void AddDateTime2Image(Bitmap bitmap, DateTime date)
        {
            if (bitmap == null)
                return;
            using (Font font = new Font("Arial", 22))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, 170, 170, 170)))
            using (Graphics graphic = Graphics.FromImage(bitmap))
            {
                string text = date.ToString("yyyy-MM-dd HH:mm:ss");
                //PointF point = new PointF(x, y);

                SizeF textSize = graphic.MeasureString(text, font);
                float angle = 0;//-45; // 倾斜45度角
                float margin_right = 50;
                float margin_bottom = 5;
                PointF location = new PointF(bitmap.Width - textSize.Width - margin_right, bitmap.Height - textSize.Height - margin_bottom);
                graphic.TranslateTransform(location.X + textSize.Width / 2, location.Y + textSize.Height / 2);
                graphic.RotateTransform(angle);
                graphic.TranslateTransform(-location.X - textSize.Width / 2, -location.Y - textSize.Height / 2);
                graphic.DrawString(text, font, brush, location);
                graphic.ResetTransform();

                // graphic.DrawString(text, font, brush, point);
            }
        }

        private static Bitmap RgbToGray(Bitmap img, Boolean inverted)
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

        //public static Boolean SaveImage(String file, PicFormat pf, PicArea region)
        //{
        //    try
        //    {
        //        Bitmap img;
        //        if (Program.Oscilloscope.View is not DsoForm form)
        //        {
        //            return false;
        //        }

        //        if (region == PicArea.FullScreen)
        //        {
        //            //img = new Bitmap(form.Bounds.Width, form.Bounds.Height);
        //            //using Graphics g = Graphics.FromImage(img);
        //            //g.CopyFromScreen(form.Bounds.Location, new Point(0, 0), form.Bounds.Size);

        //            img = new(form.ClientRectangle.Width, form.ClientRectangle.Height);
        //            form.DrawToBitmap(img, form.ClientRectangle);
        //        }
        //        else
        //        {
        //            var ctrl = form.Controls["TableLayoutLv1"].Controls["TlpWaveFormRoot"];
        //            img = new(ctrl.ClientRectangle.Width, ctrl.ClientRectangle.Height);
        //            ctrl.DrawToBitmap(img, ctrl.ClientRectangle);
        //        }

        //        img.Save(file, GetImageFormat(pf));
        //        img.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error("Saving Picture fails: " + e.ToString());
        //        return false;
        //    }

        //    return true;
        //}

        public static MemoryStream GetImageStream(PicFormat pf, PicArea region, PicColor color,Boolean needCloseWeakTip=false)
        {
            var img = GetImage(region, color, needCloseWeakTip);
            if (img is null)
            {
                return null;
            }

            try
            {
                if (FilePrsnt.IsTimestamp)
                {
                    AddDateTime2Image(img, DateTime.Now);
                }
                //AddDateTime2Image(img, DateTime.Now);
                MemoryStream ms = new();
                img.Save(ms, GetImageFormat(pf));
                img.Dispose();
                return ms;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Failed to convert image to memory stream: " + e.ToString(), EventBus.LogLevel.Error));
            }

            return null;
        }

        public FileForm MakeForm()
        {
            var ff = new FileForm(Presenter)
            {
                Presenter = Presenter,
                Anchor = AnchorStyles.Top,
            };
            ff.Presenter.TryAddView(ff);

            return ff;
        }

        private static System.Drawing.Imaging.ImageFormat GetImageFormat(PicFormat pf) => pf switch
        {
            PicFormat.Gif => System.Drawing.Imaging.ImageFormat.Gif,
            PicFormat.Jpeg => System.Drawing.Imaging.ImageFormat.Jpeg,
            PicFormat.Png => System.Drawing.Imaging.ImageFormat.Png,
            PicFormat.Tiff => System.Drawing.Imaging.ImageFormat.Tiff,
            _ => System.Drawing.Imaging.ImageFormat.Bmp,
        };

        private static Bitmap CaptureControl(Control control)
        {
            IntPtr hsrce = NativeMethods.GetWindowDC(control.Handle);
            IntPtr hdest = NativeMethods.CreateCompatibleDC(hsrce);
            IntPtr hbmp = NativeMethods.CreateCompatibleBitmap(hsrce, control.Width, control.Height);
            IntPtr holdbmp = NativeMethods.SelectObject(hdest, hbmp);
            if (NativeMethods.BitBlt(hdest, 0, 0, control.Width, control.Height, hsrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt))
            {
                Bitmap bmp = Image.FromHbitmap(hbmp);
                NativeMethods.SelectObject(hdest, holdbmp);
                NativeMethods.DeleteObject(hbmp);
                NativeMethods.DeleteDC(hdest);
                NativeMethods.ReleaseDC(control.Handle, hsrce);
                return bmp;
            }
            return null;
        }

        private static Bitmap CaptureControlEx(Control control)
        {
            Int32 retrycount = 0;
            Int32 maxetries = 1;  // 允许的最大重试次数
            while (retrycount < maxetries)
            {
                IntPtr hsrce = NativeMethods.GetWindowDC(control.Handle);
                IntPtr hdest = NativeMethods.CreateCompatibleDC(hsrce);
                IntPtr hbmp = NativeMethods.CreateCompatibleBitmap(hsrce, control.Width, control.Height);
                IntPtr holdbmp = NativeMethods.SelectObject(hdest, hbmp);

                if (NativeMethods.BitBlt(hdest, 0, 0, control.Width, control.Height, hsrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt))
                {
                    Bitmap bmp = Image.FromHbitmap(hbmp);

                    // 清理资源
                    NativeMethods.SelectObject(hdest, holdbmp);
                    NativeMethods.DeleteObject(hbmp);
                    NativeMethods.DeleteDC(hdest);
                    NativeMethods.ReleaseDC(control.Handle, hsrce);

                    // 检查图像尺寸是否符合预期
                    if (CheckImageSize(bmp))
                    {
                        return bmp;  // 尺寸正确，返回图像
                    }
                }
                else
                {
                    // 如果截图失败，清理资源
                    NativeMethods.SelectObject(hdest, holdbmp);
                    NativeMethods.DeleteObject(hbmp);
                    NativeMethods.DeleteDC(hdest);
                    NativeMethods.ReleaseDC(control.Handle, hsrce);
                }

                retrycount++;  // 增加重试计数
            }

            return null;  // 返回最后一次截图的图像（即使不符合预期，也避免无限循环）
        }


        private static Boolean CheckImageSize(Bitmap image)
        {
            if (Screen.AllScreens.Length >= 1)
            {

                //get screen resolution
                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;
                return !(image.Width < screenWidth - 100);
            }
            return true;
        }

        public static Bitmap Screenshot(ScreenshotFormType screenshotFormType)
        {
            try
            {
                Bitmap img = null;
                if (screenshotFormType == ScreenshotFormType.MainWindow)
                {
                    var form = Program.Oscilloscope.View as DsoForm;
                    var ctrl = form.Controls["TlpMain"].Controls["WindowDockPanel"];
                    ctrl?.Invoke(new Action(() =>
                    {
                        img = CaptureControl(ctrl);
                    }));
                }
                //else if (screenshotFormType == ScreenshotFormType.PassFailInfo)
                //{
                //    img = new(PassFailApp.Default.InfoForm.ClientRectangle.Width, PassFailApp.Default.InfoForm.ClientRectangle.Height);

                //    PassFailApp.Default.InfoForm?.Invoke(new Action(() =>
                //    {
                //        PassFailApp.Default.InfoForm.Refresh();
                //        PassFailApp.Default.InfoForm.DrawToBitmap(img, PassFailApp.Default.InfoForm.ClientRectangle);
                //    }));
                //}
                else if (screenshotFormType == ScreenshotFormType.PassFailInfo)
                {
                    var info = PassFailApp.Default.InfoControl?.FindForm();
                    if (info is not null)
                    {
                        img = new(info.ClientRectangle.Width, info.ClientRectangle.Height);

                        info.Invoke(new Action(() =>
                        {
                            info.Refresh();
                            info.DrawToBitmap(img, info.ClientRectangle);
                        }));
                    }
                }
                else
                {
                    return null;
                }

                return img;
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Saving Picture fails: " + e.ToString(), EventBus.LogLevel.Error));
                return null;
            }
        }

        public static Boolean SaveLabNoteBook(String LabNotePath, String LabNoteName, ChannelId channelId)
        {
            if (Program.Oscilloscope.TryGetChannel(channelId, out IChnlPrsnt channel))
            {
                AnalogPrsnt analogmodel = channel as AnalogPrsnt;

                String path = LabNotePath + "\\" + LabNoteName + String.Format("{0:0000}", DateTime.Now.Year) + String.Format("{0:00}", DateTime.Now.Month)
                 + String.Format("{0:00}", DateTime.Now.Day) + String.Format("{0:00}", DateTime.Now.Hour) + String.Format("{0:00}", DateTime.Now.Minute)
                 + String.Format("{0:00}", DateTime.Now.Second) + String.Format("{0:000}", DateTime.Now.Millisecond) + ".pdf";

                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                page.Size = PageSize.A4;
                page.TrimMargins.Left = 10;
                page.TrimMargins.Right = 10;
                page.TrimMargins.Top = 30;
                page.TrimMargins.Bottom = 30;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                Double ypos = 0;
                Double fontheight = 15;
                XFont font = new XFont("Times New Roman", fontheight, XFontStyle.Regular);

                gfx.DrawString("User:ScopeXUser", font, XBrushes.Black, new XPoint(0, ypos), XStringFormats.TopLeft);
                ypos = ypos + fontheight + fontheight / 2;

                gfx.DrawString("Time: " + DateTime.Now.ToString(), font, XBrushes.Black, new XPoint(0, ypos), XStringFormats.TopLeft);
                ypos = ypos + fontheight * 2;

                Bitmap bitmap = FileApp.Screenshot(ScreenshotFormType.PassFailInfo);
                if (bitmap != null)
                {
                    String filename = LabNotePath + "\\" + Guid.NewGuid().ToString() + "temp.bmp";
                    bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                    gfx.DrawImage(XImage.FromFile(filename), 0, ypos, bitmap.Width, bitmap.Height);
                    ypos = ypos + bitmap.Height + fontheight * 2;
                    bitmap.Dispose();

                    if (!FilePrsnt.DeleteFile(filename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return false;
                    }
                }

                bitmap = FileApp.Screenshot(ScreenshotFormType.MainWindow);
                if (bitmap != null)
                {
                    String filename = LabNotePath + "\\" + Guid.NewGuid().ToString() + "temp.bmp";
                    bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
                    gfx.DrawImage(XImage.FromFile(filename), 0, ypos, page.Width - 20, bitmap.Height * ((page.Width - 20) / bitmap.Width));
                    ypos = ypos + bitmap.Height * ((page.Width - 20) / bitmap.Width) + fontheight * 2;
                    bitmap.Dispose();
                    if (!FilePrsnt.DeleteFile(filename))
                    {
                        WeakTip.Default.Write("File", MsgTipId.FileOccupied);
                        return false;
                    }
                }

                Double perheight = 20;

                if (ypos + perheight * 8 > page.Height)
                {
                    gfx.Dispose();

                    page = document.AddPage();
                    page.Size = PageSize.A4;
                    page.TrimMargins.Left = 10;
                    page.TrimMargins.Right = 10;
                    page.TrimMargins.Top = 30;
                    page.TrimMargins.Bottom = 30;
                    gfx = XGraphics.FromPdfPage(page);

                    ypos = 0;
                }
                gfx.DrawString("Channel Status", font, XBrushes.Black, new XPoint(0, ypos), XStringFormats.TopLeft);
                ypos = ypos + fontheight * 2;

                Int32 tablerow = 6;
                Int32 tablecol = 3;
                Double perwidth = page.Width / tablecol;

                gfx.DrawRectangle(XBrushes.Gray, new XRect(0, ypos, perwidth, perheight * tablerow));
                gfx.DrawRectangle(XBrushes.LightGray, new XRect(perwidth, ypos, page.Width - perwidth, perheight));
                for (int i = 1; i < tablerow; i++)
                {
                    if (i % 2 == 1)
                        gfx.DrawRectangle(XBrushes.Orange, new XRect(perwidth, ypos + perheight * i, page.Width - perwidth, perheight));
                    else
                        gfx.DrawRectangle(XBrushes.White, new XRect(perwidth, ypos + perheight * i, page.Width - perwidth, perheight));
                }

                gfx.DrawString("Vertical", font, XBrushes.Black, new XPoint(0, ypos + perheight * tablerow / 2 - fontheight / 2), XStringFormats.TopLeft);

                gfx.DrawString("V/Div", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight), XStringFormats.TopLeft);
                gfx.DrawString("Offset", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight * 2), XStringFormats.TopLeft);
                gfx.DrawString("Coupling", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight * 3), XStringFormats.TopLeft);
                gfx.DrawString("BW-Limit", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight * 4), XStringFormats.TopLeft);
                gfx.DrawString("Probe", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight * 5), XStringFormats.TopLeft);

                gfx.DrawString(channelId.ToString(), font, XBrushes.Black, new XPoint(perwidth + perwidth, ypos), XStringFormats.TopLeft);
                gfx.DrawString(new Quantity(analogmodel.ScaleBymV, analogmodel.Prefix, analogmodel.Unit).ToString(),
                   font, XBrushes.Black, new XPoint(perwidth + perwidth, ypos + perheight), XStringFormats.TopLeft);
                gfx.DrawString(analogmodel.Pack.Offset.ToString(), font, XBrushes.Black, new XPoint(perwidth + perwidth, ypos + perheight * 2), XStringFormats.TopLeft);
                gfx.DrawString(analogmodel.Coupling.ToString(), font, XBrushes.Black, new XPoint(perwidth + perwidth, ypos + perheight * 3), XStringFormats.TopLeft);

                String bandwidthstring = String.Empty;
                //if (analogmodel.Bandwidth == 0)
                //{
                //    bandwidthstring = Constants.BANDWIDTH_LV0_NAME;
                //}
                //else if(analogmodel.Bandwidth == 1)
                //{
                //    bandwidthstring = Constants.BANDWIDTH_LV1_NAME;
                //}
                //else if (analogmodel.Bandwidth == 2)
                //{
                //    bandwidthstring = Constants.BANDWIDTH_LV2_NAME;
                //}
                //else
                //{
                //    bandwidthstring = "";
                //}

                gfx.DrawString(bandwidthstring, font, XBrushes.Black, new XPoint(perwidth + perwidth, ypos + perheight * 4), XStringFormats.TopLeft);
                gfx.DrawString(analogmodel.ProbeIndex.ToString(), font, XBrushes.Black, new XPoint(perwidth + perwidth, ypos + perheight * 5), XStringFormats.TopLeft);

                ypos = ypos + perheight * tablerow + fontheight * 2;
                if (ypos + perheight * 6 > page.Height)
                {
                    gfx.Dispose();

                    page = document.AddPage();
                    page.Size = PageSize.A4;
                    page.TrimMargins.Left = 10;
                    page.TrimMargins.Right = 10;
                    page.TrimMargins.Top = 30;
                    page.TrimMargins.Bottom = 30;
                    gfx = XGraphics.FromPdfPage(page);

                    ypos = 0;
                }
                gfx.DrawString("Acquisition Status", font, XBrushes.Black, new XPoint(0, ypos), XStringFormats.TopLeft);
                ypos = ypos + fontheight * 2;

                tablerow = 4;
                tablecol = 5;
                perwidth = page.Width / tablecol;

                gfx.DrawRectangle(XBrushes.Gray, new XRect(0, ypos, perwidth, perheight * tablerow));
                gfx.DrawRectangle(XBrushes.Orange, new XRect(perwidth, ypos + 0, page.Width - perwidth, perheight));
                gfx.DrawRectangle(XBrushes.White, new XRect(perwidth, ypos + perheight, page.Width - perwidth, perheight));
                gfx.DrawRectangle(XBrushes.Orange, new XRect(perwidth, ypos + perheight * 2, page.Width - perwidth, perheight));
                gfx.DrawRectangle(XBrushes.White, new XRect(perwidth, ypos + perheight * 3, page.Width - perwidth, perheight));

                gfx.DrawString("Horizontal", font, XBrushes.Black, new XPoint(0, ypos + perheight - fontheight / 2), XStringFormats.TopLeft);
                gfx.DrawString("Trigger", font, XBrushes.Black, new XPoint(0, ypos + perheight * 3 - fontheight / 2), XStringFormats.TopLeft);

                gfx.DrawString("Time/Div", font, XBrushes.Black, new XPoint(perwidth, ypos), XStringFormats.TopLeft);
                gfx.DrawString("Trigger Delay", font, XBrushes.Black, new XPoint(perwidth * 3, ypos), XStringFormats.TopLeft);
                gfx.DrawString("Sampling Rate", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight), XStringFormats.TopLeft);
                gfx.DrawString("Sample Mode", font, XBrushes.Black, new XPoint(perwidth * 3, ypos + perheight), XStringFormats.TopLeft);
                gfx.DrawString("Mode", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight * 2), XStringFormats.TopLeft);
                gfx.DrawString("Source", font, XBrushes.Black, new XPoint(perwidth * 3, ypos + perheight * 2), XStringFormats.TopLeft);
                gfx.DrawString("Type", font, XBrushes.Black, new XPoint(perwidth, ypos + perheight * 3), XStringFormats.TopLeft);
                gfx.DrawString("Coupling", font, XBrushes.Black, new XPoint(perwidth * 3, ypos + perheight * 3), XStringFormats.TopLeft);

                gfx.DrawString(new Quantity(analogmodel.Sampling.ScaleByus, analogmodel.Sampling.Prefix, analogmodel.Sampling.Unit).ToString(),
                   font, XBrushes.Black, new XPoint(perwidth * 2, ypos), XStringFormats.TopLeft);
                gfx.DrawString(new Quantity(analogmodel.Sampling.PositionByus, analogmodel.Sampling.Prefix, analogmodel.Sampling.Unit).ToString("##0.######", true),
                    font, XBrushes.Black, new XPoint(perwidth * 4, ypos), XStringFormats.TopLeft);
                if (analogmodel.VuDatabase?.Current != null)
                    gfx.DrawString(analogmodel.VuDatabase?.Current.ZoomRatio.ToString("##0.######"), font, XBrushes.Black, new XPoint(perwidth * 2, ypos + perheight), XStringFormats.TopLeft);
                gfx.DrawString(analogmodel.Sampling.Mode.ToString(), font, XBrushes.Black, new XPoint(perwidth * 4, ypos + perheight), XStringFormats.TopLeft);
                gfx.DrawString(TriggerPrsnt.Mode.ToString(), font, XBrushes.Black, new XPoint(perwidth * 2, ypos + perheight * 2), XStringFormats.TopLeft);

                ChannelId channelid = ChannelId.C1;
                if (Program.Oscilloscope.CurrentTrigger is TrigSingleSrcPrsnt singletrgp)
                {
                    channelid = singletrgp.Source!.Value;
                }
                else if (Program.Oscilloscope.CurrentTrigger is TrigMultiLevelPrsnt trgp)
                {
                    channelid = trgp.Source;
                }
                gfx.DrawString(channelid.ToString(), font, XBrushes.Black, new XPoint(perwidth * 4, ypos + perheight * 2), XStringFormats.TopLeft);
                gfx.DrawString(TriggerPrsnt.Type.ToString(), font, XBrushes.Black, new XPoint(perwidth * 2, ypos + perheight * 3), XStringFormats.TopLeft);
                if (TriggerPrsnt.Type == TriggerType.Edge)
                {
                    if (Program.Oscilloscope.CurrentTrigger is TrigEdgePrsnt trgp)
                        gfx.DrawString(trgp.Coupling.ToString(), font, XBrushes.Black, new XPoint(perwidth * 4, ypos + perheight * 3), XStringFormats.TopLeft);
                }
                document.Save(path);
                return true;
            }

            return false;
        }
    }
}
