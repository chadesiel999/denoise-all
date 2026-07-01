using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class TimebaseTableByus
    {
        public static readonly Dictionary<AnaChnlTimebaseIndex, (Double Scale, Double MinPosIndex)> Table = new()
        {
            [AnaChnlTimebaseIndex.Lv2p] = (2e-6, 0),
            [AnaChnlTimebaseIndex.Lv5p] = (5e-6, 0),
            [AnaChnlTimebaseIndex.Lv10p] = (10e-6, 0),
            [AnaChnlTimebaseIndex.Lv20p] = (20e-6, 0),
            [AnaChnlTimebaseIndex.Lv50p] = (50e-6, 0),
            [AnaChnlTimebaseIndex.Lv100p] = (100e-6, 0),
            [AnaChnlTimebaseIndex.Lv200p] = (200e-6, 0),
            [AnaChnlTimebaseIndex.Lv500p] = (500e-6, 0),
            [AnaChnlTimebaseIndex.Lv1n] = (1e-3, 0),
            [AnaChnlTimebaseIndex.Lv2n] = (2e-3, 0),
            [AnaChnlTimebaseIndex.Lv5n] = (5e-3, 0),
            [AnaChnlTimebaseIndex.Lv10n] = (10e-3, 0),
            [AnaChnlTimebaseIndex.Lv20n] = (20e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-8) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50n] = (50e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-8) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100n] = (100e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-7) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200n] = (200e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-7) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500n] = (500e-3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-7) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1u] = (1e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-6) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2u] = (2e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-6) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5u] = (5e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-6) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10u] = (10e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20u] = (20e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50u] = (50e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100u] = (100e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-4) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200u] = (200e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-4) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500u] = (500e0, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-4) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1m] = (1e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 1E-3) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2m] = (2e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 2E-3) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5m] = (5e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 5E-3) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10m] = (10e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.01) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20m] = (20e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.02) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50m] = (50e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.05) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100m] = (100e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.1) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200m] = (200e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.2) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500m] = (500e3, Math.Round((5 - Constants.MIN_XPOS_TIME / 0.5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1] = (1e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 1) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2] = (2e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 2) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5] = (5e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 5) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv10] = (10e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 10) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20] = (20e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 20) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50] = (50e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 50) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100] = (100e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 100) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200] = (200e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 200) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500] = (500e6, Math.Round((5 - Constants.MIN_XPOS_TIME / 500) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1k] = (1e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 1_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2k] = (2e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 2_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5k] = (5e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 5_000) * 1_000, MidpointRounding.AwayFromZero)),

            [AnaChnlTimebaseIndex.Lv10k] = (10e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 10_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv20k] = (20e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 20_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv50k] = (50e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 50_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv100k] = (100e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 100_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv200k] = (200e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 200_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv500k] = (500e9, Math.Round((5 - Constants.MIN_XPOS_TIME / 500_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv1M] = (1e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 1_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv2M] = (2e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 2_000_000) * 1_000, MidpointRounding.AwayFromZero)),
            [AnaChnlTimebaseIndex.Lv5M] = (5e12, Math.Round((5 - Constants.MIN_XPOS_TIME / 5_000_000) * 1_000, MidpointRounding.AwayFromZero)),
        };
    }
}
