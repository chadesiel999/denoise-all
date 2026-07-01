using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiSet_SystemCommon(SCPICommandProcessFuncParam analyResult)
        {
            List<string> param = ParamListToStrList(analyResult.Params);
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.Tag == null)
                return false;
            switch (scpiTagObj.Tag.ToString())
            {
                case "SysAuto":
                    //todo 
                    AutoSet();
                    break;
                case "SysRun":
                    if (TriggerPrsnt.State == SysState.Stop)
                        Presenter.Resume();
                    else
                        return false;
                    return true;
                case "SysStop":
                    if (TriggerPrsnt.State != SysState.Stop)
                        Presenter.Stop();
                    else
                        return false;
                    return true;
                case "SysSingle":
                    TriggerPrsnt.Mode = TriggerMode.OneShot;
                    Presenter.Resume();
                    return true;
                case "SysClear":
                    //todo 
                    Presenter.VKClear();
                    break;
                case "SysTFORce":
                    TriggerPrsnt.Force();
                    break;
                case "SysCALI":
                    AutoCali();
                    break;
                case "SysAiSet":
                    {
                        if (Presenter?.ArtificialIntelligence == null)
                            return false;

                        if (param == null || param.Count == 0 || string.IsNullOrWhiteSpace(param[0]))
                        {
                            Presenter.ArtificialIntelligence.ActionAiSetByScpi();
                            return true;
                        }

                        if (!TryNormalizeAiSetSignalType(param[0], out string signalType))
                            return false;

                        Presenter.ArtificialIntelligence.ActionAiSetByScpi(signalType);
                        return true;
                    }
                case "SysMini":
                    Presenter.VuMinimize?.Invoke();
                    break;
                case "SysClose":
                    Presenter.VuClose?.Invoke();
                    break;
                case "SysShutDown":
                    Presenter.VuShutDowm?.Invoke();
                    break;
                case "SysRestart":
                    Presenter.VuRestart?.Invoke();
                    break;
                case "SysLogout":
                    Presenter.VuLogout?.Invoke();
                    break;
                default:
                    return false;
            }
            return true;
        }



        public static bool scpiQuy_SystemCommon(SCPIManager.SCPICommandProcessFuncParam analyResult, ref SCPIManager.SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.Tag == null)
                return false;

            switch (scpiTagObj.Tag.ToString())
            {
                case "SysAiSetStatus":
                    if (Presenter?.ArtificialIntelligence == null)
                        return false;
                    sendMessage.SendData = decodeStr(Presenter.ArtificialIntelligence.GetAiSetScpiStatusJson());
                    return true;
                default:
                    return false;
            }
        }
        public static bool scpiSet_SystemAutoCommon(SCPICommandProcessFuncParam analyResult)
        {
            return false;
        }
        public static bool scpiQuy_SystemAutoCommon(SCPIManager.SCPICommandProcessFuncParam analyResult, ref SCPIManager.SCPISendMessage sendMessage)
        {
            return false;
        }

        public static bool scpiQuy_SystemPrintScreen(SCPIManager.SCPICommandProcessFuncParam analyResult, ref SCPIManager.SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var image = Presenter.File.GetImageByteArray();
            if (image != null)
            {
                sendMessage.SendData = image;
                sendMessage.IsDataBlock = true;
                returnResult = true;
            }
            return returnResult;
        }

        public static bool scpiSet_SegementCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            List<string> param = ParamListToStrList(analyResult.Params);
            if (param.Count > 0)
            {
                if (param[0] == "1" || param[0].ToUpper() == "ON")
                {
                    Presenter.Timebase.ResetAcq();
                }
                returnResult = true;
            }
            return returnResult;
        }
        private static async void AutoSet()
        {
            await Task.Run(() =>
            {
                Presenter.AutoSet.Run(new System.Threading.CancellationToken());
            });

        }
        private static async void AutoCali()
        {
            await Task.Run(() =>
            {
                Presenter.AutoCalibration.Use2SCPI?.Invoke();
            });
        }

        private static bool TryNormalizeAiSetSignalType(string input, out string signalType)
        {
            signalType = string.Empty;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var key = input.Trim().ToUpperInvariant();
            signalType = key switch
            {
                "SINE" or "SIN" or "SINUSOID" => "Sine",
                "SQUARE" => "Square",
                "TRI" or "TRIANGLE" => "Tri",
                "PULSE" => "Pulse",
                "AM" => "AM",
                "LFM" => "LFM",
                "SFM" => "SFM",
                "BPSK" => "BPSK",
                "QPSK" => "QPSK",
                "8PSK" => "8PSK",
                "16QAM" => "16QAM",
                "32QAM" => "32QAM",
                "64QAM" => "64QAM",
                "128QAM" => "128QAM",
                _ => string.Empty
            };

            return !string.IsNullOrEmpty(signalType);
        }
    }
}
