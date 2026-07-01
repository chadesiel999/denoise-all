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
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2;
public partial class MeasureExtPage : UserControl, IMeasView
{
    private Boolean _ArgToCtrl;

    private Int32 _PxIndex;

    public MeasureExtPage()
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

    public MeasPrsnt Presenter
    {
        get => (MeasPrsnt)(ParentForm as IMeasView).Presenter;
        set => (ParentForm as IMeasView).Presenter = value;
    }

    IMeasPrsnt IView<IMeasPrsnt>.Presenter
    {
        get => Presenter;
        set => Presenter = (MeasPrsnt)value;
    }

    public void UpdateView(Object prsnt, String propertyName)
    {
        if (_PxIndex > 0)
        {
            _ArgToCtrl = true; 
            var item = Presenter[_PxIndex - 1];
            switch (propertyName)
            {
                case "Active":
                    break;
                case "Source":
                    CbxSource1.SetItemText(item.Source);
                    break;
                case "Source2nd":
                    CbxSource2.SetItemText(item.Source2nd);
                    break;
                case "Name":
                    BtnOperator.Text = item.Name;
                    ChangeCtrlState();
                    break;
            }
            _ArgToCtrl = false;
        }
    }

    protected void UpdateView()
    {
        if (!DesignMode)
        {
            if (_PxIndex > 0)
            {
                _ArgToCtrl = true;
                CbxSource1.SetItemText(Presenter[_PxIndex - 1].Source);
                CbxSource2.SetItemText(Presenter[_PxIndex - 1].Source2nd);
                BtnOperator.Text = "";
                
                _ArgToCtrl = false;
            }
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadSourceList(Enum.GetValues<ChannelId>().Where(id => id.IsMeasure()));
        ChangeCtrlState();
    }

    private readonly List<(MeasureExtCalc Name, String Alias, String CmdKey)> _Items = new()
    {
        {(MeasureExtCalc.Add, MeasureExtCalc.Add.GetAlias(), $"Math.{MeasureExtCalc.Add}") },
        {(MeasureExtCalc.Sub, MeasureExtCalc.Sub.GetAlias(), $"Math.{MeasureExtCalc.Sub}") },
        {(MeasureExtCalc.Mul, MeasureExtCalc.Mul.GetAlias(), $"Math.{MeasureExtCalc.Mul}") },
        {(MeasureExtCalc.Div, MeasureExtCalc.Div.GetAlias(), $"Math.{MeasureExtCalc.Div}") },
        {(MeasureExtCalc.Abs, MeasureExtCalc.Abs.GetAlias(), $"Math.{MeasureExtCalc.Abs}") },
    };


    private void ChangeCtrlState()
    {
        CbxSource1.Enabled = BtnOperator.Enabled = _PxIndex > 0;

        if (_PxIndex > 0)
        {
            CbxSource2.Visible = Presenter[_PxIndex - 1].Name != MeasureExtCalc.Abs.GetAlias();
        }
    }

    private void LoadSourceList(IEnumerable<ChannelId> sources)
    {
        CbxSource1.DataSource = sources.Select(x => new KeyValuePair<String, ChannelId>(x.ToString(), x)).ToList();
        CbxSource1.DisplayMember = "Key";
        CbxSource1.ValueMember = "Value";

        CbxSource1.SelectedValueChanged += (_, _) =>
        {
            if (!_ArgToCtrl)
            {
                Presenter[_PxIndex - 1].Source = (ChannelId)CbxSource1.SelectedValue;
            }
        };

        CbxSource2.DataSource = sources.Select(x => new KeyValuePair<String, ChannelId>(x.ToString(), x)).ToList();
        CbxSource2.DisplayMember = "Key";
        CbxSource2.ValueMember = "Value";

        CbxSource2.SelectedValueChanged += (_, _) =>
        {
            if (!_ArgToCtrl)
            {
                Presenter[_PxIndex - 1].Source2nd = (ChannelId)CbxSource2.SelectedValue;
            }
        };
    }

    private void CbxDestination_SelectedIndexChanged(Object sender, EventArgs e)
    {
        if (!_ArgToCtrl)
        {
            _PxIndex = CbxDestination.SelectedIndex;
            if (_PxIndex > 0)
            {
                UpdateView();
                ChangeCtrlState();
            }
        }
    }

    private void BtnOperator_Click(Object sender, EventArgs e)
    {
        MeasureOpForm msf = new(o => Presenter[_PxIndex - 1].Name = o)
        {
            StartPosition = FormStartPosition.CenterScreen,
        };

        (ParentForm as FloatForm).CanClose = false;
        msf.ShowDialogByPosition();
        (ParentForm as FloatForm).CanClose = true;
    }
}
