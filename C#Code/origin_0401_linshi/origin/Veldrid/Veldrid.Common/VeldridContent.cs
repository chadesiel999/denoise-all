using EventBus;
using FontStashSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common
{
    public sealed partial class VeldridContent : IVeldridContent
    {

        private bool sizechanged = false;
        private Boolean first = true;
        private Stopwatch stopwatch = new Stopwatch();
        private GraphicsManger graphicsManger;
        private double fps = 0;
        private VeldridText fpstext;
        private DateTime lastTime;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private Boolean OnlyCompute = false;
        private Int32 minWidth = 10;
        private Int32 minHeight = 10;

        public VeldridContent(Boolean onlyCompute = false)
        {
            OnlyCompute = onlyCompute;
            graphicsManger = new GraphicsManger();
            if (!onlyCompute)
            {
                FontManger.Instance.AddFontDirectory(AppDomain.CurrentDomain.BaseDirectory + "Fonts");
                InitEvent();
                fpstext = new VeldridText(this, true, false);
                fpstext.Visibily = false;
                fpstext.CreateResources();
                fpstext.FontSize = 10;
                fpstext.Color = Color.Black;
                fpstext.BackColor = Color.FromArgb(100, Color.Yellow);
                fpstext.Local = new PointF(graphicsManger.DefaultLineRange.MinX + 20, graphicsManger.DefaultLineRange.MinY + 20);
                fpstext.VerticalAlignment = VerticalAlignment.Bottom;
                fpstext.HorizontalAlignment = HorizontalAlignment.Left;
                lastTime = DateTime.Now;
            }
        }
        public Boolean FPSVisibily { get => fpstext.Visibily; set => fpstext.Visibily = value; }
        GraphicsManger IVeldridContent.GraphicsManger => graphicsManger;
        public double FPS => fps;

        public List<IAxis> Axes { get; } = new List<IAxis>();

        public List<ISeries> Series { get; } = new List<ISeries>();

        public List<ICursor> Cursors { get; } = new List<ICursor>();

        public bool IsInitialized { get; private set; }
        public void Init()
        {
            if (!graphicsManger.IsExists) return;
            if (!IsInitialized)
            {
                System.Threading.Tasks.Task.Run(() => MainRender(), tokenSource.Token);
                IsInitialized = true;
            }
        }
        Object locker = new Object();
        private void MainRender()
        {
            if (OnlyCompute) return;
            while (!tokenSource.IsCancellationRequested)
            {
                if (graphicsManger.Window.Exists && !disposedValue)
                {
                    try
                    {
                        var deltaSeconds = (DateTime.Now - lastTime).TotalMilliseconds / 1000f;
                        lastTime = DateTime.Now;

                        if (sizechanged || first)
                        {
                            if (graphicsManger.Window.Width > minWidth && graphicsManger.Window.Height > minHeight)
                            {
                                lock (locker)
                                {
                                    graphicsManger.SetWindowSize();
                                    Series.ForEach(x => x.WindowSizeState = true);
                                    Axes.ForEach(y => y.WindowSizeState = true);
                                    Cursors.ForEach(x => x.WindowSizeState = true);
                                    Sundries.ForEach(y => y.WindowSizeState = true);
                                    fpstext.WindowSizeState = true;
                                    sizechanged = false;
                                    first = false;

                                    Render();
                                }
                            }
                        }
                        graphicsManger.Camera.Update((float)deltaSeconds, graphicsManger.Window.PumpEvents());
                    }
                    catch (Exception ex)
                    {
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Error));
                    }
                }
                Thread.Sleep(10);
            }
        }

        private List<ICursor> _TempCursors = new List<ICursor>();

        private void Render()
        {
            if (!graphicsManger.IsExists || !IsInitialized || first) return;
            if(graphicsManger.Window.Width <= minWidth || graphicsManger.Window.Height <= minHeight)
            {
                return;
            }
            try
            {
                if (graphicsManger.CommandList.IsDisposed)
                {
                    return;
                }
                stopwatch.Restart();
                graphicsManger.CommandList.Begin();
                graphicsManger.CommandList.SetFramebuffer(graphicsManger.Device.MainSwapchain.Framebuffer);
                graphicsManger.CommandList.ClearColorTarget(0, _backColor);

                // 2024年6月6日  时兴，修改原因：原来Series中的Cursor在Serises绘制的时候就绘制了，导致层级不正确，这里将所有的Cursor统一绘制
                _TempCursors.Clear();
                Series.OrderBy(x => x.ZIndex).ToList().ForEach(x =>
                {
                    x.Draw();
                    if (x.Cursors != null && x.Cursors.Count > 0)
                        _TempCursors.AddRange(x.Cursors);
                });

                Axes.OrderBy(x => x.ZIndex).ToList().ForEach(x => x.Draw());
                //Cursors.OrderBy(x => x.ZIndex).ToList().ForEach(x => x.Draw());
                _TempCursors.AddRange(Cursors);
                _TempCursors.OrderBy(x => x.ZIndex).ToList().ForEach(x => x.Draw());

                Sundries.OrderBy(x => x.ZIndex).ToList().ForEach(y => y.Draw());
                RenderEventHandler?.Invoke(this, EventArgs.Empty);
                fpstext.Draw();
                graphicsManger.CommandList.End();
                graphicsManger.Device.SubmitCommands(graphicsManger.CommandList);
                graphicsManger.Device.SwapBuffers();
                graphicsManger.Device.WaitForIdle();
                stopwatch.Stop();
                if (stopwatch.Elapsed.TotalMilliseconds == 0)
                {
                    fps = 1000;
                }
                else fps = Math.Round(1000 / stopwatch.Elapsed.TotalMilliseconds, 2);
                fpstext.Text = $"Render:{String.Format("{0:0000.00}", fps)}FPS/{String.Format("{0:000.00}", stopwatch.Elapsed.TotalMilliseconds)}ms";
                //fpstext.Text = "AB";
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Error));
            }
        }

        public void DoRender()
        {

            lock (locker)
            {
                Render();
            }
        }
        RgbaFloat _backColor;

        public Color BackColor
        {
            get => backColor;
            set
            {
                backColor = value;
                _backColor = value.ColorConverToRGBA();
            }
        }
        public Size WindowSize
        {
            get => new Size(graphicsManger.Window.Width, graphicsManger.Window.Height);
            set => graphicsManger.Window.SetWindowSize(value.Width, value.Height);
        }

        public List<IRender> Sundries { get; } = new List<IRender>();

        private Boolean disposedValue;
        private Color backColor = Color.Black;

        private void Dispose(bool disposing)
        {
            tokenSource.Cancel();
            // _Locker.Wait();
            lock (locker)
            {
                if (!disposedValue)
                {
                    IsInitialized = false;
                    if (disposing)
                    {
                        this.ClearEventHandle();
                        graphicsManger.Device.WaitForIdle();
                        Axes.ForEach(x => x.Dispose());
                        Series.ForEach(x => x.Dispose());
                        Cursors.ForEach(x => x.Dispose());
                        Sundries.ForEach(x => x.Dispose());
                        fpstext.DisposeResources();
                        graphicsManger.Dispose();
                        // TODO: 释放托管状态(托管对象)
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                    // TODO: 将大型字段设置为 null
                    disposedValue = true;
                }
            }
            //_Locker.Dispose();
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~VeldridContent()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
