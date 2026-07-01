using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
namespace ScopeX.Hardware.Calibration.Tool
{
    class BatchTask_AutoCaliTiAdc : BatchTaskBase
    {
        private int maxTimes = 10;
        public override int MaxStepCount
        {
            get => maxTimes;
        }
        public override string ResultTipMessage
        {
            get => "校准结束";
        }
        public override string TaskParameterDescription(string tag)
        {
            return "";
        }
        private string sgVisaAddress = "";
        private int currChannelID = 0;
        public override bool Init(IInstrumentSession instrumentInteract, string tipMessage, string description, string tag, out string ErrorMsg)
        {
            ErrorMsg="";
            base.Init(instrumentInteract, tipMessage, description, tag,out ErrorMsg);
            this.ourInstrument = instrumentInteract;
            string[] addressAndch = tag.Split(',');
            if (addressAndch.Length != 2)
                return false;
            sgVisaAddress = addressAndch[0].Trim();
            currChannelID = addressAndch[1].Trim().ToUpper() switch
            {
                "CH1" => 0,
                "CH2" => 1,
                "CH3" => 2,
                "CH4" => 3,
                _ => 0,
            };
            return true;
        }
        IInstrumentSession? sgInstrumentSession = null;//信号源仪器
        public override bool CheckPrepareOk(ref string fileMessage,ref string InstrumentationInfo)
        {
            if (MessageBox.Show("该任务是用于自动校准ADC,请做好如下准备:\r\n1、确认信号源[" + sgVisaAddress + "]已经连接到示波器的通道" + (currChannelID + 1) + "，并打开输出，设置好相应的幅度；\r\n2、示波器软件运行中，并设置好幅度档位和时基。\r\n你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            sgInstrumentSession = new VISASession(sgVisaAddress, 500);

            //if (!sgInstrumentSession.Open())
            //{
            //    MessageBox.Show("对应的仪器不能打开！");
            //    return false;
            //}
            return true;
        }
        protected override void TaskBody()
        {
            state = BatchTaskState.Running;
            int sgFrequencyByHz = 100_000_000;//100Mhz
            sgInstrumentSession?.WriteString("SOUR1:FREQ " + sgFrequencyByHz.ToString());

            Thread.Sleep(500);
            
            for (int i = 0; i < maxTimes; i++)
            {
                updateAction?.Invoke(i, $"正在进行第[{i}]次处理{i}...", "上步处理OK");
                List<AdcRegisterDataFormat>?  readbackAdcRegisterValues=InstrumentInteract.Factory_Cali_Specail_ReadbackAdcRegisterValue(ourInstrument!);
                if (readbackAdcRegisterValues!=null)
                {
                    List<ushort[]>? adcData = InstrumentInteract.Factory_WaveData_Adc(ourInstrument!,6_000);
                    if (adcData != null)
                    {
                        List<short> adc1WaveData = new List<short>();
                        List<short> adc2WaveData = new List<short>();
                        for(int dotIndex=0;dotIndex< adcData[0].Length;dotIndex++)
                        {
                            int index=(currChannelID) switch
                            {
                                0=>0,
                                1=>4,
                                _=>0
                            };
                            adc1WaveData.Add((short)adcData[index+0][dotIndex]);
                            adc1WaveData.Add((short)adcData[index+1][dotIndex]);

                            adc2WaveData.Add((short)adcData[index+2][dotIndex]);
                            adc2WaveData.Add((short)adcData[index+3][dotIndex]);

                        }
                        WaveOffsetGainPhase waveOffsetGainPhaseAdc1 = SineFitFunc.SineFit(adc1WaveData.ToArray(), 10_000, 100);
                        WaveOffsetGainPhase waveOffsetGainPhaseAdc2 = SineFitFunc.SineFit(adc2WaveData.ToArray(), 10_000, 100);
                        WaveOffsetGainPhase deltaWaveOffsetGainPhase = new WaveOffsetGainPhase();
                        deltaWaveOffsetGainPhase.Gain = waveOffsetGainPhaseAdc2.Gain - waveOffsetGainPhaseAdc1.Gain;
                        deltaWaveOffsetGainPhase.Offset = waveOffsetGainPhaseAdc2.Offset - waveOffsetGainPhaseAdc1.Offset;
                        deltaWaveOffsetGainPhase.Phase = waveOffsetGainPhaseAdc2.Phase - waveOffsetGainPhaseAdc1.Phase;
                        //AdcPhaseOffsetGainItem tiAdcItem = TiAdc_PhaseOffsetGain.Default[currChannelID, 1, 0];
                        //if (deltaWaveOffsetGainPhase.Gain > 0)
                        //    tiAdcItem.GainErr--;
                        //else if (deltaWaveOffsetGainPhase.Gain < 0)
                        //    tiAdcItem.GainErr++;
                        //if (deltaWaveOffsetGainPhase.Offset > 0)
                        //    tiAdcItem.OffsetErr--;
                        //else if (deltaWaveOffsetGainPhase.Offset < 0)
                        //    tiAdcItem.OffsetErr++;
                        //if (deltaWaveOffsetGainPhase.Phase > 0)
                        //    tiAdcItem.PhaseErr--;
                        //else if (deltaWaveOffsetGainPhase.Phase < 0)
                        //    tiAdcItem.PhaseErr++;
                        //TiAdc_PhaseOffsetGain.Default[currChannelID, 1, 0] = tiAdcItem;
                        //InstrumentInteract.CaliData_Send(ourInstrument, CaliDataType.TiAdc_PhaseOffsetGain);
                        Thread.Sleep(1000);
                    }
                }

                if (cancelTokenSrc != null)
                {
                    try
                    {
                        cancelTokenSrc.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        state = BatchTaskState.Canceled;
                        return;
                    }
                }
            }
            state = BatchTaskState.FinishedOK;
        }
    }
}
