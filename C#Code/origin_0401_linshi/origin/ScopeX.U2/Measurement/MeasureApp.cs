// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/12</date>

namespace ScopeX.U2
{
    using EventBus;
    using ScottPlot;
    using ScottPlot.Drawing;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Controls.Common.Structs;
    using ScopeX.U2.LanguageSupoort;
    using System.Threading;

    public class MeasureApp
    {
        private IReadOnlyList<String> _NotAllowItems = new List<String>()
        {
            "UI",
            "DataRate",
            "OutsideTime",
            "NPeriods",
            "HighTime",
            "LowTime",
            "RSlewRate",
            "FSlewRate",
            "BurstLen",
            "BurstCycle",
            "BurstInterval",
            "BurstPeriod",
        };//todo
        private IReadOnlyList<String> _HistItems = new List<String>()
        {
            "HistMu1Sigma",
            "HistMu2Sigma",
            "HistMu3Sigma",
            "HistMode",
            "HistMean",
            "HistSigma",
            "HistMax",
            "HistMin",
            "HistMid",
            "HistRange",
            "HistMaxPop",
            "HistTotalPop",
            "HistWfmCnt",
        };
        public MeasureApp(MeasPrsnt prsnt)
        {
            Presenter = prsnt;
            Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Init();
        }

        public static MeasureApp Default
        {
            get;
            internal set;
        }

        public MeasPrsnt Presenter
        {
            get;
        }
        public ReadOnlyDictionary<String, MeasureItemProperties> HistCandidates => new ReadOnlyDictionary<String, MeasureItemProperties>(Candidates.Where(x => _HistItems.Contains(x.Key)).ToDictionary(keySelector: (key) => key.Key, elementSelector: (value) => value.Value));

        public ReadOnlyDictionary<String, MeasureItemProperties> MeasCandidates => new ReadOnlyDictionary<String, MeasureItemProperties>(Candidates.Where(x => !_HistItems.Contains(x.Key)).ToDictionary(keySelector: (key) => key.Key, elementSelector: (value) => value.Value));
        //用户可选的所有参数，Key是参数名
        private ReadOnlyDictionary<String, MeasureItemProperties> Candidates
        {
            get;
            set;
        }

        public Control SnapshotCtrl
        {
            get;
            private set;
        }

        public void CloseInfoForm(Boolean needInvokeClose)
        {
            if (needInvokeClose)
            {
                SnapshotCtrl?.FindForm()?.Close();
            }

            SnapshotCtrl = null;
            KeyboardLed.Default.LedStateControl(LedEnum.LedQuickMeasure, false);
        }

