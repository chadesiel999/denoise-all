using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
/******************************探头自校正流程************************************/

/******************************探头自校正流程************************************/
namespace ScopeX.Hardware.Driver
{
    internal class SoGoodProbeCaliValue
    {
        public UInt16 ProbeDacA { get; set; }

        public UInt16 ProbeDacB { get; set; }

        public double ProbeAcqDelta { get; set; }

        public void Update(double delta, UInt16 A, UInt16 B)
        {
            if (Math.Abs(delta) < Math.Abs(ProbeAcqDelta))
            {
                ProbeAcqDelta = delta;
                ProbeDacA = A;
                ProbeDacB = B;
            }
        }

    }

    public partial class Cali
    {
        private volatile Int32 _ProbeCaliFinishedProgress = 0;//探头校准完成度
        private ChannelId _ProbeCaliChannel = ChannelId.C1; //探头校准的通道

        private const Double _ProbeCaliError = 0.01D;//偏差
        public Int32 GetProbeCaliTotalItemCount() => 1000000;

        public Int32 GetProbeCaliFinishedCount() => _ProbeCaliFinishedProgress;

        public void ClearProbeCaliFinishedCount() => _ProbeCaliFinishedProgress = 0;

        /// <summary>
        /// 强制探头校准完成
        /// </summary>
        public void ForceProbeCaliFinished() => _ProbeCaliFinishedProgress = GetProbeCaliTotalItemCount();

