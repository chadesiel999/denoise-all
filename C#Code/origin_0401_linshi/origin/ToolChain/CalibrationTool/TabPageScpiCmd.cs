using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;

using NationalInstruments.Visa;
using Ivi.Visa;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageScpiCmd : UserControl, IMainFormTabPage
    {
        public TabPageScpiCmd()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            string jsonFileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".PrivateScpiCmd.config";
            if (!File.Exists(jsonFileName))
                return;
            string fileContent = File.ReadAllText(jsonFileName, Encoding.UTF8);
            
            List<PrivateScpiCmdDefine>? allDefinedCmd = JsonSerializer.Deserialize<List<PrivateScpiCmdDefine>>(fileContent);
            if (allDefinedCmd == null)
                return;
            //List<PrivateScpiCmdDefine> allDefinedCmd = new List<PrivateScpiCmdDefine>();
            //allDefinedCmd.Clear();
            //allDefinedCmd.Add(new PrivateScpiCmdDefine() { Title = "这是一个测试", CommandStr = "FACT:DGT", Description = "设置数字触发打开或关闭。参数包括{on|off|1|0}" });
            //allDefinedCmd.Last<PrivateScpiCmdDefine>().ParamList.AddRange(new string[]{ "on","off","1","0"});
            //JsonSerializerOptions options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            //File.WriteAllText(jsonFileName, JsonSerializer.Serialize<List<PrivateScpiCmdDefine>>(allDefinedCmd, options), Encoding.UTF8);
            foreach(PrivateScpiCmdDefine cmdDefine in allDefinedCmd)
                comboBoxDefinedCmdList.Items.Add(new ComboxItemObject { Text = cmdDefine.Title, Value = cmdDefine });
        }
        IInstrumentSession? currInstrumentSession = null;

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

        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract) => currInstrumentSession = instrumentInteract;
        public void RefreshData()
        {

        }
        IInstrumentSession? testInstrumentSession = null;
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            string filter = "?*";
            textBoxSelectResourceName.Text = "";
            comboBoxVisaResource.Items.Clear();
            List<String> definedInstrument = InstrumentSessionEngine.GetAllExistsResource();
            foreach (String instrument in definedInstrument)
            {
                comboBoxVisaResource.Items.Add(instrument);
            }
            if (comboBoxVisaResource.Items.Count > 0)
                comboBoxVisaResource.SelectedIndex = 0;
        }

        private void comboBoxVisaResource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxVisaResource.SelectedItem == null)
                textBoxSelectResourceName.Text = "";
            else
                textBoxSelectResourceName.Text = comboBoxVisaResource.SelectedItem.ToString();
        }
        private bool CheckInstrument()
        {
            if (textBoxSelectResourceName.Text == "")
            {
                MessageBox.Show("请先选择对应的仪器！");
                return false;
            }

            if (testInstrumentSession != null)
            {
                if (testInstrumentSession.Address != textBoxSelectResourceName.Text)
                {
                    if (testInstrumentSession.bOpened)
                        testInstrumentSession.Close();
                    testInstrumentSession = new VISASession(textBoxSelectResourceName.Text, 500);
                    if (!testInstrumentSession.Open())
                    {
                        MessageBox.Show("对应的仪器不能打开！");
                        return false;
                    }
                    return true;
                }
                else if (testInstrumentSession.bOpened)
                    return true;
                return false;
            }
            else
            {
                testInstrumentSession = new VISASession(textBoxSelectResourceName.Text, 500);
                if (!testInstrumentSession.Open())
                {
                    MessageBox.Show("对应的仪器不能打开！");
                    return false;
                }
                return true;
            }
        }
        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxSCPICMD.Text == "")
            {
                MessageBox.Show("请先输入scpi命令！");
                return;
            }
            if (!CheckInstrument())
                return;
            testInstrumentSession?.WriteString(textBoxSCPICMD.Text);
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            if (!CheckInstrument())
                return;
            string result = testInstrumentSession!.ReadString();
            if (result != "")
            {
                if (richTextBoxReadBackResult.Text != "")
                    richTextBoxReadBackResult.Text = richTextBoxReadBackResult.Text + System.Environment.NewLine + result;
                else
                    richTextBoxReadBackResult.Text = result;
            }
            else
                MessageBox.Show("没有读到数据！");
        }

        private void buttonClearReadbackResult_Click(object sender, EventArgs e)
        {
            richTextBoxReadBackResult.Clear();
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            if (textBoxSCPICMD.Text == "")
            {
                MessageBox.Show("请先输入scpi命令！");
                return;
            }
            if (!CheckInstrument())
                return;
            testInstrumentSession!.WriteString(textBoxSCPICMD.Text);
            string result = testInstrumentSession!.ReadString();
            if (result != "")
            {
                if (richTextBoxReadBackResult.Text != "")
                    richTextBoxReadBackResult.Text = richTextBoxReadBackResult.Text + System.Environment.NewLine + result;
                else
                    richTextBoxReadBackResult.Text = result;
            }
            else
                MessageBox.Show("没有读到数据！");
        }

        private void comboBoxDefinedCmdList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDefinedCmdList.SelectedIndex == -1)
                return;
            if (comboBoxDefinedCmdList?.SelectedItem== null)
                return;
            PrivateScpiCmdDefine? tmpDefine = (comboBoxDefinedCmdList.SelectedItem as ComboxItemObject)!.Value as PrivateScpiCmdDefine;
            textBoxLocalCmdSourceCode.Text = tmpDefine!.CommandStr;
            richTextBoxLocalCmdDescription.Text = tmpDefine.Description;
            comboBoxLocalScpiParam.Items.Clear();
            foreach (string str in tmpDefine.ParamList)
                comboBoxLocalScpiParam.Items.Add(str);
            if (comboBoxLocalScpiParam.Items.Count > 0)
                comboBoxLocalScpiParam.SelectedIndex = 0;
        }

        private void buttonLocalSCPISend_Click(object sender, EventArgs e)
        {
            if (!BaseHelper.CheckAndTipInstrumentSession(currInstrumentSession))
                return;
            currInstrumentSession?.WriteString(textBoxLocalCmdSourceCode.Text + " " + comboBoxLocalScpiParam.Text);
        }

        private void buttonLocalSCPIReadBack_Click(object sender, EventArgs e)
        {
            if (!BaseHelper.CheckAndTipInstrumentSession(currInstrumentSession))
                return;
            string scpiCmd = textBoxLocalCmdSourceCode.Text.Trim();
            if (scpiCmd.IndexOf('?') > 0)
            {
                if (comboBoxLocalScpiParam.Text.Trim() != "")
                    scpiCmd += " " + comboBoxLocalScpiParam.Text.Trim();
            }
            else
            {
                scpiCmd += " ?" + comboBoxLocalScpiParam.Text.Trim();
            }
            currInstrumentSession!.WriteString(scpiCmd);
            string readBack = currInstrumentSession.ReadString();
            if (richTextBoxReadBackMessage.Text != "")
                richTextBoxReadBackMessage.Text += System.Environment.NewLine + readBack;
            else
                richTextBoxReadBackMessage.Text += readBack;
        }
    }
    public class PrivateScpiCmdDefine
    {
        public string Title
        {
            get;
            set;
        } = "";
        public string Description
        {
            get;
            set;
        } = "";
        public string CommandStr
        {
            get;
            set;
        } = "";
        public List<string> ParamList
        {
            get;
            set;
        } = new List<string>();
    }
}
