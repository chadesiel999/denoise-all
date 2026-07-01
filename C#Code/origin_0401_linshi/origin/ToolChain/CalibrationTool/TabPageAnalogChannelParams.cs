using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
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

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageAnalogChannelParams : UserControl, IMainFormTabPage
    {
        public TabPageAnalogChannelParams()
        {
            InitializeComponent();

            InitDgvGodParams();
            InitDvgInfo();
            DgvVersionAndDate.Rows.Add();
            (DgvVersionAndDate.Rows[0].Cells[0] as DataGridViewComboBoxCell)?.Items.AddRange(Enum.GetNames<AnalogGodVersionEnum>());
            (DgvVersionAndDate.Rows[0].Cells[1] as DataGridViewComboBoxCell)?.Items.AddRange(Enum.GetNames<AnalogItemVersionEnum>());
            DgvVersionAndDate.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private Type? _OldGodType = null;
        private void InitDgvGodParams()
        {
            if (_OldGodType != null && _OldGodType == AnalogChannelParams.Default.GodType)
            {
                return;
            }
            _OldGodType = AnalogChannelParams.Default.GodType;
            DgvGodParams.Columns.Clear();
            PropertyInfo[] tmp = AnalogChannelParams.Default.GodType.GetProperties();
            for (Int32 columnId = 0; columnId < tmp.Length; columnId++)
            {
                DgvGodParams.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = tmp[columnId].Name, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells, });
            }
            DgvGodParams.Rows.Add();
            DgvGodParams.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private Type? _OldItemType = null;
        private void InitDvgInfo()
        {
            if (_OldItemType != null && _OldItemType == AnalogChannelParams.Default.ItemType)
            {
                return;
            }
            _OldItemType = AnalogChannelParams.Default.ItemType;
            DgvInfo.Columns.Clear();
            PropertyInfo[] tmp = AnalogChannelParams.Default.ItemType.GetProperties();

            DataGridViewTextBoxColumn namecolumn = new()
            {
                HeaderText = "Name",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 300,
                Frozen = true,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.DarkGray },
            };
            DgvInfo.Columns.Add(namecolumn);

            for (Int32 columnId = 0; columnId < tmp.Length; columnId++)
            {
                DgvInfo.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = tmp[columnId].Name, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });
            }

            DgvInfo.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private const String _SelectAll = "*";
        private List<String> _DealedNames = new();

        private Boolean CheckStringTableEqual(String[] source1, String[] source2)
        {
            if (source1.Length != source2.Length)
                return false;

            for (Int32 i = 0; i < source1.Length; i++)
            {
                if (source1[i] != source2[i])
                    return false;
            }
            return true;
        }

        private void InitDgvFiltrate()
        {
            Boolean tableequal = CheckStringTableEqual(AnalogChannelParams.Default.AllNames, _DealedNames.ToArray());
            if (tableequal)
                return;

            String[] oldpick = new String[DgvFiltrate.RowCount];
            for (Int32 rowid = 0; rowid < DgvFiltrate.RowCount; rowid++)
            {
                oldpick[rowid] = DgvFiltrate.Rows[rowid].Cells[0].Value.ToString() ?? _SelectAll;
            }

            DgvFiltrate.Rows.Clear();
            _DealedNames.Clear();
            _DealedNames.AddRange(AnalogChannelParams.Default.AllNames);

            foreach (String name in _DealedNames)
            {
                String[] tmp = name.Split(CaliConstants.NameSpiltChar);
                if (DgvFiltrate.RowCount < tmp.Length)
                {
                    DgvFiltrate.Rows.Add(tmp.Length - DgvFiltrate.RowCount);
                }

                for (Int32 i = 0; i < tmp.Length; i++)
                {
                    DataGridViewComboBoxCell? curcell = DgvFiltrate.Rows[i].Cells[0] as DataGridViewComboBoxCell;
                    if (curcell == null)
                        continue;
                    if (!curcell.Items.Contains(tmp[i]))
                    {
                        curcell.Items.Add(tmp[i]);
                    }
                }
            }

            for (Int32 rowid = 0; rowid < DgvFiltrate.RowCount; rowid++)
            {
                DataGridViewComboBoxCell? curcell = DgvFiltrate.Rows[rowid].Cells[0] as DataGridViewComboBoxCell;
                curcell?.Items.Add(_SelectAll);
                if (rowid < oldpick.Length && (curcell?.Items.Contains(oldpick[rowid]) ?? false))
                {
                    DgvFiltrate.Rows[rowid].Cells[0].Value = oldpick[rowid];
                }
                else
                {
                    DgvFiltrate.Rows[rowid].Cells[0].Value = _SelectAll;
                }

            }
        }

        public CaliDataType CaliDataType { get => CaliDataType.AnalogParams; }

        private List<ProductType> _Used4ProductTypes = new List<ProductType>()
        {
            ProductType.Base,
            ProductType.B21_JinHui_PXI,
            ProductType.B21_HB8G,
            ProductType.B21_HD4G,
            ProductType.B21_DBI20G,
            ProductType.B21_DBI16G,
            ProductType.B21_MD8G,
            ProductType.B21_HR1G,
            ProductType.B21_MS2G,
            ProductType.ForTest,
            ProductType.JiHe_MSO8000X,
            ProductType.JiHe_MSO7000HD,
        };

        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;

        private void RefreshDgvGodParams()
        {
            for (Int32 columnid = 0; columnid < DgvGodParams.ColumnCount; columnid++)
            {
                PropertyInfo? info = AnalogChannelParams.Default.GodType.GetProperty(DgvGodParams.Columns[columnid].HeaderText);
                if (info != null)
                {
                    DgvGodParams.Rows[0].Cells[columnid].Value = info.GetValue(AnalogChannelParams.Default.GodParams);
                }
            }
        }

        private void RefreshDgvVersionAndCalcTime()
        {
            if (DgvVersionAndDate.RowCount > 0)
            {
                DgvVersionAndDate.Rows[0].Cells[0].Value = AnalogChannelParams.Default.GodVersion.ToString();
                DgvVersionAndDate.Rows[0].Cells[1].Value = AnalogChannelParams.Default.ItemVersion.ToString();
                DgvVersionAndDate.Rows[0].Cells[2].Value = AnalogChannelParams.Default.CalcTimeStr;
            }
        }

        private List<String> GetNeedViewNames()
        {
            List<String> ans = new();
            String[] nametable = AnalogChannelParams.Default.AllNames;
            for (Int32 nameid = 0; nameid < nametable.Length; nameid++)
            {
                String[] spiltname = nametable[nameid].Split(CaliConstants.NameSpiltChar);
                Boolean pickok = true;
                for (Int32 spiltid = 0; spiltid < spiltname.Length && spiltid < DgvFiltrate.RowCount; spiltid++)
                {
                    String pickstr = DgvFiltrate.Rows[spiltid].Cells[0].Value?.ToString() ?? _SelectAll;
                    if (pickstr != _SelectAll && pickstr != spiltname[spiltid])
                    {
                        pickok = false;
                        break;
                    }
                }
                if (pickok) ans.Add(nametable[nameid]);
            }
            return ans;
        }

        bool isSendToOscilloscope = true;

        private void RefreshDgvInfo()
        {
            List<String> viewnametable = GetNeedViewNames();

            if (DgvInfo.RowCount != viewnametable.Count)
            {
                if (DgvInfo.RowCount < viewnametable.Count)
                    DgvInfo.Rows.Add(viewnametable.Count - DgvInfo.RowCount);
                else
                {
                    for (Int32 rowid = DgvInfo.RowCount - 1; rowid >= viewnametable.Count; rowid--)
                    {
                        DgvInfo.Rows.RemoveAt(rowid);
                    }
                }
            }
            for (Int32 rowid = 0; rowid < viewnametable.Count; rowid++)
            {
                DgvInfo.Rows[rowid].Cells[0].Value = viewnametable[rowid];
                DgvInfo.Rows[rowid].Cells[0].ReadOnly = true;

                for (Int32 columnid = 1; columnid < DgvInfo.Columns.Count; columnid++)
                {
                    PropertyInfo? info = AnalogChannelParams.Default.ItemType.GetProperty(DgvInfo.Columns[columnid].HeaderText);
                    if (info != null)
                    {
                        DgvInfo.Rows[rowid].Cells[columnid].Value = info.GetValue(AnalogChannelParams.Default[viewnametable[rowid]]);
                    }

                }
            }
        }

        public void RefreshData()
        {
            isSendToOscilloscope = false;
            InitDgvGodParams();
            InitDvgInfo();
            InitDgvFiltrate();
            RefreshDgvGodParams();
            RefreshDgvVersionAndCalcTime();
            RefreshDgvInfo();
            isSendToOscilloscope = true;
        }

        private IInstrumentSession? currInstrument = null;

        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
        }

        private void DgvGodParams_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvGodParams.IsCurrentCellDirty)
                DgvGodParams.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvGodParams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvGodParams.RowCount &&
                e.ColumnIndex >= 0 && e.ColumnIndex < DgvGodParams.ColumnCount)
            {
                {
                    PropertyInfo? info = AnalogChannelParams.Default.GodType.GetProperty(DgvGodParams.Columns[e.ColumnIndex].HeaderText);
                    Boolean convertflag = Int32.TryParse(DgvGodParams.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out Int32 data);
                    if (convertflag && info != null && info.PropertyType == typeof(Int32))
                    {
                        Object tmp = AnalogChannelParams.Default.GodParams;
                        info.SetValue(tmp, data);
                        AnalogChannelParams.Default.GodParams = tmp;
                    }
                }
            }
        }

        private void DgvInfo_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvInfo.IsCurrentCellDirty)
                DgvInfo.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvInfo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvInfo.RowCount &&
                e.ColumnIndex >= 1 && e.ColumnIndex < DgvInfo.ColumnCount)
            {
                String? name = DgvInfo.Rows[e.RowIndex].Cells[0].Value?.ToString();
                PropertyInfo? info = AnalogChannelParams.Default.ItemType.GetProperty(DgvInfo.Columns[e.ColumnIndex].HeaderText);
                Boolean convertflag = Int32.TryParse(DgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out Int32 data);
                if (convertflag && name != null && info != null && info.PropertyType == typeof(Int32))
                {
                    Object tmp = AnalogChannelParams.Default[name];
                    info.SetValue(tmp, data);
                    AnalogChannelParams.Default[name] = tmp;
                }
                if (checkBoxCaliData_TiAdc_AutoSend.Checked && isSendToOscilloscope)
                {
                    bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.AnalogParams);
                }
            }
        }

        private void buttonSend2Origin_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.AnalogParams);
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

            Boolean bResult = InstrumentInteract.CaliData_Get(this.currInstrument, CaliDataType.AnalogParams);
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
            bool bResult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.AnalogParams);
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
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.AnalogParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void DgvVersionAndDate_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvVersionAndDate.IsCurrentCellDirty)
                DgvVersionAndDate.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvVersionAndDate_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != 0)
                return;

            String? versionstr = DgvVersionAndDate.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
            switch (e.ColumnIndex)
            {
                case 0:
                    Boolean godok = Enum.TryParse<AnalogGodVersionEnum>(versionstr, out AnalogGodVersionEnum godversion);
                    if (godok)
                    {
                        AnalogChannelParams.Default.UpdateGodVersion(godversion);
                    }
                    break;
                case 1:
                    Boolean itemok = Enum.TryParse<AnalogItemVersionEnum>(versionstr, out AnalogItemVersionEnum itemversion);
                    if (itemok)
                    {
                        AnalogChannelParams.Default.UpdateItemVersion(itemversion);
                    }
                    break;
            }
            RefreshData();
        }

        private void DgvFiltrate_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvFiltrate.RowCount &&
                e.ColumnIndex >= 0 && e.ColumnIndex < DgvFiltrate.ColumnCount)
            {
                RefreshData();
            }
        }

        private void DgvFiltrate_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvFiltrate.IsCurrentCellDirty)
                DgvFiltrate.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvFiltrate_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var tmp = DgvFiltrate.EditingControl as DataGridViewComboBoxEditingControl;
            if (tmp != null) tmp.DroppedDown = true;
        }
    }
}
