using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Data.Base;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_AnalogCaliDataCopy: BatchTaskPartBase
    {
        /// <summary>
        /// 校准的数据类型
        /// </summary>
        private enum AnalogCaliDataType
        {
            Gain,
            BaseLine,
            Bias
        }

        string _ChnlsText = string.Empty;
        List<int> _Chnls = new List<int>();     //需要操作的通道
        uint _SourceLevel;                      //数据源档位(mv)
        string _DestLevelsText = string.Empty;
        List<uint> _DestLevels = new List<uint>();//数据目标档位(mv)
        int _Impedance = 0;                     //高阻、低阻，低阻为Low=1，高阻为High=0
        AnalogCaliDataType _CaliDataType;       //表示数据类型
        bool _PerformFlag = false;              //是否执行

        public override string ParametersDescription
        {
            get => $"[0]表示通道，编号从1开始{System.Environment.NewLine}" +
                   $"[1]表示数据源档位(mv)，只能由一个{System.Environment.NewLine}" +
                   $"[2]表示数据目标档位(mv)，可以有多个，使用‘|’分隔{System.Environment.NewLine}" +
                   $"[3]表示高阻还是低阻，low=50欧姆，低阻，higf表示高阻{System.Environment.NewLine}"+
                   $"[4]表示数据类型，Gain，BaseLine,Bias" +
                   $"[5]表示是否执行，true=执行，false=不执行"; 
        }

        private bool AnalyParameter(string parameter)
        {
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            ///解析输入参数
            try
            {
                //[0]: _Chnls
                _ChnlsText = paramList[0];
                string[] channelIDList = _ChnlsText.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (channelIDList.Length == 0)
                    throw new ArgumentException("_Chnls");
                foreach (var item in channelIDList)
                {
                    int.TryParse(item, out int chnlId);
                    _Chnls.Add(chnlId);
                }
                //[1]: _SourceLevel
                _SourceLevel = uint.Parse(paramList[1]);
                //[2]: _DestChnls
                _DestLevelsText = paramList[2];
                string[] levelsList = _DestLevelsText.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (levelsList.Length == 0)
                    throw new ArgumentException("_DestLevels");
                foreach (var item in levelsList)
                {
                    uint.TryParse(item, out uint level);
                    _DestLevels.Add(level);
                }
                //[3]:高阻、低阻，低阻为Low=1，低阻为High=0
                _Impedance = paramList[3].ToUpper() switch
                {
                    "HIGH" => 0,
                    _ => 1
                };
                //[4]:_CaliDataType
                _CaliDataType = (AnalogCaliDataType)Enum.Parse(typeof(AnalogCaliDataType), paramList[4], true);

                //[5]:_PerformFlag
                _PerformFlag = bool.Parse(paramList[5]);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myName_ParameterPair == null)
                return false;
            return AnalyParameter(myName_ParameterPair[1]);
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            message = String.Empty;
            if(!_PerformFlag)
            {
                message = "Succeed";
                return BatchTaskPartResult.Succeed;
            }

            try
            {
                ///遍历通道，把源档位数据拷贝到目标档位数据
                foreach (var chnl in _Chnls)
                {
                    int chnlId = chnl - 1;
                    var chnlParamsKeyMapSrc = new ChnlParamsKeyMap((ChannelId)chnlId, _Impedance == 0 ? true : false, _SourceLevel);
                    var chnlParamsSrc = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParamsKeyMapSrc)!.Value;

                    //遍历档位
                    foreach (var level in _DestLevels)
                    {
                        var chnlParamsKeyMapDest = new ChnlParamsKeyMap((ChannelId)chnlId, _Impedance == 0 ? true : false, level);
                        var chnlParamsDest = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParamsKeyMapDest)!.Value;

                        //数据更新
                        if(_CaliDataType == AnalogCaliDataType.Gain)
                        {
                            chnlParamsDest.Offset = chnlParamsSrc.Offset;
                            chnlParamsDest.Gain = chnlParamsSrc.Gain;
                            chnlParamsDest.Gain_FineByFpgaThousand = (int)((double)chnlParamsSrc.Gain_FineByFpgaThousand * (_SourceLevel / level));
                        }
                        else if(_CaliDataType == AnalogCaliDataType.BaseLine)
                        {
                            chnlParamsDest.Offset = chnlParamsSrc.Offset;
                            chnlParamsDest.Offset_Pos3Div = (int)((double)chnlParamsSrc.Offset_Pos3Div / (_SourceLevel / level));
                            chnlParamsDest.Offset_Neg3Div = (int)((double)chnlParamsSrc.Offset_Neg3Div / (_SourceLevel / level));
                        }
                        else if (_CaliDataType == AnalogCaliDataType.Bias)
                        {
                            chnlParamsDest.Bias = chnlParamsSrc.Bias;
                            chnlParamsDest.Bias_Pos3Div = (int)((double)chnlParamsSrc.Bias_Pos3Div / (_SourceLevel / level));
                            chnlParamsDest.Bias_Neg3Div = (int)((double)chnlParamsSrc.Bias_Neg3Div / (_SourceLevel / level));
                        }
                        ProductDataTranslate_MSO8000X.SetChnlParamsItem(chnlParamsKeyMapDest, chnlParamsDest);
                    }
                }
            }
            catch(Exception ex)
            {
                message = "Failed: " + ex.Message;
                return BatchTaskPartResult.ErrorGeneral;
            }

            if (InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams))
            {
                message = "Succeed";
                return BatchTaskPartResult.Succeed;
            }
            else
            {
                message = "Failed";
                return BatchTaskPartResult.ErrorGeneral;
            }
        }
    }
}
