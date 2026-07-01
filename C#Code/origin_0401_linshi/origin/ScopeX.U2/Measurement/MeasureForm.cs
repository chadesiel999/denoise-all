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
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2;
public partial class MeasureForm : FloatForm, IView/*IMeasView, ICymometerView, IVoltmeterView*/
{
    private readonly MeasurePage _MeasurePage;

    private readonly MeasureExtPage _MeasureExtPage;

    private String _RecordKey = String.Empty;

    public MeasureForm()
    {
        InitializeComponent();

        _MeasurePage = new MeasurePage()
        {
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill
        };
        _MeasureExtPage = new MeasureExtPage()
        {
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill
        };
        Size = new(_MeasurePage.Size.Width, _MeasurePage.Size.Height + HeadHeight + NbgMeasure.CurrentGroupNum * NbgMeasure.NavBarHeight);

        NbgMeasure.SetGroupContent(0, _MeasurePage);
        NbgMeasure.SetGroupContent(1, _MeasureExtPage);
        _RecordKey = $"{this.Name}_{NbgMeasure.Name}";
    }
    protected override void OnHandleDestroyed(EventArgs e)
    {
        DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgMeasure.CurrentGroupIndex;
        base.OnHandleDestroyed(e);
    }
    private void SetGroupIndex()
    {
        var index = -1;
        if (index < 0)
        {
            if (!DsoPrsnt.NavBarGroupRecords.ContainsKey(_RecordKey))
            {
                DsoPrsnt.NavBarGroupRecords.AddOrUpdate(_RecordKey, 0, (k, v) => 0);
            }
            index = DsoPrsnt.NavBarGroupRecords[_RecordKey];
        }
        if (index > NbgMeasure.CurrentGroupNum)
        {
            index = NbgMeasure.CurrentGroupNum - 1;
        }
        NbgMeasure.CurrentGroupIndex = index;
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

    public MeasPrsnt MeasPresenter
    {
        get;
        set;
    }

    //IMeasPrsnt IView<IMeasPrsnt>.Presenter
    //{
    //    get => MeasPresenter;
    //    set => MeasPresenter = (MeasPrsnt)value;
    //}

    public CymometerPrsnt CymometerPresenter
    {
        get;
        set;
    }

    //ICymometerPrsnt ICymometerView.Presenter
    //{
    //    get => CymometerPresenter;
    //    set => CymometerPresenter = (CymometerPrsnt)value;
    //}

    public VoltmeterPrsnt VoltmeterPresenter
    {
        get;
        set;
    }

    //IVoltmeterPrsnt IVoltmeterView.Presenter
    //{
    //    get => VoltmeterPresenter;
    //    set => VoltmeterPresenter = (VoltmeterPrsnt)value;
    //}

    //IBadge IView<IBadge>.Presenter 
    //{ 
    //    get => throw new NotImplementedException(); 
    //    set => throw new NotImplementedException(); 
    //}

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
        if (String.IsNullOrEmpty(propertyName))
        {
            return;
        }

        _MeasurePage.UpdateView(prsnt, propertyName);
        _MeasureExtPage.UpdateView(prsnt, propertyName);    
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        MeasPresenter.TryRemoveView(this);
        CymometerPresenter.TryRemoveView(this);
        VoltmeterPresenter.TryRemoveView(this);
        base.OnFormClosed(e);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Style();
        SetGroupIndex();
#if SaveLanguage
        // LanguageFactory.CacheFormLanguageControls(this);
#endif
    }

    private void Style()
    {
        _MeasurePage.StylizeFlag = true;
        DefaultStyleManager.Instance.RegisterControlRecursion(this);
        //HeadBackColor = Color.FromArgb(62, 62, 62);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
    }
}
