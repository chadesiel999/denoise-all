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

namespace ScopeX.U2.RadioFrequency;
public partial class RadioInfo : ChnlBadgeInfo
{
    public RadioInfo(ChannelPrsnt cp) : base(cp, typeof(RadioFrequencyForm))
    {
        InitializeComponent();
        ColumnStyles = new List<(Single Data, ContentAlignment Align)>()
        {
            (30, ContentAlignment.MiddleLeft),
            (70, ContentAlignment.MiddleRight),
        };

        Size = new(150, 110);

        this.MouseDown += RadioInfo_MouseDown;
        this.MouseUp += RadioInfo_MouseUp;
        this.MouseMove += RadioInfo_MouseMove;
    }

    public RadioFrequencyPrsnt Presenter
    {
        get => (RadioFrequencyPrsnt)InternalPrsnt;
        set => InternalPrsnt = value;
    }

    protected override void Update(Object prsnt, String propertyName)
    {
        switch (propertyName)
        {
            case nameof(ReferencePrsnt.Active):
                Presenter.AmpVSTime.TryRemoveView(this);
                Presenter.PhaseVSTime.TryRemoveView(this);
                Presenter.PhaseVSFrequency.TryRemoveView(this);
                break;
            case nameof(Presenter.Source):
                Invalidate();
                break;
            case nameof(Presenter.RBW):               
            case nameof(Presenter.CenterFrequency):                
            case nameof(Presenter.Span):
                UpdateView();
                break;
        }
    }

    protected override void UpdateView()
    {
        DataSource = new List<Object>() 
        {
            new List<Object>{"CF", CenterFrequencyToString() },
            new List<Object>{"Span", SpanToString() },
            new List<Object>{"RBW", RBWToString() },
        };
    }

    protected override void OnDrawHeader(Graphics g)
    {
        base.OnDrawHeader(g);

        TextRenderer.DrawText(g,
           Presenter.Source.ToString(),
           HeaderFont,
           new Rectangle(0, 0, Width, HeaderHeight),
           HeaderForeColor,
           TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
    }

    private String CenterFrequencyToString()
    {
        return new Quantity(Presenter.CenterFrequency, Presenter.Prefix, Presenter.UnitH).ToString();
    }

    private String SpanToString()
    {
        return new Quantity(Presenter.Span, Presenter.Prefix, Presenter.UnitH).ToString();
    }

    private String RBWToString()
    {
        return new Quantity(Presenter.RBW, Presenter.Prefix, "").ToString();
    }
    private Point _StartPosition;
    private Boolean _IsMouseDown;
    private void RadioInfo_MouseDown(Object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _StartPosition = e.Location;
            _IsMouseDown = true;
        }
    }

    private void RadioInfo_MouseUp(Object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _StartPosition = e.Location;
            _IsMouseDown = false;
        }
    }
    private void RadioInfo_MouseMove(Object sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left && _IsMouseDown)
        {
            var offsety = e.Location.Y - _StartPosition.Y;
            if (offsety > 0)
            {
                Presenter.Active = false;

                (Program.Oscilloscope.View as DsoForm).RemoveWaveformUI(Presenter);
            }
        }
    }
}
