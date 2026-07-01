using System;

namespace ScopeX.Core.Model
{
    /// <summary>
    /// 硬件警告消息
    /// </summary>
    /// <param name="Channel1">通道一是否高压警告</param>
    /// <param name="Channel2">通道二是否高压警告</param>
    /// <param name="Channel3">通道三是否高压警告</param>
    /// <param name="Channel4">通道四是否高压警告</param>
    /// <param name="ExtTrigger">外触发是否高压警告</param>
    public record HardwareWarningEventMessageArgs(Boolean Channel1, Boolean Channel2, Boolean Channel3, Boolean Channel4, Boolean ExtTrigger);
}
