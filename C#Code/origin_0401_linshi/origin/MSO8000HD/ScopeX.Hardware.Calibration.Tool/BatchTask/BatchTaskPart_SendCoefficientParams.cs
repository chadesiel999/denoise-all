using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal class BatchTaskPart_SendCoefficientParams : BatchTaskPartBase
    {
        public override String FuncionDescription
        {
            get => $"发送系数到示波器";
        }
        public override String ParametersDescription
        {
            get
            {
                Int32 argindex = 1;
                return $"第{argindex++}个参数：通道组合构造成的交织方式，请使用Driver端代码中的AnalogAcquireModel.cs 之 AcqModeInterleaveDefines 之InterleaveName{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，需要校准的通道，Ch1、Ch2、Ch3、Ch4{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，阻抗，low=50欧姆，低阻，higf表示高阻{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，以mV为单位的档位值，浮点数{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，频点文件路径{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，保存该次扫频数据的文件夹{System.Environment.NewLine}" +
                       $"第{argindex++}个参数，信号源VISA地址{System.Environment.NewLine}";
            }
        }
        public override String Example
        {
            get => "BatchTaskPart_SendCoefficientParams C1C3_20G,Ch1,HIGH,50,USB0::0x1AB1::0x0641::DG4E183302205::INSTR";
        }
        //参数样式为：
        //第1个参数，CoefficientType ，系数类型，应该与CreateProduct中PFC定义的一致。如Coefficients1，Coefficients2，Coefficients3
        //第2个参数，SaveCoefficientPathAndFileName:保存系数文件路径及名称。可以是相对路径或绝对路径。用于保存系数，供示波器软件使用。名称应该与软件中使用的名称一致
        private String _MatLabDLLName = "";
        private String _CoefficientType = "";
        private ChannelId _CurrChannelID = ChannelId.C1;
        private String _SaveCoefficientPath = "";
        private String _SaveCoefficientPathAndFileName = "";
        private String _InterleaveName = "";
        private Int32 _CoeQuantiBit = 0;
        private IInstrumentSession? _SigInstrumentSession;
        private String _CoefficientKey = String.Empty;
        private Int32 _MagCompOrder = 0;

        private Boolean AnalyParameter(String parameter)
        {
            if (parameter == "")
                return false;
            String[] paramlist = parameter.Split(',');
            //1.CoefficientType
            String tmpstr = paramlist[0].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            _CoefficientType = tmpstr;
            tmpstr = paramlist[1].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            _CoeQuantiBit = int.Parse(tmpstr);
            //2 系数Key
            tmpstr = paramlist[2].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            _CoefficientKey = tmpstr;
            //3 保存系数文件路径
            tmpstr = paramlist[3].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            _SaveCoefficientPath = tmpstr;
            //4 保存系数文件名称
            tmpstr = paramlist[4].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            _SaveCoefficientPathAndFileName = tmpstr;
            tmpstr = paramlist[5].Trim();
            if (tmpstr.IndexOf('=') > 0)
                tmpstr = tmpstr.Split('=')[1];
            _MagCompOrder = int.Parse(tmpstr);
            return true;
        }
        public override Boolean SetParameter(XmlScpiCmd? xmlScpiCmd, String parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            String[]? myname_parameterpair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myname_parameterpair == null)
                return false;
            return AnalyParameter(myname_parameterpair[1]);
        }

        private void SendCoefficientParams(String filepath, String debugsavefileprefixnamearray, ref String message, ref BatchTaskPartResult batchTaskPartResult)
        {

        }

        public override BatchTaskPartResult Exec(Double overtimeBySec, out String outMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            outMsg = String.Empty;
            List<Double> alldata = new List<Double>();
            String scpicmd = String.Empty;
            String filepath = CalibrationOscilloscopeInfo.Defalut!.FileDir + _SaveCoefficientPath + "\\" + _CoefficientKey + "_" + _SaveCoefficientPathAndFileName;
            if (_CoefficientType == "IFC" || _CoefficientType == "AFC" || _CoefficientType == "TI" || _CoefficientType == "PFC")
            {
                if (!File.Exists(filepath))
                {
                    outMsg = $"Fail,未查询到文件路径:{_SaveCoefficientPathAndFileName}!";// 文件保存为:{Path.GetFileName(fileName)}";
                    return BatchTaskPartResult.ErrorFatal;
                }
                else
                {
                    using (StreamReader sr = new StreamReader(filepath))
                    {
                        while (!sr.EndOfStream)
                        {
                            if (Double.TryParse(sr.ReadLine(), out Double data))
                            {
                                alldata.Add(data);
                            }
                        }
                    }
                   
                    String tempkey = _CoeQuantiBit == 0 ? _CoefficientKey : _CoefficientKey + $"_{_CoeQuantiBit}bit";
                    if (_SaveCoefficientPathAndFileName.Contains("_acq1"))
                    {
                        tempkey += "_acq1";
                    }
                    if (_SaveCoefficientPathAndFileName.Contains("_acq2"))
                    {
                        tempkey += "_acq2";
                    }
                    if (_SaveCoefficientPathAndFileName.Contains("_pro1"))
                    {
                        tempkey += "_pro1";
                    }
                    CoefficientsParams.Default[tempkey] = alldata.ToArray();
                    if (_CoefficientType != "PFC")
                    {
                        String coefficientsparamskey = _CoefficientKey.Replace("AFC", "IFC");
                        CoefficientsParams.Default[coefficientsparamskey] = alldata.ToArray();
                    }
                    //InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.CoefficientsParams);//在保存时，自动进行了数据传输
                                                                                                                   //当前的系数类型
                    scpicmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
                    scpicmd += " CoefficientsTableType," + _CoefficientType;
                    currInstrumentSession?.WriteString(scpicmd);

                }
            }
            else
            {
                #region 重发系数，让新系数生效
                scpicmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
                scpicmd += " Message2AcquirerAnalogChannel,ResendAllCoefficientsTables";
                currInstrumentSession!.WriteString(scpicmd);
                Thread.Sleep(2000);//保证系数 重发所需时间
                #endregion
            }

            if (_CoefficientType == "AFC")
            {
                if (_MagCompOrder != 0)
                {
                    String afckey = _CoefficientKey.Replace("AFC_", "DelayIFC_");
                    afckey = afckey.Remove(afckey.LastIndexOf("_"), afckey.Length - afckey.LastIndexOf("_"));
                    String pfckey = _CoefficientKey.Replace("AFC_", "DelayPFC_");
                    pfckey = pfckey.Remove(pfckey.LastIndexOf("_"), pfckey.Length - pfckey.LastIndexOf("_"));

                    CoefficientsParams.Default[afckey] = new Double[] { (Int32)(_MagCompOrder) };
                    CoefficientsParams.Default[pfckey] = new Double[] { 0 };
                }
            }
            if (_CoefficientType == "PFC")
            {
                alldata.Clear();
                String pfcfilepath = CalibrationOscilloscopeInfo.Defalut!.FileDir + $"{_SaveCoefficientPathAndFileName}\\{_CoefficientKey}_PFCDelay.txt";
                String pfckey = _CoefficientKey.Replace("PFC_", "DelayPFC_");
                //pfcKey = pfcKey.Remove(pfcKey.LastIndexOf("_"), pfcKey.Length - pfcKey.LastIndexOf("_"));
                if (!String.IsNullOrEmpty(pfckey) && File.Exists(pfcfilepath))
                {
                    using (StreamReader sr = new StreamReader(pfcfilepath))
                    {
                        while (!sr.EndOfStream)
                        {
                            String delaystring = sr.ReadLine();
                            if (Double.TryParse(delaystring.Split(" ")[4], out Double data))
                            {
                                alldata.Add(Math.Abs(data));
                            }
                        }
                    }
                    CoefficientsParams.Default[pfckey] = new Double[] { ((Int32)alldata.Min()) };
                }
            }
            if (_CoefficientType == "IFC")
            {
                if (_MagCompOrder != 0)
                {
                    String ifckey = _CoefficientKey.Replace("IFC_", "DelayIFC_");
                    ifckey = ifckey.Remove(ifckey.LastIndexOf("_"), ifckey.Length - ifckey.LastIndexOf("_"));
                    String pfckey = _CoefficientKey.Replace("IFC_", "DelayPFC_");
                    pfckey = pfckey.Remove(pfckey.LastIndexOf("_"), pfckey.Length - pfckey.LastIndexOf("_"));

                    CoefficientsParams.Default[ifckey] = new Double[] { (Int32)(_MagCompOrder) };
                    CoefficientsParams.Default[pfckey] = new Double[] { 0 };
                }
            }
            InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.CoefficientsParams);//在保存时，自动进行了数据传输
            return BatchTaskPartResult.Succeed;
        }
    }
}