        public Int32 AutoCaliProbeBaseline_Exec(ChannelId caliChannel, CancellationToken? cancelToken/*不用*/, out string message/*不用*/)
        {
            _ProbeCaliChannel = caliChannel;
            var res = 0;
            message = "";

            var currentCouping = Coupling.LowImpedance;         //探头校准选用阻抗：抵抗
            var currentYScale = (Int32)AnaChnlScaleIndex.Lv50m;//探头校准选用档位：垂直一格为1V
            var currentPosdiv = PosDiv.Pos0Div;                 //探头校准选用零线：零线在0Div

            //************************** 配置(缓存)校准通道 *************************//
            ChannelInfo chnlinfo = new();
            chnlinfo.ChannelId = (Int32)_ProbeCaliChannel;
            chnlinfo.IsFinish = false;
            chnlinfo.YScaleCurrent = currentYScale;

            //配置模拟通道，更新Hd.UIMessage.Analog[] 必须是全家桶,不能单个通道设置
            var devAnalogChlSize = Hd.UIMessage!.Analog.Length;
            var newAnalogOptions = new List<HdMessage.AnalogOptions>();
            {
                for (var chnl = 0; chnl < devAnalogChlSize; chnl++)
                {
                    var scaleValueBymV = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[currentYScale] / 1e3;
                    var positionindex = (Int32)currentPosdiv * 1000;
                    if (chnl == (Int32)caliChannel)
                    {
                        HdMessage.AnalogOptions ch = Hd.UIMessage!.Analog![(Int32)chnl] with
                        {
                            //校准通道特殊设置
                            Bias = 0,
                            Bandwidth = 0,
                            IsInverted = false,                             //是否反相
                            ProbeIndex = AnaChnlProbe.x10,                  //放大倍率
                            ProbeGain = 10,
                            InputSource = AnaChnlIpnutSource.BNC,           //BNC接入

                            Coupling = (AnaChnlCoupling)currentCouping,     //低阻(即是AnaChnlCoupling.DC50)                            

                            Scale = scaleValueBymV,                         //1V档位
                            ScaleIndex = currentYScale,                     //1V档位

                            PositionIndex = positionindex,                  //0Div
                            Position = positionindex / 1e3 * scaleValueBymV,//0Div
                        };
                        newAnalogOptions.Add(ch);
                        chnlinfo.Impedance_H_Is0 = ch.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
                        chnlinfo.CalcRadio();
                    }
                    else
                    {
                        HdMessage.AnalogOptions ch = Hd.UIMessage!.Analog![(Int32)chnl] with
                        {
                            //其它通道保持现状
                        };
                        newAnalogOptions.Add(ch);
                    }
                }
                Hd.UIMessage = Hd.UIMessage! with { Analog = newAnalogOptions.ToArray() };
                ConfigHardware(Hd.UIMessage, 400);
            }


            //************************** 计算零值的码值理论值 *************************//
            var posdivByAdc = (Int32)currentPosdiv * Constants.SAMPS_PER_YDIV;
            var theoryValue = posdivByAdc + (Math.Pow(2, Constants.ADC_BITS) / 2);
            var soGoodValue = new SoGoodProbeCaliValue() { };//存放最佳值

            UInt32 codeBegin = 0;      //码值开始值
            UInt32 codeEndOf = 0;      //码值结束值
            Boolean codeTrend = true;   //码值趋势
            Boolean need2cali = true;   //是否校准
            Boolean need2save = false;  //是否需要保存
            var acqDataAvge = 0.0;  //采集数据平均值
            var acqDeltaVal = 0.0;  //平均值与理论值差

            do
            {
                //************************** 调试预读码值 *************************//
                List<ushort[]>? channeldata = new();
                if (!AcqWaveData(out channeldata))
                {
                    _ProbeCaliFinishedProgress = GetProbeCaliTotalItemCount();
                    message = "获取采集数据失败，校准未成功";
                    return 0;
                }
                AverageAcqData(channeldata![(Int32)_ProbeCaliChannel], out acqDataAvge);


                //************************** 设置初始码值 *************************//
                if (false == setProbeRegCode((Int32)caliChannel, 0, 0))
                {
                    _ProbeCaliFinishedProgress = GetProbeCaliTotalItemCount();
                    message = "获取采集数据失败，校准未成功";
                    return 0;
                }

                //************************** 读取采集数据 *************************//                
                if (!AcqWaveData(out channeldata))
                {
                    _ProbeCaliFinishedProgress = GetProbeCaliTotalItemCount();
                    message = "调整探头AD模块失败，校准未成功";
                    return 0;
                }

                //************************** 分析校准趋势 *************************//
                AverageAcqData(channeldata![(Int32)_ProbeCaliChannel], out acqDataAvge);
                acqDeltaVal = acqDataAvge - theoryValue;
                soGoodValue.ProbeDacA = 0x0000;
                soGoodValue.ProbeDacB = 0x0000;
                soGoodValue.ProbeAcqDelta = Math.Abs(acqDeltaVal);

                if (Math.Abs(acqDeltaVal) < _ProbeCaliError)
                {
                    need2cali = false;
                    need2save = true;
                    _ProbeCaliFinishedProgress = GetProbeCaliTotalItemCount();
                    message = "校准成功";
                    break;
                }
                else if (acqDataAvge < theoryValue)
                {
                    codeBegin = 0x0000;
                    codeEndOf = 0x7FFF;
                    codeTrend = true;
                    need2cali = true;
                    need2save = false;
                }
                else if (acqDataAvge > theoryValue)
                {
                    codeBegin = 0xFFFF;
                    codeEndOf = 0x8000;
                    codeTrend = false;
                    need2cali = true;
                    need2save = false;
                }
            } while (false);


            UInt32 codeValue = codeBegin;
            UInt32 codeDelta = 1;      //码值变化
            do
            {
                //现有机制不支持这样做
                //if (!Hd.ProbeManager.Read()[_ProbeCaliChannel].IsConnected)
                //{
                //    need2save = false;
                //    message = "探头与示波器连接断开，校准未成功";
                //    break;
                //}
                //************************** 码值计算 *************************//
                if (!need2cali)
                {
                    break;
                }

                if (codeTrend)
                {
                    codeValue += codeDelta;
                    if (codeValue > codeEndOf)
                    {
                        break;
                    }
                }
                else
                {
                    codeValue -= codeDelta;
                    if (codeValue < codeEndOf)
                    {
                        break;
                    }
                }

                //************************** 调整一次码值 *************************//
                if (false == setProbeRegCode((Int32)caliChannel, (UInt16)codeValue, 0))
                {
                    need2save = false;
                    message = "调整探头AD模块失败，校准未成功";
                    break;
                }

                //************************** 读取采集数据 *************************//
                List<ushort[]>? channeldata = new();
                if (!AcqWaveData(out channeldata))
                {
                    need2save = false;
                    message = "获取采集数据失败，校准未成功";
                    break;
                }

                //************************** 判断校准完成 *************************//
                AverageAcqData(channeldata![(Int32)_ProbeCaliChannel], out acqDataAvge);
                acqDeltaVal = acqDataAvge - theoryValue;
                soGoodValue.Update(acqDeltaVal, (UInt16)codeValue, 0);

                uint curVal = (uint)Math.Abs(Math.Max(codeValue, codeBegin) - Math.Min(codeValue, codeBegin));
                uint allVal = (uint)Math.Abs(Math.Max(codeEndOf, codeBegin) - Math.Min(codeEndOf, codeBegin));
                _ProbeCaliFinishedProgress = (int)((curVal / allVal) * 100);

                if (Math.Abs(acqDeltaVal) < _ProbeCaliError)
                {
                    need2save = true;
                    _ProbeCaliFinishedProgress = GetProbeCaliTotalItemCount();
                    message = "校准成功";
                    break;
                }
                else if (Math.Abs(acqDeltaVal) <= 0.5)
                {
                    codeDelta = 1;
                }
                else if (Math.Abs(acqDeltaVal) <= 1)
                {
                    codeDelta = 10;
                }
                else if (Math.Abs(acqDeltaVal) <= 100)
                {
                    codeDelta = 50;
                }
                else
                {
                    codeDelta = 100;
                }

            } while (true);

            if (need2save)
            {
                //保存最佳值到探头
                setProbeRomCode((Int32)caliChannel, soGoodValue.ProbeDacA, soGoodValue.ProbeDacB);
            }

            return res;

        }


