namespace ScopeX.U2.Search
{
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.Presenter;
    using ScopeX.Core.Tools;
    using ScopeX.U2.BaseControl;
    using ScopeX.U2.File;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    public partial class SearchInfoForm : FloatForm, IPageInfoView, IEmbeddableDataView, IDataExportView
    {
        private Int64 _RowCount = 10;//固定显示10行
        private Semaphore @lock = new Semaphore(1, 1);
        private List<String> _Header = new List<String>();//todo
        private List<ListViewItem> _Infos = new List<ListViewItem>();//存储需要更新的数据
        private DataTableFigure dtFigure = null;

        public Boolean IsDataTableFiguer
        {
            get;
            private set;
        } = false;

        public Boolean IsClosed
        {
            get;
            private set;
        } = false;

        public SearchInfoForm(SearchItemPrsnt prsnt, PageInfoPrsnt pageinfo)
        {
            Presenter = pageinfo;
            Presenter?.TryAddView(this);
            PageControl = new PageControl(Presenter);
            InitializeComponent();
            FixedToolIconInfos[2].Icon = Properties.Resources.FormEmbed;
            FixedToolIconInfos[3].Icon = Properties.Resources.Save;
            FixedToolIconInfos[3].IsShow = true;
            SearchItem = prsnt;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            TlpSnapshot.SizeChanged += TlpSnapshot_SizeChanged;
            PageControl.Updater = () =>
            {
                PageControlFirst = true;
                NeedUpdate = true;
            };
        }

        private void TlpSnapshot_SizeChanged(object sender, EventArgs e)
        {
            if (TlpSnapshot.Width > 0 && TlpSnapshot.Height > 0)
            {
                PageControl.Size = new Size(TlpSnapshot.Width, 30);
                PageControl.Location = new Point(0, TlpSnapshot.Height - PageControl.Height);
                LvSearch.Location = new Point(0, 0);
                LvSearch.Size = new Size(TlpSnapshot.Width, TlpSnapshot.Height - PageControl.Height);
                _RowCount = LvSearch.Height / ImageListPad.ImageSize.Height - 1;
                LvSearch.Items.Clear();
                NeedUpdate = true;
            }
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            InitView();
        }

        private void InitView()
        {
            Text = SearchItem.Name + " " + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoShiJianLieBiao");
            Title = SearchItem.Name + " " + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoShiJianLieBiao");
            Index.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BianHao");
            Type.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LeiXing");
            Position.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiZhi");
            Delta.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChaZhi");
            Description.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MiaoShu");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            //!!!Close embeded figure
            var ef = GetDataView?.FindForm();
            if (ef != this)
            {
                ef?.Close();
            }
            SearchItem.EventEnable = false;
            Presenter?.TryRemoveView(this);
            if (TmUpdate != null)
            {
                TmUpdate.Stop();
                TmUpdate.Elapsed -= TmUpdate_Tick;
                TmUpdate.Enabled = false;
            }
            IsClosed = true;
            //SearchApp.Default.FoucsItem.EventEnable = false;
            SearchApp.Default.HideInfoForm();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        public PageInfoPrsnt Presenter { get; set; }

        public SearchItemPrsnt SearchItem { get; set; }

        public Control GetDataView => TlpSnapshot;

        private Size _ListSize;

        public Size LastSize { get; set; }
        IPageInfoPrsnt IView<IPageInfoPrsnt>.Presenter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Size _IndependentSize; //独立控件的大小

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            _IndependentSize = TlpSnapshot.PreferredSize;
            _ListSize = LvSearch.Size;
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                UpdateInfo();
            }
        }

