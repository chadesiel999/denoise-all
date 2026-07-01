using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPage_FPGAAllWritedRegisterValueReadback : UserControl, IMainFormTabPage
    {
        public TabPage_FPGAAllWritedRegisterValueReadback()
        {
            InitializeComponent();
            comboBoxBoardSelect.SelectedIndex = 0;
            comboBoxWriteReadSelect.SelectedIndex = 0;
            comboBoxCompareWith.SelectedIndex = 0;
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO8000X,
            ProductType.JiHe_MSO7000HD,
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
            buttonReadBack.Enabled = currInstrument != null;
            buttonWriteRegister.Enabled= currInstrument != null;
        }
        public void RefreshData()
        {
        }
        private List<string> readBackContent = new List<string>();
        private void ShowContent()
        {
            richTextBox1.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            if ((comboBoxBoardSelect.SelectedIndex == 0) && (comboBoxWriteReadSelect.SelectedIndex == 0))
            {
                foreach (string str in readBackContent)
                    stringBuilder.AppendLine(str);
            }
            else
            {
                string selectBoardStr = comboBoxBoardSelect.SelectedItem.ToString() + "====";
                int startIndex = -1;
                for (int i = 0; i < readBackContent.Count; i++)
                {
                    if (readBackContent[i].StartsWith(selectBoardStr))
                    {
                        startIndex = i;
                        break;
                    }
                }
                if (startIndex == -1)
                    return;
                stringBuilder.AppendLine(readBackContent[startIndex]);
                int index = startIndex + 1;
                while (index< readBackContent.Count)
                {
                    bool bIsWrite = readBackContent[index].Contains("[W]");
                    bool bIsRead = readBackContent[index].Contains("[R]");
                    if ((!bIsWrite) && (!bIsRead))
                        break;
                    string? line = (comboBoxWriteReadSelect.SelectedIndex, bIsWrite, bIsRead) switch
                    {
                        (1, true, _) => readBackContent[index],
                        (2, _, true) => readBackContent[index],
                        (0,_,_) => readBackContent[index],
                        _=>null,
                    };
                    if (line!=null)
                        stringBuilder.AppendLine(line);
                    index++;
                }
            }
            richTextBox1.Text = stringBuilder.ToString();
        }
        private void buttonReadBack_Click(object sender, EventArgs e)
        {
            string readBack= InstrumentInteract.Factory_ReadbackFPGAWritedRegisterValue(currInstrument);
            string[] allLine = readBack.Split(System.Environment.NewLine);
            readBackContent.Clear();
            readBackContent.AddRange(allLine);
            ShowContent();
            MessageBox.Show("OK");
        }

        private void buttonWriteRegister_Click(object sender, EventArgs e)
        {
            if (!UInt32.TryParse(textBoxWriteRegister_Addr.Text.Trim(), NumberStyles.HexNumber, null,out UInt32 register_addr))
            {
                MessageBox.Show("寄存器地址输入有问题！");
                return;
            }
            if (!UInt32.TryParse(textBoxWriteRegister_Value.Text.Trim(), NumberStyles.HexNumber, null, out UInt32 register_data))
            {
                MessageBox.Show("寄存器数据 输入有问题！");
                return;
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_WriteFPGA_Register);
            scpiCmd += " " + register_addr.ToString() + "," + register_data.ToString() + ",0," + (checkBoxWriteRegister_IsAcq.Checked ? "1" : "0");
            currInstrument!.WriteString(scpiCmd);
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            ShowContent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (readBackContent.Count == 0)
            {
                MessageBox.Show("没有回读数据！");
                return;
            }
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in readBackContent)
                stringBuilder.AppendLine(str);
            string fileName = saveFileDialog1.FileName;
            if (Path.GetExtension(fileName) == null)
                fileName += ".regReadback";
            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        private void buttonOpenA_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxHistoryA_FileName.Text = openFileDialog1.FileName;
            }
        }

        private void buttonOpenB_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxHistoryB_FileName.Text = openFileDialog1.FileName;
            }
        }

        private void comboBoxCompareWith_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelHistoryB.Visible = comboBoxCompareWith.SelectedIndex != 0;
        }
        record address_valuePair ( string address,string value);
        private Dictionary<string, Dictionary<string, address_valuePair>> Split2FpgaRegister(string[] content)
        {
            Dictionary<string, Dictionary<string, address_valuePair>> result = new Dictionary<string, Dictionary<string, address_valuePair>>();
            string fpgaName = "";
            foreach (string line in content)
            {
                if (line.Trim() == "")
                {
                    fpgaName = "";
                    continue;
                }
                int posSymbolEqu = line.IndexOf("====");
                if (posSymbolEqu > 0)
                {
                    fpgaName = line.Substring(0, posSymbolEqu);
                    result.Add(fpgaName, new Dictionary<string, address_valuePair>());
                    continue;
                }
                string name;
                string address;
                string value;
                int pos = line.IndexOf("Name=");
                int pos1 = line.IndexOf(',', pos);
                if (line.IndexOf("[W]") >= 0)
                    name = "[W]" + line.Substring(pos, pos1 - pos).Trim();
                else
                    name = "[R]" + line.Substring(pos, pos1 - pos).Trim();
                pos = line.IndexOf("address=");
                pos1 = line.IndexOf("value=");
                address = line.Substring(pos, pos1 - pos).Trim();
                value = line.Substring(pos1).Trim();
                result[fpgaName].Add(name, new address_valuePair(address, value));
            }
            return result;
        }
        private void buttonCompare_Click(object sender, EventArgs e)
        {
            if (textBoxHistoryA_FileName.Text=="")
            {
                MessageBox.Show("请先选择历史记录A！");
                return;
            }
            if (!File.Exists(textBoxHistoryA_FileName.Text))
            {
                MessageBox.Show("历史记录A文件不存在！");
                return;
            }
            if (comboBoxCompareWith.SelectedIndex == 0)
            {
                if (readBackContent.Count == 0)
                {
                    MessageBox.Show("当前数据还没有读取回来！");
                    return;
                }
            }
            else
            {
                if (textBoxHistoryB_FileName.Text == "")
                {
                    MessageBox.Show("请先选择历史记录B！");
                    return;
                }
                if (!File.Exists(textBoxHistoryB_FileName.Text))
                {
                    MessageBox.Show("历史记录B文件不存在！");
                    return;
                }
            }
            string[] sourceAList = File.ReadAllLines(textBoxHistoryA_FileName.Text);

            string[] sourceBList;
            string compareA_Name = "History_A:";
            string compareB_Name="";
            if (comboBoxCompareWith.SelectedIndex == 0)
            {
                sourceBList = readBackContent.ToArray();
                compareB_Name = "当前读回:";
            }
            else
            {
                sourceBList = File.ReadAllLines(textBoxHistoryB_FileName.Text);
                compareB_Name = "History_B:";
            }
            Dictionary<string, Dictionary<string, address_valuePair>> sourceA = Split2FpgaRegister(sourceAList);
            Dictionary<string, Dictionary<string, address_valuePair>> sourceB = Split2FpgaRegister(sourceBList);
            StringBuilder sb = new StringBuilder();
            foreach (string fpgaNameA in sourceA.Keys)
            {
                if (sourceB.ContainsKey(fpgaNameA))
                {
                    foreach (var a_list in sourceA[fpgaNameA])
                    {
                        if (sourceB[fpgaNameA].ContainsKey(a_list.Key))
                        {
                            if (sourceB[fpgaNameA][a_list.Key] != a_list.Value)
                            {
                                sb.AppendLine($"====在 [{compareA_Name}] 与 [{compareB_Name}] FPGA[{fpgaNameA}] 的寄存器{a_list.Key} 值或地址不同!");
                                if (sourceB[fpgaNameA][a_list.Key].address != a_list.Value.address)
                                    sb.AppendLine($"   [{compareA_Name}].[{a_list.Value.address} <> [{compareB_Name}].[{sourceB[fpgaNameA][a_list.Key].address}]");
                                if (sourceB[fpgaNameA][a_list.Key].value != a_list.Value.value)
                                    sb.AppendLine($"   [{compareA_Name}].[{a_list.Value.value} <> [{compareB_Name}].[{sourceB[fpgaNameA][a_list.Key].value}]");
                            }
                        }
                        else
                            sb.AppendLine($"====在 [{fpgaNameA}] 中不存在 [{compareA_Name}] 中的如下寄存器:{a_list.Key}!");
                    }
                    foreach (var b_list in sourceB[fpgaNameA])
                    {
                        if (!sourceA[fpgaNameA].ContainsKey(b_list.Key))
                            sb.AppendLine($"====在 [{compareA_Name}] 中不存在 [{compareB_Name}] 中的如下寄存器:{b_list.Key}!");
                    }
                }
            }

            foreach(string fpgaNameA in sourceA.Keys)
            {
                if (!sourceB.ContainsKey(fpgaNameA))
                {
                    //B 中压根不存在此FPGA的东西
                    sb.AppendLine($"======在 [{compareA_Name}] FPGA[{fpgaNameA}]存在的以下寄存器，在 [{compareB_Name}] 中全部不存在:");
                    foreach (var keyValuePairs in sourceA[fpgaNameA])
                    {
                        sb.AppendLine($"    {keyValuePairs.Key},{keyValuePairs.Value.address},{keyValuePairs.Value}");
                    }
                }
            }
            foreach (string fpgaNameB in sourceA.Keys)
            {
                if (!sourceA.ContainsKey(fpgaNameB))
                {
                    //B 中压根不存在此FPGA的东西
                    sb.AppendLine($"======在 [{compareB_Name}] FPGA[{fpgaNameB}]存在的以下寄存器，在 [{compareA_Name}] 中全部不存在:");
                    foreach (var keyValuePairs in sourceB[fpgaNameB])
                    {
                        sb.AppendLine($"    {keyValuePairs.Key},{keyValuePairs.Value.address},{keyValuePairs.Value}");
                    }
                }
            }
            richTextBoxNotSample.Clear();
            richTextBoxNotSample.Text = sb.ToString();
            if (sb.ToString() == "")
                MessageBox.Show("比较完成，没有发现不同！");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            List<(UInt32 addr, UInt32 value, Int32 delayMS)> defines = new List<(uint addr, uint value, int delayMS)>();
            foreach(string line in richTextBoxMultiRegister.Lines)
            {
                if (line.Trim() == "")
                    continue;
                string[] groups= line.Split(',');
                if (groups.Length <3)
                {
                    MessageBox.Show($"{line}   格式输入不正确，应该有3个参数，每个用逗号分隔！");
                    return;
                }
                string str = groups[0].Trim().ToLower();
                str = str.Substring(2, str.Length - 2);
                if (!UInt32.TryParse(str, NumberStyles.HexNumber, null, out UInt32 register_addr))
                {
                    MessageBox.Show($"{line}  寄存器地址输入有问题！");
                    return;
                }
                str = groups[1].Trim().ToLower();
                str = str.Substring(2, str.Length - 2);
                if (!UInt32.TryParse(str, NumberStyles.HexNumber, null, out UInt32 register_data))
                {
                    MessageBox.Show($"{line}  寄存器数据 输入有问题！");
                    return;
                }
                if (!Int32.TryParse(groups[2].Trim(), NumberStyles.Integer, null, out Int32 DelayMS))
                {
                    MessageBox.Show($"{line}  延迟时间输入有问题！");
                    return;
                }
                defines.Add((register_addr, register_data, DelayMS));
            }
            if (defines.Count == 0)
            {
                MessageBox.Show("请输入有效数据！");
                return;
            }
            string scpiParams = "";
            foreach (var v in defines)
            {
                scpiParams = scpiParams + $"{v.addr},{v.value},{v.delayMS},0|";
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_WriteFPGA_Register);
            scpiCmd += " "+scpiParams;
            currInstrument!.WriteString(scpiCmd);
            MessageBox.Show("OK");
        }
    }
}
