using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.DataSource;
using ScopeX.Core.PowerAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{

    public enum DemoType
    {
        None,
        PWRQuality,
        PWRHarmonic,
        SwitchingLoss,
        Ripple,
    }
    internal class Demo
    {
        public DemoType Type { get; set; }
        private DsoPrsnt _DsoPrsnt = null;
        private String _PWRQualityC1FilePath = "./Resources\\DemoWaves\\PWQUASCREENC1.bin";
        private String _PWRQualityC2FilePath = "./Resources/DemoWaves/PWQUASCREENC2.bin";
        private String _RIPPERC1FilePath = "./Resources/DemoWaves/RIPPERC1.bin";

        
        private Boolean _IsControl = false;
        public Demo(DsoPrsnt dso)
        {
            _DsoPrsnt = dso;
        }

        public void AddDemo(DemoType demoType)
        {
            _IsControl = true;
            switch (demoType)
            {
                case DemoType.None:
                    break;
                case DemoType.PWRQuality:
                    {
                        AddPWRQuality();
                    }
                    break;
                case DemoType.PWRHarmonic:
                    {
                        AddPWRHarmonic();
                    }
                    break;
                case DemoType.SwitchingLoss:
                    {
                        AddSwitchingLoss();
                    }
                    break;
                case DemoType.Ripple:
                    {
                        AddRipple();
                    }
                    break;
                default:
                    break;
            }

            Type = demoType;
            _IsControl = false;
        }

        private void AddPWRQuality()
        {
            //恢复到默认状态
            _DsoPrsnt.Default();

            //设置垂直档位
            if (_DsoPrsnt.TryGetChannel(ChannelId.C1, out IChnlPrsnt? channel) && channel is AnalogPrsnt analogprsnt)
            {
                analogprsnt.ScaleBymV = 10000;
                analogprsnt.ProbeIndex = AnaChnlProbe.x10;
                analogprsnt.Active = true;
            }
            if (_DsoPrsnt.TryGetChannel(ChannelId.C2, out IChnlPrsnt? channelc2) && channelc2 is AnalogPrsnt analogprsntc2)
            {
                analogprsntc2.Unit = "A";
                analogprsntc2.ProbeUnitRatio = 1;
                analogprsntc2.ProbeUnitIsCustomized = true;
                analogprsntc2.ProbeIndex = AnaChnlProbe.x1;
                analogprsntc2.ScaleBymV = 200;
                analogprsntc2.Active = true;
            }

            //绑定数据源
            DataSrcFile datasrcfile = new DataSrcFile(_PWRQualityC1FilePath);

            var analogmodelc1 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C1).FirstOrDefault();
            if (analogmodelc1 != null)
            {
                analogmodelc1.PrepareSamples = datasrcfile.Prepare;
                analogmodelc1.ReadSamples = datasrcfile.Read;
                analogmodelc1.ProcessSamples = datasrcfile.Process;
            }

            DataSrcFile datasrcfilec2 = new DataSrcFile(_PWRQualityC2FilePath);
            var analogmodelc2 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C2).FirstOrDefault();
            if (analogmodelc2 != null)
            {
                analogmodelc2.PrepareSamples = datasrcfilec2.Prepare;
                analogmodelc2.ReadSamples = datasrcfilec2.Read;
                analogmodelc2.ProcessSamples = datasrcfilec2.Process;
            }
            //添加电源分析—电源质量
            PowerAnalysisPrsnt pwprsnt;
            PowerAnalysisPrsnt.TryAddPowerAnalysis(PowerAnalysisOpt.PowerQuality, out pwprsnt, ChannelId.C1, ChannelId.C2);
            
            //设置时基
            _DsoPrsnt.Timebase.PositionByus = 0;
            _DsoPrsnt.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv10m;
            _DsoPrsnt.Timebase.IsZoom = false;

            Thread.Sleep(400);

            //设置为Stop，触发为Normal
            TriggerModel.State = SysState.Stop;
            _DsoPrsnt.Stop();
            TriggerModel.Mode = TriggerMode.Normal;

            pwprsnt!.BoundMathPrsnt1!.Scale = 50000;
        }
        private void AddPWRHarmonic()
        {
            //恢复到默认状态
            _DsoPrsnt.Default();

            //设置垂直档位
            if (_DsoPrsnt.TryGetChannel(ChannelId.C1, out IChnlPrsnt? channel) && channel is AnalogPrsnt analogprsnt)
            {
                analogprsnt.ScaleBymV = 10000;
                analogprsnt.ProbeIndex = AnaChnlProbe.x10;
                analogprsnt.Active = true;
            }
            if (_DsoPrsnt.TryGetChannel(ChannelId.C2, out IChnlPrsnt? channelc2) && channelc2 is AnalogPrsnt analogprsntc2)
            {
                analogprsntc2.Unit = "A";
                analogprsntc2.ProbeUnitRatio = 1;
                analogprsntc2.ProbeUnitIsCustomized = true;
                analogprsntc2.ProbeIndex = AnaChnlProbe.x1;
                analogprsntc2.ScaleBymV = 200;
                analogprsntc2.Active = true;
            }

            //绑定数据源
            DataSrcFile datasrcfile = new DataSrcFile(_PWRQualityC1FilePath);

            var analogmodelc1 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C1).FirstOrDefault();
            if (analogmodelc1 != null)
            {
                analogmodelc1.PrepareSamples = datasrcfile.Prepare;
                analogmodelc1.ReadSamples = datasrcfile.Read;
                analogmodelc1.ProcessSamples = datasrcfile.Process;
            }

            DataSrcFile datasrcfilec2 = new DataSrcFile(_PWRQualityC2FilePath);
            var analogmodelc2 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C2).FirstOrDefault();
            if (analogmodelc2 != null)
            {
                analogmodelc2.PrepareSamples = datasrcfilec2.Prepare;
                analogmodelc2.ReadSamples = datasrcfilec2.Read;
                analogmodelc2.ProcessSamples = datasrcfilec2.Process;
            }
            //添加电源分析—谐波分析
            if (PowerAnalysisPrsnt.TryAddPowerAnalysis(PowerAnalysisOpt.Harmonic, out var pwprsnt, ChannelId.C1, ChannelId.C2))
            {
                pwprsnt!.BoundMathPrsnt1!.Scale = 50000;
            }
            //设置时基
            _DsoPrsnt.Timebase.PositionByus = 0;
            _DsoPrsnt.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv10m;
            _DsoPrsnt.Timebase.IsZoom = false;

            Thread.Sleep(1000);

            //设置为Stop，触发为Normal
            TriggerModel.State = SysState.Stop;
            _DsoPrsnt.Stop();
            TriggerModel.Mode = TriggerMode.Normal;
        }
        private void AddSwitchingLoss()
        {
            //恢复到默认状态
            _DsoPrsnt.Default();

            //设置垂直档位
            if (_DsoPrsnt.TryGetChannel(ChannelId.C1, out IChnlPrsnt? channel) && channel is AnalogPrsnt analogprsnt)
            {
                analogprsnt.ScaleBymV = 10000;
                analogprsnt.ProbeIndex = AnaChnlProbe.x10;
                analogprsnt.Active = true;
            }
            if (_DsoPrsnt.TryGetChannel(ChannelId.C2, out IChnlPrsnt? channelc2) && channelc2 is AnalogPrsnt analogprsntc2)
            {
                analogprsntc2.Unit = "A";
                analogprsntc2.ProbeUnitRatio = 1;
                analogprsntc2.ProbeUnitIsCustomized = true;
                analogprsntc2.ProbeIndex = AnaChnlProbe.x1;
                analogprsntc2.ScaleBymV = 200;
                analogprsntc2.Active = true;
            }

            //绑定数据源
            DataSrcFile datasrcfile = new DataSrcFile(_PWRQualityC1FilePath);

            var analogmodelc1 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C1).FirstOrDefault();
            if (analogmodelc1 != null)
            {
                analogmodelc1.PrepareSamples = datasrcfile.Prepare;
                analogmodelc1.ReadSamples = datasrcfile.Read;
                analogmodelc1.ProcessSamples = datasrcfile.Process;
            }

            DataSrcFile datasrcfilec2 = new DataSrcFile(_PWRQualityC2FilePath);
            var analogmodelc2 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C2).FirstOrDefault();
            if (analogmodelc2 != null)
            {
                analogmodelc2.PrepareSamples = datasrcfilec2.Prepare;
                analogmodelc2.ReadSamples = datasrcfilec2.Read;
                analogmodelc2.ProcessSamples = datasrcfilec2.Process;
            }
            //添加电源分析—开关损耗
            if (PowerAnalysisPrsnt.TryAddPowerAnalysis(PowerAnalysisOpt.SwitchingLoss, out var pwprsnt, ChannelId.C1, ChannelId.C2))
            {
                pwprsnt!.BoundMathPrsnt1!.Scale = 50000;
            }
            //设置时基
            _DsoPrsnt.Timebase.PositionByus = 0;
            _DsoPrsnt.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv10m;
            _DsoPrsnt.Timebase.IsZoom = false;

            Thread.Sleep(1000);

            //设置为Stop，触发为Normal
            TriggerModel.State = SysState.Stop;
            _DsoPrsnt.Stop();
            TriggerModel.Mode = TriggerMode.Normal;
        }
        private void AddRipple()
        {
            //恢复到默认状态
            _DsoPrsnt.Default();

            //设置垂直档位
            if (_DsoPrsnt.TryGetChannel(ChannelId.C1, out IChnlPrsnt? channel) && channel is AnalogPrsnt analogprsnt)
            {
                analogprsnt.ScaleBymV = 10;
                analogprsnt.ProbeIndex = AnaChnlProbe.x1;
                analogprsnt.Coupling = AnaChnlCoupling.AC1M;
                analogprsnt.Bandwidth = analogprsnt.BandWidthNames.Last().Index;
                analogprsnt.Active = true;
            }

            //绑定数据源
            DataSrcFile datasrcfile = new DataSrcFile(_RIPPERC1FilePath);

            var analogmodelc1 = DsoModel.Default.AnalogChnls.Where(x => x.Id == ChannelId.C1).FirstOrDefault();
            if (analogmodelc1 != null)
            {
                analogmodelc1.PrepareSamples = datasrcfile.Prepare;
                analogmodelc1.ReadSamples = datasrcfile.Read;
                analogmodelc1.ProcessSamples = datasrcfile.Process;
            }

            //添加电源分析—纹波分析
            if (PowerAnalysisPrsnt.TryAddPowerAnalysis(PowerAnalysisOpt.Ripple, out var pwprsnt, ChannelId.C1, ChannelId.C2))
            {
                pwprsnt!.BoundMathPrsnt1!.Scale = 50000;
            }
            //设置时基
            _DsoPrsnt.Timebase.PositionByus = 0;
            _DsoPrsnt.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv5u;
            _DsoPrsnt.Timebase.IsZoom = false;

            Thread.Sleep(1000);

            //设置为Stop，触发为Normal
            TriggerModel.State = SysState.Stop;
            _DsoPrsnt.Stop();
            TriggerModel.Mode = TriggerMode.Normal;
        }
        public void RemoveDemo()
        {
            switch (Type)
            {
                case DemoType.None:
                    break;
                case DemoType.PWRQuality:
                    break;
                case DemoType.PWRHarmonic:
                    break;
                default:
                    break;
            }
            if (Type != DemoType.None && _IsControl == false)
            {
                Acquisition.Default.BindDataSource(DsoModel.DataSrcOpt);
                Type = DemoType.None;
            }
        }
    }
}
