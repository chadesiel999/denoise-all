using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Presenter;
using ScopeX.U2.Properties;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2.BaseControl
{
    public partial class PageControl :UserControl,IPageInfoView,IStylize
    {
        public Delegate Updater;

        private Color btnBorderColor = Color.FromArgb(53, 54, 58);

        private PageInfoPrsnt PageInfo { get; set; }
        IPageInfoPrsnt IView<IPageInfoPrsnt>.Presenter { get => PageInfo; set=> PageInfo=(value as PageInfoPrsnt); }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public PageControl(PageInfoPrsnt prsnt)
        {
            PageInfo = prsnt;
            PageInfo?.TryAddView(this);
            InitializeComponent();
            InitView();
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void InitView()
        {
            LblCurrentPageNum.Enabled = PageInfo.CanModifyCurrentPage;
            BtnFirstPage.Enabled = PageInfo.CanFirst;
            BtnFirstPage.Icon = PageInfo.CanFirst ? Resources.First_Enable : Resources.First_Disenable;
            BtnPreviousPage.Enabled = PageInfo.CanPrevious;
            BtnPreviousPage.Icon = PageInfo.CanPrevious ? Resources.Previous_Enable : Resources.Previous_Disenable;
            BtnNextPage.Enabled = PageInfo.CanNext;
            BtnNextPage.Icon = PageInfo.CanNext ? Resources.Next_Enable : Resources.Next_Disenable;
            BtnLastPage.Enabled = PageInfo.CanLast;
            BtnLastPage.Icon = PageInfo.CanLast ? Resources.Last_Enable : Resources.Last_Disenable;
            String txt = $"{PageInfo.CurrentPageNum} / {PageInfo.TotalPageNum}";
            if (!LblCurrentPageNum.Text.Equals(txt))
            {
                LblCurrentPageNum.Text = txt;
            }
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            this.Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width > 0)
            {
                BtnLastPage.Location = new Point(Width - BtnLastPage.Width - 5, 1);

                BtnNextPage.Location = new Point(BtnLastPage.Left - BtnNextPage.Width - 20, 1);

                LblCurrentPageNum.Location = new Point(BtnNextPage.Left - LblCurrentPageNum.Width - 20, 7);

                BtnPreviousPage.Location = new Point(LblCurrentPageNum.Left - BtnPreviousPage.Width - 20, 1);

                BtnFirstPage.Location = new Point(BtnPreviousPage.Left - BtnFirstPage.Width - 20, 1);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            // 使用黑色画笔绘制边框
            using (var pen = new Pen(Color.FromArgb(53, 54, 58), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
            SizeF textSize;//文本尺寸
            using (Brush textBrush = new SolidBrush(ForeColor))
            {
                String totaltxt = $"{LanguageManger.Instance.GetIDMessage("PageControlSum")}{SIHelper.ValueChangeToSI(PageInfo.TotalNum, 0)}";
                textSize = g.MeasureString(totaltxt, Font);
                g.DrawString(totaltxt, Font, textBrush, new PointF(5, (Height - textSize.Height) / 2.0f));
                if(PageInfo.TotalNum>0)
                {
                    String currenttxt = $"{LanguageManger.Instance.GetIDMessage("PageControlCurrent")}{SIHelper.ValueChangeToSI(PageInfo.ShowIndexs.StartIndex + 1, 0) }~{SIHelper.ValueChangeToSI(PageInfo.ShowIndexs.EndIndex, 0)}";
                    var x = textSize.Width + 5 + 5;
                    textSize = g.MeasureString(totaltxt, Font);
                    g.DrawString(currenttxt, Font, textBrush, new PointF(x, (Height - textSize.Height) / 2.0f));
                }
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if(this.IsHandleCreated)
            {
                this.LblCurrentPageNum.Font = this.Font;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            PageInfo?.TryRemoveView(this);
            LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void TbxCurrentPageNum_Click(object sender, System.EventArgs e)
        {            
            NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            Action<Double> onokclickeventaction = (data) =>
            {
               if(PageInfo.CurrentPageNum!= (Int64)data)
                {
                    PageInfo.UpdateCurrentPage((Int64)data);
                    Updater?.DynamicInvoke();
                }
                nkf.Close();
            };

            nkf.SetKeyBoardValue(LanguageManger.Instance.GetIDMessage("PageControlCurrentPageNum"), String.Empty, 0, onokclickeventaction,
                PageInfo.CurrentPageNum,
                PageInfo.TotalPageNum,
                1);
            nkf.ShowDialogByPosition();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var btn = sender as ScopeX.UserControls.ScopeXIconButton;

            switch (btn.Name)
            {
                case nameof(BtnFirstPage):
                    PageInfo.UpdateCurrentPage(1);
                    break;
                case nameof(BtnPreviousPage):
                    PageInfo.UpdateCurrentPage(PageInfo.CurrentPageNum - 1);
                    break;
                case nameof(BtnNextPage):
                    PageInfo.UpdateCurrentPage(PageInfo.CurrentPageNum + 1);
                    break;
                case nameof(BtnLastPage):
                    PageInfo.UpdateCurrentPage(PageInfo.TotalPageNum);
                    break;
            }
            Updater?.DynamicInvoke();
        }

        void IView.UpdateView(object prsnt, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        private void Update(Object prsnt, String propertyName)
        {
            switch (propertyName)
            {
                case nameof(PageInfo.TotalNum):
                case nameof(PageInfo.ShowIndexs):
                    this.Invalidate();
                    break;
                case nameof(PageInfo.CanModifyCurrentPage):
                    if(LblCurrentPageNum.Enabled = PageInfo.CanModifyCurrentPage)
                    {
                        LblCurrentPageNum.Enabled = PageInfo.CanModifyCurrentPage;
                    }
                    break;
                case nameof(PageInfo.CanFirst):
                    if (PageInfo.CanFirst != BtnFirstPage.Enabled)
                    {
                        BtnFirstPage.Enabled = PageInfo.CanFirst;
                        BtnFirstPage.Icon = PageInfo.CanFirst ? Resources.First_Enable : Resources.First_Disenable;
                    }
                    break;
                case nameof(PageInfo.CanPrevious):
                    if (PageInfo.CanPrevious != BtnPreviousPage.Enabled)
                    {
                        BtnPreviousPage.Enabled = PageInfo.CanPrevious;
                        BtnPreviousPage.Icon = PageInfo.CanPrevious ? Resources.Previous_Enable : Resources.Previous_Disenable;
                    }
                    break;
                case nameof(PageInfo.CanNext):
                    if (PageInfo.CanNext != BtnNextPage.Enabled)
                    {
                        BtnNextPage.Enabled = PageInfo.CanNext;
                        BtnNextPage.Icon = PageInfo.CanNext ? Resources.Next_Enable : Resources.Next_Disenable;
                    }
                    break;
                case nameof(PageInfo.CanLast):
                    if (PageInfo.CanLast != BtnLastPage.Enabled)
                    {
                        BtnLastPage.Enabled = PageInfo.CanLast;
                        BtnLastPage.Icon = PageInfo.CanLast ? Resources.Last_Enable : Resources.Last_Disenable;
                    }
                    break;
                case nameof(PageInfo.CurrentPageNum):
                {
                    String txt = $"{ PageInfo.CurrentPageNum} / {PageInfo.TotalPageNum}";
                    if (!LblCurrentPageNum.Text.Equals(txt))
                    {
                        LblCurrentPageNum.Text = txt;
                    }
                }
                break;
                case nameof(PageInfo.TotalPageNum):
                {
                    String txt = $"{PageInfo.CurrentPageNum} / {PageInfo.TotalPageNum}";
                    if (!LblCurrentPageNum.Text.Equals(txt))
                    {
                        LblCurrentPageNum.Text = txt;
                    }
                }
                break;
            }
        }

       
    }


    //public class PageInfo
    //{
    //    private Semaphore @lock = new Semaphore(1, 1);//信号量保证多线程更新

    //    private Int64 _TotalNum;

    //    //总项数
    //    public Int64 TotalNum
    //    {
    //        get
    //        {
    //            return _TotalNum;
    //        }
    //        private set
    //        {
    //            if (_TotalNum != value)
    //            {
    //                _TotalNum = value;
    //                OnPropertyChanged(nameof(TotalNum));
    //            }
    //        }
    //    }

    //    private Int64 _PageNum;

    //    //每页项数
    //    public Int64 PageNum
    //    {
    //        get
    //        {
    //            return _PageNum;
    //        }
    //        private set
    //        {
    //            if (_PageNum != value)
    //            {
    //                _PageNum = value;
    //                OnPropertyChanged(nameof(PageNum));
    //            }
    //        }
    //    }

    //    private (Int64 StartIndex, Int64 EndIndex) _ShowIndexs = (-1, -1);

    //    /// <summary>
    //    /// 当前页显示项索引
    //    /// </summary>
    //    public (Int64 StartIndex, Int64 EndIndex) ShowIndexs
    //    {
    //        get
    //        {
    //            return _ShowIndexs;
    //        }
    //        private set
    //        {
    //            if (_ShowIndexs != value)
    //            {
    //                _ShowIndexs = value;
    //                OnPropertyChanged(nameof(ShowIndexs));
    //            }
    //        }
    //    }

    //    private Int64 _TotalPageNum;
    //    //总页数
    //    public Int64 TotalPageNum
    //    {
    //        get
    //        {
    //            return _TotalPageNum;
    //        }
    //        private set
    //        {
    //            if (_TotalPageNum != value)
    //            {
    //                _TotalPageNum = value;
    //                OnPropertyChanged(nameof(TotalPageNum));
    //            }
    //        }
    //    }

    //    private Int64 _CurrentPageNum;
    //    //当前页数
    //    public Int64 CurrentPageNum
    //    {
    //        get
    //        {
    //            return _CurrentPageNum;
    //        }
    //        private set
    //        {
    //            if (_CurrentPageNum != value)
    //            {
    //                _CurrentPageNum = value;
    //                OnPropertyChanged(nameof(CurrentPageNum));
    //            }
    //        }
    //    }

    //    private Int64 _CurrentSelectedIndex = -1;


    //    public Int64 CurrentSelectedIndex
    //    {
    //        get
    //        {
    //            return _CurrentSelectedIndex;
    //        }
    //        private set
    //        {
    //            if (_CurrentSelectedIndex != value)
    //            {
    //                _CurrentSelectedIndex = value;
    //                OnPropertyChanged(nameof(CurrentSelectedIndex));
    //            }
    //        }
    //    }

    //    private Boolean _CanModifyCurrentPage = false;

    //    /// <summary>
    //    /// 是否可以执行第一页指令
    //    /// </summary>
    //    public Boolean CanModifyCurrentPage
    //    {
    //        get
    //        {
    //            return _CanModifyCurrentPage;
    //        }
    //        private set
    //        {
    //            if (_CanModifyCurrentPage != value)
    //            {
    //                _CanModifyCurrentPage = value;
    //                OnPropertyChanged(nameof(CanModifyCurrentPage));
    //            }
    //        }
    //    }


    //    private Boolean _CanFirst = false;

    //    /// <summary>
    //    /// 是否可以执行第一页指令
    //    /// </summary>
    //    public Boolean CanFirst
    //    {
    //        get
    //        {
    //            return _CanFirst;
    //        }
    //        private set
    //        {
    //            if (_CanFirst != value)
    //            {
    //                _CanFirst = value;
    //                OnPropertyChanged(nameof(CanFirst));
    //            }
    //        }
    //    }

    //    private Boolean _CanLast = false;

    //    /// <summary>
    //    /// 是否可以执行最后一页指令
    //    /// </summary>
    //    public Boolean CanLast
    //    {
    //        get
    //        {
    //            return _CanLast;
    //        }
    //        private set
    //        {
    //            if (_CanLast != value)
    //            {
    //                _CanLast = value;
    //                OnPropertyChanged(nameof(CanLast));
    //            }
    //        }
    //    }

    //    private Boolean _CanPrevious = false;
    //    /// <summary>
    //    /// 是否可以执行上一页指令
    //    /// </summary>
    //    public Boolean CanPrevious
    //    {
    //        get
    //        {
    //            return _CanPrevious;
    //        }
    //        private set
    //        {
    //            if (_CanPrevious != value)
    //            {
    //                _CanPrevious = value;
    //                OnPropertyChanged(nameof(CanPrevious));
    //            }
    //        }
    //    }

    //    private Boolean _CanNext = false;

    //    /// <summary>
    //    /// 是否可以执行下一页指令
    //    /// </summary>
    //    public Boolean CanNext
    //    {
    //        get
    //        {
    //            return _CanNext;
    //        }
    //        private set
    //        {
    //            if (_CanNext != value)
    //            {
    //                _CanNext = value;
    //                OnPropertyChanged(nameof(CanNext));
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 整体更新
    //    /// </summary>
    //    /// <param name="totalnum">总项数</param>
    //    /// <param name="pagenum">每页项数</param>
    //    /// <param name="currentpage">当前页</param>
    //    public (Int64 StartIndex, Int64 EndIndex) UpdateInfo(Int64 totalnum, Int64 pagenum, Int64 currentpage)
    //    {
    //        if (totalnum == _TotalNum && pagenum == _PageNum && currentpage == _CurrentPageNum)
    //            return ShowIndexs;

    //        if (totalnum > 0 && pagenum > 0 )
    //        {
    //            @lock.WaitOne();
    //            TotalNum = totalnum;
    //            PageNum = pagenum;
    //            if (totalnum % pagenum == 0)
    //                TotalPageNum = totalnum / pagenum;
    //            else
    //                TotalPageNum = totalnum / pagenum + 1;
    //            @lock.Release();
    //            UpdateCurrentPage(currentpage);
    //        }
    //        else
    //        {
    //            Reset();
    //        }
            
    //        return ShowIndexs;
    //    }

    //    /// <summary>
    //    /// 更新当前页码
    //    /// </summary>
    //    public (Int64 StartIndex, Int64 EndIndex) UpdateCurrentPage(Int64 current)
    //    {
    //        @lock.WaitOne();
    //        if (current <= 0)
    //            current = 1;
    //        if (current > _TotalPageNum)
    //            current = _TotalPageNum;

    //        if (current > 0 && current <= _TotalPageNum)
    //        {
    //            CurrentPageNum = current;
    //            Update();
    //        }
    //        @lock.Release();
    //        return ShowIndexs;
    //    }

    //    public void UpdateCurrentIndex(Int64 Index)
    //    {

    //    }

    //    public void Reset()
    //    {
    //        if (_TotalNum == 0 && _PageNum == 0 && _TotalPageNum == 0)
    //            return;

    //        @lock.WaitOne();
    //        TotalNum = 0;
    //        PageNum = 0;
    //        TotalPageNum = 0;
    //        @lock.Release();
    //        UpdateCurrentPage(0);
    //    }

    //    public (Int64 StartIndex, Int64 EndIndex) PreUpdate(Int64 totalnum, Int64 pagenum, Int64 currentpage)
    //    {
    //        if (totalnum == 0)
    //            return (0,0);

    //        Int64 totalpagenum = 0;
    //        if (totalnum % pagenum == 0)
    //            totalpagenum = totalnum / pagenum;
    //        else
    //            totalpagenum = totalnum / pagenum + 1;
    //        if (currentpage <= 0)
    //            currentpage = 1;
    //        if (currentpage > totalpagenum)
    //            currentpage = totalpagenum;

    //      return  (pagenum * (currentpage - 1), Math.Min(currentpage * pagenum, totalnum));
    //    }

    //    private void Update()
    //    {
    //        if (_CurrentPageNum == 1)
    //        {
    //            CanModifyCurrentPage = _CurrentPageNum != _TotalPageNum;
    //            CanFirst = false;
    //            CanLast = _CurrentPageNum!= _TotalPageNum;
    //            CanPrevious = false;
    //            CanNext = _CurrentPageNum != _TotalPageNum;
    //        }
    //        else if (_CurrentPageNum == _TotalPageNum && _CurrentPageNum != 0)
    //        {
    //            CanModifyCurrentPage = _TotalPageNum != 1;
    //            CanLast = false;
    //            CanFirst = _TotalPageNum != 1;
    //            CanNext = false;
    //            CanPrevious = _TotalPageNum != 1;
    //            ShowIndexs = (_TotalNum - _PageNum * (_CurrentPageNum - 1) - 1, _TotalNum - 1);
    //        }
    //        else if (_CurrentPageNum == 0)
    //        {
    //            CanModifyCurrentPage = false;
    //            CanFirst = false;
    //            CanPrevious = false;
    //            CanLast = false;
    //            CanNext = false;
    //            ShowIndexs = (-1, -1);
    //        }
    //        else if(_CurrentPageNum != _TotalPageNum)
    //        {
    //            CanModifyCurrentPage = true;
    //            CanFirst = true;
    //            CanPrevious = true;
    //            CanLast = true;
    //            CanNext = true;
    //        }


    //        if (_CurrentPageNum != 0)
    //        {
    //            CanModifyCurrentPage = true;
    //            ShowIndexs = (_PageNum * (_CurrentPageNum - 1), Math.Min(_CurrentPageNum * _PageNum, _TotalNum));
    //        }
    //    }

    //    protected PropertyChangedEventHandler? _PropertyChanged;

    //    public event PropertyChangedEventHandler? PropertyChanged
    //    {
    //        add
    //        {
    //            _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
    //        }
    //        remove
    //        {
    //            _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
    //        }
    //    }

    //    protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
    //    {
    //        _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
}
