// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Resources;
    using System.Windows.Forms;
    using ScopeX.Core;
    using ScopeX.U2.Properties;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [Description(nameof(MathType.Custom))]
    public partial class MathCustomSubPage : MathSubPageBase
    {
        MathCustomArg Arg;
        public MathCustomSubPage()
        {
            InitializeComponent();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (Arg is null)
                {
                    return;
                }

                BtnFormula.Text = FixAmpersand(Arg.Expression);
                if (BtnFormula.Text == "")
                {
                    BtnFormula.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathCustomSubPage.BtnFormula");// Resources.MathCustomSubPage_BtnFormulaText;
                }

                if (Arg.Occupier is not null)
                {
                    LblAdditionalInfo.Text = $"Channel {Presenter.Id} is occupied by {Arg.Occupier},  DONOT CHANGE IT!";
                }
                else
                {
                    LblAdditionalInfo.Text = "";
                }
                //_CustomArg.Init();
            }
        }

        private static String FixAmpersand(String str)
        {
            if (str == string.Empty || str == "")
            {
                ResourceManager rm = new ResourceManager("ScopeX.U2.MathCustomSubPage", System.Reflection.Assembly.GetExecutingAssembly());
                return rm.GetString("BtnFormula_DefaultText");
            }

            var i = str.IndexOf('&');
            if (i >= 0)
            {
                return str[0..i] + "&" + str[i..];
            }

            return str;
        }

        protected override void SubPageUpdateView(object prsnt, string propertyName)
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            Arg = (MathCustomArg)Presenter.GetOrMakeArg(MathType.Custom);
            UpdateView();
        }

        private void BtnFormula_Click(object sender, EventArgs e)
        {
            var editor = new CustomFormulaForm(Program.Oscilloscope)
            {
                Presenter = Presenter,
            };
            editor.BringToFront();
            Presenter.TryAddView(editor);

            (ParentForm as FloatForm).CanClose = false;
            if (editor.ShowDialogByEvent() == DialogResult.OK)
            {
                BtnFormula.Text = FixAmpersand(Arg.Expression);
                if (BtnFormula.Text == "")
                {
                    BtnFormula.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MathForm.MathPage.TlpMath.MathCustomSubPage.BtnFormula"); // Resources.MathCustomSubPage_BtnFormulaText;
                }
            }
            (ParentForm as FloatForm).CanClose = true;
        }
    }
}
