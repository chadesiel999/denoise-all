// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;
    using ScopeX.Hardware.Driver;

    public class WidgetPrsnt
    {

        private static Int32 _Combo = 0;

        public static Int32 Combo
        {
            get => _Combo;
            set
            {
                if (_Combo != value)
                {
                    _Combo = value;
                    Hardware.HdCmdFactory.Push(ComModel.HdCmd.Combo);
                }
            }
        }

        public static Double GetAnalogChnlTemperature(Int32 index = 0) => Hd.SystemMonitor.GetAnalogChannelTemperature(index);

        public static Action<Int32, Int32>? SetMousePosHandler { get; set; }

        public static Action<UInt32, Int32>? MouseEventHandler { get; set; }

        public static Action<Byte, Int32>? KeyboradEventHandler { get; set; }

        public static void PushHdCmd(HdCmd cmd)
        {
            Hardware.HdCmdFactory.Push(cmd);
        }

        public static String HardwareMiscFunc(String funcName,String param)
        {
            if (DsoPrsnt.DataSrcOpt == DataSourceOpt.Simulator)
                return "";
            Dispatcher.Cancel();
            String result=Hd.MiscFunc(funcName, param);
            DsoModel.Default.TempCtrl.UpdateCaliTemp();
            Dispatcher.Run();
            return result;
        }


        //public static void SetColor(ChannelId id, Color color)
        //{
        //    ColorConfig.Default[id.ToString()] = color;

        //    if (DsoModel.Default.TryGetChannel(id, out var cm))
        //    {
        //        cm.DrawColor = color;
        //    }
        //}

        //public static Color GetColor(ChannelId id)
        //{
        //    if (DsoModel.Default.TryGetChannel(id, out var cm))
        //    {
        //        return cm.DrawColor;
        //    }
        //    return ColorConfig.Default[id.ToString()];
        //}

        //public static Int32 ColorLength => ColorConfig.Default.Count;

        //public static IReadOnlyCollection<ColorInfo> Colors => ColorConfig.Default.Colors;

        //public static void ResetColor()
        //{
        //    ColorConfig.Default.Reset();
        //    foreach (var id in ChannelId.GetValues<ChannelId>())
        //    {
        //        if (DsoModel.Default.TryGetChannel(id, out var cm))
        //        {
        //            cm.DrawColor = ColorConfig.Default[id.ToString()];
        //        }
        //    }
        //}
    }
}
