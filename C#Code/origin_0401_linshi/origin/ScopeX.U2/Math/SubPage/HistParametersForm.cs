using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls;
using ScopeX.Core.Tools;
using ScopeX.Controls.Language;
using ScopeX.UserControls.Style;

namespace ScopeX.U2;
public partial class HistParametersForm : FloatForm, IChnlView, IEmbeddableDataView
{
    public Size LastSize { get; set; }
    private Size _IndependentSize; //独立控件的大小

    public HistParametersForm()
    {
        InitializeComponent();
        FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
        ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        //HelpClick += (_, _) =>
        //{
        //    var res = Int32.TryParse(HelpLabel, out var index);
        //    if (!res)
        //    {
        //        HelpProcessManager.SendCommand();
        //        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
        //        return;
        //    }
        //    HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(index));
        //};
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
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
            return cp;
        }
    }

    public MathPrsnt Presenter
    {
        get;
        set;
    }

    IBadge IView<IBadge>.Presenter
    {
        get => Presenter;
        set => Presenter = (MathPrsnt)value;
    }

    private MathHistArg _HistArg;

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

    protected void Update(Object prsnt, String propertyName)
    {
        switch (propertyName)
        {
            case nameof(Presenter.Active):
            case nameof(Presenter.Formula):
                if (!Presenter.Active || Presenter.Args.Type != MathType.Histgram)
                {
                    if (ScContent.Parent is Control ctl && ctl.Parent is Form cf)
                    {
                        cf.Close();
                    }
                    else if (ScContent.Parent is Form frm)
                    {
                        frm.Close();
                    }
                }
                break;
        }
    }

    protected void UpdateView()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(Update));
        }
        else
        {
            Update();
        }
    }

    protected void Update()
    {
        if (!DesignMode)
        {
            LvContent.BeginUpdate();
            UpdateParameters();
            LvContent.EndUpdate();
        }
    }

    private void UpdateParameters()
    {
        var pm = _HistArg.CalcParameters();

        if (pm is not null)
        {
            ChannelId id = _HistArg.Source;
            Prefix pre = Prefix.Empty;
            string unit = String.Empty;
            bool ismeasure = false;
            if (id.IsMeasure())
            {
                var result = Program.Oscilloscope.Measure.SelectedItems.FirstOrDefault(x => x.Id == id);
                int index = Program.Oscilloscope.Measure.SelectedItems.IndexOf(result);
                (pre, unit) = Program.Oscilloscope.Measure.GetPfxUnitString(index);
                ismeasure = true;
                if (result != null)
                    id = result.Source;
            }
            else if(id== ChannelId.DVM)
            {
                (pre, unit) = (Prefix.Milli, DsoPrsnt.DefaultDsoPrsnt.Voltmeter.Unit);
            }
            else if(id== ChannelId.CYM)
            {
                (pre, unit) = (Prefix.Empty, DsoPrsnt.DefaultDsoPrsnt.Cymometer.Unit);
            }
            //!!!Ambiguity Histogram
            //var p = Presenter.Sampling.Prefix; 
            //var u = Presenter.Sampling.Unit;

            if (Program.Oscilloscope.TryGetChannel(id, out var cp))
            {
                if (!ismeasure)
                {
                    var (p, u) = cp.Pack.Properties.ChnlUnit;
                    pre = p;
                    unit = u;
                }
            }

            LvContent.Items[0].SubItems[1].Text = new Quantity(pm.Average, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[1].SubItems[1].Text = new Quantity(pm.Sigma, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[2].SubItems[1].Text = new Quantity(pm.Median, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[3].SubItems[1].Text = new Quantity(pm.High, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[4].SubItems[1].Text = new Quantity(pm.Low, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[5].SubItems[1].Text = new Quantity(pm.Mode, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[6].SubItems[1].Text = new Quantity(pm.Range, pre, unit).ToString("##0.00#", true, 7);
            LvContent.Items[7].SubItems[1].Text = pm.Sigma1Pop.ToString(@"##0.00%");
            LvContent.Items[8].SubItems[1].Text = pm.Sigma2Pop.ToString(@"##0.00%");
            LvContent.Items[9].SubItems[1].Text = pm.Sigma3Pop.ToString(@"##0.00%");
            LvContent.Items[10].SubItems[1].Text = new Quantity(pm.MaxPop, Prefix.Empty, Presenter.Unit).ToString("##0", true, 3);
            LvContent.Items[11].SubItems[1].Text = new Quantity(pm.TotalPop, Prefix.Empty, Presenter.Unit).ToString("##0", true, 3);

        }
        HeadBackColor = Presenter.DrawColor.GetBrightnessColor(-0.2);
        Text = @$"{Presenter.Id}";
    }

    public override void Refresh()
    {
        base.Refresh();
        UpdateView();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadContentItems();
        _HistArg = (MathHistArg)Presenter.GetOrMakeArg(MathType.Histgram);
        Stylize();
        UpdateView();
        LvContent.IsIndependentWindow = true;
#if SaveLanguage
        // LanguageFactory.CacheFormLanguageControls(this);
#endif
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);
        ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
    }

    private void Instance_LanguageChanged(object sender, ILanguage e)
    {
        LoadContentItems();
    }

    private void LoadContentItems()
    {
        LvContent.BeginUpdate();
        if (LvContent.Items.Count == 0)
        {
            for (Int32 i = 0; i < 12; i++)
            {
                LvContent.Items.Add(new ListViewItem(new String[LvContent.Columns.Count]));
            }
        }
        var candidates = MeasureApp.Default.HistCandidates;
        LvContent.Items[0].SubItems[0].Text = candidates["HistMean"].Text;
        LvContent.Items[1].SubItems[0].Text = candidates["HistSigma"].Text;
        LvContent.Items[2].SubItems[0].Text = candidates["HistMid"].Text;
        LvContent.Items[3].SubItems[0].Text = candidates["HistMax"].Text;
        LvContent.Items[4].SubItems[0].Text = candidates["HistMin"].Text;
        LvContent.Items[5].SubItems[0].Text = candidates["HistMode"].Text;
        LvContent.Items[6].SubItems[0].Text = candidates["HistRange"].Text;
        LvContent.Items[7].SubItems[0].Text = candidates["HistMu1Sigma"].Text;
        LvContent.Items[8].SubItems[0].Text = candidates["HistMu2Sigma"].Text;
        LvContent.Items[9].SubItems[0].Text = candidates["HistMu3Sigma"].Text;
        LvContent.Items[10].SubItems[0].Text = candidates["HistMaxPop"].Text;
        LvContent.Items[11].SubItems[0].Text = candidates["HistTotalPop"].Text;
        LvContent.EndUpdate();
    }

    private void Stylize()
    {
        ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
        //HeadBackColor = Color.FromArgb(62, 62, 62);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (TmUpdate != null)
        {
            TmUpdate.Stop();
            TmUpdate.Elapsed -= TmUpdate_Tick;
            TmUpdate.Enabled = false;
        }
        Presenter.TryRemoveView(this);
        base.OnFormClosed(e);
    }

    private void TmUpdate_Tick(Object sender, EventArgs e)
    {
        UpdateView();
    }

    private void LvContent_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
        Color tc;

        //值列处理
        if (e.ColumnIndex % 2 == 1)
        {
            tc = AppStyleConfig.DefaultContextForeColor;
            e.Graphics.FillRectangle(new SolidBrush(AppStyleConfig.DefaultContextDarkBackColor), e.Bounds);
        }
        else
        {
            tc = LvContent.ForeColor;
            e.Graphics.FillRectangle(new SolidBrush(LvContent.BackColor), e.Bounds);
        }

        e.Graphics.DrawRectangle(new Pen(AppStyleConfig.DefaultTitleBackColor, 1.0F), e.Bounds);

        Int32 ContentOffsetY = (e.Bounds.Height - TextRenderer.MeasureText(e.Header.Text, LvContent.Font).Height) / 2;
        TextRenderer.DrawText(
            e.Graphics,
            e.SubItem.Text,
            LvContent.Font,
            new Point(e.Bounds.Left + 5, e.Bounds.Top + ContentOffsetY),
            tc);
    }

    private void HistParametersForm_LeftIconClick(object sender, EventArgs e)
    {
        _IndependentSize = ScContent.Size;
        (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this);
    }

    public Control GetDataView => ScContent;

    public void IndependentControl(Control control)
    {
        control.Dock = DockStyle.Top;
        Controls.Add(control);
        Controls.SetChildIndex(control, 0);
        control.Size = _IndependentSize;
    }
}

