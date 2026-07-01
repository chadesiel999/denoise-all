using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Presenter.RadioFrequency;
using ScopeX.Core.Tools;
using WeifenLuo.WinFormsUI.Docking;

namespace ScopeX.U2
{
    public sealed class MultiWindowManager : IDisposable
    {
        public Int32 RenderInterval { get; set; } = 35;

        private readonly Object _Object = new();

        private static readonly Int32 _MinLength = 1;

        private static readonly Int32 _MaxLength = Constants.MAX_FIGURE_NUM;

        private readonly ConcurrentDictionary<Int64, BaseDisplayForm> _FigureContainer = new();



        private Thread _RenderThread = null;

        private volatile Boolean _IsRunning = true;

        public IWaveformFigure MainFigure
        {
            get;
            private set;
        }

        private DsoPrsnt DsoPrsnt
        {
            get;
        }

        private DockPanel DockPanel
        {
            get;
        }

        public MultiWindowManager(DsoPrsnt dso, DockPanel dockPanel)
        {
            DsoPrsnt = dso;

            DockPanel = dockPanel;

            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            if (_RenderThread == null)
            {
                _RenderThread = new Thread(DrawWaveform)
                {
                    IsBackground = true,
                    Name = "DrawWaveThread",
                    Priority = ThreadPriority.Highest
                };
                _RenderThread.Start();
            }

            AddMainFig(dso);
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            foreach (var item in _FigureContainer)
            {
                if (item.Value == null)
                    continue;
                String temp = String.Empty;
                List<String> lanugages = new List<String>();
                if (item.Value.TitleLanugageIDs != null)
                {
                    foreach (var ids in item.Value.TitleLanugageIDs)
                    {
                        lanugages.Add(LanguageManger.Instance.GetIDMessage(ids));
                    }
                }
                

                if(!String.IsNullOrWhiteSpace(item.Value.ExtTitle))
                {
                    temp = String.Format(item.Value.ExtTitle, lanugages.ToArray());
                }
                else
                {
                    temp = String.Concat(lanugages);
                }
                //string temp = $"{item.Value.ExtTitle}{LanguageManger.Instance.GetIDMessage(item.Value.TitleLanugageID)}";
                if (!string.IsNullOrEmpty(temp))
                    item.Value.Title = temp;
            }
        }

        public void SetDrawEnable(Boolean Enable)
        {
            List<BaseDisplayForm> forms = null;
            lock (_Object)
            {
                forms = _FigureContainer.Select(o => o.Value).ToList();
                forms.ForEach(fm => fm.IsRender = Enable);
            }
        }

        private void DrawWaveform(Object obj)
        {
            TimeSpan span;
            while (_IsRunning)
            {
                List<BaseDisplayForm> forms = null;
                lock (_Object)
                {
                    forms = _FigureContainer.Select(o => o.Value).ToList();
                }
                //DateTime dt = DateTime.Now;
                span = TimeSpanUtility.GetTimestampSpan();

                List<Task> rendertasks = new(_MaxLength);
                foreach (var f in forms)
                {
                    if (f is IWaveformFigure fig)
                    {
                        rendertasks.Add(Task.Run(() => fig.Render()));
                    }
                }
                Task.WaitAll(rendertasks.ToArray());

                Double elapse = (TimeSpanUtility.GetTimestampSpan() - span).TotalMilliseconds;
                elapse = elapse < 0 ? 0 : elapse;

                if (elapse < RenderInterval)
                {
                    Task.Delay((Int32)(RenderInterval - elapse)).Wait();
                }
            }
        }

