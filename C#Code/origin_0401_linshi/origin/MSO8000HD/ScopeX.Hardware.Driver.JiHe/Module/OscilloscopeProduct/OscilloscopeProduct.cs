using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver.Module;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 只定义不同型号可能不一样的地方
    /// </summary>
    internal partial class OscilloscopeProduct
    {
        #region Board
        public AbstractPcieBd? PcieBd;
        public AbstractS6Bd? S6Bd;
        public AbstractProcBd? ProcBd;
        public AbstractAcqBd? AcqBd;
        #endregion

        #region Controller
        public AbstractController_AnalogChannel? Ctrl_AnalogChannel;
        public AbstractController_Trigger? Ctrl_Trigger;
        public AbstractController_Misc? Ctrl_Misc;
        public AbstractController_RadioFrequency? Ctrl_RadioFrequency;
        #endregion

        #region Acquirer
        public AbstractAcquirer_AnalogChannel? Acquirer_AnalogChannel;
        public AbstractAcquirer_Temperaturer? Acquirer_Temperaturer;
        public AbstractAcquirer_SimulateWave? Acquirer_SimulateWave;
        public AbstractAnalogAcquireModel? AnalogAcquireModel;
        public AbstractModule_Extram? ExtramModule;
        public AbstractModule_Interp? InterpModule;
        #endregion

        #region Debug_Tools
        //不同的型号可能涉及到不同的逻辑参数
        public Func<string, bool>? LogicValue_Set;
        public Func<string>? LogicValue_Get;
        public Action? AutoCaliAtInit;

        //不同的型号可能涉及到不同的逻辑参数
        public Func<string,HdCmd>? SpecialData_Set;
        public Func<string,string>? SpecialData_Get;
        #endregion

        public ProductHardwareConfig? HardwareConfig;

        public Boolean EnableAutoCaliAtStart = false;

        public Int32[]? AnaChnlBitWidthDefine;

        public ProductType ProductType
        { get; set; }
        public McuComPortUpdater? McuComPortUpdater;
        #region Matlab
        public Dictionary<string/*dll name*/, int/*参数个数*/> MatlabDllsDefine = new Dictionary<string, int>();
        private Dictionary<string, MatlabDll> _MatlabDlls=new Dictionary<string, MatlabDll>();
        public Dictionary<string, MatlabDll> MatlabDlls
        {
            get
            {
                return _MatlabDlls;
            }
            private set
            {
                _MatlabDlls = value;
            }
        }
        private  MethodInfo? GetGenerateCoefficientsMatlabDllFunc(Type type, int parameterCount)
        {
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo m in methodInfos)
            {
                if (m.ReturnType.Name == "MWArray")
                {
                    if (m.GetParameters().Length == parameterCount)
                        return m;
                }
            }
            return null;
        }
        public bool InitMatlabDlls()
        {
            if (MatlabDllsDefine==null)
            {
                MatlabDlls.Clear();
                return true;
            }
            bool bErrorFound = false;
            foreach(var v  in MatlabDllsDefine)
            {
                string matLabDLLName = v.Key;
                if (MatlabDlls.ContainsKey(matLabDLLName))
                    continue;
                int parameterCount = v.Value;
                try
                {
                    Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + matLabDLLName);

                    Type? type = assembly.GetType(Path.GetFileNameWithoutExtension(matLabDLLName) + @".Class1");
                    if (type != null)
                    {
                        MethodInfo? matlabFunction = GetGenerateCoefficientsMatlabDllFunc(type, parameterCount);
                        if (matlabFunction != null)
                        {
                            object? instance = Activator.CreateInstance(type);
                            if (instance != null)
                                MatlabDlls.Add(matLabDLLName, new() { Instance = instance, Method = matlabFunction });
                            else
                                bErrorFound = true;
                        }
                        else
                            bErrorFound = true;
                    }
                    else
                        bErrorFound = true;
                }
                catch(Exception e)
                {
                    Debug.WriteLine($"==MatlabDll Load Exception:{e}");
                    bErrorFound = true;
                }
            }
            return !bErrorFound;
        }
        #endregion
    }
}
