using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2;
public partial class MeasurePage : UserControl, IView,/*IMeasView, ICymometerView, IVoltmeterView,*/ IStylize
{
    private Boolean _ArgToCtrl;

    public MeasurePage()
    {
        InitializeComponent();
    }

    [Browsable(false)]
    public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

    [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
    public Boolean StylizeFlag { get; set; } = false;

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

    private MeasPrsnt MeasPresenter
    {
        get => (ParentForm as MeasureForm).MeasPresenter;
        //set => (ParentForm as MeasureForm).MeasPresenter = value;
    }

    //IMeasPrsnt IView<IMeasPrsnt>.Presenter
    //{
    //    get => MeasPresenter;
    //    set => MeasPresenter = (MeasPrsnt)value;
    //}

    private CymometerPrsnt CymometePresenter
    {
        get => (ParentForm as MeasureForm).CymometerPresenter;
        //set => (ParentForm as ICymometerView).Presenter = value;
    }

    //ICymometerPrsnt ICymometerView.Presenter
    //{
    //    get => CymometePresenter;
    //    set => CymometePresenter = (CymometerPrsnt)value;
    //}

    private VoltmeterPrsnt VoltmeterPresenter
    {
        get => (ParentForm as MeasureForm).VoltmeterPresenter;
        //set => (ParentForm as IVoltmeterView).Presenter = value;
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
        _ArgToCtrl = true;
        switch (propertyName)
        {
            case "Active":
                ChkVoltmeter.Checked = VoltmeterPresenter.Active;
                ChkCymometer.Checked = CymometePresenter.Active;
                ChkMeasure.Checked = MeasPresenter.Active;

                break;
            case nameof(MeasPresenter.SnapshotActive):
                ChkSnapshot.Checked = MeasPresenter.SnapshotActive;
                break;
            case nameof(MeasPresenter.Strobe):
                RdoGate.ChoosedButtonIndex = (Int32)MeasPresenter.Strobe;
                break;
            case nameof(MeasPresenter.IsStatActive):
                ChkStatistic.Checked = MeasPresenter.IsStatActive;
                break;
        }
        _ArgToCtrl = false;
    }

    protected void UpdateView()
    {
        if (!DesignMode)
        {
            _ArgToCtrl = true;
            ChkMeasure.Checked = MeasPresenter.Active;
            RdoGate.ChoosedButtonIndex = (Int32)MeasPresenter.Strobe;
            ChkStatistic.Checked = MeasPresenter.IsStatActive;

            ChkSnapshot.Checked = MeasPresenter.SnapshotActive;

            ChkVoltmeter.Checked = VoltmeterPresenter.Active;
            ChkCymometer.Checked = CymometePresenter.Active;
            _ArgToCtrl = false;
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        UpdateView();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateView();
    }

    //private void BtnAddItem_Click(Object sender, EventArgs e)
    //{
    //    if (Presenter.IsAllActive())
    //    {
    //        WeakTip.Default.Write("Measure", MsgTipId.NoMoreMeasuerLabel);
    //        return;
    //    }

    //    var appenditem = Presenter.SelectedItems.First(o => !o.Active);
    //    MeasSelectionForm msf = new(appenditem.Name, DsoPrsnt.FocusId, (n, s) =>
    //    {
    //        if (Presenter.IsAllActive())
    //        {
    //            WeakTip.Default.Write("Measure", MsgTipId.NoMoreMeasuerLabel);
    //            return false;
    //        }

    //        var ai = Presenter.SelectedItems.First(o => !o.Active);
    //        ai.Name = n;
    //        ai.Source = s;
    //        ai.Active = true;

    //        return true;
    //    })
    //    {
    //        StartPosition = FormStartPosition.CenterScreen,
    //    };

    //    (ParentForm as FloatForm).CanClose = false;
    //    msf.ShowDialogByEvent();
    //    //if (msf.DialogResult == DialogResult.Yes)
    //    //{
    //    //    appenditem.Name = msf.SelectedItemName;
    //    //    appenditem.Source = msf.Source;
    //    //    appenditem.Active = true;
    //    //}
    //    (ParentForm as FloatForm).CanClose = true;

    //    ParentForm.Close();
    //}

    private void ChkMeasure_CheckedChangedEvent(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            MeasPresenter.Active = ChkMeasure.Checked;
        }
    }

    private void ChkStatistic_CheckedChangedEvent(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            MeasPresenter.IsStatActive = ChkStatistic.Checked;
        }
    }

    private void BtnResetStat_Click_1(Object sender, EventArgs e)
    {
        MeasPresenter.ResetAllStats();
    }

    private void CbxIndicator_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            MeasPresenter.Indicator = CbxIndicator.SelectedIndex;
        }
    }

    private void RdoGate_IndexChanged(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            MeasPresenter.Strobe = (MeasureGate)RdoGate.ChoosedButtonIndex;
        }
    }

    private void ChkSnapshot_CheckedChangedEvent(Object sender, EventArgs e)
    {
        //if (MeasureApp.Default.SnapshotCtrl is null)
        //{
        //    MeasureApp.Default.ShowInfoForm();
        //}
        //else
        //{
        //    MeasureApp.Default.CloseInfoForm(true);
        //}

        if (!_ArgToCtrl)
        {
            MeasPresenter.SnapshotActive = ChkSnapshot.Checked;
        }

    }

    private void ChkCymometer_CheckedChangedEvent(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            CymometePresenter.Active = ChkCymometer.Checked;
        }
    }

    private void ChkVoltmeter_CheckedChangedEvent(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            VoltmeterPresenter.Active = ChkVoltmeter.Checked;
        }
    }
}
