using System;
using System.Threading;
using ScopeX.Core.Model;

namespace ScopeX.Core.Presenter
{
    public class PageInfoPrsnt :MulticastPrsnt<IPageInfoView>, IPageInfoPrsnt
    {
        private Semaphore @lock = new Semaphore(1, 1);//信号量保证多线程更新

        public PageInfoPrsnt(IDsoPrsnt idp, IPageInfoView? view = null) : base(idp)
        {
            model = new PageInfoModel();
            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
            Model.PropertyChanged += OnPropertyChanged;
        }

        private PageInfoModel model;
        private protected override PageInfoModel Model => model;

        //总项数
        public Int64 TotalNum
        {
            get => Model.TotalNum;
            private set
            {
                if (Model.TotalNum != value)
                {
                    Model.TotalNum = value;
                }
            }
        }
        //每页项数
        public Int64 PageNum
        {
            get => Model.PageNum;
            private set
            {
                if (Model.PageNum != value)
                {
                    Model.PageNum = value;
                }
            }
        }
        /// <summary>
        /// 当前页显示项索引
        /// </summary>
        public (Int64 StartIndex, Int64 EndIndex) ShowIndexs
        {
            get => Model.ShowIndexs;
            private set
            {
                if (Model.ShowIndexs != value)
                {
                    Model.ShowIndexs = value;
                }
            }
        }
        //总页数
        public Int64 TotalPageNum
        {
            get => Model.TotalPageNum;
            private set
            {
                if (Model.TotalPageNum != value)
                {
                    Model.TotalPageNum = value;
                }
            }
        }
        //当前页数
        public Int64 CurrentPageNum
        {
            get => Model.CurrentPageNum;
            private set
            {
                if (Model.CurrentPageNum != value)
                {
                    Model.CurrentPageNum = value;
                }
            }
        }
        /// <summary>
        /// 是否可以执行第一页指令
        /// </summary>
        public Boolean CanModifyCurrentPage
        {
            get => Model.CanModifyCurrentPage;
            private set
            {
                if (Model.CanModifyCurrentPage != value)
                {
                    Model.CanModifyCurrentPage = value;
                }
            }
        }
        /// <summary>
        /// 是否可以执行第一页指令
        /// </summary>
        public Boolean CanFirst
        {
            get => Model.CanFirst;
            private set
            {
                if (Model.CanFirst != value)
                {
                    Model.CanFirst = value;
                }
            }
        }
        /// <summary>
        /// 是否可以执行最后一页指令
        /// </summary>
        public Boolean CanLast
        {
            get => Model.CanLast;
            private set
            {
                if (Model.CanLast != value)
                {
                    Model.CanLast = value;
                }
            }
        }
        /// <summary>
        /// 是否可以执行上一页指令
        /// </summary>
        public Boolean CanPrevious
        {
            get => Model.CanPrevious;
            private set
            {
                if (Model.CanPrevious != value)
                {
                    Model.CanPrevious = value;
                }
            }
        }
        /// <summary>
        /// 是否可以执行下一页指令
        /// </summary>
        public Boolean CanNext
        {
            get => Model.CanNext;
            private set
            {
                if (Model.CanNext != value)
                {
                    Model.CanNext = value;
                }
            }
        }
        #region Method

        /// <summary>
        /// 整体更新
        /// </summary>
        /// <param name="totalnum">总项数</param>
        /// <param name="pagenum">每页项数</param>
        /// <param name="currentpage">当前页</param>
        public (Int64 StartIndex, Int64 EndIndex) UpdateInfo(Int64 totalnum, Int64 pagenum, Int64 currentpage)
        {
            try
            {
                if (totalnum == TotalNum && pagenum == PageNum && currentpage == CurrentPageNum)
                    return ShowIndexs;

                if (totalnum > 0 && pagenum > 0)
                {
                    @lock.WaitOne();
                    TotalNum = totalnum;
                    PageNum = pagenum;
                    if (totalnum % pagenum == 0)
                        TotalPageNum = totalnum / pagenum;
                    else
                        TotalPageNum = totalnum / pagenum + 1;
                    @lock.Release();
                    UpdateCurrentPage(currentpage);
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return ShowIndexs;
        }

        /// <summary>
        /// 更新当前页码
        /// </summary>
        public (Int64 StartIndex, Int64 EndIndex) UpdateCurrentPage(Int64 current)
        {
            try
            {
                @lock.WaitOne();
                if (current <= 0)
                    current = 1;
                if (current > TotalPageNum)
                    current = TotalPageNum;

                if (current > 0 && current <= TotalPageNum)
                {
                    CurrentPageNum = current;
                    Update();
                }
                if (current == 0 && TotalPageNum == 0)
                {
                    CurrentPageNum = current;
                    Update();
                }
                @lock.Release();
                return ShowIndexs;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void Reset()
        {
            try
            {
                if (TotalNum == 0 && PageNum == 0 && TotalPageNum == 0)
                    return;

                @lock.WaitOne();
                TotalNum = 0;
                PageNum = 0;
                TotalPageNum = 0;
                @lock.Release();
                UpdateCurrentPage(0);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public (Int64 StartIndex, Int64 EndIndex) PreUpdate(Int64 totalnum, Int64 pagenum, Int64 currentpage)
        {
            if (totalnum == 0)
                return (0, 0);

            Int64 totalpagenum = 0;
            if (totalnum % pagenum == 0)
                totalpagenum = totalnum / pagenum;
            else
                totalpagenum = totalnum / pagenum + 1;
            if (currentpage <= 0)
                currentpage = 1;
            if (currentpage > totalpagenum)
                currentpage = totalpagenum;

            return (pagenum * (currentpage - 1), Math.Min(currentpage * pagenum, totalnum));
        }
        private void Update()
        {
            if (CurrentPageNum == 1)
            {
                CanModifyCurrentPage = CurrentPageNum != TotalPageNum;
                CanFirst = false;
                CanLast = CurrentPageNum != TotalPageNum;
                CanPrevious = false;
                CanNext = CurrentPageNum != TotalPageNum;
            }
            else if (CurrentPageNum == TotalPageNum && CurrentPageNum != 0)
            {
                CanModifyCurrentPage = TotalPageNum != 1;
                CanLast = false;
                CanFirst = TotalPageNum != 1;
                CanNext = false;
                CanPrevious = TotalPageNum != 1;
                ShowIndexs = (TotalNum - PageNum * (CurrentPageNum - 1) - 1, TotalNum - 1);
            }
            else if (CurrentPageNum == 0)
            {
                CanModifyCurrentPage = false;
                CanFirst = false;
                CanPrevious = false;
                CanLast = false;
                CanNext = false;
                ShowIndexs = (-1, -1);
            }
            else if (CurrentPageNum != TotalPageNum)
            {
                CanModifyCurrentPage = true;
                CanFirst = true;
                CanPrevious = true;
                CanLast = true;
                CanNext = true;
            }


            if (CurrentPageNum != 0)
            {
                CanModifyCurrentPage = true;
                ShowIndexs = (PageNum * (CurrentPageNum - 1), Math.Min(CurrentPageNum * PageNum, TotalNum));
            }
        } 
        #endregion
    }
}
