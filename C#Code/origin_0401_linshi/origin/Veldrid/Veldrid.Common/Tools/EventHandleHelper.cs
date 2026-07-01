using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Veldrid.Common.Tools
{
    public static class EventHandleHelper
    {
        public static void ClearEventHandle<T>(this T obj) where T : class
        {
            if (obj == null) return;
            var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            var eventinfos = obj.GetType().GetEvents(flag);
            if (eventinfos == null || eventinfos.Length == 0) return;
            foreach (var field in eventinfos.Select(x=>obj.GetType().GetFieldInfo(x.Name,flag)).Where(x=>x!=null))
            {
                if (field == null) return;
                object fieldvalue = field.GetValue(obj);
                var eventinfo = eventinfos.First(x => x.Name == field.Name);
                if(eventinfo.AddMethod.IsPublic)
                {
                    if (fieldvalue is Delegate dele)
                    {
                        foreach (Delegate del in dele.GetInvocationList())
                        {
                            eventinfo.RemoveEventHandler(obj, del);
                        }
                    }

                }
                else
                {
                    field.SetValue(obj, null);
                }
            }
            foreach(var member in GetMemberInfos(obj.GetType(),flag))
            {
                if(member is System.Reflection.FieldInfo fieldinfo)
                {
                    fieldinfo.SetValue(obj, null);
                }
                if(member is System.Reflection.PropertyInfo propertyinfo)
                {
                    propertyinfo.SetValue(obj, null);
                }
            }
        }

        private static System.Reflection.FieldInfo? GetFieldInfo(this Type objtype, string name, System.Reflection.BindingFlags flags)
        {
            if (objtype == null || objtype.GetEvents(flags).Length == 0) return null;
            var info = objtype.GetFields(flags).FirstOrDefault(x => x.Name == name);
            if (info == null && objtype.BaseType != null) return objtype.BaseType.GetFieldInfo(name, flags);
            else return info;
        }
        private static List<System.Reflection.MemberInfo> GetMemberInfos(this Type objtype, System.Reflection.BindingFlags flags)
        {
            if (objtype == null) return new List<System.Reflection.MemberInfo>();
            List<System.Reflection.MemberInfo> memberInfos = new List<System.Reflection.MemberInfo>();
            while (objtype != null)
            {
                memberInfos.AddRange(objtype.GetFields(flags).Where(x => x.FieldType != null && x.FieldType.BaseType == typeof(MulticastDelegate)));
                memberInfos.AddRange(objtype.GetProperties(flags).Where(x => x.PropertyType != null && x.PropertyType.BaseType == typeof(MulticastDelegate)));
                objtype = objtype.BaseType;
            }
            return memberInfos;
        }
    }
}
