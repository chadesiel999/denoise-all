using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.UserControls;
using SharpGen.Runtime;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewItem;

namespace ScopeX.U2
{
    internal class EventTablePage : ScopeXListViewEx, IProtocolView, IPageInfoView
    {
        public ChannelId Id { get; set; }
        public Boolean IsLoationTrigger { get; set; } = false;//定位触发帧
        public Int32 TriggerIndex { get; private set; } = -1;//触发帧
        private Boolean HasSelectTrigger = true;//是否精准定位触发帧
        private SerialProtocolType _CurrentType = SerialProtocolType.Close;
        private SerialProtocolType? _LastType = null;
        private List<ColumnHeader> _CurrentHeaders = new List<ColumnHeader>();
        private List<ColumnHeader> _LastHeaders = new List<ColumnHeader>();
        private List<ProtocolEventInfo> _CurrentEventInfos = new List<ProtocolEventInfo>();
        private List<String[]> _CurrentShowInfos = new List<String[]>();
        private Boolean _NeedUpdateCellWidth = false;
        private Int32 _CurrentRowCount = 10;
        private (Int64 start, Int64 end) _CurrentShowRowIndexs = (0, 0);

        private Boolean _FirstInit = true;

        private Boolean _ColumnNumCHangedFlag = false;  //列数有更改

        private Int32 _ShowDataColumns = 0;
        private Int32 ShowDataColumns
        {
            get
            {
                if (this.Width > InitWidth && InitWidth != 0)
                {
                    return _ShowDataColumns < 4 ? 4 : _ShowDataColumns;
                }
                else
                {
                    return 5;
                }
            }
            set
            {
                _ShowDataColumns = value;
            }
        }

        private Int32 InitWidth = 0;

        #region properties

        private DecodePrsnt DecodePrsnt { get; set; }

        public IProtocolPrsnt Presenter { get => DecodePrsnt.DecodeChPrsnt; set => _ = value; }

        IPageInfoPrsnt IView<IPageInfoPrsnt>.Presenter { get => DecodeApp.Default.PageInfo; set => throw new NotImplementedException(); }

        #endregion properties

        public EventTablePage()
        {
            //添加一个初始化的列
            int columnnum = 5;
            for (int i = 0; i < columnnum; i++)
            {
                _CurrentHeaders.Add(new ColumnHeader()
                {
                    Text = "Column" + i.ToString(),
                    Width = 100,
                });
                Columns.Add(new ColumnHeader()
                {
                    Text = "Column" + i.ToString(),
                    Width = 100,
                });
            }

            DecodeApp.Default.PageInfo.TryAddView(this);
        }


        public List<(Double StartPosition, Double EndPosition)> DecodeEventIndexes = new List<(Double StartPosition, Double EndPosition)>();

        public void ReLoadSource()
        {

        }

        public void SwitchInfoSource(ChannelId id)
        {
            if (!id.IsDecode())
            {
                return;
            }

            if (DecodePrsnt != null)
            {
                DecodePrsnt.DecodeChPrsnt.TryRemoveView(this);
            }

            if (Id != id)
            {
                SerialProtocolType? tempType = null;
                List<ColumnHeader> tempColumns = null;
                if (Columns != null && Columns.Count > 0)
                {
                    tempType = _CurrentType;
                    tempColumns = new List<ColumnHeader>();
                    foreach (ColumnHeader item in Columns)
                    {
                        tempColumns.Add(new ColumnHeader()
                        {
                            Text = item.Text,
                            Width = item.Width,
                        });
                    }
                }
                if (_LastType.HasValue)
                {
                    _CurrentType = _LastType.Value;
                    _CurrentHeaders = _LastHeaders;
                    _ColumnNumCHangedFlag = true;
                }
                _LastType = tempType;
                _LastHeaders = tempColumns;

                Id = id;
                DecodePrsnt = DecodeApp.Default.GetPresenter(id);
                DecodePrsnt.DecodeChPrsnt.TryAddView(this);
            }
        }

        //public void Init(ChannelId id)
        //{
        //    if (!id.IsDecode())
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        if (!_FirstInit)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            ChannelId = id;
        //            _FirstInit = false;
        //            DecodePrsnt = DecodeApp.Default.GetPresenter(id);
        //            DecodePrsnt.DecodeChPrsnt.TryAddView(this);
        //        }
        //    }
        //}

        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }

