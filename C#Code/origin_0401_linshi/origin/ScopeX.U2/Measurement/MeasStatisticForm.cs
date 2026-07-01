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
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;

namespace ScopeX.U2;
public partial class MeasStatisticForm : FloatForm, IEmbeddableDataView
{
    public Size LastSize { get; set; }
    private Size _IndependentSize;

    public MeasStatisticForm(MeasPrsnt prsnt)
    {
        InitializeComponent();

        FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
        CustomToolIconInfos = new List<ToolIconInfo>()
        {
            new ToolIconInfo()
            {
                IsShow = true,
                Icon = Properties.Resources.AddParams,
                ClickHandler = MeasStatisticForm_AddItemClick
            }
        };

        Presenter = prsnt;
    }

    public Control GetDataView => ScMeasure;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            //Turn on WS_EX_COMPOSITED
            cp.ExStyle |= 0x02000000;
            //Turn off ALT+F4
            cp.ClassStyle |= 0x200;
            return cp;
        }
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

    private MeasPrsnt Presenter { get; }

    public override void Refresh()
    {
        base.Refresh();
        UpdateView();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Stylize();
        UpdateView();
        LvMeasure.IsIndependentWindow = true;
#if SaveLanguage
        // LanguageFactory.CacheFormLanguageControls(this);
#endif
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (TmUpdate != null)
        {
            TmUpdate.Stop();
            TmUpdate.Elapsed -= TmUpdate_Tick;
            TmUpdate.Enabled = false;
        }
        base.OnHandleDestroyed(e);
    }

    private void Stylize()
    {
        ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
        //HeadBackColor = Color.FromArgb(62, 62, 62);
    }

    //protected override void OnKeyPress(KeyPressEventArgs e)
    //{
    //    if (e.KeyChar == (Char)Keys.Escape)
    //    {
    //        MeasStatisticForm_RightIconClick(this, new());
    //        return;
    //    }
    //    base.OnKeyPress(e);
    //}

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        Presenter.IsStatActive = false;
        //!!!Close embeded figure
        var ef = GetDataView?.FindForm();
        if (ef != this)
        {
            ef?.Close();
        }

        base.OnFormClosed(e); 
    }

    private void UpdateView()
    {
        if (!DesignMode)
        {
            CustomToolIconInfos[0].IsShow = !Presenter.IsAllActive();
            LvMeasure.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(-0.05);
            //LvMeasure.SelectedRowColor = LvMeasure.BackColor;

            LvMeasure.BeginUpdate();

            Int32 row = 0;
            var candidates = MeasureApp.Default.MeasCandidates;
            for (Int32 i = 0; i < Presenter.Length; i++)
            {
                if (Presenter.Active && Presenter[i].Active)
                {
                    if (row == LvMeasure.Items.Count)
                    {
                        LvMeasure.Items.Add(new ListViewItem(new String[LvMeasure.Columns.Count]));
                    }

                    LvMeasure.Items[row].BackColor = Presenter[i].DrawColor;

                    var index = Presenter[i].Id.ToString();
                    if (Presenter.Indicator == i + 1)
                    {
                        index = "● " + index;
                        //LvMeasure.Items[row].Font = new Font("Arial", 9F, FontStyle.Underline);
                    }    
                    
                    if (LvMeasure.Items[row].Text != index)
                    {
                        LvMeasure.Items[row].Text = index;
                    }
                    
                    var name = Presenter[i].Source.ToString() + " " + candidates[Presenter[i].Name].Text;
                    LvMeasure.Items[row].SubItems[1].Text = name;

                    var (pfx, unit) = Presenter.GetPfxUnitString(i);

                    var value = Presenter.GetResult(i) ?? Double.NaN;
                    var max = Presenter.GetStatMax(i) ?? Double.NaN;
                    var min = Presenter.GetStatMin(i) ?? Double.NaN;
                    var ave = Presenter.GetStatAverage(i) ?? Double.NaN;
                    var sigma = Presenter.GetStatStddev(i) ?? Double.NaN;
                    var pop = (Double)Presenter.GetStatCount(i);

                    LvMeasure.Items[row].SubItems[2].Text = new Quantity(ave, pfx, unit).ToString("##0.###", true, 7); 
                    LvMeasure.Items[row].SubItems[3].Text = new Quantity(value, pfx, unit).ToString("##0.###", true, 6);
                    LvMeasure.Items[row].SubItems[4].Text = new Quantity(max, pfx, unit).ToString("##0.###", true, 6);
                    LvMeasure.Items[row].SubItems[5].Text = new Quantity(min, pfx, unit).ToString("##0.###", true, 6);
                    LvMeasure.Items[row].SubItems[6].Text = new Quantity(sigma, pfx, unit).ToString("##0.###", true, 6);
                    LvMeasure.Items[row].SubItems[7].Text = new Quantity(pop, Prefix.Empty, QuantityUnit.Count).ToString("##0.###", true, 7);

                    row++;
                }
            }

            if (row < LvMeasure.Items.Count)
            {
                LvMeasure.Items.RemoveAt(row);
            }


            LvMeasure.EndUpdate();
        }
    }

    //private void LvMeasure_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    LvMeasure.SelectedItems.Clear();
    //    _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
    //}

    private void TmUpdate_Tick(object sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(UpdateView));
        }
        else
        {
            UpdateView();
        }
    }

    private Int32 _SelectedIndex = -1;
    private void LvMeasure_Click(Object sender, EventArgs e)
    {
        if (LvMeasure.SelectedItems.Count > 0 && !String.IsNullOrWhiteSpace(LvMeasure.SelectedItems[0].Text))
        {
            var idx = LvMeasure.SelectedItems[0].Text.LastIndexOf('P') + 1;
            var pos = Int32.Parse(LvMeasure.SelectedItems[0].Text[idx..]) - 1;

            if (_SelectedIndex == pos)
            {
                return;    
            }
            
            
            (Program.Oscilloscope.View as DsoForm)?.MakeOperateForm("MearureConfig", PointToScreen(new (Width / 2, 0)), PopOrientation.Above, () =>
            {
                var mif = new MeasItemCfgForm(pos)
                {
                    Presenter = Presenter,
                    Anchor = AnchorStyles.Bottom,
                };
                //mif.CanClose = false;
                mif.Text = $@"{mif.Text} P{pos + 1}";
                mif.Presenter.TryAddView(mif);
                return mif;
            });
            _SelectedIndex = pos;
        }
    }

    private void LvMeasure_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
        Color tc;

        if (e.ColumnIndex == 0)
        {
            tc = Color.Black;
            e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), e.Bounds);
            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                LvMeasure.Font,
                e.Bounds,
                tc,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            return;

        }
        
        if (e.ColumnIndex % 2 == 1)
        {
            tc = AppStyleConfig.DefaultContextForeColor;
            e.Graphics.FillRectangle(new SolidBrush(AppStyleConfig.DefaultContextDarkBackColor), e.Bounds);
        }
        else
        {
            tc = LvMeasure.ForeColor;
            e.Graphics.FillRectangle(new SolidBrush(LvMeasure.BackColor), e.Bounds);
        }
    
        //e.Graphics.DrawRectangle(new Pen(AppStyleConfig.DefaultTitleBackColor, 1.0F), e.Bounds);

        //Int32 ContentOffsetY = (e.Bounds.Height - TextRenderer.MeasureText(e.Header.Text, LvMeasure.Font).Height) / 2;
        //TextRenderer.DrawText(
        //    e.Graphics,
        //    e.SubItem.Text,
        //    LvMeasure.Font,
        //    new Point(e.Bounds.Left + 5, e.Bounds.Top + ContentOffsetY),
        //    tc);
        TextRenderer.DrawText(
            e.Graphics,
            e.SubItem.Text,
            LvMeasure.Font,
            e.Bounds,
            tc,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }

    private void MeasStatisticForm_EmbededClick(Object sender, EventArgs e)
    {
        _IndependentSize = ScMeasure.Size;
        (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this); 
    }

    private void MeasStatisticForm_CloseStatisticClick(Object sender, EventArgs e)
    {
        Presenter.IsStatActive = false;
    }

    public void IndependentControl(Control control)
    {
        control.Dock = DockStyle.Top;
        Controls.Add(control);
        Controls.SetChildIndex(control, 0);
        control.Size = _IndependentSize;
    }

    private void MeasStatisticForm_AddItemClick(Object sender, EventArgs e)
    {
        var appenditem = Presenter.SelectedItems.First(o => !o.Active);
        MeasSelectionForm msf = new(appenditem.Name, DsoPrsnt.FocusId, (n, s) =>
        {
            if (Presenter.IsAllActive())
            {
                WeakTip.Default.Write("Measure", MsgTipId.NoMoreMeasuerLabel);
                return false;
            }
            var mi = MeasureApp.Default.Presenter.SelectedItems.FirstOrDefault(o => o.Active && o.Name == n && o.Source == s);
            if (mi != null)
            {
                WeakTip.Default.Write("Measure", MsgTipId.MeasuerLabelExisted);
                return false;
            }

            var ai = Presenter.SelectedItems.First(o => !o.Active);
            ai.Name = n;
            ai.Source = s;
            ai.Active = true;

            return true;
        })
        {
            StartPosition = FormStartPosition.CenterScreen,
        };

        msf.ShowDialogByEvent();
    }

    
}