        public static PlotDrawType GetPlotDrawTypeBy(ChannelId cid)
        {
            if (cid.IsAnalog())
            {
                return PlotDrawType.Analog;
            }
            else if (cid.IsMath())
            {
                return PlotDrawType.Math;
            }
            else if (cid.IsDigital())
            {
                return PlotDrawType.Digital;
            }
            else if (cid.IsDecode())
            {
                return PlotDrawType.Decode;
            }
            else if (cid.IsReference())
            {
                return PlotDrawType.Ref;
            }
            else if (cid.IsRadioFrequency())
            {
                return PlotDrawType.RadioFrequency;
            }
            else
            {
                return cid.IsAmpVSTime()
                    ? PlotDrawType.AmpVSTime
                    : cid.IsPhaseVSFrequency()
                                    ? PlotDrawType.PhaseVSFrequency
                                    : cid.IsTimeVSFrequency()
                                                    ? PlotDrawType.TimeVSFrequency
                                                    : cid.IsFrequencyVSTime()
                                                                    ? PlotDrawType.FrequencyVSTime
                                                                    : cid.IsPhaseVSTime() ? PlotDrawType.PhaseVSTime : PlotDrawType.Analog;
            }
        }

        private void AddMainFig(DsoPrsnt dso)
        {
            if (MainFigure is null)
            {
                string mainFigText = LanguageManger.Instance.GetIDMessage("MainFigText");

                IWaveformFigure fig = Constants.RENDERINGMODE switch
                {
                    RenderingMode.GPU => new WaveformGPUFigure(true)
                    {
                        //Text = Properties.Resources.MainFigText,
                        Text = mainFigText,
                        TitleLanugageIDs = new List<String>() { "MainFigText"},
                        Margin = new Padding(0),
                    },
                    _ => new WaveformFigure(true)
                    {
                        //Text = Properties.Resources.MainFigText,
                        Text = mainFigText,
                        TitleLanugageIDs = new List<String>() { "MainFigText" },
                        Margin = new Padding(0),
                        DispPresenter = dso.Display,
                        VisibleCursorBox = true,
                    },
                };
                fig.AddWave(PlotDrawType.TimeBase, dso.Timebase);
                fig.AddWave(PlotDrawType.Trigger);
                fig.AddWave(PlotDrawType.Display, dso.Display);
                fig.AddWave(PlotDrawType.Cursor, dso.Cursor);
                fig.AddWave(PlotDrawType.PassFail, dso.PassFail);
                fig.AddWave(PlotDrawType.SearchMarker, dso.Search);
                fig.AddWave(PlotDrawType.Rectangle);

                AddWindow((BaseDisplayForm)fig);

                MainFigure = fig;
            }
            else
            {
                throw new Exception("Main figure has already benn made.");
            }

        }

        /// <summary>
        /// 是否当前进行标记
        /// </summary>
        /// <param name="form"></param>
        private MarkerItemPrsnt GetMarkerSource(BaseDisplayForm form)
        {
            var isreplace = false;
            //两种情况
            var id = Program.Oscilloscope.TryGetRange(c => c.Active && c.Id.IsMath() && form.Text.Contains(c.Id.ToString())).Last()?.Id;
            if (id != null && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id.Value, out var chprsnt) && chprsnt is MathPrsnt math && math.Args is MathFftArg fft) //首先检查是否当前待添加窗口为FFT
            {
                fft.Marker = DsoPrsnt.DefaultDsoPrsnt.Markers[id.Value];
                return fft.Marker;
            }
            var prsnt = DsoPrsnt.Markers[id.Value];
            prsnt.AtuoMarkerActive = false;
            return prsnt;
        }

        public Boolean AddWindow(BaseDisplayForm form)
        {
            lock (_Object)
            {
                if (_FigureContainer.Count >= _MaxLength)
                {
                    return false;
                }
                if (form.WindowId == null)
                {
                    return false;
                }
                if (_FigureContainer.ContainsKey(form.WindowId.Value))
                {
                    _FigureContainer[form.WindowId.Value] = form;
                }
                else
                {
                    _FigureContainer.TryAdd(form.WindowId.Value, form);
                }
            }

            form.Show(DockPanel);
            form.DockTo(DockPanel, DockStyle.Fill);
            return true;
        }

