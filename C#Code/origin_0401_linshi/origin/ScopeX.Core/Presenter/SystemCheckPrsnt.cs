using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class SystemCheckPrsnt : MulticastPrsnt<ISystemCheckView>, ISystemCheckPrsnt
    {
        private protected override SystemCheckModel Model
        {
            get;
        }

        public SystemCheckPrsnt(IDsoPrsnt idp, ISystemCheckView? view) : base(idp)
        {
            Model = DsoModel.Default.SystemCheck;
            Model.PropertyChanged += OnPropertyChanged;

            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        /// <summary>
        /// 自检功能重置
        /// </summary>
        public void SystemCheckRst()
        {
            Model.CheckEnable = false;
            Model.ScopeCheckType = CheckType.Close;
            Model.ExitCount = 0;
        }

        /// <summary>
        /// 自检使能开关，可以去DisplayModel 关闭触摸使能TouchLock
        /// </summary>
        public Boolean CheckEnable
        {
            get => Model.CheckEnable;
            set
            {
                Model.CheckEnable = value;
            }
        }

        public UInt16 ExitCount
        {
            get => Model.ExitCount;
            set
            {
                Model.ExitCount = value;
            }
        }

        public CheckType ScopeCheckType
        {
            get => Model.ScopeCheckType;
            set
            {
                Model.ScopeCheckType = value;
            }
        }

        #region 屏幕检测模块

        public ScreenMaskColor ScreenColorDisplay
        {
            get => Model.ScreenColorDisplay;
            set
            {
                Model.ScreenColorDisplay = value;
            }
        }

        #endregion

        #region 触摸检测模块

        public TouchTestTextColor TextColorDisplay
        {
            get => Model.TextColorDisplay;
            set
            {
                Model.TextColorDisplay = value;
                
            }
        }
        #endregion

        #region 按键板检测模块
        public Int32 KeyCheckCode
        {
            get => Model.KeyCheckCode;
            set
            {
                Model.KeyCheckCode = value;
            }
        }
        #endregion
    }
}
