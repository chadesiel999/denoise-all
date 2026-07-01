using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Data.Base.AnalogChannelEx;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPage_DbiAnalogParams_Common : UserControl, IMainFormTabPage
    {
        public TabPage_DbiAnalogParams_Common()
        {
            InitializeComponent();
            toolTip1.SetToolTip(this.labelKeyID, "(可以使用 xxxx[以此开头]，*yyyy[以此结尾]，xxxx*yyy*zzz等格式，*表示任意匹配)");
            comboBoxIncludeOrNotInclude.SelectedIndex = 0;
        }
        public CaliDataType CaliDataType { get => CaliDataType.DbiAnalogParams_Common; }
        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B25_XunXin40GDBI,
            ProductType.ForTest,
        };

        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;
        private IInstrumentSession? currInstrument = null;

        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
            if (instrumentInteract != null)
            {
                StringBuilder sb = new StringBuilder();
                //foreach (var s in GlobalData.CurrentAnalogChannelItemsDefine.Constrain)
                //    sb.Append(s.ToString());
                richTextBox1.Text = sb.ToString();
            }
            else
                richTextBox1.Text = "";
        }

        public void RefreshData()
        {
            return;//?????
            InitDgvInof();
            isSendToOscilloscope = false;
            var viewNameTable = DbiAnalogParams_Common.Default.AllNames;
            if (GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems!=null)
            {
                DgvInfo.ColumnCount = GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems.Count + 1;
            }
         
            String filter = textBoxKeyIDFilter.Text.Trim();
            int rowCount = 0;
            foreach (String key in viewNameTable)
            {
                if (comboBoxIncludeOrNotInclude.SelectedIndex == 0)
                {
                    //包含
                    if (!((IMainFormTabPage)this).CheckMatch(key, filter))
                    {
                        continue;
                    }
                }
                else
                {
                    //不包含
                    if (((IMainFormTabPage)this).CheckMatch(key, filter))
                    {
                        continue;
                    }
                }
                rowCount++;
                if (DgvInfo.RowCount < rowCount)
                    DgvInfo.Rows.Add(1);
                DgvInfo.Rows[rowCount - 1].Cells[0].Value = key;
            }
            DgvInfo.RowCount = rowCount;

            for (Int32 rowid = 0; rowid < rowCount; rowid++)
            {
                String key = DgvInfo.Rows[rowid].Cells[0].Value.ToString();
                DgvInfo.Rows[rowid].Cells[0].ReadOnly = true;
                DbiAnalogChannelItem_Common currItemsValue = DbiAnalogParams_Common.Default[key];
                for (Int32 columnIndex = 1; columnIndex < DgvInfo.Columns.Count; columnIndex++)
                {
                    int storageAtIndex = GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems[columnIndex - 1].ItemIndex;
                    DgvInfo.Rows[rowid].Cells[columnIndex].Value = currItemsValue[storageAtIndex];
                }
            }
            isSendToOscilloscope = true;
            DgvInfo.Columns[0].HeaderCell.SortGlyphDirection = oldSortDirection;
            DgvInfo.Sort(DgvInfo.Columns[0], oldSortDirection == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
        }
        SortOrder oldSortDirection = SortOrder.Ascending;

        (String, int) StringToEnumValue(string fullName)
        {
            // 分割类型名和枚举成员名
            string[] parts = fullName.Split('.');
            if (parts.Length != 2)
                throw new ArgumentException("字符串格式应为 '枚举类型.成员名'");

            string typeName = parts[0];
            string memberName = parts[1];

            // 获取枚举类型
            Type? enumType = Type.GetType($"ScopeX.Hardware.Calibration.Data.Base.{typeName},ScopeX.Hardware.Calibration.Data.Base");

            if (enumType == null || !enumType.IsEnum)
                throw new ArgumentException($"找不到枚举类型: {typeName}");

            // 获取枚举成员的值
            object enumValue = Enum.Parse(enumType, memberName);
            return (memberName, (int)enumValue);
        }
        private void InitDgvInof()
        {
            if (GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems == null)
                return;
            if (DgvInfo.Columns.Count > 0)
            {
                oldSortDirection = DgvInfo.Columns[0].HeaderCell.SortGlyphDirection;
            }

            DgvInfo.Columns.Clear();

            DataGridViewTextBoxColumn namecolumn = new()
            {
                HeaderText = "Name",
                SortMode = DataGridViewColumnSortMode.Automatic,
                Width = 300,
                Frozen = true,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.DarkGray },
            };
            DgvInfo.Columns.Add(namecolumn);

            for (int viewItemIndex = 0; viewItemIndex < GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems.Count; viewItemIndex++)
            {
                String headerText = GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems[viewItemIndex].ItemName;
                DgvInfo.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = headerText, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader });
            }

            DgvInfo.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void buttonSend2Origin_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, this.CaliDataType);
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

            Boolean bResult = InstrumentInteract.CaliData_Get(this.currInstrument, this.CaliDataType);
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
            bool bResult = InstrumentInteract.CaliData_SaveToFile(this.currInstrument, this.CaliDataType);
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
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.DbiAnalogParams_Common);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void DgvInfo_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvInfo.IsCurrentCellDirty)
                DgvInfo.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        bool isSendToOscilloscope = true;


        private void buttonAddNewRow_Click(object sender, EventArgs e)
        {
            var viewNameTable = AiAnalogChannelParams.Default.AllNames;
            String newKey = textBoxNewKey.Text.Trim();
            if (string.IsNullOrEmpty(newKey))
            {
                MessageBox.Show("Key必须填写有效字符");
            }
            var existKey = Array.Find(viewNameTable, (o => o == newKey));
            if (existKey == null)
            {
                DbiAnalogChannelItem_Common newRow = new DbiAnalogChannelItem_Common();
                DbiAnalogParams_Common.Default[newKey] = newRow;
                RefreshData();
            }
            else
            {
                MessageBox.Show("该Key已经存在！");
            }
        }

        private void buttonDeleteCurrentRow_Click(object sender, EventArgs e)
        {
            if (DgvInfo.SelectedCells.Count > 0)
            {
                String key = DgvInfo.Rows[DgvInfo.SelectedCells[0].RowIndex].Cells[0].Value.ToString().Trim();
                DbiAnalogParams_Common.Default.Remove(key);
                RefreshData();
            }
        }

        private void textBoxKeyIDFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshData();
            }
        }

        private void buttonDefaultRows_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"这个将生成目前产品定义的全部缺省Key记录\r\n现有的数据将全部覆盖，你确认要执行此操作吗？", "重要提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            textBoxKeyIDFilter.Text = "";
            DbiAnalogParams_Common.Default.Clear();
            foreach (var perRecord in GlobalData.CurrentAnalogChannelAllDefault.AnalogChannelAllDefaultRecords)
            {
                DbiAnalogChannelItem_Common currItemsValue = new DbiAnalogChannelItem_Common();
                foreach (var item in perRecord.AnalogChannelItemNameAndValues)
                {
                    currItemsValue[item.ItemIndex] = item.ItemValue;
                }
                DbiAnalogParams_Common.Default[perRecord.KeyStr] = currItemsValue;

            }
            RefreshData();
            //FormGenerateDbiAnalogCommonDefaultRecords form = new FormGenerateDbiAnalogCommonDefaultRecords();
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    textBoxKeyIDFilter.Text = "";
            //    List<String> newKeys = form.ResultList;
            //    DbiAnalogParams_Common.Default.Clear();
            //    foreach (String key in newKeys)
            //    {
            //        DbiAnalogChannelItem_Common currItemsValue = new DbiAnalogChannelItem_Common();
            //        DbiAnalogParams_Common.Default[key] = currItemsValue;
            //    }
            //    RefreshData();
            //}
        }
        private void DgvInfo_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvInfo.RowCount &&
                e.ColumnIndex >= 1 && e.ColumnIndex < DgvInfo.ColumnCount)
            {
                String? name = DgvInfo.Rows[e.RowIndex].Cells[0].Value?.ToString();
                Boolean convertflag = Int64.TryParse(DgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out Int64 data);
                if (convertflag && name != null)
                {
                    DbiAnalogChannelItem_Common currItemsValue = DbiAnalogParams_Common.Default[name];
                    currItemsValue[GlobalData.CurrentAnalogChannelItemsDefine.AnalogChannelItems[e.ColumnIndex - 1].ItemIndex] = data;
                    DbiAnalogParams_Common.Default[name] = currItemsValue;
                }
                else
                {
                    MessageBox.Show("输入数据非法！");
                    RefreshData();
                    return;
                }
                if (checkBoxCaliData_TiAdc_AutoSend.Checked && isSendToOscilloscope)
                {
                    bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, this.CaliDataType);
                }
            }
        }

        private void comboBoxIncludeOrNotInclude_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currInstrument != null)
                RefreshData();
        }
    }
}
