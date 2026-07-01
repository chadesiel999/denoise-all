using EventBus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.UserControls;
namespace ScopeX.U2;

[DefaultEvent("Click")]
[DefaultProperty("Text")]
public partial class BadgeInfo :Control
{
    public BadgeInfo()
    {
        InitializeComponent();

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.FromArgb(53, 57, 68);//Color.FromArgb(32, 32, 32);
        ForeColor = Color.Silver;
        Font = new("MiSans", 12.5F);
        _PromptTimer = new System.Timers.Timer();
        _PromptTimer.Interval = 2000;
        _PromptTimer.Elapsed += OnTick;
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (_PromptTimer != null)
        {
            _PromptTimer.Stop();
            _PromptTimer.Elapsed -= OnTick;
            _PromptTimer.Enabled = false;
        }
        base.OnHandleDestroyed(e);
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
            CreateParams parms = base.CreateParams;
            parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
            return parms;
        }
    }

    [Category("ScopeX")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Description("Parent Form")]
    public Form ParentForm
    {
        get
        {
            if (Parent is not null)
            {
                return Parent.FindForm();
            }

            return FindForm();
        }
    }
    //public Form ParentForm
    //{
    //    get;
    //    private set;
    //}

    //protected Form FindParent()
    //{
    //    if (Parent is not null)
    //    {
    //        return Parent.FindForm();
    //    }

    //    return FindForm();
    //}

    protected override Size DefaultSize => new(120, 112);

    protected override Size DefaultMinimumSize => new(60, 28 * 2);

    [Editor(), Browsable(true), DefaultValue(28), Category("ScopeX"), Description("Header Height")]
    public Int32 HeaderHeight
    {
        get;
        set;
    } = 28;

    private Color _HeaderBackColor = Color.White;
    [Editor(), Browsable(true), DefaultValue(typeof(Color), "White"), Category("ScopeX"), Description("Channel Header BackColor")]
    public Color HeaderBackColor
    {
        get => _HeaderBackColor;
        set
        {
            if (_HeaderBackColor != value)
            {
                _HeaderBackColor = value;
                Invalidate();
            }
        }
    }

    [Editor(), Browsable(true), DefaultValue(typeof(Color), "Black"), Category("ScopeX"), Description("Channel Header ForeColor")]
    public Color HeaderForeColor
    {
        get;
        set;
    } = Color.White;

    private Font _HeaderFont = new("MiSans", 12.5F);
    [Editor(), Browsable(true), DefaultValue(typeof(Font), "MiSans, 15pt"), Category("ScopeX"), Description("Channel Header Font")]
    public Font HeaderFont
    {
        get => _HeaderFont;
        set
        {
            if (_HeaderFont.Name != value.Name || _HeaderFont.Size != value.Size || _HeaderFont.Bold != value.Bold)
            {
                _HeaderFont = value;
                Invalidate();
            }
        }
    }

    private Boolean _VisibleFocusFlag = false;
    [Editor(), Browsable(true), DefaultValue(true), Category("ScopeX"), Description("Visible Focus Flag")]
    public Boolean VisibleFocusFlag
    {
        get => _VisibleFocusFlag;
        set
        {
            if (_VisibleFocusFlag != value)
            {
                _VisibleFocusFlag = value;
                Invalidate();
            }
        }
    }

    private Int32 _RowCount = 3;
    [Editor(), Browsable(true), DefaultValue(3), Category("ScopeX"), Description("Row Count")]
    public Int32 RowCount
    {
        get => _RowCount;
        set
        {
            if (value < 0)
            {
                value = 1;
            }

            if (_RowCount != value)
            {
                _RowCount = value;
                Invalidate();
            }
        }
    }

    private (Single Data, ContentAlignment Align)[] _ColumnStyles = { (100, ContentAlignment.MiddleRight) };
    public IList<(Single Data, ContentAlignment Align)> ColumnStyles
    {
        get => Array.AsReadOnly(_ColumnStyles);
        set
        {
            if (value != null)
            {
                var cstyles = value.ToArray();
                if (Math.Round(cstyles.Sum(o => o.Data), 3) != 100.0)
                {
                    for (Int32 i = 0; i < cstyles.Length; i++)
                    {
                        cstyles[i].Data = 100.0F / cstyles.Length;
                    }
                }

                _ColumnStyles = cstyles;
                Invalidate();
            }
        }
    }


    private ContentAlignment _ContentAlign = ContentAlignment.MiddleRight;
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DefaultValue(typeof(ContentAlignment), "MiddleRight")]
    [Category("ScopeX")]
    public ContentAlignment ContentAlign
    {
        get => _ContentAlign;
        set
        {
            if (_ContentAlign != value)
            {
                _ContentAlign = value;
                //TextAlign = GetTextFormat(value);
                Invalidate();
            }
        }
    }

    //protected TextFormatFlags TextAlign = TextFormatFlags.Right | TextFormatFlags.VerticalCenter;

    private static ContentAlignment GetTextAlign(TextFormatFlags ta)
    {
        return ta switch
        {
            TextFormatFlags.Left | TextFormatFlags.Top => ContentAlignment.TopLeft,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter => ContentAlignment.MiddleLeft,
            TextFormatFlags.Left | TextFormatFlags.Bottom => ContentAlignment.BottomLeft,

            TextFormatFlags.HorizontalCenter | TextFormatFlags.Top => ContentAlignment.TopCenter,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter => ContentAlignment.MiddleCenter,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom => ContentAlignment.BottomCenter,

            TextFormatFlags.Right | TextFormatFlags.Top => ContentAlignment.TopRight,
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter => ContentAlignment.MiddleRight,
            TextFormatFlags.Right | TextFormatFlags.Bottom => ContentAlignment.BottomRight,

            _ => ContentAlignment.MiddleLeft,
        };
    }

    private static TextFormatFlags GetTextFormat(ContentAlignment ca)
    {
        return ca switch
        {
            ContentAlignment.TopLeft => TextFormatFlags.Left | TextFormatFlags.Top,
            ContentAlignment.TopCenter => TextFormatFlags.HorizontalCenter | TextFormatFlags.Top,
            ContentAlignment.TopRight => TextFormatFlags.Right | TextFormatFlags.Top,
            ContentAlignment.MiddleLeft => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
            ContentAlignment.MiddleCenter => TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            ContentAlignment.MiddleRight => TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
            ContentAlignment.BottomLeft => TextFormatFlags.Left | TextFormatFlags.Bottom,
            ContentAlignment.BottomCenter => TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom,
            ContentAlignment.BottomRight => TextFormatFlags.Right | TextFormatFlags.Bottom,
            _ => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
        };
    }

    private List<Object> _DataSource = new();

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual IList<Object> DataSource
    {
        get => _DataSource.AsReadOnly();
        set
        {
            if (value != null)
            {
                _DataSource = value.ToList();
                Invalidate();
            }
        }
    }

    private Image _Buffer;

    protected override void OnResize(EventArgs e)
    {
        //_Buffer = new Bitmap(Width - (VisibleHeader ? HeaderWidth : 0), Height); 
        base.OnResize(e);
    }

    private Boolean _SetStrColor = false;
    private object _PromptLock = new object();
    private System.Timers.Timer _PromptTimer;
    private Boolean _IsPaint = false;
    public void Prompt()
    {
        lock (_PromptLock)
        {
            _PromptTimer.Stop();
            _SetStrColor = true;
        }
        if (!_PromptTimer.Enabled)
        {
            //开启计时器准备改变值
            _PromptTimer.Start();
            Invalidate();
        }
    }

    private void OnTick(object sender, EventArgs args)
    {
        lock (_PromptLock)
        {
            if (_SetStrColor)
            {
                _SetStrColor = false;
            }
            else
            {
                _PromptTimer.Stop();
                Invalidate();
            }
        }
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        if (_IsPaint == false)
        {
            _IsPaint = true;
            try
            {
                Color bkc = BackColor;
                Color bkh = HeaderBackColor;

                //if (!Enabled)
                //{
                //    bkc = ControlPaint.Dark(bkc, 0.2F);
                //    bkh = ControlPaint.Dark(bkh, 0.2F);
                //}
                //else if (Focused/* && ShowFocusCues*/)
                //{
                //    bkc = ControlPaint.Light(bkc, 0.2F);
                //    bkh = ControlPaint.Light(bkh, 0.2F);
                //}

                pe.Graphics.Clear(bkc);

                pe.Graphics.FillRectangle(new SolidBrush(bkh), new Rectangle(0, 0, Width, HeaderHeight));

                OnDrawHeader(pe.Graphics);
                _Buffer?.Dispose();
                _Buffer = new Bitmap(Width, Height - HeaderHeight - (VisibleFocusFlag ? 5 : 0));
                Graphics g = Graphics.FromImage(_Buffer);
                try
                {
                    var h = _Buffer.Height / RowCount;

                    g.Clear(bkc);
                    for (Int32 i = 0; i < DataSource.Count; i++)
                    {
                        if (_Buffer.Width <= 0 || h <= 0)
                        {
                            continue;
                        }
                        if (i == 0 && _SetStrColor)
                        {
                            //_SetStrColor = false;
                            ForeColor = HeaderBackColor;
                            Font = new("MiSans", 12.5F, FontStyle.Bold);
                            OnDrawRow(g, 0, i * h, _Buffer.Width, h, DataSource[i]);
                        }
                        else
                        {
                            ForeColor = Color.Silver;
                            Font = new("MiSans", 12.5F);
                            OnDrawRow(g, 0, i * h, _Buffer.Width, h, DataSource[i]);
                        }
                    }

                    pe.Graphics.DrawImageUnscaledAndClipped(_Buffer, new Rectangle(0, HeaderHeight, _Buffer.Width, _Buffer.Height));

                    if (VisibleFocusFlag)
                    {
                        pe.Graphics.FillRectangle(new SolidBrush(bkh), new Rectangle(0, Height - 5, Width, 5));
                    }
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
                }
                finally
                {
                    g.Dispose();
                    base.OnPaint(pe);
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(ex, LogLevel.Error));
            }
            finally
            {
                _IsPaint = false;
            }
        }
    }

    protected virtual void OnDrawHeader(Graphics g)
    {
        TextRenderer.DrawText(g,
            Text,
            HeaderFont,
            new Rectangle(0, 0, Width, HeaderHeight),
            HeaderForeColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }

    protected virtual void OnDrawRow(Graphics g, Int32 x, Int32 y, Int32 w, Int32 h, Object o)
    {
        if (o is List<Object> { Count: > 0 } row)
        {
            for (Int32 i = 0; i < Math.Min(_ColumnStyles.Length, row.Count); i++)
            {
                var cw = (Int32)Math.Round(_ColumnStyles[i].Data / 100.0 * w);
                DrawItem(g, x, y, cw, h, _ColumnStyles[i].Align, row[i]);
                x += cw;
            }
        }
        else
        {
            DrawItem(g, x, y, w, h, ContentAlign, o);
        }
    }

    protected void DrawItem(Graphics g, Int32 x, Int32 y, Int32 w, Int32 h, ContentAlignment align, Object o)
    {
        switch (o)
        {
            //case List<Object> { Count: > 0 } objs:
            //    foreach (var ele in objs)
            //    {
            //        DrawItem(g, x, y, w, h, align, ele);
            //        x += 5;
            //    }
            //    break;

            case Image img:
                var wh = w > h ? h : w;
                var ax = align switch
                {
                    ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => x,
                    ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter =>
                        x + (w - wh) / 2,
                    _ => x + w - (w - wh) / 2,
                };
                g.DrawImage(img, new Rectangle(ax, y, wh, wh));
                break;

            default:
                var str = o?.ToString();
                //Size ts = TextRenderer.MeasureText(os, Font);
                //var strw = ts.Width > Width ? Width : ts.Width;
                if (String.IsNullOrEmpty(str) == false && w > 0 && h > 0)
                {
                    TextRenderer.DrawText(
                        g,
                        str,
                        Font,
                        new Rectangle(x, y, w, h),
                        ForeColor,
                        GetTextFormat(align));
                }
                break;
        }
    }

    //protected override void OnClick(EventArgs e)
    //{
    //    if (e is MouseEventArgs { Button: MouseButtons.Right })
    //    {
    //        return;
    //    }

    //    base.OnClick(e);
    //}

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
        {
            OnClick(e);
        }
        base.OnKeyDown(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        Focus();
        base.OnMouseDown(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate(false);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate(false);
    }
}

public class ChnlBadgeInfo :BadgeInfo, IChnlView
{
    public ChnlBadgeInfo(ChannelPrsnt cp, Type formType)
    {
        FormType = formType;

        InternalPrsnt = cp;
        InternalPrsnt.TryAddView(this);

        HeaderBackColor = cp.DrawColor;


        _doubleClickMaxTime = TimeSpan.FromMilliseconds(SystemInformation.DoubleClickTime);

        _clickTimer = new Timer();
        _clickTimer.Interval = SystemInformation.DoubleClickTime;
        _clickTimer.Tick += ClickTimer_Tick;

        //_singleClickAction = () => MessageBox.Show("Single clicked");
        //_doubleClickAction = () => MessageBox.Show("Double clicked");

       
    }

    protected Type FormType
    {
        get;
    }

    protected ChannelPrsnt InternalPrsnt
    {
        get;
        set;
    }

    IBadge IView<IBadge>.Presenter
    {
        get => InternalPrsnt;
        set => InternalPrsnt = (ChannelPrsnt)value;
    }

    public void UpdateView(Object prsnt, String propertyName)
    {
        try
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(InternalUpdate), new[] { prsnt, propertyName });
            }
            else
            {
                InternalUpdate(prsnt, propertyName);
            }
        }
        catch (Exception ex)
        {
            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
        }
    }

    protected void InternalUpdate(Object prsnt, String propertyName)
    {
        if (String.IsNullOrEmpty(propertyName))
        {
            InternalUpdateView();
            return;
        }

        if (!DesignMode)
        {
            switch (propertyName)
            {
                //!!!Remove itself from presnter
                case nameof(InternalPrsnt.Active):
                    InternalPrsnt.TryRemoveView(this);
                    (Program.Oscilloscope.View as DsoForm).RemoveWaveformUI(InternalPrsnt);
                    break;
                case nameof(DsoPrsnt.FocusId):
                    VisibleFocusFlag = DsoPrsnt.FocusId == InternalPrsnt.Id;
                    break;
                case nameof(InternalPrsnt.WindowId):
                    (Program.Oscilloscope.View as DsoForm).ChangeWaveformFig(InternalPrsnt);
                    break;
                case nameof(InternalPrsnt.DrawColor):
                    HeaderBackColor = InternalPrsnt.DrawColor;
                    InternalUpdateView();
                    break;
            }

            Update(prsnt, propertyName);
        }
    }

    protected virtual void Update(Object prsnt, String propertyName)
    { }

    protected void InternalUpdateView()
    {
        if (!DesignMode)
        {
            VisibleFocusFlag = DsoPrsnt.FocusId == InternalPrsnt.Id;
            UpdateView();
        }
    }

    protected virtual void UpdateView()
    { }

    public override void Refresh()
    {
        base.Refresh();
        InternalUpdateView();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        InternalUpdateView();
        base.OnHandleCreated(e);
    }
 	protected DateTime _lastClick;
    protected bool _inDoubleClick;
    protected Rectangle _doubleClickArea;
    protected TimeSpan _doubleClickMaxTime;
    //private Action _doubleClickAction;
    //private Action _singleClickAction;
    protected Timer _clickTimer;

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
        //if (b)
        //{
        //    if (DsoPrsnt.FocusId != InternalPrsnt.Id)
        //    {
        //        DsoPrsnt.FocusId = InternalPrsnt.Id;
        //    }
        //    else
        //    {
        //        InternalPrsnt.Active = false;
        //    }
        //}
        //else
        if (!b)
        {
            DsoPrsnt.FocusId = InternalPrsnt.Id;
            (Program.Oscilloscope.View as DsoForm).MakeOperateForm(InternalPrsnt.Name, PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
            {
                //MathForm form = new()
                //{
                //    Presenter = InternalPrsnt,
                //    Anchor = AnchorStyles.Bottom,
                //    Text = InternalPrsnt.Name,
                //};
                var form = Activator.CreateInstance(FormType) as FloatForm;
                form.Anchor = AnchorStyles.Bottom;
                form.Text = Text;

                var vu = form as IChnlView;
                vu.Presenter = InternalPrsnt;
                _ = (vu.Presenter as MulticastPrsnt<IChnlView>).TryAddView(vu);

                //!!!Sometimes press the Escape key to close form, the focus does not return back.
                form.FormClosed += (s, e) => Focus();
                return form;
            });
            base.OnClick(e);
        }
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
                bool activeState = InternalPrsnt.Active;
                InternalPrsnt.Active = !activeState;

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
    /// <summary>
    /// 显示参数窗体
    /// </summary>
    /// <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
    public void ShowForm()
    {
        OnClick(null);
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
        //    if (DsoPrsnt.FocusId != InternalPrsnt.Id)
        //    {
        //        DsoPrsnt.FocusId = InternalPrsnt.Id;
        //    }
        //}
        //else
        //{
        //    DsoPrsnt.FocusId = InternalPrsnt.Id;
        //    (Program.Oscilloscope.View as DsoForm).MakeOperateForm(InternalPrsnt.Name, PointToScreen(new Point(Width / 2, 0)), PopOrientation.Above, () =>
        //    {
        //        //MathForm form = new()
        //        //{
        //        //    Presenter = InternalPrsnt,
        //        //    Anchor = AnchorStyles.Bottom,
        //        //    Text = InternalPrsnt.Name,
        //        //};
        //        var form = Activator.CreateInstance(FormType) as FloatForm;
        //        form.Anchor = AnchorStyles.Bottom;
        //        form.Text = Text;

        //        var vu = form as IChnlView;
        //        vu.Presenter = InternalPrsnt;
        //        _ = (vu.Presenter as MulticastPrsnt<IChnlView>).TryAddView(vu);

        //        //!!!Sometimes press the Escape key to close form, the focus does not return back.
        //        form.FormClosed += (s, e) =>
        //        {
        //            form.Dispose();
        //            Focus();
        //        };
        //        return form;
        //    });
        //    base.OnClick(e);
        //}
    }
}
