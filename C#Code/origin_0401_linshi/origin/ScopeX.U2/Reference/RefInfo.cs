using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2;
public partial class RefInfo : ChnlBadgeInfo
{
    public RefInfo(ChannelPrsnt cp) : base(cp, typeof(ReferenceForm))
    {
        InitializeComponent();
    }

    public ReferencePrsnt Presenter
    {
        get => (ReferencePrsnt)InternalPrsnt;
        set => InternalPrsnt = value;
    }

    protected override void Update(Object prsnt, String propertyName)
    {
        switch (propertyName)
        {
            case "ConditioningScale":
            case "ConditioningScaleUnit":
            case "SamplingScale":
            case nameof(Presenter.ScaleBymV):
                DataSource = new List<Object>() { VScaleToString(), HScaleToString() };
                break;
        }
    }

    protected override void UpdateView()
    {
        DataSource = new List<Object>() { VScaleToString(), HScaleToString() };
    }

    private String VScaleToString()
    {
        return new Quantity(Presenter.ScaleBymV, Presenter.Prefix, Presenter.Unit).ToString() + "/div";
    }

    private String HScaleToString()
    {
        return new Quantity(Presenter.Sampling.Scale, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString() + "/div";
    }
    protected override void OnDrawHeader(Graphics g)
    {
        base.OnDrawHeader(g);

        TextRenderer.DrawText(g,
            "",
           //Presenter.Id.ToString(),
           HeaderFont,
           new Rectangle(0, 0, Width, HeaderHeight),
           HeaderForeColor,
           TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        if (Presenter.Ylevel_SelectStatus)
        {
            g.FillEllipse(new SolidBrush(HeaderForeColor), 27, 6, 15, 15);
        }
    }
}

