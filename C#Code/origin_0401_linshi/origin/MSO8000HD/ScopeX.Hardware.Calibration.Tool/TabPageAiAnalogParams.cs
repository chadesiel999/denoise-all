using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Data.Base.AnalogChannelEx;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageAiAnalogParams : UserControl, IMainFormTabPage
    {
        public TabPageAiAnalogParams()
        {
            InitializeComponent();
            InitDgvInof();
        }

        public CaliDataType CaliDataType { get => CaliDataType.TiadcPhaseOffsetGainParams; }

        private void InitDgvInof()
        {
            DgvInfo.Columns.Clear();

            DataGridViewTextBoxColumn namecolumn = new()
            {
                HeaderText = "Name",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 300,
                Frozen = true,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.DarkGray },
            };
            DgvInfo.Columns.Add(namecolumn);

            PropertyInfo[] tmp = typeof(AiAnalogChannelItem).GetProperties();
            for (Int32 columnId = 0; columnId < tmp.Length; columnId++)
            {
                DgvInfo.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = tmp[columnId].Name, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });
            }

            DgvInfo.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_DBI20G,
            ProductType.B23_DBI13G,
            ProductType.B24_AI20G,
            ProductType.B24_XunXin40G,
            ProductType.B21_DBI16G,
            ProductType.ForTest,
        };
        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;

        public void RefreshData()
        {
            isSendToOscilloscope = false;
            var viewnametable = AiAnalogChannelParams.Default.AllNames;
            if (DgvInfo.RowCount < viewnametable.Length)
            {
                DgvInfo.Rows.Add(viewnametable.Length - DgvInfo.RowCount);
            }
            for (Int32 rowid = 0; rowid < viewnametable.Length; rowid++)
            {
                DgvInfo.Rows[rowid].Cells[0].Value = viewnametable[rowid];
                DgvInfo.Rows[rowid].Cells[0].ReadOnly = true;

                for (Int32 columnid = 1; columnid < DgvInfo.Columns.Count; columnid++)
                {
                    PropertyInfo? info = typeof(AiAnalogChannelItem).GetProperty(DgvInfo.Columns[columnid].HeaderText);
                    if (info != null)
                    {
                        DgvInfo.Rows[rowid].Cells[columnid].Value = info.GetValue(AiAnalogChannelParams.Default[viewnametable[rowid]]);
                    }
                }
            }
            isSendToOscilloscope = true;
        }

        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
        }

        private void buttonSend2Origin_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.AiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void BtnReadFromOrigin_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }

            Boolean bResult = InstrumentInteract.CaliData_Get(this.currInstrument, CaliDataType.AiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonSave2OriginFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_SaveToFile(this.currInstrument, CaliDataType.AiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonLoadFromOriginFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("从文件中装载将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.AiAnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void DgvInfo_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvInfo.IsCurrentCellDirty)
                DgvInfo.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        bool isSendToOscilloscope = true;

        private void DgvInfo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvInfo.RowCount &&
                e.ColumnIndex >= 1 && e.ColumnIndex < DgvInfo.ColumnCount)
            {
                String? name = DgvInfo.Rows[e.RowIndex].Cells[0].Value?.ToString();
                PropertyInfo? info = typeof(AiAnalogChannelItem).GetProperty(DgvInfo.Columns[e.ColumnIndex].HeaderText);
                Boolean convertflag = Int32.TryParse(DgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out Int32 data);
                if (convertflag && name != null && info != null && info.PropertyType == typeof(Int32))
                {
                    Object tmp = AiAnalogChannelParams.Default[name];
                    info.SetValue(tmp, data);
                    AiAnalogChannelParams.Default[name] = (AiAnalogChannelItem)tmp;
                }
                if (checkBoxCaliData_TiAdc_AutoSend.Checked && isSendToOscilloscope)
                {
                    bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.AiAnalogParams);
                }
            }
        }
    }
}
