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
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageMiscData : UserControl, IMainFormTabPage
    {
        public TabPageMiscData()
        {
            InitializeComponent();
            Init();
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            //ProductType.B21_JinHui_PXI,
            //ProductType.B21_HB8G,
            //ProductType.B21_HD4G,
            //ProductType.B21_DBI20G,
            //ProductType.B21_DBI16G,
            //ProductType.B21_MD8G,
            //ProductType.B21_HR1G,
            //ProductType.B21_MS2G,
            ProductType.JiHe_MSO7000X,
            ProductType.JiHe_MSO7000A,
            ProductType.JiHe_MSO8000X,
            ProductType.JiHe_MSO7000HD,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;

        private int miscNamesCount = 0;
        public CaliDataType CaliDataType { get => CaliDataType.Misc; }
        private bool bInitFinished = false;
        private void Init()
        {
            string[] miscNames = Enum.GetNames(typeof(MiscDefine));
            miscNamesCount = miscNames.Length;
            dataGridViewCaliData_Misc.RowCount = miscNamesCount;
            for (int i = 0; i < miscNamesCount; i++)
            {
                dataGridViewCaliData_Misc.Rows[i].Cells[0].Value = ((int)Enum.Parse(typeof(MiscDefine), miscNames[i])).ToString();
                dataGridViewCaliData_Misc.Rows[i].Cells[1].Value = miscNames[i];
                dataGridViewCaliData_Misc.Rows[i].Cells[2].Value = 0;
            }
            bInitFinished = true;
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
        }
        public void RefreshData()
        {
            if (!bInitFinished)
                return;
            for (int i = 0; i < miscNamesCount; i++)
            {
                dataGridViewCaliData_Misc.Rows[i].Cells[2].Value = MiscData.Default[int.Parse(dataGridViewCaliData_Misc.Rows[i].Cells[0].Value!.ToString()!)];
            }
        }
        private void CaliData_Misc_SendGridData(int rowIndex, int columnIndex, bool bNeedSend)
        {
            DataGridViewCell currCell = dataGridViewCaliData_Misc.Rows[rowIndex].Cells[columnIndex];
            Int32 data = 0;
            Int32.TryParse(currCell.Value.ToString(), out data);
            Int32 oldData = MiscData.Default[rowIndex];
            if (oldData == data) //判断新旧值，避免连续发送
                return;
            MiscData.Default[int.Parse(dataGridViewCaliData_Misc.Rows[rowIndex].Cells[0].Value!.ToString()!)] = data;
            if (bNeedSend && currInstrument != null)
                InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.Misc);
        }
        private void CaliData_Misc_AllSaveToMemory()
        {
            for (int rowIndex = 0; rowIndex < miscNamesCount; rowIndex++)
            {
                CaliData_Misc_SendGridData(rowIndex, 2, false);
            }
        }
        private void dataGridViewCaliData_Misc_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;
            CaliData_Misc_SendGridData(e.RowIndex, e.ColumnIndex, checkBoxCaliData_Misc_AutoSend.Checked);
        }

        private void buttonCaliData_Misc_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            CaliData_Misc_AllSaveToMemory();
            bool bResult = InstrumentInteract.CaliData_Send(currInstrument, CaliDataType.Misc);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_Misc_SaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.Misc);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_Misc_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("从文件中装载将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.Misc);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_LoadDefualtValue_TiAdc_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("装载缺省值将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            MiscData.Default.LoadDefaultValue();
            RefreshData();
            if (currInstrument != null)
                InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.Misc);
        }

        private void BtnSaveFile_Click(object sender, EventArgs e)
        {
            dataGridViewCaliData_Misc.SaveContentAsCsv("MiscCali");
        }

        private Boolean CmpRetAcquireFlag = false;
        private void BtnExtTrigCmpRetAcquire_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            CmpRetAcquireFlag = !CmpRetAcquireFlag;
            BtnExtTrigCmpRetAcquire.Text = CmpRetAcquireFlag ? "停止获取" : "开始获取";
            TbCmpRet.BackColor = CmpRetAcquireFlag ? Color.Green : Color.Red;

            //开启任务
            if (CmpRetAcquireFlag)
            {
                Task.Run(async () => 
                {
                    while(CmpRetAcquireFlag)
                    {
                        string scpiCMD = $":FACT:CALI:SPEC:DATA? GetExtTrigCompareResults";
                        currInstrument!.WriteString(scpiCMD);
                        string cmpRets = currInstrument!.ReadString();
                        StringBuilder sb = new StringBuilder();
                        foreach (char c in cmpRets)
                        {
                            //0-输入信号 > 触发电平;1-输入信号 < 触发电平;
                            sb.Append(c == '0' ? " >" : " <"); 
                        }
                        TbCmpRet.Invoke(() => TbCmpRet.Text = sb.ToString());
                        await Task.Delay(100);
                    }
                });
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TbCmpRet.Text = "";
        }
    }
}
