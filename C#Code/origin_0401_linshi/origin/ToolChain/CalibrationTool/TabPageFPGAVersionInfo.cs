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
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageFPGAVersionInfo : UserControl, IMainFormTabPage
    {
        public TabPageFPGAVersionInfo()
        {
            InitializeComponent();
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            buttonReadBack.Enabled = (currInstrument!=null);
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,
            ProductType.B21_HB8G,
            ProductType.B21_HD4G,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_MS2G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        public void RefreshData()
        {
        }

        private void buttonReadBack_Click(object sender, EventArgs e)
        {
            richTextBoxFPGA_ContentVersionInfo.Text = InstrumentInteract.Factory_ReadbackFPGAVersionInfo(currInstrument);
            richTextBoxFPGAVersionInfoAtFlash.Text= InstrumentInteract.Factory_ReadbackFPGAVersionInfoAtFlash(currInstrument);          
        }
    }
}
