using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MathWorks.MATLAB.NET.Arrays;
//using MatlabTool;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.Core.Tools
{
    /// <summary>
    /// 调用Matlab函数库生成文件，需要release下调用
    /// </summary>
    public class MatlabAux
    {
        //public static MatlabAux Instance { get; private set; } = new MatlabAux();

        //private static Boolean _Init = false;

        //private static MatlabHelper mat;

        //private MatlabAux()
        //{
        //}

        //public Boolean Init()
        //{
        //    try
        //    {
        //        mat = new MatlabHelper();
        //    }
        //    catch
        //    {
        //        _Init = false;
        //    }
        //    return _Init;
        //}


        //public Boolean SaveWaveByMatlab(WfmPack pkg, String path)
        //{
        //    if(!_Init)
        //        return false;

        //    String flag = String.Empty;
        //    try
        //    {
        //        Int32 saveparacount = 1 + 10 + 1 + 1;//一个地址，10个独立参数，1个时间数组，1个幅度数组
        //        MWArray[] infos = new MWArray[11];

        //        infos[0] = path;
        //        infos[1] = FilePrsnt.FileInfo[0] + $":{DsoModel.Default.File.WfmSource}";
        //        infos[2] = FilePrsnt.FileInfo[1] + $":{SIHelper.ValueChangeToSI(pkg.Properties.TmbScale.Value / 1E6, 0, "s")}";
        //        infos[3] = FilePrsnt.FileInfo[2] + $":{SIHelper.ValueChangeToSI(pkg.Properties.ChnlScale.Value / 1E3, 0, "V")}";
        //        infos[4] = FilePrsnt.FileInfo[3] + $":{SIHelper.ValueChangeToSI(DsoModel.Default.Timebase.AnalogSamplingRate, 2, "Sa/s")}";
        //        infos[5] = FilePrsnt.FileInfo[4] + $":{DsoModel.Default.Timebase.StorageWaveDotsCnt}";
        //        infos[6] = FilePrsnt.FileInfo[5] + $":{pkg.Properties.TmbScale.Value}" + $":{pkg.Properties.TmbScale.Index}";
        //        infos[7] = FilePrsnt.FileInfo[6] + $":{pkg.Properties.TmbPosition.Value}" + $":{pkg.Properties.TmbPosition.Index}";
        //        infos[8] = FilePrsnt.FileInfo[7] + $":{pkg.Properties.ChnlScale.Value}" + $":{pkg.Properties.ChnlScale.Index}";
        //        infos[9] = FilePrsnt.FileInfo[8] + $":{pkg.Properties.ChnlPosition.Value}" + $":{pkg.Properties.ChnlPosition.Index}";
        //        infos[10] = FilePrsnt.FileInfo[9] + $":{pkg.Properties.SampInterval}";

        //        Int32 length = pkg.Buffer.GetLength(1);
        //        Int32 wfmcnt = pkg.Buffer.GetLength(0);
        //        Double sp = pkg.Properties.TmbScale.Value * Constants.VIS_XDIVS_NUM / length;
        //        Double pos0 = pkg.Properties.TmbPosition.Index;
        //        Double[] ampls = new Double[wfmcnt];
        //        Double[] times = new Double[length];
        //        for (Int32 j = 0; j < length; j++)
        //        {
        //            times[j] = (j * sp - pos0) + pkg.Properties.TrigErrorTime;
        //            for (Int32 i = 0; i < wfmcnt; i++)
        //            {
        //                ampls[i] = pkg.Buffer[i, j];
        //            }
        //        }

        //        MWNumericArray arr1 = (MWNumericArray)ampls;
        //        MWNumericArray arr2 = (MWNumericArray)times;

        //        var result = mat.MatSave(saveparacount, infos[0], infos[1], infos[2], infos[3], infos[4], infos[5], infos[6], infos[7], infos[8], infos[9], infos[10], arr1, arr2);

        //        if (result != null && result.Count() > 0)
        //        {
        //            flag = new String((result[0].ToArray() as char[]));
        //        }
        //        if(flag.Equals("1"))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(flag, EventBus.LogLevel.Debug));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(e.ToString(), EventBus.LogLevel.Debug));
        //    }

        //    return false;
        //}

    }
}
