using System;
using System.ComponentModel;

namespace ScopeX.ComModel
{
    public enum MathDefineFormulaType
    {
        [Description("源")]
        Source,
        [Description("数字")]
        Numberic,
        [Description("函数")]
        Func,
    }
    /// <summary>
    /// 数学自定义公式函数描述信息
    /// </summary>
    /// <param name="Type">类型</param>
    /// <param name="Name">名称</param>
    /// <param name="Symbol">标识</param>
    /// <param name="Expression">表达式</param>
    /// <param name="ImageKey">对应icon名</param>
    /// <param name="WeakTipWrapper">描述，包括函数功能、参数解释</param>
    public sealed record MathFormulaInfo(MathDefineFormulaType Type, String Name, String Symbol, String Expression, String ImageKey, String WeakTipWrapper);
}
