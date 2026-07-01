using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public class Matlab
    {
        private Matlab() 
        {
            _InitMatlabEngineTask = new(Init);
            if (_InitMatlabEngineTask.Status == TaskStatus.Created)
                _InitMatlabEngineTask.Start();
        }

        public static Matlab Default = new Matlab();

        #region 内部变量和内部函数
        private const BindingFlags _DEFAULT_BINDING_FLAGS = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static;

        private Object? _MatlabInstance = null;
        private Type? _MatlabType = null;

        private Boolean _IsInitOk = false;
        private String _WorkFolder = "";
        private readonly Task? _InitMatlabEngineTask = null;
        
        /// <summary>
        /// 初始化耗时较大
        /// </summary>
        private void Init()
        {
            if (_IsInitOk)
                return;
            try
            {
                _MatlabType = Type.GetTypeFromProgID("Matlab.Application");
                if (_MatlabType == null)
                {
                    return;
                }
                _MatlabInstance = Activator.CreateInstance(_MatlabType);
                if (_MatlabInstance == null)
                {
                    return;
                }
                _MatlabType.InvokeMember("Visible", BindingFlags.SetProperty, null, _MatlabInstance, new Object[] { false });
                _IsInitOk = true;
            }
            catch
            {
                _IsInitOk = false;
            }
        }

        private Boolean CheckMatlabState()
        {
            if (_InitMatlabEngineTask?.Status != TaskStatus.RanToCompletion)
                return false;

            if (!_IsInitOk || _MatlabType == null || _MatlabInstance == null)
                return false;

            return true;
        }
        #endregion

        /// <summary>
        /// 设置工作目录
        /// </summary>
        /// <param name="workFolder"></param>
        /// <returns></returns>
        public Object? SetWorkFolder(String workFolder)
        {
            if (!Directory.Exists(workFolder))
            {
                return $"{workFolder} is unvalid.";
            }

            if (!CheckMatlabState())
            {
                return "Matlab Engine is unvalid.";
            }

            _WorkFolder = workFolder;
            return _MatlabType?.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new object[] { $"cd('{_WorkFolder}')" });
        }

        public Object? PutData(Object data, String dataName)
        {
            if (!CheckMatlabState())
            {
                return "Matlab Engine is unvalid.";
            }

            return _MatlabType?.InvokeMember("PutWorkspaceData", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new object[] { dataName, "base", new object[] { data } });
        }

        public Object? ExcuteCode(String codeStr)
        {
            if (!CheckMatlabState())
            {
                return "Matlab Engine is unvalid.";
            }
            return _MatlabType?.InvokeMember("Execute", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new object[] { codeStr });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="data"></param>
        /// <returns>错误提示</returns>
        public Object? TryGetData(String dataName, out Object? data)
        {
            data = null;
            if (!CheckMatlabState())
            {
                return "Matlab Engine is unvalid.";
            }

            try
            {
                data = _MatlabType?.InvokeMember("GetVariable", _DEFAULT_BINDING_FLAGS, null, _MatlabInstance, new object[] { dataName, "base" });
            }
            catch(Exception e)
            {
                return e.Message;
            }
            return String.Empty;
        }
    }
}
