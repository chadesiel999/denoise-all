using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ScopeX.Hardware.Driver
{
    internal class MatlabDllEngine
    {
        internal MatlabDllEngine(String dllName, String typeName, Dictionary<String, Type[]> methodDefineTable)
        {
            _DllName = dllName;
            _TypeName = typeName;
            _MethodDefineTable = methodDefineTable;

            Init();
        }

        #region 实例化必须的参数
        private String _DllName;

        private String _TypeName;

        private Dictionary<String, Type[]> _MethodDefineTable;
        #endregion

        #region DLL加载后的属性
        private Type? _DllType;

        private Dictionary<String, MethodInfo> _MethodInfoTable = new();

        private Object? _Object;

        private volatile Boolean _InitOk = false;
        internal String InitInfo = "";
        #endregion

        internal Object? Excute(String methodName, Object[] param)
        {
            if (_InitOk && _MethodInfoTable.ContainsKey(methodName))
            {
                return _MethodInfoTable[methodName].Invoke(_Object, param);
            }
            return null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (!File.Exists(_DllName))
            {
                InitInfo = $"{_DllName} Not Exist!";
                return;
            }


            _DllType = Assembly.LoadFrom(_DllName).GetType(_TypeName);
            if (_DllType == null)
            {
                InitInfo = $"{_TypeName} not contain {_TypeName}!";
                return;
            }

            foreach (String methodname in _MethodDefineTable.Keys)
            {
                try
                {
                    var methodinfo = _DllType.GetMethod(methodname, _MethodDefineTable[methodname]);
                    if (methodinfo != null)
                        _MethodInfoTable[methodname] = methodinfo;
                }
                catch (Exception e)
                {
                    InitInfo = $"{_DllType} not contain {methodname}={_MethodDefineTable[methodname]}!\n{e.Message}";
                    return;
                }
            }

            try
            {
                _Object = Activator.CreateInstance(_DllType);
            }
            catch (Exception e)
            {
                InitInfo = $"{_DllType} can not create.\n{e.Message}!";
                return;
            }

            _InitOk = true;
        }
    }
}