        private void LvSearch_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (TriggerPrsnt.State == SysState.Stop && LvSearch.SelectedItems.Count > 0)
            {
                SearchApp.Default.UpdateNavigationIndex(int.Parse(LvSearch.SelectedItems[0].Text) - 1);
            }
        }

        public Boolean NeedUpdate = false;

        public Boolean PageControlFirst = true;//暂停态下，页码优先还是导航索引优先

        private void TmUpdate_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }
            void TickEvent()
            {
                if (TriggerPrsnt.State != SysState.Stop || NeedUpdate)//非暂停态，或者更新标志位
                {
                    UpdateInfo();
                    NeedUpdate = false;
                    PageControlFirst = true;
                }
            }
        }

        private void UpdateInfo()
        {
            if (SearchItem == null)
                return;


            var exstr = " " + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SouSuoShiJianLieBiao");
            if (Text != SearchItem.Name + exstr)
            {
                Text = SearchItem.Name + exstr;
                if(IsDataTableFiguer)
                {
                    dtFigure.Title = Text;
                }
            }
            try
            {
                var results = SearchItem.GetSearchResults();
                var source = DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Where(x => x.Id == SearchItem.Source).FirstOrDefault();
                Int64? selectindexpage = null;
                ListViewItem selectitem = null;
                bool isupdate = NeedUpdate || SearchItem.IsNewResult;
                if (results != null && source != null)
                {
                    if (TriggerPrsnt.State == SysState.Stop && SearchApp.Default.NavigationIndex >= 0 && !PageControlFirst)
                    {
                        selectindexpage = (SearchApp.Default.NavigationIndex + 1) % _RowCount == 0
                            ? (SearchApp.Default.NavigationIndex + 1) / _RowCount
                            : (SearchApp.Default.NavigationIndex + 1) / _RowCount + 1;
                    }

                    if (selectindexpage != null && selectindexpage.HasValue && !isupdate)
                    {
                        isupdate = selectindexpage.Value != Presenter.CurrentPageNum;
                    }

                    if (isupdate)
                    {
                        var indexs = Presenter.PreUpdate(results.GetLength(1), _RowCount, (selectindexpage != null && selectindexpage.HasValue) ? selectindexpage.Value : Presenter.CurrentPageNum);

                        int length = results.GetLength(1);
                        List<ListViewItem> list = new List<ListViewItem>(length);
                        Double position;
                        Double delta = indexs.StartIndex <= 0 ? 0 : (SearchItem.TransToTmb(results[0, indexs.StartIndex - 1]));
                        String deltastr;
                        String positionstr;
                        String description = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(SearchItem.GetTypeDescription());
                        for (Int64 i = indexs.StartIndex; i < indexs.EndIndex; i++)
                        {
                            position = SearchItem.TransToTmb(results[0, i]);
                            //position = (results[0, i] - source.Sampling.PosIndexBymDiv) * source.Pack.Properties.SampInterval;
                            delta = i == 0 ? 0 : position - delta;
                            deltastr = new Quantity(delta, SearchItem.ResultPrefix, SearchItem.ResultTmbUnitName).ValueChangeToSI(3).ToFormat("0.000");
                            positionstr = new Quantity(position, source.Pack.Properties.TmbUnit.Prefix, SearchItem.ResultTmbUnitName).ValueChangeToSI(3).ToFormat("0.000");
                            var value = new ListViewItem(new string[]
                            {
                         (i+1).ToString(),
                         ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(SearchItem.Type.ToString()),
                        positionstr,
                        deltastr,
                       description
                            }, -1);
                            list.Add(value);
                            if (TriggerPrsnt.State == SysState.Stop && i == SearchApp.Default.NavigationIndex)
                            {
                                selectitem = value;
                            }
                            delta = position;
                        }
                        _Infos = list;
                        Presenter.UpdateInfo(results.GetLength(1), _RowCount, (selectindexpage != null && selectindexpage.HasValue) ? selectindexpage.Value : Presenter.CurrentPageNum);
                    }
                }
                else
                {
                    _Infos.Clear();
                    Presenter.Reset();
                    isupdate = true;
                }


                if (isupdate)
                {
                    List<ListViewItem> tmp;
                    tmp = _Infos;
                    LvSearch.BeginUpdate();
                    List<ListViewItem> needremove = new List<ListViewItem>();

                    for (int i = 0, l = _Infos.Count; i < l; i++)
                    {
                        if (i > LvSearch.Items.Count - 1)
                        {
                            LvSearch.Items.Add(_Infos[i]);
                        }
                        else
                        {
                            LvSearch.Items[i] = _Infos[i];

                        }

                        if (TriggerPrsnt.State == SysState.Stop)
                        {
                            if ((selectitem != null && LvSearch.Items[i] == selectitem) || int.Parse(LvSearch.Items[i].Text) - 1 == SearchApp.Default.NavigationIndex)
                            {
                                LvSearch.Items[i].Selected = true;
                            }
                        }
                    }

                    for (int i = _Infos.Count, l = LvSearch.Items.Count; i < l; i++)
                    {
                        needremove.Add(LvSearch.Items[i]);

                    }
                    if (needremove.Count > 0)
                    {
                        needremove.ForEach(x => LvSearch.Items.Remove(x));
                    }

                    SearchItem.IsNewResult = false;
                }
                else
                {
                    if (TriggerPrsnt.State == SysState.Stop && SearchApp.Default.NavigationIndex >= 0)
                    {
                        for (int i = 0; i < LvSearch.Items.Count; i++)
                        {
                            if (int.Parse(LvSearch.Items[i].Text) - 1 == SearchApp.Default.NavigationIndex)
                            {
                                LvSearch.Items[i].Selected = true;
                            }
                        }

                    }
                }


            }
            catch (Exception)
            {
            }
            finally
            {
                LvSearch.EndUpdate();
            }
        }



        public void IndependentControl(Control control)
        {
            control.Dock = DockStyle.Fill;
            control.Size = _IndependentSize;
            Controls.Add(control);
            Controls.SetChildIndex(control, 0);
            LvSearch.Size = _ListSize;
            IsDataTableFiguer = false;
            //NeedUpdate = true;
        }

        public void FormIndependent()
        {
            if (dtFigure != null && !dtFigure.IsDisposed)
            {
                dtFigure.FormIndependent();
            }
        }

        private void MeasSnapShotForm_LeftIconClick(object sender, EventArgs e)
        {
            IsDataTableFiguer = true;
            dtFigure = (Program.Oscilloscope.View as DsoForm).CreateDataTableFig(this, true);
            PageControlFirst = true;
            //NeedUpdate = true;
        }



        private void MeasSnapShotForm_ToolIconClick(object sender, System.EventArgs e)
        {
            this.DataExport();
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            //todo
        }

        public List<System.Data.DataTable> GetDataTables()
        {
            var results = SearchItem.GetSearchResults();
            var source = DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Where(x => x.Id == SearchItem.Source).FirstOrDefault();
            int length = results.GetLength(1);
            Double position = 0;
            Double delta = 0;

            DataTable dt = new DataTable();
            DataRow dr;
            foreach (ColumnHeader column in LvSearch.Columns)
                dt.Columns.Add(column.Text);
            for (Int64 i = 0; i < results.Length; i++)
            {
                position = SearchItem.TransToTmb(results[0, i]);
                //position = (results[0, i] - source.Sampling.PosIndexBymDiv) * source.Pack.Properties.SampInterval;
                delta = i == 0 ? 0 : position - delta;
                dr = dt.NewRow();
                dr[dt.Columns[0]] = (i + 1).ToString();
                dr[dt.Columns[1]] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(SearchItem.Type.ToString());
                dr[dt.Columns[2]] = new Quantity(position, source.Pack.Properties.TmbUnit.Prefix, SearchItem.ResultTmbUnitName).ValueChangeToSI(3).ToFormat("0.000");
                dr[dt.Columns[3]] = new Quantity(delta, SearchItem.ResultPrefix, SearchItem.ResultTmbUnitName).ValueChangeToSI(3).ToFormat("0.000");
                dr[dt.Columns[4]] = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(SearchItem.Type.ToString());
                dt.Rows.Add(dr);
                delta = position;
            }

            return new List<DataTable>() { dt };
        }
    }
}
