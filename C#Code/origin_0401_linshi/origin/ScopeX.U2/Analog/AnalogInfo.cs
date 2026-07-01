using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2;
public sealed partial class AnalogInfo : BadgeInfo, IChnlView
{
    public AnalogInfo() : base()
    {
        InitializeComponent();
        _doubleClickMaxTime = TimeSpan.FromMilliseconds(SystemInformation.DoubleClickTime);

        _clickTimer = new Timer();
        _clickTimer.Interval = SystemInformation.DoubleClickTime;
        _clickTimer.Tick += ClickTimer_Tick;
    }

    public AnalogPrsnt Presenter
    {
        get;
        set;
    }

    IBadge IView<IBadge>.Presenter
    {
        get => Presenter;
        set => Presenter = (AnalogPrsnt)value;
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

    private void Update(Object prsnt, String propertyName)
    {
        if (String.IsNullOrEmpty(propertyName))
        {
            UpdateView();
            return;
        }

        if (!DesignMode)
        {
            switch (propertyName)
            {
                case nameof(Presenter.Active):
                    {
                        if (Presenter.Active)
                        {
                            (Program.Oscilloscope.View as DsoForm).MultiWindowManager.AddWaveform(Presenter);
                            VisibleFocusFlag = DsoPrsnt.FocusId == Presenter.Id;
                            HeaderBackColor = Presenter.DrawColor;
                            ForeColor = Color.Silver;
                        }
                        else
                        {
                            (Program.Oscilloscope.View as DsoForm).MultiWindowManager.RemoveWaveform(Presenter);
                            VisibleFocusFlag = false;
                            HeaderBackColor = Color.Gray;
                            ForeColor = ControlPaint.Dark(Color.Silver, 0.2F);
                        }
                        (Program.Oscilloscope.View as DsoForm).UpdateAllAnalogInfo(Presenter.Id);
                        break;
                    }
                case nameof(DsoPrsnt.FocusId):
                    if (Presenter.Active)
                    {
                        VisibleFocusFlag = DsoPrsnt.FocusId == Presenter.Id;
                        HeaderBackColor = Presenter.DrawColor;
                        ForeColor = Color.Silver;
                    }
                    else
                    {
                        VisibleFocusFlag = false;
                        HeaderBackColor = Color.Gray;
                        ForeColor = ControlPaint.Dark(Color.Silver, 0.2F);
                    }
                    break;
                case nameof(Presenter.WindowId):
                    (Program.Oscilloscope.View as DsoForm).ChangeWaveformFig(Presenter);
                    break;
                case nameof(Presenter.DrawColor):
                    UpdateView();
                    break;
                case "ConditioningScale":
                case "ConditioningScaleUnit":
                case nameof(Presenter.ScaleBymV):
                case nameof(Presenter.ProbeIndex):
                case nameof(Presenter.ProbeGain):
                case nameof(Presenter.Bandwidth):
                case nameof(Presenter.Coupling):
                case nameof(Presenter.ProbeConnected):
                case nameof(Presenter.SerailNumber):
                    if (Presenter.Active&&!Presenter.IsInCopy)
                    {
                        DsoPrsnt.FocusId = Presenter.Id;
                    }
                    var gainText = new Quantity(Presenter.ProbeGain, Prefix.Empty, "X").ToString("#0.###", true);
					//deleted by lihuijun 探头单位删除
                    //if (Presenter.ProbeUnit == ProbeUnitType.A)
                    //{
                    //    double Val = (1D / Presenter.ProbeGain);
                    //    if (Val < 1)
                    //    {
                    //        gainText = (1D / Presenter.ProbeGain).ToString() + " m/A";
                    //    }
                    //    else
                    //    {
                    //        int iVal = (int)Val;
                    //        gainText = (1D / Presenter.ProbeGain).ToString() + " V/A";
                    //    }
                    //}
                    DataSource = new List<Object>() { ScaleToString() + "/div", BandwidthToString(), gainText };
                    if (propertyName.Equals("ConditioningScale"))
                        Prompt();
                    UpdateView();
                    break;
                case nameof(Presenter.IsInverted):
                    Invalidate();
                    break;
            }

            switch (propertyName)
            {
                case nameof(Presenter.Unit):
                case nameof(Presenter.ProbeIndex):
                case nameof(Presenter.ProbeGain):
                case "ConditioningScaleUnit":
                case nameof(Presenter.ProbeUnitRatio):
                    SynchAnalogChangeToRelevantBusView();
                    break;
            }
        }
    }

    private void UpdateView()
    {
        if (!DesignMode)
        {
            if (Presenter.Active)
            {
                HeaderBackColor = Presenter.DrawColor;
                VisibleFocusFlag = DsoPrsnt.FocusId == Presenter.Id;
            }
            else
            {
                HeaderBackColor = Color.Gray; /*ControlPaint.Dark(Presenter.DrawColor, 0.5F);*/
                VisibleFocusFlag = false;
            }
            //探头单位删除，不用以ProbeUnit更新Unit属性
            //Presenter.Unit = Presenter.ProbeUnit.ToString();

            var gainText = new Quantity(Presenter.ProbeGain, Prefix.Empty, "X").ToString("#0.###", true);
            DataSource = new List<Object>() { ScaleToString() + "/div", BandwidthToString(), gainText };
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        UpdateView();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        UpdateView();
        //！！！不能删除可以移动，初始化时需要给FPGA发送当前初始增益值
        if (!Presenter.ProbeConnected)
        {
            Presenter.ProbeIndex = AnaChnlProbe.x1;
        }
        base.OnHandleCreated(e);
    }
  	private DateTime _lastClick;
    private bool _inDoubleClick;
    private Rectangle _doubleClickArea;
    private TimeSpan _doubleClickMaxTime;
    //private Action _doubleClickAction;
    //private Action _singleClickAction;
    private Timer _clickTimer;

    private void ClickTimer_Tick(object sender, EventArgs e)
    {
        // Clear double click watcher and timer
        _inDoubleClick = false;
        _clickTimer.Stop();

        //_singleClickAction();
        //OnClick(e);

        if (e is MouseEventArgs { Button: MouseButtons.Right })
        {
            return;
        }

        Boolean b = RectangleToScreen(new Rectangle(0, 0, Width, HeaderHeight)).Contains(MousePosition);
        if (b)
        {
            if (Presenter.Active)
            {
                if (DsoPrsnt.FocusId != Presenter.Id)
                {
                    DsoPrsnt.FocusId = Presenter.Id;
                }
            }
            else
            {
                Presenter.Active = true;
                DsoPrsnt.FocusId = Presenter.Id;
            }
        }
        else
        {
            if (Presenter.Active)
            {
                DsoPrsnt.FocusId = Presenter.Id;
            }
           (Program.Oscilloscope.View as DsoForm).MakeOperateForm(Presenter.Name, PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
           {
               AnalogForm form = new()
               {
                   Presenter = Presenter,
                   Anchor = AnchorStyles.Bottom,
                   Text = Presenter.Name,
               };

               _ = form.Presenter.TryAddView(form);

               //!!!Sometimes press the Escape key to close form, the focus does not return back.
               form.FormClosed += (s, e) => Focus();
               return form;
           });
            base.OnClick(e);
        }
    }

    protected override void OnClick(EventArgs e)
    {
        //if (e is MouseEventArgs { Button: MouseButtons.Right })
        //{
        //    return;
        //}

        //Boolean b = RectangleToScreen(new Rectangle(0, 0, Width, HeaderHeight)).Contains(MousePosition);
        //if (b)
        //{
        //    if (Presenter.Active)
        //    {
        //        if (DsoPrsnt.FocusId != Presenter.Id)
        //        {
        //            DsoPrsnt.FocusId = Presenter.Id;
        //        }
        //    }
        //    else
        //    {
        //        Presenter.Active = true;
        //        DsoPrsnt.FocusId = Presenter.Id;
        //    }
        //}
        //else
        //{
        //    if (Presenter.Active)
        //    {
        //        DsoPrsnt.FocusId = Presenter.Id;
        //    }
        //   (Program.Oscilloscope.View as DsoForm).MakeOperateForm(Presenter.Name, PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
        //    {
        //        AnalogForm form = new()
        //        {
        //            Presenter = Presenter,
        //            Anchor = AnchorStyles.Bottom,
        //            Text = Presenter.Name,
        //        };

        //        _ = form.Presenter.TryAddView(form);

        //        //!!!Sometimes press the Escape key to close form, the focus does not return back.
        //        form.FormClosed += (s, e) => Focus();
        //        return form;
        //    });
        //    base.OnClick(e);
        //}
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        //base.OnMouseDown(e);
        if (_inDoubleClick)
        {
            _inDoubleClick = false;

            TimeSpan length = DateTime.Now - _lastClick;

            // If double click is valid, respond
            if (_doubleClickArea.Contains(e.Location) && length < _doubleClickMaxTime)
            {
                _clickTimer.Stop();
                //_doubleClickAction();
                bool activeState = Presenter.Active;
                Presenter.Active = !activeState;

            }

            return;
        }

        // Double click was invalid, restart 
        _clickTimer.Stop();
        _clickTimer.Start();
        _lastClick = DateTime.Now;
        _inDoubleClick = true;
        _doubleClickArea = new Rectangle(e.Location - (SystemInformation.DoubleClickSize / 2),
                                         SystemInformation.DoubleClickSize);

    }
    protected override void OnDrawHeader(Graphics g)
    {
        //base.OnDrawHeader(g);
        TextRenderer.DrawText(g,
            Text,
            HeaderFont,
            new Rectangle(0, 0, Width / 4, HeaderHeight),
            Color.Black,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        if (Presenter.Active)
        {
            HeaderBackColor = Presenter.DrawColor;
        }
        else
        {
            Color colorup = AppStyleConfig.DefaultGradualBackColorOne;
            Color colordown = AppStyleConfig.DefaultGradualBackColorTwo;
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, Width, HeaderHeight), colorup, colordown, LinearGradientMode.Vertical);
            g.FillRectangle(brush, new Rectangle(0, 0, Width, HeaderHeight));
            TextRenderer.DrawText(g,
            Text,
            HeaderFont,
            new Rectangle(0, 0, Width / 4, HeaderHeight),
            Color.White,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        //画幅度细调标志
        if (Presenter.Ylevel_SelectStatus)
        {
            g.FillEllipse(new SolidBrush(HeaderForeColor), 27, 6, 15, 15);
        }

        TextRenderer.DrawText(g,
           Presenter.IsInverted ? "↓ " : "  ",
           HeaderFont,
           new Rectangle(0, 0, Width, HeaderHeight),
           HeaderForeColor,
           TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        Color color;// = Presenter.Coupling == AnaChnlCoupling.DC50 ? Color.Black : HeaderForeColor;
        if (Presenter.Coupling == AnaChnlCoupling.DC50)
        {
            color = Color.Black;
            HeaderFont = new("MiSans", 12.5F, FontStyle.Bold);
        }
        else
        {
            color = HeaderForeColor;
            HeaderFont = new("MiSans", 12.5F);
        }
        TextRenderer.DrawText(g,
            Presenter.Coupling.ToString(),
            HeaderFont,
            new Rectangle(0, 0, Width, HeaderHeight),
            color,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
    }
    private String ScaleToString()
    {
        return new Quantity(Presenter.ScaleBymV, Presenter.Prefix, Presenter.Unit).ToString();
    }

    private String BandwidthToString()
    {
        var bwnames = Presenter.BandWidthNames;
        return bwnames.First(o => o.Index == Presenter.Bandwidth)!.Name;
    }

    /// <summary>
    /// 通道变化同步至总线解码相关的View
    /// </summary>
    private void SynchAnalogChangeToRelevantBusView()
    {
        var dcodechl = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsDecode() && c.Active);
        foreach (var item in dcodechl)
        {
            var decodeprsnt = Core.Decode.ProtocolPrsnt.GetCurrentChannelDecodePrsnt(item.Id, DsoPrsnt.DefaultDsoPrsnt);
            if (decodeprsnt == null)
            {
                continue;
            }
            var chlview = decodeprsnt.GetViewList();
            foreach (var view in chlview)
            {
                if (view is IProtocolView pv)
                {
                    pv?.UpdateThresholdUnit();
                }
            }
        }
    }
}