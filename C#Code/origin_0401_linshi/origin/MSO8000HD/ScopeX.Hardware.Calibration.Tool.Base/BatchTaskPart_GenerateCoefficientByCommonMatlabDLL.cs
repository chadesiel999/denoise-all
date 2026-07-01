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
    public class BatchTaskPart_GenerateCoefficientByCommonMatlabDLL : BatchTaskPartBase
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
        private ChannelId currChannelID = ChannelId.C1;
        private String SaveCoefficientPathAndFileName = "";
        private String SourceFiles_Path = "";
        private String SourceFilesFilterPrefixName = "";
        private String DebugSaveFilePath = "";
        private String DebugSaveFilePrefixName = "";
        private String OtherTransparencyParameter = " ";
        private String ReverveParameters = "";
        private String InterleaveName = "";
        private Boolean AnalyParameter(String parameter)
        {
            if (parameter == "")
                return false;
            String[] paramlist = parameter.Split(',');
            //if (paramlist.Length < 8)
            //    return false;
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
            currChannelID = (ChannelId)(int.Parse(tmpstr.Substring(tmpstr.Length - 1)) - 1);

            //4 保存系数文件路径及名称
            tmpstr = paramlist[3].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            SaveCoefficientPathAndFileName = tmpstr;

            //5 源数据文件存放路径
            tmpstr = paramlist[4].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            SourceFiles_Path = tmpstr;
            //6 源数据文件过滤名称前缀
            tmpstr = paramlist[5].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            SourceFilesFilterPrefixName = tmpstr;
            //7 Debug文件保存路径
            tmpstr = paramlist[6].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            DebugSaveFilePath = tmpstr;
            //8 Debug文件名称前缀
            tmpstr = paramlist[7].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            DebugSaveFilePrefixName = tmpstr;
            InterleaveName = paramlist[8].Trim();
            //9 otherParameter
            if (paramlist.Length > 8)
            {
                tmpstr = paramlist[9].Trim();
                if (tmpstr.IndexOf('=') > 0)
                    //tmpstr = tmpstr.Split('=')[1];
                    tmpstr = tmpstr.Substring(tmpstr.IndexOf("=")+1);
                OtherTransparencyParameter = tmpstr;
                OtherTransparencyParameter = OtherTransparencyParameter.Replace($@"@ToolRootPath@.\", AppDomain.CurrentDomain.BaseDirectory);
                OtherTransparencyParameter = BaseHelper.ReplaceESCChar(OtherTransparencyParameter);
            }
            if (paramlist.Length > 9)
                ReverveParameters = paramlist[9].Trim();
            return true;
        }
        private static Dictionary<String/*MatLabDLLName*/, (object instance, MethodInfo? methodInfo)> ExistsInstance = new Dictionary<String, (object instance, MethodInfo? methodInfo)>();

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out String message, CancellationTokenSource? cancelTokenSrc = null)
        {
            BatchTaskPartResult batchtaskpartresult = BatchTaskPartResult.ErrorGeneral;
            message = "";
            #region 参数检查
            String[] sourcefilesfilterprefixnamearray = SourceFilesFilterPrefixName.Split('|');
            String[] sourcefiles_patharray = SourceFiles_Path.Split('|');
            String[] debugsavefilepatharray = DebugSaveFilePath.Split('|');
            String[] debugsavefileprefixnamearray = DebugSaveFilePrefixName.Split('|');
            Int32 totallength = sourcefilesfilterprefixnamearray.Length;
            if (OtherTransparencyParameter.Trim() == "" && totallength > 1)
                OtherTransparencyParameter = String.Empty.PadRight(totallength - 1, '|');
            String[] OtherTransparencyParameterArray = OtherTransparencyParameter.Split('|');
            if (sourcefilesfilterprefixnamearray.Length != totallength
                || sourcefiles_patharray.Length != totallength
                || debugsavefilepatharray.Length != totallength
                || debugsavefileprefixnamearray.Length != totallength
                || OtherTransparencyParameterArray.Length != totallength
                )
            {
                message = $"参数不正确，没有正确的个数！";
                return batchtaskpartresult;
            }
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
                generatecoefficients_matlabfunction = BaseHelper.GetGenerateCoefficientsMatlabDllFunc(type, paramcount);
                if (generatecoefficients_matlabfunction == null)
                {
                    message = $"Matlab DLL[{MatLabDLLName}] 异常,参数个数[需要5个]不正确！";
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

            for (Int32 partindex = 0; partindex < totallength; partindex++)
            {
                String sourcefilespath = "";//保证可以使用不需要外部数据的DLL进行调用
                if (sourcefiles_patharray[partindex].Trim() != "")
                {
                    sourcefilespath = sourcefiles_patharray[partindex];
                    if (sourcefiles_patharray[partindex].Trim()[0] != '.')
                        sourcefilespath = CalibrationOscilloscopeInfo.Defalut?.FileDir + sourcefilespath;
                }
                else
                    continue;
                String debugsavefilepath = debugsavefilepatharray[partindex];
                if (debugsavefilepath[0] != '.')
                    debugsavefilepath = CalibrationOscilloscopeInfo.Defalut?.FileDir + debugsavefilepath;
                debugsavefilepath += $@"\";
                if (!Directory.Exists(debugsavefilepath!))
                    Directory.CreateDirectory(debugsavefilepath!);
                try
                {
                    if (paramcount == 3)
                    {
                        String[] parameters = OtherTransparencyParameterArray[partindex].Split(' ');
                        if (MatLabDLLName.Contains("MatlabGenerateCoefficientsTable_CoeMerging"))
                        {
                            //6 AFCTimeCoeFileName	AFC时域滤波器系数
                            //7 PFCTimeCoeFileName	PFC时域滤波器系数
                            //8 TIADCFreqCoeFileName TIADC频域系数的绝对地址
                            for (int i = 5; i < 8; i++)
                            {
                                parameters[i] = String.IsNullOrEmpty(parameters[i]) || parameters[i] == "-" ? parameters[i] : CalibrationOscilloscopeInfo.Defalut?.FileDir + parameters[i];
                            }
                            //parameters[3] = String.IsNullOrEmpty(parameters[3]) || parameters[3] == "-" ? parameters[3] : CalibrationOscilloscopeInfo.Defalut?.FileDir + parameters[3];
                            //parameters[4] = String.IsNullOrEmpty(parameters[4]) || parameters[4] == "-" ? parameters[4] : CalibrationOscilloscopeInfo.Defalut?.FileDir + parameters[4];
                            //parameters[5] = String.IsNullOrEmpty(parameters[5]) || parameters[5] == "-" ? parameters[5] : CalibrationOscilloscopeInfo.Defalut?.FileDir + parameters[5];
                        }
                        String transparencyparameter = String.Join(' ', parameters);

                        generatecoefficients_matlabfunction!.Invoke(matlabDllInstance,
                        new MWArray[]
                        {
                             debugsavefilepath,//Debug 文件保存路径
                             debugsavefileprefixnamearray[partindex],//DLL中用于保存Debug信息文件的路径
                             transparencyparameter//DLL中用于保存Debug信息的文件名称前缀
                        });
                    }
                    else
                    {
                        generatecoefficients_matlabfunction!.Invoke(matlabDllInstance,
                        new MWArray[]
                        {
                            sourcefilespath,//源数据文件存放的目录
                            sourcefilesfilterprefixnamearray[partindex],//源数据文件从存放目录中过滤前缀名称
                            debugsavefilepath,//Debug 文件保存路径
                            debugsavefileprefixnamearray[partindex],//DLL中用于保存Debug信息文件的路径
                            OtherTransparencyParameterArray[partindex]//DLL中用于保存Debug信息的文件名称前缀
                        });
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                        batchtaskpartresult = BatchTaskPartResult.ErrorFatal;
                    return batchtaskpartresult;
                }
                #endregion
            }
            batchtaskpartresult = BatchTaskPartResult.Succeed;
            return batchtaskpartresult;
        }
    }
}
