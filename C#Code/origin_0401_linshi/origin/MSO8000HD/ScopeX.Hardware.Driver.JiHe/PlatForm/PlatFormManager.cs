using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.PlatForm
{
    /// <summary>
    /// 平台化类管理器，提供当前产品实现IPlatForm接口的对象实例
    /// </summary>
    internal class PlatFormManager
    {
        //显示指定产品类型,需要在产品初始化的时候赋值
        //todo wcj
        public static ProductType ProductType { get; set; } = ProductType.Base;

        private static IPlatForm _CurrPlatForm;

        public static IPlatForm CurrPlatForm
        {
            get
            {
                if (_CurrPlatForm != null)
                    return _CurrPlatForm;

                switch (ProductType)
                {
                    case ProductType.B23_DBI13G:
                        _CurrPlatForm = new PlatForm_DBI13G();
                        break;
                    default:
                        throw new ArgumentException($"不支持的产品类型：{ProductType}");
                }
                return _CurrPlatForm;
            }
        }
    }
}