        public Boolean RemoveWindow(BaseDisplayForm fig)
        {
            lock (_Object)
            {
                if (_FigureContainer.Count <= _MinLength)
                {
                    return false;
                }

                //if(fig==null||fig.IsDisposed)
                //{
                //    return false;
                //}

                if (fig.IsMainForm && fig.CanClosed == false)
                {
                    return false;
                }

                if (_FigureContainer.TryRemove(fig.WindowId.Value, out _))
                {
                    fig?.Close();
                    fig?.Dispose();
                    //fig.IsRemoveBadge = true;
                    return true;
                }
            }
            return false;
        }

        public void RemoveAllWindows()
        {
            while (_FigureContainer.Count > _MinLength)
            {
                var fig = _FigureContainer.FirstOrDefault(o =>
                    o.Value is not IWaveformFigure f || !f.IsMainForm).Value;
                RemoveWindow(fig);
            }
        }

        public BaseDisplayForm GetWindow(Int64? wid)
        {
            if (!wid.HasValue)
            {
                return null;
            }

            lock (_Object)
            {
                if (_FigureContainer.TryGetValue(wid.Value, out var form))
                {
                    return form;
                }
            }
            return null;
        }

        public IWaveformFigure GetMainWindow()
        {
            return MainFigure ?? throw new NullReferenceException("Main figure does not made.");
        }

        public IWaveformFigure GetFigure(ChannelId id)
        {
            lock (_Object)
            {
                return _FigureContainer.Where(w => w.Value is IWaveformFigure).Select(w => (IWaveformFigure)w.Value).FirstOrDefault(f => f.IsExitWaveByChannelId(id));
            }
        }

        public void RemoveWaveform(IChnlPrsnt cp)
        {
            var fig = GetFigure(cp.Id);
            fig?.RemoveWave(GetPlotDrawTypeBy(cp.Id), cp.Id);

            if (fig?.IsExitWave() == false)
            {
                RemoveWindow((BaseDisplayForm)fig);
            }
        }

        public void AddWaveform(IChnlPrsnt cp)
        {
            PlotDrawType drawtype = GetPlotDrawTypeBy(cp.Id);

            var fig = GetWindow(cp.WindowId);

            if (fig is not null)
            {
                (fig as IWaveformFigure)?.AddWave(drawtype, cp);

                //if (cp.Active)
                //{
                //    DsoPrsnt.FocusId = cp.Id;
                //}
                //fig.Activate();
            }
            else
            {
                IWaveformFigure form = null;
                switch (ComModel.Constants.RENDERINGMODE)
                {
                    case RenderingMode.Default:
                    case RenderingMode.CPU:
                    default:
                        WaveformFigure newfig = new WaveformFigure()
                        {
                            Text = cp.Id.ToString(),
                            Margin = new Padding(0),
                            WindowId = cp.WindowId,
                        };
                        newfig.DispPresenter = new DisplayPrsnt(DsoPrsnt, newfig, ModelCreateOptions.Standalone);
                        newfig.AddWave(PlotDrawType.Display, newfig.DispPresenter);
                        form = newfig;
                        break;
                    case RenderingMode.GPU:
                        Boolean isMainF = false;
                        Boolean isLogX = false;
                        Boolean isLogY = false;
                        if (((cp as MathPrsnt)?.Args as MathArgPrsnt)?.IsJitterTypeOccupier(Constants.JITTER_BATHTUB_FORMULA) == true)
                        {
                            isLogY = true;
                        }

                        WaveformGPUFigure figure = new WaveformGPUFigure(isMainF, isLogX, isLogY)
                        {
                            Text = cp.Id.ToString(),
                            Margin = new Padding(0),
                            WindowId = cp.WindowId,
                        };
                        if (cp is MathPrsnt mathpst)
                        {
                            figure.Title = mathpst.Args is MathCustomArg ca ? mathpst.Id.ToString() + " : " + ca.Expression : mathpst.Id.ToString() + " : " + mathpst.Args.Description;
                        }
                        if (cp is MathPrsnt math && math.Args?.Type == MathType.FFT)
                        {
                            SetFigMarkBtnVisible(figure, true);
                        }
                        else
                        {
                            SetFigMarkBtnVisible(figure, false);
                        }
                        form = figure;
                        break;
                }
                if (drawtype == PlotDrawType.RadioFrequency)
                {
                    form.AddWave(PlotDrawType.Marker, DsoPrsnt.Markers);
                }
                else if (drawtype == PlotDrawType.AmpVSTime ||
                    drawtype == PlotDrawType.PhaseVSTime ||
                    drawtype == PlotDrawType.PhaseVSFrequency ||
                    drawtype == PlotDrawType.FrequencyVSTime)
                {
                    form.AddWave(PlotDrawType.Trigger, null);
                    form.AddWave(PlotDrawType.Cursor, DsoPrsnt.Cursor);
                }
                else
                {
                    if (drawtype == PlotDrawType.Math)
                    {
                        var markeritem = GetMarkerSource((BaseDisplayForm)form);
                        form.AddWave(PlotDrawType.Marker, markeritem);
                    }
                    form.AddWave(PlotDrawType.TimeBase, DsoPrsnt.Timebase);

                    // 数学和引用不应该有触发元素显示
                    if (drawtype != PlotDrawType.Ref && drawtype != PlotDrawType.Math)
                        form.AddWave(PlotDrawType.Trigger, null);

                    //form.AddWave(PlotDrawType.Trigger, null);
                    form.AddWave(PlotDrawType.Cursor, DsoPrsnt.Cursor);
                    //form.AddWave(PlotDrawType.PassFail, DsoPrsnt.PassFail);
                    form.AddWave(PlotDrawType.SearchMarker, DsoPrsnt.Search);
                }

                form.AddWave(drawtype, cp);
                AddWindow((BaseDisplayForm)form);
            }
        }

