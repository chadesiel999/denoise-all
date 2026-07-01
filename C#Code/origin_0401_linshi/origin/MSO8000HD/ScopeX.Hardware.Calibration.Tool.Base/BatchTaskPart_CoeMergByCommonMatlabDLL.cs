using System;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.ComModel;
using System.Threading;
using MathWorks.MATLAB.NET.Arrays;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class BatchTaskPart_CoeMergByCommonMatlabDLL : BatchTaskPartBase
    {
        //
        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, String parameter)
        {
            base.SetParameter(xmlScpiCmd, parameter);
            String[]? myname_parameterpair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd?.ProgramFuncName.Trim() ?? "");
            if (myname_parameterpair != null)
                return AnalyParameter(myname_parameterpair[1]);
            else
                return false;
        }
        //参数样式为：
        //第1个参数，MatLabDLLName:生成系数的Matlab DLL的名称,包括路径及名称全称
        //第2个参数，CoefficientType ，系数类型，应该与CreateProduct中PFC定义的一致。如Coefficients1，Coefficients2，Coefficients3
        //第3个参数，Channel,以最后一个字符为1,2,3,4表示哪个通道。从1开始。如Ch1,Ch2 
        //第4个参数，SaveCoefficientPathAndFileName:保存系数文件路径及名称。可以是相对路径或绝对路径。用于保存系数，供示波器软件使用。名称应该与软件中使用的名称一致
        //第5个参数，SourceFiles_Path:源数据文件存放路径
        //第6个参数，SourceFilesFilterPrefixName:从源数据文件目录中过滤的文件名称前缀
        //第7个参数，DebugSaveFilePath：DLL中保存Debug文件所在的路径
        //第8个参数，DebugSaveFilePrefixName DLL中保存Debug文件的前缀名称
        //第9个参数，OtherTransparencyParameter其他透传参数，使用空格分割
        //第10个参数，Reserve
        private String MatLabDLLName = "";
        private String CoefficientType = "";
        private String TIADCCoe = "";
        private String AFCCoe = "";
        private String PFCCoe = "";
        private Int32 TIADCCoeLength = 0;
        private Int32 AFCCoeLength = 0;
        private Int32 PFCCoeLength = 0;
        private String ReverveParameters = "";
        private String OtherTransparencyParameter = " ";
        private Boolean AnalyParameter(String parameter)
        {
            if (parameter == "")
                return false;
            String[] paramlist = parameter.Split(',');
            //1 生成系数的Matlab DLL的名称
            String tmpstr = paramlist[0].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            MatLabDLLName = tmpstr;
            //2.CoefficientType
            tmpstr = paramlist[1].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            CoefficientType = tmpstr;
            //3 currChannelID
            tmpstr = paramlist[2].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            TIADCCoe = tmpstr;
            //3 currChannelID
            tmpstr = paramlist[3].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            AFCCoe = tmpstr;

            //4 保存系数文件路径及名称
            tmpstr = paramlist[4].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            PFCCoe = tmpstr;
            if (paramlist.Length > 5)
            {
                tmpstr = paramlist[5].Trim();
                //if (tmpstr.IndexOf('=') > 0)
                //    tmpstr = tmpstr.Split('=')[1];
                OtherTransparencyParameter = tmpstr.Replace(' ', ',');
                OtherTransparencyParameter = OtherTransparencyParameter.Replace("OtherParameter=", "");
                OtherTransparencyParameter = OtherTransparencyParameter.Replace($@"@ToolRootPath@.\", AppDomain.CurrentDomain.BaseDirectory);
                OtherTransparencyParameter = BaseHelper.ReplaceESCChar(OtherTransparencyParameter);
            }
            if (paramlist.Length > 9)
                ReverveParameters = paramlist[9].Trim();
            return true;
        }
        private static Dictionary<String/*MatLabDLLName*/, (object instance, MethodInfo? methodInfo)> ExistsInstance = new Dictionary<String, (object instance, MethodInfo? methodInfo)>();
        private Double[] GetData(String path)
        {
            List<Double> datas = new List<Double>();
            if (path != null && File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        string data = sr.ReadLine();
                        if(data.Contains(','))
                        {
                            string[] datasplit=data.Split(',');
                            foreach (string item in datasplit)
                            {
                                if (Double.TryParse(item, out Double result))
                                {
                                    datas.Add(result);
                                }
                            }
                        }
                        else
                        {
                            if (Double.TryParse(data, out Double result))
                            {
                                datas.Add(result);
                            }
                        }
                        
                    }
                }
            }
            if (datas.Count == 0)
            {
                datas.Add(0.0);
            }
            return datas.ToArray();
        }

        public static MethodInfo? GetGenerateCoefficientsMatlabDllFunc(Type type, int parameterCount)
        {
            MethodInfo[] methodInfos = type.GetMethods();
            foreach (MethodInfo m in methodInfos)
            {
                //if (m.ReturnType.Name == "MWArray[]")
                {
                    if (m.GetParameters().Length == parameterCount)
                        return m;
                }
            }
            return null;
        }
        public override BatchTaskPartResult Exec(double overtimeOfSecond, out String message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchtaskpartresult = BatchTaskPartResult.ErrorGeneral;
            message = "";
            #region 参数检查
            Double[] tiadcdata = GetData(CalibrationOscilloscopeInfo.Defalut?.FileDir + TIADCCoe);
            Double[] afcdata = GetData(CalibrationOscilloscopeInfo.Defalut?.FileDir + AFCCoe);
            Double[] pfcdata = GetData(CalibrationOscilloscopeInfo.Defalut?.FileDir + PFCCoe);
            String[] OtherTransparencyParameterArray = OtherTransparencyParameter.Split('|');
            #endregion 参数检查

            #region Matlab DLL调用
            MethodInfo? generatecoefficients_matlabfunction = null;
            Int32 paramcount = CoefficientType.Contains("IFC") ? 3 : 5;
            object? matlabDllInstance = null;
            if (!ExistsInstance.ContainsKey(MatLabDLLName))
            {
                Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + MatLabDLLName);
                Type? type = assembly.GetType(Path.GetFileNameWithoutExtension(MatLabDLLName) + @".Class1");
                if (type == null)
                {
                    message = $"Matlab DLL[{MatLabDLLName}] 异常！";
                    return batchtaskpartresult;
                }
                generatecoefficients_matlabfunction = GetGenerateCoefficientsMatlabDllFunc(type, 7);
                if (generatecoefficients_matlabfunction == null)
                {
                    message = $"Matlab DLL[{MatLabDLLName}] 异常,参数个数[需要7个]不正确！";
                    return batchtaskpartresult;
                }
                matlabDllInstance = Activator.CreateInstance(type);
                if (matlabDllInstance == null)
                {
                    message = $"Matlab DLL[{MatLabDLLName}] 异常,创建实例失败！";

                    return batchtaskpartresult;
                }
                ExistsInstance.Add(MatLabDLLName, (matlabDllInstance, generatecoefficients_matlabfunction));
            }
            else
            {
                matlabDllInstance = ExistsInstance[MatLabDLLName].instance;
                generatecoefficients_matlabfunction = ExistsInstance[MatLabDLLName].methodInfo;
            }
            List<Double> alldata = new List<Double>();

            try
            {
                String[] parameters = OtherTransparencyParameter.Split(',');
                if (MatLabDLLName.Contains("MatlabGenerateCoefficientsTable_CoeMerging"))
                {
                    string[] dd = parameters[6].Split('=');
                    parameters[6] = dd[0]+"=";
                    parameters[6] +=  String.IsNullOrEmpty(dd[1]) || dd[1] == "-" ? dd[1] : CalibrationOscilloscopeInfo.Defalut?.FileDir + dd[1];
                }
                //String transparencyparameter = String.Join(',', parameters);
                String transparencyparameter = String.Join(' ', parameters);
                MWNumericArray tiadcdatamw = ConvertToColumnVector(tiadcdata);
                MWNumericArray afcdatamw = ConvertToColumnVector(afcdata);
                MWNumericArray pfcdatamw = ConvertToColumnVector(pfcdata);
                generatecoefficients_matlabfunction!.Invoke(matlabDllInstance,
                new MWArray[]
                {
                    (MWArray)tiadcdatamw,
                    (MWArray)(tiadcdata.Length),
                    (MWArray)afcdatamw,
                    (MWArray)(afcdata.Length),
                    (MWArray)pfcdatamw,
                    (MWArray)(pfcdata.Length),
                    (MWArray)transparencyparameter//DLL中用于保存Debug信息的文件名称前缀
                });
                batchtaskpartresult = BatchTaskPartResult.Succeed;
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
            #endregion

            return batchtaskpartresult;
        }

        public static MWNumericArray ConvertToColumnVector(double[] data)
        {
            return new MWNumericArray(data.Length, 1, data);
        }
    }
}
