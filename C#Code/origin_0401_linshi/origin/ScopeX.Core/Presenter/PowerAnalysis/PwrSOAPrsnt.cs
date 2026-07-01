using System;
using System.Collections.Generic;
using ScopeX.ComModel;

namespace ScopeX.Core.PowerAnalysis
{
    public class PwrSOAPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrSOAPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.SafeOpArea;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public readonly Int32 Count = 3;

        public Boolean TestSucceed => Model.TestSucceed;

        public Int32 AcqWfms => Model.AcqWfms;

        public Int32 FailWfms => Model.FailWfms;

        public Boolean StopOnFail
        {
            get => Model.StopOnFault;
            set => Model.StopOnFault = value;
        }

        public Double MaxVoltage
        {
            get => Model.MaxVoltage;
            set => Model.MaxVoltage = value;
        }
        public Int64? WindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }
        public Double MaxMaxVoltage => Model.MaxMaxVoltage;

        public Double MinMaxVoltage => Model.MinMaxVoltage;

        public Double MaxCurrent
        {
            get => Model.MaxCurrent;
            set => Model.MaxCurrent = value;
        }

        public Double MaxMaxCurrent => Model.MaxMaxCurrent;

        public Double MinMaxCurrent => Model.MinMaxCurrent;

        public Double MaxPower
        {
            get => Model.MaxPower;
            set => Model.MaxPower = value;
        }

        public Double MaxMaxPower => Model.MaxMaxPower;

        public Double MinMaxPower => Model.MinMaxPower;

        public AxisType AxisType
        {
            get => Model.AxisType;
            set => Model.AxisType = value;
        }

        public Double MaxLogY
        {
            get => Model.MaxLogY;
            set => Model.MaxLogY = value;
        }

        public Double MaxMaxLogY => Model.MaxMaxLogY;

        public Double MinMaxLogY => Model.MinMaxLogY;

        public Double MinLogY
        {
            get => Model.MinLogY;
            set => Model.MinLogY = value;
        }

        public Double MaxMinLogY => Model.MaxMinLogY;

        public Double MinMinLogY => Model.MinMinLogY;

        public Double MaxLogX
        {
            get => Model.MaxLogX;
            set => Model.MaxLogX = value;
        }

        public Double MaxMaxLogX => Model.MaxMaxLogX;

        public Double MinMaxLogX => Model.MinMaxLogX;

        public Double MinLogX
        {
            get => Model.MinLogX;
            set => Model.MinLogX = value;
        }

        public Double MaxMinLogX => Model.MaxMinLogX;

        public Double MinMinLogX => Model.MinMinLogX;

        public Double MaxLinYBymV
        {
            get => MaxLinY * 1_000D;
            set => MaxLinY = value / 1_000D;
        }
        public Double MaxLinY
        {
            get => Model.MaxLinY;
            set => Model.MaxLinY = value;
        }

        public Double MaxMaxLinY => Model.MaxMaxLinY;

        public Double MinMaxLinY => Model.MinMaxLinY;

        public Double MinLinYBymV
        {
            get => MinLinY * 1_000D;
            set => MinLinY = value / 1_000D;
        }
        public Double MinLinY
        {
            get => Model.MinLinY;
            set => Model.MinLinY = value;
        }

        public Double MaxMinLinY => Model.MaxMinLinY;

        public Double MinMinLinY => Model.MinMinLinY;

        public Double MaxLinXBymV
        {
            get => MaxLinX * 1_000D;
            set => MaxLinX = value / 1_000D;
        }

        public Double MaxLinX
        {
            get => Model.MaxLinX;
            set => Model.MaxLinX = value;
        }

        public Double MaxMaxLinX => Model.MaxMaxLinX;

        public Double MinMaxLinX => Model.MinMaxLinX;

        public Double MinLinXBymV
        {
            get => MinLinX * 1_000D;
            set => MinLinX = value / 1_000D;
        }
        public Double MinLinX
        {
            get => Model.MinLinX;
            set => Model.MinLinX = value;
        }

        public Double MaxMinLinX => Model.MaxMinLinX;

        public Double MinMinLinX => Model.MinMinLinX;
        public IReadOnlyList<IReadOnlyList<(Double x, Double y)>> Hits => Model.Hits;
        public List<(Double x, Double y)> MaskData
        {
            get => Model.MaskData;
            set => Model.MaskData = value;
        }
        public Boolean MaskDataChanged { get => Model.MaskDataChanged; }
        public Boolean CalcCompleted
        {
            get => Model.RunCompleted;
            set => Model.RunCompleted = value;
        }
        private protected override PwrSOAModel Model { get; }

        public void Reset() => Model.Reset();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public void TryShowSOAWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                mp.Args.Occupier = null;
                mp.Label = "";
                mp.Active = false;
                mp.IsAutoUnit = false;
            }
        }

        public List<(Double x, Double y)> GetMaskRegion(Int32 width, Int32 height)
        {
            var region = new List<(Double x, Double y)>();
            if (AxisType == AxisType.Linear)
            {
                //foreach (var (x, y) in Model.MaskRegion)
                //{
                //    region.Add(((x - MinLinX) / (MaxLinX - MinLinX) * width, (y - MinLinY) / (MaxLinY - MinLinY) * height - Constants.MAX_VCURSOR_IDX));
                //}
                foreach (var (x, y) in MaskData)
                {
                    region.Add(((x - MinLinX) / (MaxLinX - MinLinX) * width, (y - MinLinY) / (MaxLinY - MinLinY) * height - Constants.MAX_VCURSOR_IDX));
                }
            }
            else
            {
                var ydiv = Math.Log10(MaxLogY / MinLogY);
                var xdiv = Math.Log10(MaxLogX / MinLogX);

                foreach (var (x, y) in Model.MaskRegion)
                {
                    if (x <= 0 || y <= 0)
                        continue;
                    region.Add((Math.Log10(x / MinLogX) / xdiv * width, Math.Log10(y / MinLogY) / ydiv * height - Constants.VIS_YDIVS_NUM * Constants.IDX_PER_YDIV / 2));
                }
            }

            return region;
        }

    }
}