        public void SetFigMarkBtnVisible(WaveformGPUFigure figure, Boolean visible)
        {
            if (figure.ButtonSource != null)
            {
                var btnlist = figure.ButtonSource.Where((o) => { return o.BtnType == WeifenLuo.WinFormsUI.Docking.ButtonType.Cursor; });
                if (btnlist != null && btnlist.Count() > 0)
                {
                    btnlist.First().IsVisible = visible;
                }
            }
        }

        public void AddZoomRect(MathPrsnt mp)
        {
            List<BaseDisplayForm> figures = null;
            lock (_Object)
            {
                figures = _FigureContainer.Select(o => o.Value).ToList();
            }

            if (mp.Args is MathZoomArg zoom)
            {
                if (DsoPrsnt.TryGetChannel(zoom.Source, out IChnlPrsnt srcprsnt) == true)
                {
                    var srcfig = (IWaveformFigure)GetWindow(srcprsnt.WindowId);
                    if (srcfig?.IsExitZoomWaveByChannelId(mp.Id) == false)
                    {
                        foreach (var o in figures)
                        {
                            if (o is IWaveformFigure fig)
                            {
                                fig.RemoveWave(PlotDrawType.MathZoom, mp.Id);
                            }
                        }

                        srcfig.AddWave(PlotDrawType.MathZoom, mp);
                    }
                }
            }
        }

        public void RemoveZoomRect(MathPrsnt mp)
        {
            List<BaseDisplayForm> figures = null;
            lock (_Object)
            {
                figures = _FigureContainer.Select(o => o.Value).ToList();
            }

            foreach (var o in figures)
            {
                if (o is IWaveformFigure fig)
                {
                    fig.RemoveWave(PlotDrawType.MathZoom, mp.Id);
                }
            }
        }


        #region Window transformation [Obsolete]
        public void Dispose()
        {
            _IsRunning = false;
            if (_RenderThread != null && _RenderThread.IsAlive)
            {
                _RenderThread.Join(50);
                _RenderThread = null;
            }
            foreach (KeyValuePair<Int64, BaseDisplayForm> item in _FigureContainer)
            {
                item.Value.CanClosed = true;
                item.Value.Close();
                item.Value.Dispose();
            }
        }
        #endregion
    }
}