        private bool setProbeRegCode(int channelID, UInt16 dacA, UInt16 dacB, Int32 tryCount = 3)
        {
            Int32 tryReadMax = 2;

            for (int syncIdx = 0; syncIdx < tryCount; syncIdx++)
            {
                //写码值-同步
                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeDnRegSet((Int32)channelID, dacA, 0);

                //读码值-检查                
                for (int readIdx = 0; readIdx < tryReadMax; readIdx++)
                {
                    Thread.Sleep(30);

                    var probeDnValue = CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeDnRegGet((Int32)channelID);
                    if (probeDnValue.Length == 0 || probeDnValue.Length != 4)
                    {
                        continue;
                    }

                    UInt16 probeDacA = (UInt16)((UInt16)probeDnValue[0] | (UInt16)probeDnValue[1] << 8);
                    UInt16 probeDacB = (UInt16)((UInt16)probeDnValue[2] | (UInt16)probeDnValue[3] << 8);
                    if (probeDacA == dacA && probeDacB == dacB)
                    {
                        return true;
                    }
                }

                //再次通信前
                Thread.Sleep(30);
            }

            return false;
        }


        private bool setProbeRomCode(int channelID, UInt16 dacA, UInt16 dacB, Int32 tryCount = 3)
        {
            Int32 tryReadMax = 2;

            for (int syncIdx = 0; syncIdx < tryCount; syncIdx++)
            {
                //写码值-同步
                CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeDnRomSet((Int32)channelID, dacA, 0);

                //读码值-检查                
                for (int readIdx = 0; readIdx < tryReadMax; readIdx++)
                {
                    Thread.Sleep(30);

                    var probeDnValue = CtrlAnalogChannel_JiHe2d5G.COMPort_RequestProbeDnRomGet((Int32)channelID);
                    if (probeDnValue.Length == 0 || probeDnValue.Length != 4)
                    {
                        continue;
                    }

                    UInt16 probeDacA = (UInt16)((UInt16)probeDnValue[0] | (UInt16)probeDnValue[1] << 8);
                    UInt16 probeDacB = (UInt16)((UInt16)probeDnValue[2] | (UInt16)probeDnValue[3] << 8);
                    if (probeDacA == dacA && probeDacB == dacB)
                    {
                        return true;
                    }
                }

                //再次通信前
                Thread.Sleep(30);
            }

            return false;
        }
    }
}