            switch (propertyName)
            {

            }
        }

        public void RefreshData()
        {
            if (DecodePrsnt == null ||
                DecodePrsnt.DecodeChPrsnt == null ||
                DecodePrsnt.DecodeChPrsnt.EventInfoTitles == null ||
                DecodePrsnt.DecodeChPrsnt.EventInfoTitles.Count == 0)
            {
                return;
            }
            CheckDecodeType();
            CheckDecodeEventInfos();
            this.BeginUpdate();
            if (_ColumnNumCHangedFlag)
            {
                UpdateColumns();
            }
            var isUpdate = !IsEquel(GetItemInfos(), _CurrentShowInfos) || _ColumnNumCHangedFlag;
            if (isUpdate)
            {
                UpdateTable();
            }

            _ColumnNumCHangedFlag = false;
            this.EndUpdate();
        }

        private void CheckDecodeType()
        {
            if (DecodePrsnt.DecodeChPrsnt.ProtocolType == _CurrentType && DecodePrsnt.DecodeChPrsnt.EventInfoTitles.SequenceEqual(_CurrentHeaders.Select(x => x.Text).ToList()) && !_NeedUpdateCellWidth)
                return;
            _CurrentType = DecodePrsnt.DecodeChPrsnt.ProtocolType;
            List<String> titles = DecodePrsnt.DecodeChPrsnt.EventInfoTitles.ToList();
            var width = TextRenderer.MeasureText(titles.OrderByDescending(s => s.Length).FirstOrDefault(), this.Font).Width + 10;
            width = AutoHeaderSize(width, titles.Count);
            _ColumnNumCHangedFlag = false;

            string[] texts = _CurrentHeaders.Cast<ColumnHeader>()
                                       .Select(header => header.Text)
                                       .ToArray();
            ShowDataColumns = titles.Count;
            if (texts.Length != titles.Count || !texts.SequenceEqual(titles) || _NeedUpdateCellWidth)
            {
                _CurrentHeaders.Clear();
                _CurrentHeaders.Add(new ColumnHeader());
                _CurrentHeaders[0].Text = titles[0];
                _CurrentHeaders[0].Width = 80;
                //匹配最新的标题
                for (int i = 1; i < titles.Count; i++)
                {
                    _CurrentHeaders.Add(new ColumnHeader()
                    {
                        Text = titles[i],
                        Width = width,
                    });
                    _ColumnNumCHangedFlag = true;
                }
            }
            _NeedUpdateCellWidth = false;
        }

        private int AutoHeaderSize(int width, int count)
        {
            var autowidth = width;
            var sum = width * (count - 1) + 80;//index固定80；
            this.Dock = DockStyle.None;
            if (sum < InitWidth)
            {
                (this.Parent as Panel).AutoScroll = false;
                this.MinimumSize = new Size(0, 0);
                this.Width = InitWidth;
                autowidth = (this.Width - 80) / (count - 1);
            }
            else
            {
                int w = 80 + (count - 1) * width;
                (this.Parent as Panel).AutoScroll = true;
                this.MinimumSize = new Size(w, 0);
                this.Width = w;
                //this.Height -= 5;
            }
            return autowidth;
        }

        public void LoadInitWidth(int width)
        {
            if (InitWidth != width)
            {
                InitWidth = width;
                _NeedUpdateCellWidth = true;
            }
        }

        public void Reload()
        {
            _NeedUpdateCellWidth = true;
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
            DecodePrsnt.DecodeChPrsnt.TryRemoveView(this);
            DecodeApp.Default.PageInfo.TryRemoveView(this);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Height > 20)
            {
                _CurrentRowCount = Height / (int)(RowHeight * 1.35) - 1;
            }
        }


        private void CheckDecodeEventInfos()
        {
            _CurrentEventInfos = DecodePrsnt.DecodeChPrsnt.ProtocolEvents.Where(x => x != null && x.StartPosition >= 0 && x.StartPosition < Constants.MAX_XPOS_IDX).ToList();

            if (IsLoationTrigger)
            {
                Int32 index = -1;
                Double min_startorend = Double.MaxValue;
                Int32 min_index = -1;
                for (int i = 0; i < _CurrentEventInfos.Count; i++)
                {
                    if (_CurrentEventInfos[i].StartTimeByPs <= 0 && _CurrentEventInfos[i].EndTimeByPs >= 0)
                    {
                        index = i;
                        break;
                    }

                    if (min_startorend > Math.Abs(_CurrentEventInfos[i].StartTimeByPs))
                    {
                        min_startorend = Math.Abs(_CurrentEventInfos[i].StartTimeByPs);
                        min_index = i;
                    }
                    if (min_startorend > Math.Abs(_CurrentEventInfos[i].EndPosition))
                    {
                        min_startorend = Math.Abs(_CurrentEventInfos[i].EndPosition);
                        min_index = i;
                    }
                }

                var pagenum = -1;
                if (index != -1)
                {
                    pagenum = (index + 1) / _CurrentRowCount;
                    pagenum++;
                    TriggerIndex = (index + 1) % _CurrentRowCount;
                    HasSelectTrigger = false;
                }
                else if (min_index != -1)
                {
                    pagenum = (min_index + 1) / _CurrentRowCount;
                    TriggerIndex = (min_index + 1) % _CurrentRowCount;
                    HasSelectTrigger = false;
                }
                else
                {
                    HasSelectTrigger = true;
                }

                if (pagenum > 0 && DecodeApp.Default.PageInfo.CurrentPageNum != pagenum)
                {
                    DecodeApp.Default.PageInfo.UpdateCurrentPage(pagenum);
                }
                else if (DecodeApp.Default.PageInfo.CurrentPageNum == pagenum)
                {
                    if (!HasSelectTrigger)
                    {
                        if (Items[TriggerIndex - 1].Selected)
                        {
                            Items[TriggerIndex - 1].Selected = false;
                        }
                        Items[TriggerIndex - 1].Selected = true;

                        HasSelectTrigger = true;
                    }
                }
                else
                {
                    HasSelectTrigger = true;
                }

                IsLoationTrigger = false;
            }

            _CurrentShowRowIndexs = DecodeApp.Default.PageInfo.PreUpdate(_CurrentEventInfos.Count, _CurrentRowCount, DecodeApp.Default.PageInfo.CurrentPageNum);
            //_CurrentEventInfos = _CurrentEventInfos
            //    .Skip((int)_CurrentShowRowIndexs.start) // 跳过开始的N个元素
            //    .Take((int)_CurrentShowRowIndexs.end - (int)_CurrentShowRowIndexs.start).ToList(); 
            _CurrentShowInfos.Clear();
            //提取更新信息
            // 使用 for 循环来赋值索引
            for (int index = 0; index < _CurrentEventInfos.Count; index++)
            {
                _CurrentEventInfos[index].Index = index;
            }
            _CurrentShowInfos = GetDecodeData(_CurrentEventInfos, Columns.Count, _CurrentShowRowIndexs);
        }

        private void UpdateColumns()
        {
            try
            {
                Items.Clear();
                Columns.Clear();
                for (int i = 0; i < _CurrentHeaders.Count; i++)
                {
                    Columns.Add(new ColumnHeader());
                }
                for (int i = 0; i < _CurrentHeaders.Count; i++)
                {
                    Columns[i].Text = _CurrentHeaders[i].Text;
                    Columns[i].Width = _CurrentHeaders[i].Width;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void UpdateTable()
        {
            try
            {
                if (Items.Count > 0 && Items[0].SubItems.Count != _CurrentHeaders.Count)
                {
                    Items.Clear();
                }
                //更新表格信息
                for (Int32 rowindex = 0; rowindex < _CurrentShowInfos.Count; rowindex++)
                {
                    //替换内容/新增内容
                    if (rowindex >= Items.Count)
                    {
                        Items.Add(new ListViewItem(_CurrentShowInfos[rowindex]));
                    }

                    for (Int32 columnindex = 0; columnindex < Columns.Count; columnindex++)
                    {
                        if (_CurrentShowInfos.First().Count() != Columns.Count)
                        {
                            return;
                        }

                        String text = _CurrentShowInfos[rowindex][columnindex];
                        Int32 index = 10;
                        while (GetTextWidth(text, Items[rowindex].SubItems[columnindex].Font) > Columns[columnindex].Width && Columns[columnindex].Width != 0)
                        {
                            index--;
                            text = _CurrentShowInfos[rowindex][columnindex].Substring(0, (Int32)(_CurrentShowInfos[rowindex][columnindex].Length * index / 10.0)) + "...";
                        }
                        Items[rowindex].SubItems[columnindex].Text = text;

                        if (!HasSelectTrigger)
                        {
                            if (TriggerIndex - 1 == rowindex)
                            {
                                Items[rowindex].Selected = true;
                            }
                            else
                            {
                                Items[rowindex].Selected = false;
                            }
                        }
                    }
                }
                //删除多余的信息
                for (Int32 rowindex = Items.Count; rowindex > _CurrentShowInfos.Count; rowindex--)
                {
                    Items.RemoveAt(rowindex - 1);
                }
                DecodeApp.Default.PageInfo.UpdateInfo(_CurrentEventInfos.Count, _CurrentRowCount, DecodeApp.Default.PageInfo.CurrentPageNum);
                if (!HasSelectTrigger)
                {
                    HasSelectTrigger = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private String GetHexString(Byte[] data, UInt32 bitcount)
        {
            String temp = String.Empty;
            Int32 bytecount = (Int32)Math.Ceiling(bitcount / 8f);
            List<Byte> tempbytes = new List<Byte>();
            if (data.Length < bytecount)
            {
                tempbytes.AddRange(Enumerable.Repeat<Byte>(0, bytecount - data.Length));
            }
            else
            {

            }
            tempbytes.AddRange(data);
            for (Int32 i = 0; i < bytecount; i++)
            {
                Byte lenght = 0;
                if (i == 0)
                {
                    lenght = (Byte)(bitcount % 8);
                }
                else
                {
                    lenght = 7;
                }
                if (lenght == 0)
                {
                    lenght = 7;
                }
                Byte tempvalue = (Byte)(tempbytes[i] & (Byte)(Math.Pow(2, lenght + 1) - 1));
                temp += Convert.ToString(tempvalue, 16).PadLeft((Int32)Math.Ceiling(lenght / 4d), '0') + " ";
            }
            return temp.ToUpper();
        }

        private String ByteArrayToBinaryString(Byte[] data, UInt32 bitcount)
        {
            String temp = "";
            Int32 bytecount = (Int32)Math.Ceiling(bitcount / 8f);
            List<Byte> tempbytes = new List<Byte>();
            if (data.Length < bytecount)
            {
                tempbytes.AddRange(Enumerable.Repeat<Byte>(0, bytecount - data.Length));
            }
            else
            {

            }
            tempbytes.AddRange(data);

            for (Int32 i = 0; i < bytecount; i++)
            {
                Byte lenght = 0;
                if (i == 0)
                {
                    lenght = (Byte)(bitcount % 8);
                }
                else
                {
                    lenght = 8;
                }
                if (lenght == 0)
                {
                    lenght = 8;
                }
                Byte tempvalue = (Byte)(tempbytes[i] & (Byte)(Math.Pow(2, lenght) - 1));
                temp += Convert.ToString(tempvalue, 2).PadLeft(lenght, '0');
            }
            return temp + "b";
        }

        private String ByteArrayToDecimalString(Byte[] data, UInt32 bitcount)
        {
            StringBuilder decimalStringBuilder = new StringBuilder();
            foreach (Byte b in data)
            {
                decimalStringBuilder.Append(b.ToString());
                decimalStringBuilder.Append(" "); // 添加空格分隔每个字节
            }
            return decimalStringBuilder.ToString().Trim();
        }

        private String ByteArrayToASCIIString(Byte[] data)
        {
            //StringBuilder asciiStringBuilder = new StringBuilder();
            //foreach (byte b in data)
            //{
            //    if (b >= 32 && b <= 126) // 可打印的ASCII字符范围
            //    {
            //        asciiStringBuilder.Append(Convert.ToChar(b));
            //    }
            //    else
            //    {
            //        asciiStringBuilder.Append("."); // 用点表示不可打印字符
            //    }
            //}
            //return asciiStringBuilder.ToString();
            return data.GetASCIIStr();
        }

        private Int32 GetTextWidth(String text, Font font)
        {
            Size textSize = TextRenderer.MeasureText(text, font);

            return textSize.Width;
        }


        public List<String[]> GetDecodeData(List<ProtocolEventInfo> eventInfos, int colunmcount, (Int64 start, Int64 end) range)
        {
            DecodeEventIndexes.Clear();
            List<String[]> result = new List<string[]>();
            //提取更新信息
            if (eventInfos == null ||
                eventInfos.Count == 0)
            {
                result.Add(Enumerable.Repeat("--", colunmcount).ToArray());
            }
            else
            {
                DecodeEventIndexes.AddRange(eventInfos.Select(x => (x.StartPosition, x.EndPosition)));

                result.AddRange(eventInfos
                 .Skip((int)range.start) // 跳过开始的N个元素
                 .Take((int)range.end - (int)range.start).Select(info =>
                 {
                     List<String> itemcontent = new List<String>();
                     if (Double.IsNaN(info.StartTimeByPs))
                     {
                         itemcontent.Add("--");
                         itemcontent.Add("--");
                     }
                     else
                     {
                         itemcontent.Add((info.Index + 1).ToString());
                         itemcontent.Add(ScopeX.Controls.Common.Helper.SIHelper.ValueChangeToSI(info.StartTimeByPs / 1E+12, 2, "s"));
                         for (int i = 0; i < info.EventInofs.Count; i++)
                         {
                             var temp = String.Empty;
                             if (info.EventInofs[i].BitCount == 0)//文字
                             {
                                 temp = Encoding.Default.GetString(info.EventInofs[i].Data);
                                 var extrainfos = info.ExtraInfos.Where(x => x.InfoIndex == i).ToList();
                                 if (extrainfos.Count > 0)
                                 {
                                     String[] extrastr = new String[extrainfos.Count];
                                     for (int ii = 0; ii < extrainfos.Count; ii++)
                                     {
                                         extrastr[ii] = extrainfos[ii].BitCount switch
                                         {
                                             0 => Encoding.Default.GetString(extrainfos[ii].Data),
                                             1 => Convert.ToBoolean(extrainfos[ii].Data[0]).ToString(),
                                             _ => GetDecodeDataStr((extrainfos[ii].Data, extrainfos[ii].BitCount))
                                         };
                                     }
                                     temp = String.Format(temp, extrastr);
                                 }
                             }
                             else if (info.EventInofs[i].BitCount == 1)//True or False
                             {
                                 temp = Convert.ToBoolean(info.EventInofs[i].Data[0]).ToString();
                             }
                             else//数字(数据量大的时候容易界面卡死)
                             {
                                 temp = GetDecodeDataStr(info.EventInofs[i]);
                             }
                             itemcontent.Add(temp);

                         }
                         info.EventInofs.ForEach(datas =>
                         {

                         });
                     }
                     //检查表头数量和内容子项长度是否一致,不足填零
                     for (int i = itemcontent.Count; i < colunmcount; i++)
                     {
                         itemcontent.Add(String.Empty);
                     }
                     return itemcontent.ToArray();
                 }));
            }

            return result;
        }

        private String GetDecodeDataStr((Byte[] Data, UInt32 BitCount) info)
        {
            String datastr = string.Empty;
            var temp = GetHexString(info.Data, info.BitCount);
            DecodeDisplayMode format = DecodeDisplayMode.Hex;
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Id, out var prsnt) && prsnt is DecodePrsnt decodePrsnt)
            {
                format = decodePrsnt.Format;
            }

            try
            {
                switch (format)
                {
                    case DecodeDisplayMode.Dec://十进制
                                               //datastr = Convert.ToInt32(temp, 16).ToString();
                        datastr = ByteArrayToDecimalString(info.Data, info.BitCount);
                        break;
                    case DecodeDisplayMode.Binary:
                        //datastr = Convert.ToString(Convert.ToInt32(temp, 16), 2).ToString();
                        datastr = ByteArrayToBinaryString(info.Data, info.BitCount);
                        break;
                    case DecodeDisplayMode.ASCII:
                        //datastr =((char)int.Parse(temp, System.Globalization.NumberStyles.HexNumber)).ToString();
                        datastr = ByteArrayToASCIIString(info.Data);
                        break;
                    case DecodeDisplayMode.Hex:
                    default:
                        datastr = temp;
                        break;
                }
            }
            catch (Exception e)
            {
                datastr = temp;
            }

            return datastr;
        }
        public DataTable GetDataTable()
        {
            if (Items == null || Items.Count <= 0)
                return null;

            DataTable dt = new DataTable();
            var titles = DecodePrsnt.DecodeChPrsnt.EventInfoTitles;
            if (titles == null || !titles.Any())
                return null;

            var columnCount = titles.Count;
            foreach (var tt in titles)
                dt.Columns.Add(tt);
            var temp = DecodePrsnt.DecodeChPrsnt.ProtocolEvents.Where(x => x != null).ToList();
            var result = GetDecodeData(temp, Columns.Count, (0, temp.Count));
            DataRow dr;
            foreach (var item in result)
            {
                if (item == null)
                    continue;

                dr = dt.NewRow();
                for (int i = 0; i < item.Length; i++)
                {
                    if (item.Length <= i)
                        break;

                    dr[dt.Columns[i]] = item[i];
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }


        private List<String[]> GetItemInfos()
        {
            var currentinfos = new List<String[]>();

            foreach (ListViewItem item in Items)
            {
                var info = item.SubItems.Cast<ListViewSubItem>().Select(x => x.Text).ToArray();
                currentinfos.Add(info);
            }
            return currentinfos;
        }

        private Boolean IsEquel(List<String[]> @old, List<String[]> @new)
        {
            // 首先检查两个列表的长度是否相同
            if (@old.Count != @new.Count)
            {
                return false;
            }
            // 检查对应位置的数组是否相等
            for (int i = 0; i < @old.Count; i++)
            {
                // 使用 SequenceEqual 来比较两个数组的内容
                if (!@old[i].SequenceEqual(@new[i]))
                {
                    return false;
                }
            }
            // 如果所有对应的数组都相等，则返回 true
            return true;
        }

        public void UpdateThresholdUnit()
        {
        }
    }
}