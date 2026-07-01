using EventBus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.UserControls;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Structs;

namespace ScopeX.U2
{
    public partial class PanelManageForm :BaseForm, IPanelManager
    {
        #region field&properties

        //窗体的最大尺寸
        private readonly Int32 _MaxFormHeight;

        //管理的所有内容面板IPanel
        private List<IPanel> _IPanelCollection = new List<IPanel>();

        #region UI属性

        private Boolean _IsFold = false;
        [Bindable(true), Category(Const.Category), Description("是否折叠窗体内容"), DefaultValue(false)]
        public Boolean IsFold
        {
            get => _IsFold;
            set
            {
                if (value != _IsFold)
                {
                    _IsFold = value;
                    CustomToolIconInfos[0].Icon = _IsFold ? Properties.Resources.FormArrowDown : Properties.Resources.FormArrowUp;

                    //窗体收缩效果，调整控件尺寸
                    if (_IsFold)
                    {
                        ScrollContainer.Visible = false;
                        Size = new Size(Size.Width, HeadHeight);
                    }
                    else
                    {
                        Size = new Size(Size.Width, HeadHeight + ScrollContainer.Height+10);
                        ScrollContainer.Visible = true;
                    }
                    this.Invalidate();
                }
            }
        }

        #endregion UI属性

        #endregion field&properties

        public PanelManageForm(Int32 maxHeight)
        {
            CustomToolIconInfos = new List<ToolIconInfo>()
            {
                new ToolIconInfo(){ IsShow = true, Icon = Properties.Resources.FormArrowUp, ClickHandler = RightPb_Click}
            };
            InitializeComponent();

            //初始化
            _MaxFormHeight = maxHeight;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        public String FunctionName;

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(String.IsNullOrWhiteSpace(FunctionName) ?"GongNengJieGuoMianBan": FunctionName);
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(String.IsNullOrWhiteSpace(FunctionName) ? "GongNengJieGuoMianBan" : FunctionName);
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

        private void RightPb_Click(object sender, EventArgs e)
        {
            IsFold = !IsFold;
        }

        /// <summary>
        /// 刷新内容面板的内容
        /// </summary>
        private void FreshContent()
        {
            //面板内容的间隔
            Int32 contentinterval = 8;

            //没有内容面板不显示窗体
            if (_IPanelCollection.Count == 0)
            {
                Visible = false;
                IsFold = !Visible;
                return;
            }
            if (this.Owner.WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                return;
            }
            
            Int32 startposx = contentinterval;
            Int32 startposy = contentinterval;
            ScrollContainer.SuspendLayout();
            PanelContent.Controls.Clear();

            //放置内容面板在PanelContent中的位置，特别是纵坐标
            foreach (var p in _IPanelCollection)
            {
                if (p is UserControl pctl)
                {
                    pctl.Location = new Point(contentinterval, startposy);
                    //pctl.Size = new Size(PanelContent.Width - 2 * contentinterval, pctl.Height);
                    startposx += pctl.Width + contentinterval;
                    startposy += pctl.Height + contentinterval;
                    PanelContent.Controls.Add(pctl);
                }
            }

            //Size = new Size(((UserControl)_IPanelCollection.FirstOrDefault()).Width + contentinterval * 3, Math.Min(startposy + HeadHeight, _MaxFormHeight));
            Size = new Size(((UserControl)_IPanelCollection.FirstOrDefault()).Width + contentinterval * 3, startposy + HeadHeight+10);
            ScrollContainer.Size = new Size(Size.Width, Size.Height - HeadHeight-10);

            ScrollContainer.ResumeLayout();
            PanelContent.PerformLayout();
            Visible = true;
            IsFold = !Visible;
            this.Invalidate();

            ScrollContainer.FreshScroller();
        }

        public void Show(Point point)
        {
            this.StartPositionInOwner = point;
            Owner = Program.Oscilloscope.View as DsoForm;
            EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new() { Current = this, Type = FormType.InfoForm });
            FreshContent();
        }

        #region 接口IPanelManager实现

        public void Add(IPanel p)
        {
            if (_IPanelCollection.Contains(p))
            {
                return;
                //throw new InvalidOperationException();
            }
            _IPanelCollection.Add(p);
            FreshContent();
            if (p is UserControl pctl)
            {
                pctl.SizeChanged += (_, _) => FreshContent();
            }
        }

        public void Remove(IPanel p)
        {
            if (!_IPanelCollection.Remove(p))
            {
                //throw new ArgumentException("IPanel");
                // EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"IPanel", EventBus.LogLevel.Debug));
            }
            FreshContent();
        }

        public void Remove()
        {
            if (_IPanelCollection.Count == 0)
            {
                throw new InvalidOperationException();
            }
            _IPanelCollection.RemoveAt(_IPanelCollection.Count - 1);
            FreshContent();
        }

        public void RemoveAt(int index)
        {
            _IPanelCollection.RemoveAt(index);
            FreshContent();
        }

        public void Insert(int index, IPanel p)
        {
            _IPanelCollection.Insert(index, p);
            FreshContent();
        }

        public void Clear()
        {
            _IPanelCollection.Clear();
            FreshContent();
        }

        public IEnumerator<IPanel> GetEnumerator()
        {
            foreach (var p in _IPanelCollection)
            {
                yield return p;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion 接口IPanelManager实现

    }
}
