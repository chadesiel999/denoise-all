using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageProductInfo : UserControl, IMainFormTabPage
    {
        public TabPageProductInfo()
        {
            InitializeComponent();
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
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            buttonRead.Enabled = (currInstrument != null);
            buttonWrite.Enabled = (currInstrument != null);
        }
        public void RefreshData()
        {
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            string setting = InstrumentInteract.Factory_ReadbackProductInfoAtFlash(currInstrument);
            string model = "";
            string manufacturers = "";
            string sn = "";
            uint funCode = 0x1;
            int index = setting.IndexOf("Model=");
            int markIndex = 0;
            if (index >= 0)
            {
                markIndex = setting.IndexOf('|', index + "Model=".Length);
                if (markIndex >= 0)
                    model = setting.Substring(index + "Model=".Length, markIndex - (index + "Model=".Length));
                else
                    model = setting.Substring(index + "Model=".Length);
            }
            index = setting.IndexOf("SN=");
            if (index >= 0)
            {
                markIndex = setting.IndexOf('|', index + "SN=".Length);
                if (markIndex >= 0)
                    sn = setting.Substring(index + "SN=".Length, markIndex - (index + "SN=".Length));
                else
                    sn = setting.Substring(index + "SN=".Length);
            }
            index = setting.IndexOf("Manufacturer=");
            if (index >= 0)
            {
                markIndex = setting.IndexOf('|', index + "Manufacturer=".Length);
                if (markIndex >= 0)
                    manufacturers = setting.Substring(index + "Manufacturer=".Length, markIndex - (index + "Manufacturer=".Length));
                else
                    manufacturers = setting.Substring(index + "Manufacturer=".Length);
            }
            index = setting.IndexOf("FunCode=");
            if (index >= 0)
            {
                markIndex = setting.IndexOf('|', index + "FunCode=".Length);
                string s = "";
                if (markIndex >= 0)
                    s = setting.Substring(index + "FunCode=".Length, markIndex - (index + "FunCode=".Length));
                else
                    s = setting.Substring(index + "FunCode=".Length);
                if (s.Trim() != "")
                    uint.TryParse(s, out funCode);
            }
            textBoxModel.Text = model;
            textBoxManufacturer.Text = manufacturers;
            textBoxFunCode.Text = "0x"+funCode.ToString("X").PadLeft(8,'0');
            textBoxSN.Text = sn;
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            UInt32 funcCode = 0;
            if (textBoxFunCode.Text.StartsWith("0x") || textBoxFunCode.Text.StartsWith("0X"))
                UInt32.TryParse(textBoxFunCode.Text.Substring(2).Trim(), NumberStyles.HexNumber, null, out funcCode);
            else
                UInt32.TryParse(textBoxFunCode.Text, NumberStyles.Integer, null, out funcCode);
            textBoxFunCode.Text = "0x"+funcCode.ToString("X").PadLeft(8,'0');
            string productInfo = $"Model={textBoxModel.Text}|SN={textBoxSN.Text}|Manufacturer={textBoxManufacturer.Text}|FunCode={funcCode}";
            InstrumentInteract.Factory_WriteProductInfoAtFlash(currInstrument,productInfo);
        }
    }
}
