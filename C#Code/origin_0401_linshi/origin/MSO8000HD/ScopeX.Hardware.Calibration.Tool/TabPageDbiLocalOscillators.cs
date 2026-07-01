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

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageDbiLocalOscillators : UserControl, IMainFormTabPage
    {
        public TabPageDbiLocalOscillators()
        {
            InitializeComponent();
            Init();
        }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private void Init()
        {
            dataGridView1.RowCount= 8;
            comboBoxCaliDataChannelSelectedChannel.SelectedIndex = 0;
        }
        public void RefreshData()
        {
            if (currInstrument == null)
                return;
            int channelIndex = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            for(int cmdIndex=0;cmdIndex<8; cmdIndex++)
            {
                dataGridView1.Rows[cmdIndex].Cells[0].Value = DbiLocalOscillators.Default[channelIndex, cmdIndex].CmdIndex;
                dataGridView1.Rows[cmdIndex].Cells[1].Value = new string(DbiLocalOscillators.Default[channelIndex, cmdIndex].CtrlWord);
            }
        }
        bool CheckValid(string s,int which)
        {
            bool result = which switch
            {
                0 => s.Length == 1,
                _ => s.Length == 4,
            };
            if (!result) return false;
            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        private void SendGridData(int rowIndex, int columnIndex, bool bNeedSend)
        {
            int channelIndex = comboBoxCaliDataChannelSelectedChannel.SelectedIndex;
            DataGridViewCell currCell = dataGridView1.Rows[rowIndex].Cells[columnIndex];
            string newData = (currCell.Value?.ToString()??"").Trim();
            string oldData = columnIndex switch
            {
                0 => new string(DbiLocalOscillators.Default[channelIndex, rowIndex].CmdIndex, 1),
                _ => new string(DbiLocalOscillators.Default[channelIndex, rowIndex].CtrlWord),
            };

            if (oldData == newData) //判断新旧值，避免连续发送
                return;
            if (!CheckValid(newData,columnIndex))
            {
                dataGridView1.Rows[rowIndex].Cells[columnIndex].Value = oldData;
                return;
            }

            switch (columnIndex)
            {
                case 0:
                    DbiLocalOscillators.Default[channelIndex, rowIndex].CmdIndex = newData[0];
                    break;
                default:
                    DbiLocalOscillators.Default[channelIndex, rowIndex].CtrlWord = newData.Substring(0,4).ToArray();
                    break;
            };
            if (bNeedSend && currInstrument != null)
                InstrumentInteract.CaliData_Send(currInstrument, CaliDataType);
        }
        public CaliDataType CaliDataType { get => CaliDataType.DbiLocalOscillators; }

        private void AllSaveToMemory()
        {
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 2; col++)
                    SendGridData(row, col, false);
        }
        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            comboBoxCaliDataChannelSelectedChannel.Enabled = (currInstrument != null);
            buttonCaliData_Channel_Send.Enabled = (currInstrument != null);
            buttonCaliData_Channel_LoadFromFile.Enabled = (currInstrument != null);
            buttonCaliData_Channel_SaveToFile.Enabled = (currInstrument != null);
            buttonCaliData_LoadDefualtValue_Channel.Enabled = (currInstrument != null);
            dataGridView1.ReadOnly= (currInstrument == null);
        }

        private void buttonCaliData_Channel_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            AllSaveToMemory();
            bool bResult = InstrumentInteract.CaliData_Send(currInstrument, CaliDataType);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SendGridData(e.RowIndex, e.ColumnIndex, checkBoxCaliData_ChannelAutoSend.Checked);
        }

        private void buttonCaliData_Channel_SaveToFile_Click(object sender, EventArgs e)
        {
            AllSaveToMemory();
            bool bResult = InstrumentInteract.CaliData_SaveData(currInstrument, CaliDataType);
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_Channel_LoadFromFile_Click(object sender, EventArgs e)
        {
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");

        }

        private void buttonCaliData_LoadDefualtValue_Channel_Click(object sender, EventArgs e)
        {
            DbiLocalOscillators.Default.LoadDefaultValue();
            RefreshData();
            MessageBox.Show("OK!");
        }

        private void comboBoxCaliDataChannelSelectedChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
