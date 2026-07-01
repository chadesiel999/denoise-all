using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Driver.Module;
using ScopeX.ComModel;
using System.Reflection;

namespace ScopeX.Hardware.Driver
{
    public static partial class Hd
    {
        internal static void NullAction() { }
        internal static void ConstructorCommandTable()
        {
            IEnumerable<Type> source = from t in Assembly.GetExecutingAssembly().GetTypes()
                                       where t.IsClass && (t.GetInterface(typeof(IAppendCommandTable).Name) != null)
                                       select t;
            foreach (Type t in source)
            {
                ConstructorInfo? constructorInfo = t.GetConstructor(Type.EmptyTypes);
                if (constructorInfo != null)
                {
                    IAppendCommandTable obj = (IAppendCommandTable)(constructorInfo.Invoke(null));
                    Dictionary<HdCmd, Action[]>? cmds=obj.AppendCommand();
                    if (cmds!=null)
                    {
                        foreach (var cmd in cmds)
                            cmdTable.TryAdd(cmd.Key, cmd.Value);
                    }
                }
            }
        }
        private static Dictionary<HdCmd, Action[]> cmdTable = new Dictionary<HdCmd, Action[]>();
    }
}
