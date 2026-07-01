using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class MathSubPageBase :SubPageBase, IMathView, IStylize
    {
        /// <summary>
        /// Gets or sets the Presenter.
        /// </summary>
        public MathPrsnt Presenter { get => (MathPrsnt)(ParentForm as IChnlView).Presenter; set => (ParentForm as IChnlView).Presenter = value; }

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

        IBadge IView<IBadge>.Presenter
        {
            get => Presenter;
            set => Presenter = (MathPrsnt)value;
        }

        public MathType Mode
        {
            get
            {
                return mode;
            }
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        protected MathType mode;
        protected Boolean _ArgToCtrl;
        protected Stopwatch sw;

        public MathSubPageBase()
        {
            InitializeComponent();
            mode = GetClassDescription(this.GetType());
        }

       private MathType GetClassDescription(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attributes.Length > 0)
            {
                var descriptionAttribute = (DescriptionAttribute)attributes[0];
                return (MathType)Enum.Parse(typeof(MathType), descriptionAttribute.Description);
            }

            return default(MathType);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                this.BeginInvoke(() =>
                {
                    LoadMethod();
                });
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadMethod();
        }

        protected virtual void LoadMethod()
        {
            if (sw == null || sw.ElapsedMilliseconds >= 300)
            {
                sw?.Stop(); // 如果 sw 不为 null，停止之前的 Stopwatch
                sw = Stopwatch.StartNew(); // 重新开始计时
            }
            else
            {
                // 如果时间间隔小于 300 毫秒，直接返回，不执行操作
                return;
            }
        }

        public void UpdateView(object prsnt, string propertyName = "")
        {
            SubPageUpdateView(prsnt, propertyName);
        }

        protected virtual void ViewInit()
        {
        }

        protected virtual void SubPageUpdateView(object prsnt, string propertyName)
        {
        }

        
    }
}
