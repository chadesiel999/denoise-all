using SharpGen.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Tools;
using System.Linq;
using System.Text.RegularExpressions;
using ScopeX.ComModel;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Controls.Language;
using ScopeX.UserControls.Style;

namespace ScopeX.U2;
public partial class MathInfo : ChnlBadgeInfo
{
    public MathInfo(ChannelPrsnt cp) : base(cp, typeof(MathForm))
    {
        InitializeComponent();
        if (cp.Id.IsAdvancedMath() && !cp.Id.IsJitterMath())
        {
            this.Width = (int)(this.Width * 1.25);
        }
        HeaderForeColor = Color.Black;
        ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
    }

    private void Instance_LanguageChanged(object sender, ILanguage e)
    {
        //if (Presenter.Id.IsJitterMath() || Presenter.Id.IsPowerAnalysisMath())
        {
            //Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(_text).Trim();
            OnTextChanged(null);
            UpdateFigure(String.Empty);
            this.Refresh();
        }
    }


    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        if (Presenter.Id.IsAdvancedMath())
        {
            if (Presenter.Args is MathCustomArg arg && arg.Occupier != null)
            {
                //抖动分析
                if (Presenter.Id.IsJitterMath())
                {
                    var type = arg.Occupier.ToString();
                    Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(type).Trim();
                }
                else if (Presenter.Id.IsPowerAnalysisMath())//电源分析
                {
                    var pwr = Program.Oscilloscope.PwrAnalysisDictionary.Values.FirstOrDefault(x => (x.BoundMathPrsnt1 != null && x.BoundMathPrsnt1.Id == Presenter.Id)|| (x.BoundMathPrsnt2 != null && x.BoundMathPrsnt2.Id == Presenter.Id));
                    if (pwr != null)
                    {
                        //Text = $"{Text}({pwr.Id})";
                        //if (pwr.Mode == PowerAnalysisOpt.PowerQuality || pwr.Mode == PowerAnalysisOpt.SwitchingLoss || pwr.Mode == PowerAnalysisOpt.PowerEfficency)
                        //{
                        //    if (!Text.Contains(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongLvTu")))
                        //    {
                        //        Text = $"{Presenter.Id.ToString()}({ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongLvTu")})";
                        //    }
                        //}
                        //else
                        //{
                        //    if (!Text.Contains(pwr.Id.ToString()))
                        //    {
                        //        Text = $"{Text}({pwr.Id})";
                        //    }
                        //}
                        switch (pwr.Mode)
                        {
                            case PowerAnalysisOpt.PowerQuality:
                            case PowerAnalysisOpt.SwitchingLoss:
                                if (!Text.Contains(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongLvTu")))
                                {
                                    Text = $"{Presenter.Id.ToString()}({ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GongLvTu")})";
                                }
                                break;
                            case PowerAnalysisOpt.PowerEfficency:
                                if (pwr.BoundMathPrsnt1.Id == Presenter.Id)
                                {
                                    if (!Text.Contains(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuGongLvTu")))
                                    {
                                        Text = $"{Presenter.Id.ToString()}({ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuRuGongLvTu")})";
                                    }
                                }
                                else
                                {
                                    if (!Text.Contains(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuGongLvTu")))
                                    {
                                        Text = $"{Presenter.Id.ToString()}({ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuChuGongLvTu")})";
                                    }
                                }
                                break;
                            case PowerAnalysisOpt.SlewRate:
                                if (pwr.BoundMathPrsnt1.Id == Presenter.Id)
                                {
                                    if (!Text.Contains(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaZhuanHuanSuLvTu")))
                                    {
                                        Text = $"{Presenter.Id.ToString()}({ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianYaZhuanHuanSuLvTu")})";
                                    }
                                }
                                else
                                {
                                    if (!Text.Contains(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuZhuanHuanSuLvTu")))
                                    {
                                        Text = $"{Presenter.Id.ToString()}({ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DianLiuZhuanHuanSuLvTu")})";
                                    }
                                }
                                break;
                            default:
                                if (!Text.Contains(pwr.Id.ToString()))
                                {
                                    Text = $"{Text}({pwr.Id})";
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);
        Presenter?.TryRemoveView(this);
        ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
    }

    public MathPrsnt Presenter
    {
        get => (MathPrsnt)InternalPrsnt;
        set
        {
            InternalPrsnt = value;
        }
    }

    protected override void Update(Object prsnt, String propertyName)
    {
        switch (propertyName)
        {
            case "ConditioningScale":
            case "ConditioningScaleUnit":
            case "SamplingScale":
            case "SamplingScaleUnit":
            case "Formula":
                String name = Presenter?.Args?.Description;
                if (Presenter?.Args?.Type == MathType.Histgram
                    || Presenter?.Args?.Type == MathType.Trend
                    || Presenter?.Args?.Type == MathType.Track
                    || Presenter?.Args?.Occupier != null)
                {
                    name = MathArgPrsnt.GetName(Presenter.Args);
                }
                if (Presenter.Id.IsPowerAnalysisMath())//电源分析
                {
                    var pwr1 = Program.Oscilloscope.PwrAnalysisDictionary.Values.FirstOrDefault(x => x.BoundMathPrsnt1 != null && x.BoundMathPrsnt1.Id == Presenter.Id);
                    if (pwr1 != null)
                    {
                        name = MathArgPrsnt.GetName(Presenter.Args);// pwr.Id.ToString();
                        name = LanguageManger.Instance.GetIDMessage(name);
                    }

                    if (String.IsNullOrEmpty(name))
                    {
                        var pwr2 = Program.Oscilloscope.PwrAnalysisDictionary.Values.FirstOrDefault(x => x.BoundMathPrsnt2 != null && x.BoundMathPrsnt2.Id == Presenter.Id);
                        if (pwr2 != null)
                        {
                            name = MathArgPrsnt.GetName(Presenter.Args);// pwr.Id.ToString();
                            name = LanguageManger.Instance.GetIDMessage(name);
                        } 
                    }
                }
                (Program.Oscilloscope.View as DsoForm).UpdateFFT();
                DataSource = new List<Object>() { VScaleToString(), HScaleToString(), name };
                break;
        }
        UpdateFigure(propertyName);
    }

    private void UpdateFigure(String propertyName)
    {
        if (propertyName == "Formula")
        {
            if (Presenter.Args.Type is MathType.FFT or MathType.Zoom or
               MathType.Histgram or MathType.Trend)
            {
                //(ParentForm as DsoForm).MultiWindowManager.RemoveWaveform(Presenter);

                (Program.Oscilloscope.View as DsoForm).AssignNewWindowId(Presenter);
            }
            if (Presenter.Args.Type is MathType.FFT)
            {
                (Program.Oscilloscope.View as DsoForm).SetFigMarkBtnVisible(Presenter, true);
            }
            else
            {
                (Program.Oscilloscope.View as DsoForm).SetFigMarkBtnVisible(Presenter, false);
            }

            if (Presenter.Args.Type == MathType.Zoom)
            {
                (Program.Oscilloscope.View as DsoForm).MultiWindowManager.AddZoomRect(Presenter);
            }
            else
            {
                (Program.Oscilloscope.View as DsoForm).MultiWindowManager.RemoveZoomRect(Presenter);
            }
        }
        var text = Text;
        //抖动分析
        if (Presenter.Id.IsJitterMath() && Presenter.Args is MathCustomArg arg && arg.Occupier != null)
        {
            var type = arg.Occupier.ToString();
            var temp = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(type).Trim();
            if (Text != temp)
            {
                Text = temp;
            }
        }
        string typename = Presenter.Args.Type switch
        {
            MathType.Histgram => ":" + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhiFangTu"),
            MathType.Track => ":" + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("zhuizongtu"),
            MathType.Trend => ":" + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuShiTu"),
            _ => string.Empty,
        };

        text += typename;
        if (Presenter.Args is MathCustomArg ca)
        {
            if (!string.IsNullOrEmpty(ca.Expression))
                text += $":{ca?.Expression}";
        }
        else if (!String.IsNullOrEmpty(Presenter?.Args?.Description))
        {
            if (string.IsNullOrEmpty(typename))
            {
                text += $":{Presenter.Args.Description}";
            }
            else
            {
                text += $"{Presenter.Args.Description}";
            }
        }
        (Program.Oscilloscope.View as DsoForm).UpdateFigTitle(Presenter, text);
    }

    protected override void UpdateView()
    {
        String name = Presenter.Args.Description;
        if (Presenter.Args.Type == MathType.Histgram
            || Presenter.Args.Type == MathType.Trend
            || Presenter.Args.Type == MathType.Track
                    || Presenter?.Args?.Occupier != null)
        {
            name = MathArgPrsnt.GetName(Presenter.Args);
            name = LanguageManger.Instance.GetIDMessage(name);
        }
        DataSource = new List<Object>() { VScaleToString(), HScaleToString(), name };
    }
    protected override void OnDrawHeader(Graphics g)
    {
        TextRenderer.DrawText(g,
           Text,
           HeaderFont,
           new Rectangle(0, 0, Width, HeaderHeight),
           HeaderForeColor,
           TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

        string typename = Presenter.Args.Type switch
        {
            MathType.Histgram => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhiFangTu"),
            MathType.Track => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("zhuizongtu"),
            MathType.Trend => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuShiTu"),
            _ => string.Empty,
        };
        if (!String.IsNullOrEmpty(typename))
        {
            TextRenderer.DrawText(g,
             typename,
              HeaderFont,
              new Rectangle(0, 0, Width, HeaderHeight),
              Color.White,
              TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }
    }
    private String VScaleToString()
    {
        return new Quantity(Presenter.Scale, Presenter.Prefix, Presenter.Unit).ToString() + "/div";
    }

    private String HScaleToString()
    {
        if (Presenter.AutoScale)
        {
            return new Quantity(Math.Abs(Presenter.Sampling.AutoScale), Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString() + "/div";
        }
        else
        {
            return new Quantity(Presenter.Sampling.Scale, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString() + "/div";
        }
    }
}
