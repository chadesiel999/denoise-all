using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;
using System.Security.Cryptography;
using Ivi.Visa;
using System.Xml.Linq;
using System.Threading;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    public partial class TabPageTiadcPhaseOffsetGain : UserControl, IMainFormTabPage
    {
        public TabPageTiadcPhaseOffsetGain()
        {
            InitializeComponent();
            InitDvgInfo();
            InitDvgError();
            InitDgvGodParams();
            DgvVersionAndDate.Rows.Add();
            (DgvVersionAndDate.Rows[0].Cells[0] as DataGridViewComboBoxCell)?.Items.AddRange(Enum.GetNames<TiadcGodVersionEnum>());
            (DgvVersionAndDate.Rows[0].Cells[1] as DataGridViewComboBoxCell)?.Items.AddRange(Enum.GetNames<TiadcItemVersionEnum>());
            DgvVersionAndDate.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            RefreshData();
        }

        public CaliDataType CaliDataType { get => CaliDataType.TiadcPhaseOffsetGainParams; }

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

        private IInstrumentSession? currInstrument = null;

        private Type? _OldGodType = null;
        private Type? _OldItemType = null;

        public List<ProductType> Used4ProductTypes => _Used4ProductTypes;

        private void InitDvgInfo()
        {
            if (_OldItemType != null && _OldItemType == TiadcPhaseOffsetGainParams.Default.TiadcItemType)
            {
                return;
            }
            _OldItemType = TiadcPhaseOffsetGainParams.Default.TiadcItemType;
            DgvInfo.Columns.Clear();
            PropertyInfo[] tmp = TiadcPhaseOffsetGainParams.Default.TiadcItemType.GetProperties();

            DataGridViewTextBoxColumn namecolumn = new()
            {
                HeaderText = "Name",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Width = 300,
                Frozen = true,
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.DarkGray },
            };
            DgvInfo.Columns.Add(namecolumn);

            DgvInfo.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "Checked", SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });

            for (Int32 columnId = 0; columnId < tmp.Length; columnId++)
            {
                DgvInfo.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = tmp[columnId].Name, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });
            }

            DgvInfo.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void InitDgvGodParams()
        {
            if (_OldGodType != null && _OldGodType == TiadcPhaseOffsetGainParams.Default.TiadcGodType)
            {
                return;
            }
            _OldGodType = TiadcPhaseOffsetGainParams.Default.TiadcGodType;
            DgvGodParams.Columns.Clear();
            PropertyInfo[] tmp = TiadcPhaseOffsetGainParams.Default.TiadcGodType.GetProperties();
            for (Int32 columnId = 0; columnId < tmp.Length; columnId++)
            {
                DgvGodParams.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = tmp[columnId].Name, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });
            }
            DgvGodParams.Rows.Add();
            DgvGodParams.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void InitDvgError()
        {
            TiadcErrorColumnId[] allerrortype = Enum.GetValues<TiadcErrorColumnId>();
            for (Int32 columnid = 0; columnid < allerrortype.Length; columnid++)
            {
                DgvError.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = allerrortype[columnid].ToString(), SortMode = DataGridViewColumnSortMode.NotSortable });
            }
            DgvError.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
            Boolean tableequal = CheckStringTableEqual(TiadcPhaseOffsetGainParams.Default.AllNames, _DealedNames.ToArray());
            if (tableequal)
                return;

            String[] oldpick = new String[DgvFiltrate.RowCount];
            for (Int32 rowid = 0; rowid < DgvFiltrate.RowCount; rowid++)
            {
                oldpick[rowid] = DgvFiltrate.Rows[rowid].Cells[0].Value.ToString() ?? _SelectAll;
            }

            DgvFiltrate.Rows.Clear();
            _DealedNames.Clear();
            _DealedNames.AddRange(TiadcPhaseOffsetGainParams.Default.AllNames);

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

        private List<String> GetNeedViewNames()
        {
            List<String> ans = new();
            String[] nametable = TiadcPhaseOffsetGainParams.Default.AllNames;
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

                for (Int32 columnid = 2; columnid < DgvInfo.Columns.Count; columnid++)
                {
                    PropertyInfo? info = TiadcPhaseOffsetGainParams.Default.TiadcItemType.GetProperty(DgvInfo.Columns[columnid].HeaderText);
                    if (info != null)
                    {
                        DgvInfo.Rows[rowid].Cells[columnid].Value = info.GetValue(TiadcPhaseOffsetGainParams.Default[viewnametable[rowid]]);
                    }

                }
            }
        }

        private void RefreshDgvGodParams()
        {
            for (Int32 columnid = 0; columnid < DgvGodParams.ColumnCount; columnid++)
            {
                PropertyInfo? info = TiadcPhaseOffsetGainParams.Default.TiadcGodType.GetProperty(DgvGodParams.Columns[columnid].HeaderText);
                if (info != null)
                {
                    DgvGodParams.Rows[0].Cells[columnid].Value = info.GetValue(TiadcPhaseOffsetGainParams.Default.GodParams);
                }
            }
        }

        private void RefreshDgvVersionAdnDate()
        {
            if (DgvVersionAndDate.RowCount > 0)
            {
                DgvVersionAndDate.Rows[0].Cells[0].Value = TiadcPhaseOffsetGainParams.Default.GodVersion.ToString();
                DgvVersionAndDate.Rows[0].Cells[1].Value = TiadcPhaseOffsetGainParams.Default.ItemVersion.ToString();
                DgvVersionAndDate.Rows[0].Cells[2].Value = TiadcPhaseOffsetGainParams.Default.CalcTimeStr;
            }
        }


        bool isSendToOscilloscope = true;

        public void RefreshData()
        {
            isSendToOscilloscope = false;
            InitDvgInfo();
            InitDgvGodParams();
            InitDgvFiltrate();
            RefreshDgvInfo();
            RefreshDgvGodParams();
            RefreshDgvVersionAdnDate();
            isSendToOscilloscope = true;
        }

        public void SetInstrumentInteract(IInstrumentSession? instrumentInteract)
        {
            currInstrument = instrumentInteract;
        }

        private void DgvInfo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < DgvInfo.RowCount)
            {
                if (e.ColumnIndex == 1)
                {
                    Int32 dgverrorrowid = 0;
                    for (Int32 rowid = 0; rowid < DgvInfo.Rows.Count; rowid++)
                    {
                        if (DgvInfo.Rows[rowid].Cells[e.ColumnIndex].Value is Boolean &&
                            (Boolean)DgvInfo.Rows[rowid].Cells[e.ColumnIndex].Value)
                        {
                            if (dgverrorrowid >= DgvError.RowCount)
                                DgvError.Rows.Add();
                            DgvError.Rows[dgverrorrowid].Cells[(Int32)TiadcErrorColumnId.Name].Value = DgvInfo.Rows[rowid].Cells[0].Value;
                            dgverrorrowid++;
                        }
                    }
                    for (; dgverrorrowid < DgvError.RowCount; dgverrorrowid++)
                    {
                        DgvError.Rows.RemoveAt(dgverrorrowid);
                    }
                }
                else if (e.ColumnIndex > 1 && e.ColumnIndex < DgvInfo.ColumnCount)
                {
                    String? name = DgvInfo.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    PropertyInfo? info = TiadcPhaseOffsetGainParams.Default.TiadcItemType.GetProperty(DgvInfo.Columns[e.ColumnIndex].HeaderText);
                    Boolean convertflag = Int32.TryParse(DgvInfo.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out Int32 data);
                    if (convertflag && name != null && info != null && info.PropertyType == typeof(Int32))
                    {
                        Object tmp = TiadcPhaseOffsetGainParams.Default[name];
                        info.SetValue(tmp, data);
                        TiadcPhaseOffsetGainParams.Default[name] = tmp;
                    }
                }
                if (checkBoxCaliData_TiAdc_AutoSend.Checked && isSendToOscilloscope)
                {
                    bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
                }
            }

        }

        private void DgvInfo_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DgvInfo.IsCurrentCellDirty)
                DgvInfo.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void buttonCaliData_TiAdc_LoadFromFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            if (MessageBox.Show("从文件中装载将覆盖现有数据，\r\n您确认要执行此操作吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            bool bResult = InstrumentInteract.CaliData_LoadFromFile(currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_TiAdc_SaveToFile_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_SaveData(this.currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCaliData_TiAdc_Send_Click(object sender, EventArgs e)
        {
            if (currInstrument == null)
            {
                MessageBox.Show("请先连接仪器！");
                return;
            }
            bool bResult = InstrumentInteract.CaliData_Send(this.currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
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

            Boolean bResult = InstrumentInteract.CaliData_Get(this.currInstrument, CaliDataType.TiadcPhaseOffsetGainParams);
            RefreshData();
            MessageBox.Show(bResult ? "OK!" : "错误！");
        }

        private void buttonCalcError_Click(object sender, EventArgs e)
        {
            Calc();
        }

        private void SplitData(List<List<UInt16[]>> channelData, String model, String channelid, Int32 index)
        {
             _sampleByM_Sps = 10_000;
            _signalFreqByMHz = double.Parse(textBoxSignalFreqByMHz.Text);
            if (channelData == null || channelData.Count == 0)
            {
                return;
            }
            //List<UInt16[]> fintdatas1 = new List<UInt16[]>();
            //List<UInt16[]> fintdatas2 = new List<UInt16[]>();
            //List<UInt16[]> fintdatas3 = new List<UInt16[]>();
            List<UInt16[]> disdatas = new List<UInt16[]>();
            List<UInt16[]> fintdatas = new List<UInt16[]>();
            switch (channelid)
            {
                case "C1":
                    //if (model == "All-10G")
                    //{
                    //    foreach (var item in channelData)
                    //    {
                    //        fintdatas.Add(item[0]);
                    //        disdatas.Add(item[1]);
                    //    }
                    //}
                    //else
                    {
                        foreach (var item in channelData)
                        {
                            ExtractWaveData(out fintdatas, out disdatas, item, 0);
                            //ExtractWaveData4adc(out fintdatas1, out fintdatas2, out fintdatas3, out disdatas, item, 0);
                        }
                    }
                    break;
                case "C2":
                    //if (model == "All-10G")
                    //{
                    //    foreach (var item in channelData)
                    //    {
                    //        fintdatas.Add(item[1]);
                    //        disdatas.Add(item[0]);
                    //    }
                    //}
                    //else
                    {
                        foreach (var item in channelData)
                        {
                            ExtractWaveData(out fintdatas, out disdatas, item, 1);
                            //ExtractWaveData4adc(out fintdatas1, out fintdatas2, out fintdatas3, out disdatas, item, 1);

                        }
                    }
                    break;
                case "C3":
                    //if (model == "All-10G")
                    //{
                    //    foreach (var item in channelData)
                    //    {
                    //        fintdatas.Add(item[2]);
                    //        disdatas.Add(item[3]);
                    //    }
                    //}
                    //else
                    {
                        foreach (var item in channelData)
                        {
                            ExtractWaveData(out fintdatas, out disdatas, item, 2);
                            //ExtractWaveData4adc(out fintdatas1, out fintdatas2, out fintdatas3, out disdatas, item, 2);

                        }
                    }
                    break;
                case "C4":
                    //if (model == "All-10G")
                    //{
                    //    foreach (var item in channelData)
                    //    {
                    //        fintdatas.Add(item[3]);
                    //        disdatas.Add(item[2]);
                    //    }
                    //}
                    //else
                    {
                        foreach (var item in channelData)
                        {
                            ExtractWaveData(out fintdatas, out disdatas, item, 3);
                            //ExtractWaveData4adc(out fintdatas1, out fintdatas2, out fintdatas3, out disdatas, item, 3);

                        }
                    }
                    break;
            }
            //var result0 = CaliSineFitDelta(CalaWaveOffsetGainPhase(disdatas), CalaWaveOffsetGainPhase(fintdatas2), 1);

            //var result1 = CaliSineFitDelta(CalaWaveOffsetGainPhase(fintdatas1), CalaWaveOffsetGainPhase(fintdatas2), 3);
            //var result2 = CaliSineFitDelta(CalaWaveOffsetGainPhase(fintdatas2), CalaWaveOffsetGainPhase(fintdatas2), 0);
            //var result3 = CaliSineFitDelta(CalaWaveOffsetGainPhase(fintdatas3), CalaWaveOffsetGainPhase(fintdatas2), 2);
            var result0 = CaliSineFitDelta(CalaWaveOffsetGainPhase(disdatas), CalaWaveOffsetGainPhase(disdatas), 0);
            var result1 = CaliSineFitDelta(CalaWaveOffsetGainPhase(fintdatas), CalaWaveOffsetGainPhase(disdatas), 1);
            //DgvError.Rows[index].Cells[1].Value = result0.Phase;
            //DgvError.Rows[index].Cells[2].Value = result0.Offset;
            //DgvError.Rows[index].Cells[3].Value = result0.Gain;
            //DgvError.Rows[index + 1].Cells[1].Value = result1.Phase;
            //DgvError.Rows[index + 1].Cells[2].Value = result1.Offset;
            //DgvError.Rows[index + 1].Cells[3].Value = result1.Gain;
            //DgvError.Rows[index + 2].Cells[1].Value = result2.Phase;
            //DgvError.Rows[index + 2].Cells[2].Value = result2.Offset;
            //DgvError.Rows[index + 2].Cells[3].Value = result2.Gain;
            //DgvError.Rows[index + 3].Cells[1].Value = result3.Phase;
            //DgvError.Rows[index + 3].Cells[2].Value = result3.Offset;
            //DgvError.Rows[index + 3].Cells[3].Value = result3.Gain;
            DgvError.Rows[index].Cells[1].Value = result0.Phase;
            DgvError.Rows[index].Cells[2].Value = result0.Offset;
            DgvError.Rows[index].Cells[3].Value = result0.Gain;
            DgvError.Rows[index + 1].Cells[1].Value = result1.Phase;
            DgvError.Rows[index + 1].Cells[2].Value = result1.Offset;
            DgvError.Rows[index + 1].Cells[3].Value = result1.Gain;
            //if (model != "All-10G")
            //{
            //    DgvError.Rows[index + 1].Cells[1].Value = 0;
            //    DgvError.Rows[index + 1].Cells[2].Value = 0;
            //    DgvError.Rows[index + 1].Cells[3].Value = 0;
            //}
            //if ((Math.Abs(result1.Offset) < 0.5) || (Math.Abs(result2.Offset) < 0.5) || (Math.Abs(result3.Offset) < 0.5))
            //{
            //    using (StreamWriter sw = new StreamWriter($"{result1.Offset}.txt"))
            //    {
            //        //        //if (channelid == "C1")
            //        //        //{
            //        //        //    sw.WriteLine(String.Join(Environment.NewLine, channelData[1][0].ToArray()));
            //        //        //}
            //        //        //else if (channelid == "C2")
            //        //        //{
            //        //        //    sw.WriteLine(String.Join(Environment.NewLine, channelData[1][1].ToArray()));
            //        //        //}
            //        //        //else if (channelid == "C3")
            //        //        //{
            //        //        //    sw.WriteLine(String.Join(Environment.NewLine, channelData[1][2].ToArray()));
            //        //        //}
            //        //        //else
            //        //        //{
            //        //        //    sw.WriteLine(String.Join(Environment.NewLine, channelData[1][3].ToArray()));
            //        //        //}
            //        //    }

            //    }
            //}
        }
            private static void ExtractWaveData(out List<UInt16[]> fintdatas, out List<UInt16[]> disdatas, List<UInt16[]> item, Int32 channelid)
            {
                fintdatas = new List<UInt16[]>();
                disdatas = new List<UInt16[]>();
                List<UInt16> fintdata = new List<UInt16>();
                List<UInt16> disdata = new List<UInt16>();
                for (int i = 0; i < item[channelid].Length; i++)
                {
                    if (i % 2 != 0)
                    {
                        fintdata.Add(item[channelid][i]);
                    }
                    else
                    {
                        disdata.Add(item[channelid][i]);
                    }
                }
                fintdatas.Add(fintdata.ToArray());
                disdatas.Add(disdata.ToArray());
            }

            private static void ExtractWaveData4adc(out List<UInt16[]> fintdatas1, out List<UInt16[]> fintdatas2, out List<UInt16[]> fintdatas3, out List<UInt16[]> disdatas, List<UInt16[]> item, Int32 channelid)
            //private static void ExtractWaveData4adc(out List<UInt16[]> fintdatas1,  out List<UInt16[]> disdatas, List<UInt16[]> item, Int32 channelid)
            {
                fintdatas1 = new List<UInt16[]>();
                fintdatas2 = new List<UInt16[]>();
                fintdatas3 = new List<UInt16[]>();
                disdatas = new List<UInt16[]>();
                List<UInt16> fintdata1 = new List<UInt16>();
                List<UInt16> fintdata2 = new List<UInt16>();
                List<UInt16> fintdata3 = new List<UInt16>();
                List<UInt16> disdata = new List<UInt16>();
                for (int i = 0; i < item[channelid].Length; i++)
                {
                    if (i % 4 == 0) fintdata2.Add(item[channelid][i]);
                    else if (i % 4 == 1) disdata.Add(item[channelid][i]);
                    else if (i % 4 == 2) fintdata3.Add(item[channelid][i]);
                    else fintdata1.Add(item[channelid][i]);
                    //if (i % 2 == 0) disdata.Add(item[channelid][i]);
                    //else fintdata1.Add(item[channelid][i]);
                    //if (i % 4 == 0) fintdata1.Add(item[channelid][i]);
                    //else if (i % 4 == 1) fintdata3.Add(item[channelid][i]);
                    //else if (i % 4 == 2) disdata.Add(item[channelid][i]);
                    //else fintdata2.Add(item[channelid][i]);
                    //if (i % 4 == 0) disdata.Add(item[channelid][i]);
                    //else if (i % 4 == 1) fintdata1.Add(item[channelid][i]);
                    //else if (i % 4 == 2) fintdata2.Add(item[channelid][i]);
                    //else fintdata3.Add(item[channelid][i]);


                }
                fintdatas1.Add(fintdata1.ToArray());
                fintdatas2.Add(fintdata2.ToArray());
                fintdatas3.Add(fintdata3.ToArray());
                disdatas.Add(disdata.ToArray());
            }

        private Double _sampleByM_Sps = 10_000;
        private Double _signalFreqByMHz = 100d;

        private WaveOffsetGainPhase CalaWaveOffsetGainPhase(List<UInt16[]> channelDatas)
        {
            List<WaveOffsetGainPhase> waveoffsetgainphases = new List<WaveOffsetGainPhase>();
            foreach (var item in channelDatas)
            {
                waveoffsetgainphases.Add(SineFitFunc.SineFit(item, _sampleByM_Sps, _signalFreqByMHz));
            }
            WaveOffsetGainPhase waveoffsetgainphase = new WaveOffsetGainPhase();
            waveoffsetgainphase.Offset = waveoffsetgainphases.Average(p => p.Offset);
            waveoffsetgainphase.Phase = waveoffsetgainphases.Average(p => p.Phase);
            waveoffsetgainphase.Gain = waveoffsetgainphases.Average(p => p.Gain);
            return waveoffsetgainphase;
        }

        public void Calc()
        {
            ///20G模式 2 1 4 3
            ///10G模式 1 2 3 4
            if (DgvError.RowCount <= 0)
                return;
            Dictionary<string, List<ushort>> keyValuePairs = new Dictionary<string, List<ushort>>();
            string key = string.Empty;
            string adcModel = string.Empty;
            List<String> channels = new List<string>();
            for (int i = 0; i < DgvError.Rows.Count; i++)
            {
                string[] dgvinfo = DgvError.Rows[i].Cells[0].Value?.ToString().Split('_');
                adcModel = dgvinfo[0];//交织模式
                if (!channels.Contains(dgvinfo[1])) //计算的通道
                {
                    channels.Add(dgvinfo[1]);
                }
            }
            Int32 avg = 10;
            Int32.TryParse(tb_Avg.Text.Trim(), out avg);
            //avg = 1;
            CommonMethod.RefreshConstDataFromServer(currInstrument);
            List<List<ushort[]>> channeldatas = new List<List<ushort[]>>();
            for (int i = 0; i < avg; i++)
            {
                var channeldata = InstrumentInteract.Factory_WaveData_Channel(currInstrument, 6_000, 4);
                Thread.Sleep(50);
                if (channeldata != null)
                {
                    channeldatas.Add(channeldata);
                }
            }
            Int32 index = 0;
            foreach (var channel in channels)
            {
                index = index * 3;
                SplitData(channeldatas, adcModel, channel, index);
                index++;
            }
        }

        #region 计算三参数

        private WaveOffsetGainPhase CaliSineFit(List<UInt16> fitData, List<UInt16> disData)
        {
            //相位理论误差
            Double theoryDelta_pS = 1000d / Double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text);
            theoryDelta_pS = textBoxTotalADCSamplingRadioByGSPS.Text == "20" ? 50 : 0;
            WaveOffsetGainPhase fintwaveoffsetgainphase = SineFitFunc.SineFit(fitData.ToArray(), _sampleByM_Sps, _signalFreqByMHz);
            WaveOffsetGainPhase waveoffsetgainphase = SineFitFunc.SineFit(disData.ToArray(), _sampleByM_Sps, _signalFreqByMHz);
            Int32 adcIndex = waveoffsetgainphase.Phase == fintwaveoffsetgainphase.Phase ? 0 : 1;
            WaveOffsetGainPhase result = new WaveOffsetGainPhase();
            result.Gain = 100 * (waveoffsetgainphase.Gain - fintwaveoffsetgainphase.Gain) / fintwaveoffsetgainphase.Gain;
            result.Offset = (waveoffsetgainphase.Offset - fintwaveoffsetgainphase.Offset);
            Double phaseerror_ps = ((waveoffsetgainphase.Phase - fintwaveoffsetgainphase.Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / _signalFreqByMHz / (2 * Math.PI) + adcIndex * theoryDelta_pS;
            if (phaseerror_ps > 1000_000 / _signalFreqByMHz / 2)
                phaseerror_ps -= 1000_000 / _signalFreqByMHz;
            else if (phaseerror_ps < -1000_000 / _signalFreqByMHz / 2)
                phaseerror_ps += 1000_000 / _signalFreqByMHz;
            result.Phase = phaseerror_ps;
            return result;
        }

        private WaveOffsetGainPhase CaliSineFitDelta(WaveOffsetGainPhase fintwaveoffsetgainphase, WaveOffsetGainPhase waveoffsetgainphase)
        {
            //相位理论误差
            Double theoryDelta_pS = 1000d / Double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text);
            theoryDelta_pS = textBoxTotalADCSamplingRadioByGSPS.Text == "20" ? 50 : 0;
            Int32 adcIndex = waveoffsetgainphase.Phase == fintwaveoffsetgainphase.Phase ? 0 : 1;
            WaveOffsetGainPhase result = new WaveOffsetGainPhase();
            result.Gain = 100 * (waveoffsetgainphase.Gain - fintwaveoffsetgainphase.Gain) / fintwaveoffsetgainphase.Gain;
            result.Offset = (waveoffsetgainphase.Offset - fintwaveoffsetgainphase.Offset);
            Double phaseerror_ps = ((waveoffsetgainphase.Phase - fintwaveoffsetgainphase.Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / _signalFreqByMHz / (2 * Math.PI) + adcIndex * theoryDelta_pS;
            if (phaseerror_ps > 1000_000 / _signalFreqByMHz / 2)
                phaseerror_ps -= 1000_000 / _signalFreqByMHz;
            else if (phaseerror_ps < -1000_000 / _signalFreqByMHz / 2)
                phaseerror_ps += 1000_000 / _signalFreqByMHz;
            result.Phase = phaseerror_ps;
            return result;
        }
        private WaveOffsetGainPhase CaliSineFitDelta(WaveOffsetGainPhase fintwaveoffsetgainphase, WaveOffsetGainPhase waveoffsetgainphase, Int32 deltanum)
        {
            //相位理论误差
            Double theoryDelta_pS = 1000d / Double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text);
            theoryDelta_pS = textBoxTotalADCSamplingRadioByGSPS.Text == "20" ? 50 : 0;
            Int32 adcIndex = waveoffsetgainphase.Phase == fintwaveoffsetgainphase.Phase ? 0 : 1;
            WaveOffsetGainPhase result = new WaveOffsetGainPhase();
            result.Gain = 100 * (waveoffsetgainphase.Gain - fintwaveoffsetgainphase.Gain) / fintwaveoffsetgainphase.Gain;
            result.Offset = (waveoffsetgainphase.Offset - fintwaveoffsetgainphase.Offset);
            Double phaseerror_ps = ((waveoffsetgainphase.Phase - fintwaveoffsetgainphase.Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / _signalFreqByMHz / (2 * Math.PI) + deltanum * theoryDelta_pS;
            if (phaseerror_ps > 1000_000 / _signalFreqByMHz / 2)
                phaseerror_ps -= 1000_000 / _signalFreqByMHz;
            else if (phaseerror_ps < -1000_000 / _signalFreqByMHz / 2)
                phaseerror_ps += 1000_000 / _signalFreqByMHz;
            result.Phase = phaseerror_ps;
            return result;
        }

        private bool CaliSineFit(Dictionary<string, List<ushort>> keyValuePairs, double sampleByM_Sps, double signalFreqByMHz, double inputsignalFreqByMHz, int fitDelta_pS)
        {
            //相位理论误差
            double theoryDelta_pS = 1000d / double.Parse(textBoxTotalADCSamplingRadioByGSPS.Text);
            theoryDelta_pS = textBoxTotalADCSamplingRadioByGSPS.Text == "20" ? 50 : 0;

            Dictionary<int, WaveOffsetGainPhase> keyValueWaveOffsetGainPhases = new Dictionary<int, WaveOffsetGainPhase>();
            WaveOffsetGainPhase[] waveOffsetGainPhasesList = new WaveOffsetGainPhase[keyValuePairs.Count];
            int i = 0;
            foreach (var item in keyValuePairs)
            {
                keyValueWaveOffsetGainPhases.Add(i, SineFitFunc.SineFit(item.Value.ToArray(), sampleByM_Sps, signalFreqByMHz));
                i++;
            }
            int index = 0;
            foreach (var item in keyValuePairs)
            {
                string fintKey = keyValuePairs.First().Key;
                if (keyValuePairs.Count != 2)
                {
                    fintKey = item.Key.Replace("Adc0", "Adc1");
                }
                else
                {
                    theoryDelta_pS = 0;
                }

                WaveOffsetGainPhase fintWaveOffsetGainPhase = SineFitFunc.SineFit(keyValuePairs[fintKey].ToArray(), sampleByM_Sps, signalFreqByMHz);
                WaveOffsetGainPhase WaveOffsetGainPhase = SineFitFunc.SineFit(keyValuePairs[item.Key].ToArray(), sampleByM_Sps, signalFreqByMHz);
                int adcIndex = WaveOffsetGainPhase.Phase == fintWaveOffsetGainPhase.Phase ? 0 : 1;
                double PhaseError_pS = ((WaveOffsetGainPhase.Phase - fintWaveOffsetGainPhase.Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalFreqByMHz / (2 * Math.PI) + adcIndex * theoryDelta_pS;
                double GainError = 100 * (WaveOffsetGainPhase.Gain - fintWaveOffsetGainPhase.Gain) / fintWaveOffsetGainPhase.Gain;
                double OffsetError = (WaveOffsetGainPhase.Offset - fintWaveOffsetGainPhase.Offset);

                if (PhaseError_pS > 1000_000 / inputsignalFreqByMHz / 2)
                    PhaseError_pS -= 1000_000 / inputsignalFreqByMHz;
                else if (PhaseError_pS < -1000_000 / inputsignalFreqByMHz / 2)
                    PhaseError_pS += 1000_000 / inputsignalFreqByMHz;
                DgvError.Rows[index].Cells[1].Value = PhaseError_pS;
                DgvError.Rows[index].Cells[2].Value = OffsetError;
                DgvError.Rows[index].Cells[3].Value = GainError;
                if (Math.Abs(PhaseError_pS) >= fitDelta_pS)
                {
                    return false;
                }
                if (PhaseError_pS != 0)
                {
                    using (StreamWriter sw = new StreamWriter(fintKey + ".txt", true))
                    {
                        sw.WriteLine(PhaseError_pS);
                    }
                }
                index++;

            }
            return true;
        }

        #endregion

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
                    PropertyInfo? info = TiadcPhaseOffsetGainParams.Default.TiadcGodType.GetProperty(DgvGodParams.Columns[e.ColumnIndex].HeaderText);
                    Boolean convertflag = Int32.TryParse(DgvGodParams.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out Int32 data);
                    if (convertflag && info != null && info.PropertyType == typeof(Int32))
                    {
                        Object tmp = TiadcPhaseOffsetGainParams.Default.GodParams;
                        info.SetValue(tmp, data);
                        TiadcPhaseOffsetGainParams.Default.GodParams = tmp;
                    }
                }
            }
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
                    Boolean godok = Enum.TryParse<TiadcGodVersionEnum>(versionstr, out TiadcGodVersionEnum godversion);
                    if (godok)
                    {
                        TiadcPhaseOffsetGainParams.Default.UpdateGodVersion(godversion);
                    }
                    break;
                case 1:
                    Boolean itemok = Enum.TryParse<TiadcItemVersionEnum>(versionstr, out TiadcItemVersionEnum itemversion);
                    if (itemok)
                    {
                        TiadcPhaseOffsetGainParams.Default.UpdateItemVersion(itemversion);
                    }
                    break;
            }
            RefreshData();
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

        private void DgvFiltrate_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var tmp = DgvFiltrate.EditingControl as DataGridViewComboBoxEditingControl;
            if (tmp != null) tmp.DroppedDown = true;
        }

        private void textBoxTotalADCSamplingRadioByGSPS_TextChanged(object sender, EventArgs e)
        {

        }
    }

    internal enum TiadcErrorColumnId
    {
        Name,
        Phase,
        Offset,
        Gain,
    }
}
