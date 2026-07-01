using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;

namespace ScopeX.U2;
public partial class MeasureOpForm : FloatForm
{
    private readonly Action<String> _GetResults;

    public MeasureOpForm(Action<String> fnGetResult)
    {
        InitializeComponent();
        _GetResults = fnGetResult;
        foreach (Control btn in TlpBody.Controls)
        {
            btn.Click += Item_Click;
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

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
            return cp;
        }
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (e.KeyChar == (Char)Keys.Escape)
        {
            Close();
            return;
        }
        base.OnKeyPress(e);
    }

    private void Item_Click(Object sender, EventArgs e)
    {
        _GetResults((sender as Control).Text);

        DialogResult = DialogResult.Yes;
        Close();
    }

    private void MeasureOpForm_Load(object sender, EventArgs e)
    {
#if SaveLanguage
        // LanguageFactory.CacheFormLanguageControls(this);
#endif
    }
}
