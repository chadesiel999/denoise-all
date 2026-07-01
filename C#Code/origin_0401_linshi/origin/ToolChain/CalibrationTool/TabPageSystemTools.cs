using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageSystemTools : UserControl, IMainFormTabPage
    {
        public TabPageSystemTools()
        {
            InitializeComponent();
            webBrowser = new WebBrowser();
            this.tabPage6.Controls.Add(webBrowser);
            webBrowser.Dock = DockStyle.Fill;
        }
        WebBrowser webBrowser;

        public void RefreshData()
        {
 
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,
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
            foreach (TabPage tabPage in this.tabControl2.TabPages)
            {
                if (tabPage.Controls[0] is IMainFormTabPage)
                    (tabPage.Controls[0] as IMainFormTabPage)?.SetInstrumentInteract(instrumentInteract);
            }
        }

        private void TabPageSystemTools_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
                webBrowser.Url = new Uri(AppDomain.CurrentDomain.BaseDirectory + "MSO7000X_SCPI.html");
        }

        private void buttonReadFromDSO_Click(object sender, EventArgs e)
        {
            if (currInstrument==null)
            {
                MessageBox.Show("请连接示波器");
                return;
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += "? ProbeInfo";
            currInstrument!.WriteString(scpiCmd);
            Thread.Sleep(50);
            string readBack = currInstrument!.ReadString();
            String[] ChannelProbeInfo=readBack.Split(System.Environment.NewLine);
            dataGridView1.RowCount = 0;
            for (int ChannelIndex=0; ChannelIndex<ChannelProbeInfo.Length; ChannelIndex++)
            {
                String[] oneChannelInfo=ChannelProbeInfo[ChannelIndex].Split(" ");
                dataGridView1.RowCount++;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = oneChannelInfo[0].Trim();
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[1].Value = (oneChannelInfo[1].Trim()=="1")? "是":"否";
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[2].Value = oneChannelInfo[1].Trim();
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[3].Value = oneChannelInfo[1].Trim();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex != 4)
                return;
            if (!double.TryParse(dataGridView1.Rows[e.RowIndex].Cells[2].ToString().Trim(),out double radio))
            {
                MessageBox.Show("增益校准系数数值不正确！");
                return;
            }
            string factoryInfo= dataGridView1.Rows[e.RowIndex].Cells[3].ToString().Trim();
            if (factoryInfo.Length == 0)
            {
                MessageBox.Show("必须填写厂家信息！");
                return;
            }
            if (factoryInfo.IndexOf(' ')>=0)
            {
                MessageBox.Show("厂家信息中不能包含空格！");
                return;
            }
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += $" ProbeInfo,{dataGridView1.Rows[e.RowIndex].Cells[0].ToString()} {factoryInfo} {radio}";
            currInstrument!.WriteString(scpiCmd);
            MessageBox.Show("OK!");
        }

        private void buttonWriteAllProbleInfo_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count==0)
            {
                MessageBox.Show("没有烧写内容！");
                return ;
            }
            for(int RowIndex = 0; RowIndex < dataGridView1.Rows.Count; RowIndex++)
            {
                if (dataGridView1.Rows[RowIndex].Cells[1].ToString() == "是")
                {
                    if (!double.TryParse(dataGridView1.Rows[RowIndex].Cells[2].ToString().Trim(), out double radio))
                    {
                        MessageBox.Show($"{dataGridView1.Rows[RowIndex].Cells[0].ToString} 增益校准系数数值不正确！");
                        return;
                    }
                    string factoryInfo = dataGridView1.Rows[RowIndex].Cells[3].ToString().Trim();
                    if (factoryInfo.Length == 0)
                    {
                        MessageBox.Show($"{dataGridView1.Rows[RowIndex].Cells[0].ToString} 必须填写厂家信息！");
                        return;
                    }
                    if (factoryInfo.IndexOf(' ') >= 0)
                    {
                        MessageBox.Show($"{dataGridView1.Rows[RowIndex].Cells[0].ToString} 厂家信息中不能包含空格！");
                        return;
                    }
                }
            }
            for (int RowIndex = 0; RowIndex < dataGridView1.Rows.Count; RowIndex++)
            {
                if (dataGridView1.Rows[RowIndex].Cells[1].ToString() == "是")
                {
                    double radio = double.Parse(dataGridView1.Rows[RowIndex].Cells[2].ToString().Trim());
                    string factoryInfo = dataGridView1.Rows[RowIndex].Cells[3].ToString().Trim();
                    string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
                    scpiCmd += $" ProbeInfo {dataGridView1.Rows[RowIndex].Cells[0].ToString()} {factoryInfo} {radio}";
                    currInstrument!.WriteString(scpiCmd);
                    Thread.Sleep(1000);
                }
            }
            MessageBox.Show("OK！");
        }
    }
}
