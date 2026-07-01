using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class ManufacturerAdatperPage : UserControl, IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        protected Boolean _ArgToCtrl = false;

        public ManufacturerAdatperPage()
        {
            InitializeComponent();
            LblAdapter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SelectScpiAdapter");
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += (_, _) =>
            {
                LblAdapter.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SelectScpiAdapter");
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        protected void UpdateView()
        {
            LoadAdapter();
        }

        private void LoadAdapter()
        {
            List<ManufacturerAdatper> adpters = new()
            {
                ManufacturerAdatper.Default
            };
            adpters.AddRange(Program.Oscilloscope.ScpiAdapters.Select(ap => ap.Manufacturer).ToList());

            CbxAdapter.DataSource = adpters.Select(x => new ComboBoxItem(x.ToString(), (Int32)x)).ToList();
            CbxAdapter.SelectValue = (Int32)Program.Oscilloscope.ManufacturerAdatper;

            CbxAdapter.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Program.Oscilloscope.ManufacturerAdatper = (ManufacturerAdatper)CbxAdapter.SelectValue;
                }
            };
        }
    }
}
