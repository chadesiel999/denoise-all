using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageValueSetting : UserControl, IMainFormTabPage
    {
        public TabPageValueSetting()
        {
            InitializeComponent();
            this.checkedListBox1.Enabled = false;
            buttonReadback.Enabled = false;
            buttonSend.Enabled = false;
            this.buttonAllClose.Enabled = false;
            this.buttonAllOpen.Enabled = false;
        }

        public void RefreshData()
        {

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
            this.checkedListBox1.Enabled = (instrumentInteract != null);
            currInstrument = instrumentInteract;
            this.buttonReadback.Enabled = (instrumentInteract != null);
            this.buttonSend.Enabled = (instrumentInteract != null);
            this.buttonAllClose.Enabled = (instrumentInteract != null);
            this.buttonAllOpen.Enabled = (instrumentInteract != null);
            if (instrumentInteract != null)
                ReadBackSetting();
        }
        public static Dictionary<string,string> ReadbackDebugVariants(IInstrumentSession nowInstrument)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? DebugVariant";
            nowInstrument!.WriteString(scpiCmd);
            Thread.Sleep(10);
            string readBack = nowInstrument!.ReadString();
            readBack = readBack.Replace(System.Environment.NewLine, "");
            readBack = readBack.Replace(" ", "");
            string[] nameValuePairList = readBack.Split(',');
            foreach (string nameValuePair in nameValuePairList)
            {
                if (nameValuePair.Trim() == "")
                    continue;
                string[] nameValue = nameValuePair.Split(':');
                if (nameValue.Length < 2)
                    continue;
                result.Add(nameValue[0], nameValue[1]);
            }
            return result;
        }
        private void ReadBackSetting()
        {
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? DebugVariant";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(10);
            string readBack = currInstrument!.ReadString();
            readBack = readBack.Replace(System.Environment.NewLine, "");
            readBack = readBack.Replace(" ", "");
            string[] nameValuePairList = readBack.Split(',');
            checkedListBox1.Items.Clear();
            dataGridView1.RowCount = 0;
            foreach (string nameValuePair in nameValuePairList)
            {
                if (nameValuePair.Trim() == "")
                    continue;
                string[] nameValue = nameValuePair.Split(':');
                if (nameValue.Length < 2)
                    continue;
                if (nameValue[0][0] != 'i')
                {
                    checkedListBox1.Items.Add(nameValue[0].Trim());
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, nameValue[1].Trim().ToUpper() == "TRUE");
                }
                else
                {
                    dataGridView1.RowCount++;
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = nameValue[0].Trim();
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = nameValue[1].Trim();
                }
            }
        }
        private void Setting()
        {
            if (checkedListBox1.Items.Count == 0)
                return;
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " DebugVariant,";
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string checkedStr = checkedListBox1.GetItemChecked(i) ? "True" : "false";
                scpiCmd += $"{checkedListBox1.Items[i].ToString()}:{checkedStr},";

            }
            for (int rowIndex = 0; rowIndex < dataGridView1.RowCount; rowIndex++)
            {
                scpiCmd += $"{dataGridView1.Rows[rowIndex].Cells[0].Value.ToString()}:{dataGridView1.Rows[rowIndex].Cells[1].Value.ToString()},";
            }
            currInstrument!.WriteString(scpiCmd);
        }

        private void buttonReadback_Click(object sender, EventArgs e)
        {
            ReadBackSetting();
            MessageBox.Show("OK!");
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Setting();
            Thread.Sleep(1000);
            ReadBackSetting();
            MessageBox.Show("OK!");
        }

        private void buttonAllClose_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.Items[i].ToString() == "bEnable_AdcDataDebugMode")//特殊的
                    checkedListBox1.SetItemChecked(i, true);
                else
                    checkedListBox1.SetItemChecked(i, false);
            }
            Setting();
            Thread.Sleep(1000);
            ReadBackSetting();
            MessageBox.Show("OK!");
        }

        private void buttonAllOpen_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.Items[i].ToString() == "bEnable_AdcDataDebugMode")//特殊的
                    checkedListBox1.SetItemChecked(i, false);
                else
                    checkedListBox1.SetItemChecked(i, true);
            }
            Setting();
            Thread.Sleep(1000);
            ReadBackSetting();
            MessageBox.Show("OK!");
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string checkedString = checkedListBox1.GetItemChecked(i) ? "true" : "false";
                sb.AppendLine($"{checkedListBox1.Items[i].ToString()}:{checkedString}");
            }
            Clipboard.SetText(sb.ToString());
        }
    }
}