        public void ShowInfoForm()
        {
            var form = SnapshotCtrl?.FindForm();

            if (form is null)
            {
                var mssf = new MeasSnapShotForm()
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new(100, 100),
                };

                mssf.Presenter = Presenter;
                mssf.Presenter.TryAddView(mssf);

                SnapshotCtrl = mssf.GetDataView;

                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = mssf, Type = FormType.InfoForm });
            }

            KeyboardLed.Default.LedStateControl(LedEnum.LedQuickMeasure, true);
        }

        public void DrawIndicator(Object _, (Bitmap Bmp, PlotDimensions Dimensions, Boolean IsLowQuality) e)
        {
            if (Presenter.Indicator == 0)
            {
                return;
            }

            var idx = Presenter.Indicator - 1;
            if (Presenter[idx].IsSourceActive && Presenter[idx].Active)
            {
                using var gfx = GDI.Graphics(e.Bmp, e.Dimensions, e.IsLowQuality, false);
                using var pen = GDI.Pen(/*Presenter[idx].SourceColor*/ AppStyleConfig.DefaultTitleForeColor, 1, LineStyle.Solid, true);
                var (xpos, ypos) = Presenter.GetIndicator(idx);

                switch (xpos, ypos)
                {
                    case (null, not null):
                        foreach (var p in ypos)
                        {
                            Single x1 = e.Dimensions.GetPixelX(e.Dimensions.XMin);
                            Single x2 = e.Dimensions.GetPixelX(e.Dimensions.XMax);
                            Single y = e.Dimensions.GetPixelY(p);
                            if (!Double.IsNaN(y) && !Double.IsNaN(x1) && !Double.IsNaN(x2))
                            {
                                gfx.DrawLine(pen, (Int32)x1, (Int32)y, (Int32)x2, (Int32)y);
                            }
                        }
                        break;

                    case (not null, null):
                        foreach (var p in xpos)
                        {
                            Single y1 = e.Dimensions.GetPixelY(e.Dimensions.YMin);
                            Single y2 = e.Dimensions.GetPixelY(e.Dimensions.YMax);
                            Single x = e.Dimensions.GetPixelX(p);
                            if (!Double.IsNaN(x) && !Double.IsNaN(y1) && !Double.IsNaN(y2))
                            {
                                gfx.DrawLine(pen, (Int32)x, (Int32)y1, (Int32)x, (Int32)y2);
                            }
                        }
                        break;

                        //case (not null, not null) when xpos.Count == ypos.Count:
                        //    for (Int32 i = 0; i < xpos.Count; i++)
                        //    {
                        //        float x = e.Dimensions.GetPixelX(xpos[i]);
                        //        float y = e.Dimensions.GetPixelY(ypos[i]);

                        //        //gfx.DrawLine(pen, x - 5, y, x + 5, y);
                        //        //gfx.Draw(pen, x - 5, y, x + 5, y);

                        //    }
                        //    break;
                }

            }
        }

        public void CloseIndicatorByGPU(Veldrid.Common.IVeldridContent control)
        {
            if (_IndicatorLine != null)
            {
                control.Sundries.Remove(_IndicatorLine);
                _IndicatorLine?.Dispose();
                _IndicatorLine = null;
                _IndicatorParent = null;
            }
        }

        private Veldrid.Common.IVeldridContent _IndicatorParent = null;
        private Veldrid.Common.Plot.LinePlot _IndicatorLine=null;
        public void DrawIndicatorByGPU(Veldrid.Common.IVeldridContent control)
        {
            if (Presenter.Indicator == 0 || (_IndicatorParent != null && _IndicatorParent != control))
            {
                if (_IndicatorLine != null)
                {
                    control.Sundries.Remove(_IndicatorLine);
                    _IndicatorLine?.Dispose();
                    _IndicatorLine = null;
                    _IndicatorParent = null;
                }
                return;
            }
            var idx = Presenter.Indicator - 1;
            if (Presenter[idx].IsSourceActive && Presenter[idx].Active)
            {
                if (_IndicatorLine == null)
                {
                    _IndicatorLine = new Veldrid.Common.Plot.LinePlot(control);
                    _IndicatorLine.Color = Color.White;
                    control.Sundries.Add(_IndicatorLine);
                    _IndicatorParent = control;
                }
                var (xpos, ypos) = MeasureApp.Default.Presenter.GetIndicator((Int32)idx);
                if (xpos != null && xpos.Any(x => x < 0))
                {
                    return;
                }
                switch (xpos, ypos)
                {
                    case (null, not null):
                        _IndicatorLine.LinePosition = Veldrid.Common.Position.Left;
                        _IndicatorLine.Points = ypos.Select(x => (float)x).ToArray();
                        break;
                    case (not null, null):

                        _IndicatorLine.LinePosition = Veldrid.Common.Position.Top;
                        _IndicatorLine.Points = xpos.Select(x => (float)x).ToArray();
                        break;
                    case (not null, not null):

                        break;

                    default:
                        break;
                }
                _IndicatorLine.Visibily = !(xpos == null && ypos == null);
                _IndicatorLine.Brightness = (Program.Oscilloscope.View as DsoForm).Presenter.Display.WfmIntensity;
                //control?.DoRender();
            }
            else
            {
                if (_IndicatorLine != null)
                {
                    control.Sundries.Remove(_IndicatorLine);
                    _IndicatorLine?.Dispose();
                    _IndicatorLine = null;
                    _IndicatorParent = null;
                }
            }
        }

        public Boolean Init(String path = null)
        {
            XmlReader reader;
            if (System.IO.File.Exists(path))
            {
                reader = XmlReader.Create(path);
            }
            else
            {
                //Load Measure.xml
                Type type = typeof(MeasureApp);
                string langSuffix = LanguageFactory.Current switch
                {
                    Language.English => "_en",
                    Language.Spanish => "_sp",
                    Language.Italian =>"_it",
                    Language.German =>"_ge",
                    Language.French =>"_fr",
                    _ => ""
                };
            
                string xmlfilepath = $"{type.Namespace}.Resources.Language.Measure.Measure{langSuffix}.xml";
                Stream sm = type.Assembly.GetManifestResourceStream(xmlfilepath);
                try
                {
                    reader = XmlReader.Create(sm);
                }
                catch
                {
                    EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new LogEventArgs($"The load of Measure.xml fails!", LogLevel.Error));
#if DEBUG
                    throw;
#else
                    return false;
#endif
                }
            }

            try
            {
                var candidates = new Dictionary<String, MeasureItemProperties>();
                var scpinames = new Dictionary<String, String>();

                reader.ReadToFollowing("Measure");

                XmlReader subreader = reader.ReadSubtree();
                while (subreader.Read())
                {
                    if (subreader.NodeType == XmlNodeType.Element && subreader.Name == "Item")
                    {
                        String name;
                        String scpi;
                        Int32 level;
                        String text;
                        String description;
                        String format;
                        String bmpfilename;
                        String catalog;
                        try
                        {
                            name = subreader.GetAttribute("name");
                            scpi = subreader.GetAttribute("scpi");
                            level = Convert.ToInt32(subreader.GetAttribute("level"));
                            text = subreader.GetAttribute("text");
                            description = subreader.GetAttribute("description");
                            format = subreader.GetAttribute("format");
                            catalog = subreader.GetAttribute("catalog");
                            bmpfilename = subreader.GetAttribute("bmpfilename");
                            if (_NotAllowItems.Contains(name)) continue;//todo
                        }
                        catch
                        {
                            subreader.Close();
                            reader.Close();
                            return false;
                        }

                        var value = new MeasureItemProperties(name, text, description, format, catalog)
                        {
                            Level = level,
                            IconFileName = bmpfilename
                        };
                        candidates.Add(name, value);
                        scpinames.Add(scpi, name);
                    }
                }
                subreader.Close();

                Candidates = new(candidates);
                Presenter.ScpiNameTable = new(scpinames);
            }
            catch
            {
                reader.Close();
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(new Object(), new LogEventArgs($"The format of Measure.xml is wrong!", LogLevel.Error));
#if DEBUG
                throw;
#else
                return false;
#endif
            }


            return true;
        }
    }

    public sealed record MeasureItemProperties(String Name/*参数名*/, String Text/*参数本地化名称*/, String Description/*参数说明*/,String Format, String Catalog/*参数类别：水平、垂直或其他*/)
    {
        //参数图标
        public String IconFileName
        {
            get;
            init;
        }

        //测量源：1为单个，2为两个
        public Int32 Level
        {
            get;
            init;
        }
    }
}
