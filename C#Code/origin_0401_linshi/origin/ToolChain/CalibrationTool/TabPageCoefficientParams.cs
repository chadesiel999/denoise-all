using CSScripting;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageCoefficientParams : UserControl, IMainFormTabPage
    {
        public TabPageCoefficientParams()
        {
            InitializeComponent();
        }

        public CaliDataType CaliDataType { get => CaliDataType.CoefficientsParams; }

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

        private const String SELECTALL = "*";
        private List<String> DealedNames = new List<string>();

        private void InitDgvFiltrate()
        {
            Boolean tableequal = CheckStringTableEqual(CoefficientsParams.Default.AllNames, DealedNames.ToArray());
            if (tableequal)
                return;

            String[] oldpick = new String[DgvFiltrate.RowCount];
            for (Int32 rowid = 0; rowid < DgvFiltrate.RowCount; rowid++)
            {
                oldpick[rowid] = DgvFiltrate.Rows[rowid].Cells[0].Value.ToString() ?? SELECTALL;
            }

            DgvFiltrate.Rows.Clear();
            DealedNames.Clear();
            DealedNames.AddRange(CoefficientsParams.Default.AllNames);

            foreach (String name in DealedNames)
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
                curcell?.Items.Add(SELECTALL);
                if (rowid < oldpick.Length && (curcell?.Items.Contains(oldpick[rowid]) ?? false))
                {
                    DgvFiltrate.Rows[rowid].Cells[0].Value = oldpick[rowid];
                }
                else
                {
                    DgvFiltrate.Rows[rowid].Cells[0].Value = SELECTALL;
                }

            }
        }

        private List<String> GetNeedViewNames()
        {
            List<String> ans = new();
            String[] nametable = CoefficientsParams.Default.AllNames;
            for (Int32 nameid = 0; nameid < nametable.Length; nameid++)
            {
                String[] spiltname = nametable[nameid].Split(CaliConstants.NameSpiltChar);
                Boolean pickok = true;
                for (Int32 spiltid = 0; spiltid < spiltname.Length && spiltid < DgvFiltrate.RowCount; spiltid++)
                {
                    String pickstr = DgvFiltrate.Rows[spiltid].Cells[0].Value?.ToString() ?? SELECTALL;
                    if (pickstr != SELECTALL && pickstr != spiltname[spiltid])
                    {
                        pickok = false;
                        break;
                    }
                }
                if (pickok) ans.Add(nametable[nameid]);
            }
            return ans;
        }

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
            }

            if (DgvInfo.RowCount > 0)
            {
                _CurSelectViewName = DgvInfo.Rows[0].Cells[0].Value?.ToString() ?? String.Empty;
            }
        }

        private String _CurSelectViewName = String.Empty;

        private void RefreshRtbUsingData()
        {
            if (_CurSelectViewName.IsNotEmpty())
            {
                UpdateCoefficientData(richTextBoxUsingCaliData, CoefficientsParams.Default[_CurSelectViewName]);
            }
        }

        private void UpdateCoefficientData(RichTextBox rtb, Double[] data)
        {
            rtb.Clear();
            StringBuilder stringbuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                stringbuilder.AppendLine((i.ToString() + ".").PadRight(6, ' ') + data[i].ToString().PadLeft(10, ' '));
            rtb.Text = stringbuilder.ToString();
        }

        public void RefreshData()
        {
            InitDgvFiltrate();
            RefreshDgvInfo();
            RefreshRtbUsingData();
        }

        private IInstrumentSession? currInstrument = null;
        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
        }

        private void DgvInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvInfo.RowCount &&
                e.ColumnIndex >= 0 && e.ColumnIndex < DgvInfo.ColumnCount)
            {
                _CurSelectViewName = DgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? String.Empty;
                RefreshRtbUsingData();
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            CoefficientsParams.Default[_CurSelectViewName] = _CurLocalData.ToArray();
            RefreshRtbUsingData();
        }

        private List<Double> _CurLocalData = new List<Double>();

        private void BtnReadFromLocalFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog()
            {
                FileName = "openFileDialog1",
                Filter = "系数文本文件|*.txt",
            };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                String[] filecontentlines = System.IO.File.ReadAllLines(fd.FileName);
                _CurLocalData.Clear();
                for (Int32 i = 0; i < filecontentlines.Length; i++)
                {
                    if (Int32.TryParse(filecontentlines[i], out Int32 tmp))
                    {
                        _CurLocalData.Add(tmp);
                    }
                }
                UpdateCoefficientData(richTextBoxFileContent, _CurLocalData.ToArray());
            }
        }

        private void BtnReadFromOrigin_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }

            Boolean bResult = InstrumentInteract.CaliData_Get(this.currInstrument, CaliDataType);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonSend2Origin_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            Boolean bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("从文件中装载将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            Boolean bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_SaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            Boolean bresult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType);
            RefreshData();
            MessageBox.Show(bresult ? "OK!" : "错误！");
        }

        private void DgvFiltrate_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var tmp = DgvFiltrate.EditingControl as DataGridViewComboBoxEditingControl;
            if (tmp != null) tmp.DroppedDown = true;
        }

        private void DgvFiltrate_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvFiltrate.IsCurrentCellDirty)
                DgvFiltrate.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvFiltrate_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvFiltrate.RowCount &&
                e.ColumnIndex >= 0 && e.ColumnIndex < DgvFiltrate.ColumnCount)
            {
                RefreshData();
            }
        }
    }
}
