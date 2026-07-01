using System;
using System.Collections.Generic;

namespace ScopeX.U2.TipMsg
{
    internal static class ControlableWeakTipCacher
    {
        /// <summary>
        /// 所有正在显示的可控弱提示框
        /// </summary>
        internal static Lazy<Dictionary<Guid, ControlableWeakTipForm>> AllCachedForm { get; private set; } = new Lazy<Dictionary<Guid, ControlableWeakTipForm>>(() => new Dictionary<Guid, ControlableWeakTipForm>(), true);

        internal static ControlableWeakTipForm GetForm(Guid id)
        {
            if (AllCachedForm.Value.ContainsKey(id))
                return AllCachedForm.Value[id];

            return null;
        }

        internal static void CacheForm(Guid id, ControlableWeakTipForm form)
        {
            if (AllCachedForm.Value.ContainsKey(id))
                return;

            AllCachedForm.Value.Add(id, form);
        }

        internal static void RemoveForm(Guid id)
        {
            if (!AllCachedForm.Value.ContainsKey(id))
                return;

            AllCachedForm.Value.Remove(id);
        }
    }
}
