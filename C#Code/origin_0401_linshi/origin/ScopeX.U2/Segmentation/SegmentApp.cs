// <author>LJW</author>
// <date>2022/6/29</date>
using ScopeX.Core;

namespace ScopeX.U2
{
    public class SegmentApp
    {
        #region 构造析构
        public SegmentApp(TimebasePrsnt prsnt)
        {
            Presenter = prsnt;
        }
        #endregion  构造析构

        #region 成员属性
        /// <summary>
        /// Gets or sets the Default.
        /// </summary>
        public static SegmentApp Default { get; internal set; }

        /// <summary>
        /// Gets the InfoStrip.
        /// </summary>
        public SegementInfoStripEx InfoStrip { get; private set; }

        /// <summary>
        /// Gets the Presenter.
        /// </summary>
        public TimebasePrsnt Presenter { get; }
        #endregion 成员属性

        #region 业务方法
        /// <summary>
        /// The Init.
        /// </summary>
        public void Init()
        {
            InfoStrip = new();
            Presenter.PositionByus = 0;//todo ???
            InfoStrip.Presenter = Presenter;
            //Presenter.TryAddView(InfoStrip);
            InfoStrip.Refresh();
        }



        #endregion 业务方法

    }
}
