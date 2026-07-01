using System;
using System.Collections.Generic;
using System.ComponentModel;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Core
{
    internal class SearchEdgeModel :TriggerEdgeModel, ISearchTypeModel
    {
        public new ChannelId Source { get => base.Source; set => base.Source = value; }

        public Int32 ResultCount { get; set; }

        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        public ISearchTypeOptions GetOption()
        {
            return new TrigEdgeOptions(this.Source, this.Slope, this.CompPosIndex)
            {
                Position = this.CompPosition,
                Coupling = this.Coupling,
                Impedance = this.Impedance,
            };
        }

        public List<String> GetKeyInfos()
        {
            if (Source== ChannelId.AuxIn)
            {
                return new List<String>()
                    {
                          nameof(SearchType.Edge),
                          String.Empty,
                          String.Empty
                    };
            }
            else if (Source == ChannelId.AC || Source.IsDigital())
            {
                return new List<String>()
                    {
                          nameof(SearchType.Edge),
                          String.Empty,
                          this.Slope.ToString()
                    };
            }
            else
            {
                return new List<String>()
                    {
                          nameof(SearchType.Edge),
                          new Quantity(CompPosition, PosPrefix, PosUnit).ToString(5, true),
                          this.Slope.ToString()
                    };
            }

            return new List<String>();
        }
    }



    internal class SearchPulseModel :TriggerWidthModel, ISearchTypeModel
    {
        public SearchPulseModel() : base()
        {
        }

        public new ChannelId Source { get => base.Source; set => base.Source = value; }

        public Int32 ResultCount { get; set; }

        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }



        public ISearchTypeOptions GetOption()
        {
            return new TrigPulseOptions(this.Source, this.Polarity, this.Condition, this.WidthByps, this.UpperWidthByps, this.CompPosIndex)
            {
                Position = this.CompPosition,
            };
        }

        public List<String> GetKeyInfos()
        {
            return GetPulseWidthInfoList();
        }

        private List<String> GetPulseWidthInfoList()
        {
            var cond = Condition switch
            {
                PulseCondition.GreaterThan => ">",
                PulseCondition.LessThan => "<",
                PulseCondition.Equal => "[ ... ]",
                _ => "",
            };
            String condwidth = String.Empty;
            if (Condition == PulseCondition.GreaterThan)
            {
                condwidth = new Tools.Quantity(WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
                condwidth = $"{cond} {condwidth}";
            }
            else if (Condition == PulseCondition.LessThan)
            {
                condwidth = new Tools.Quantity(UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true);
                condwidth = $"{cond} {condwidth}";
            }
            if (Condition == PulseCondition.Equal || Condition == PulseCondition.NotEqual)
            {
                condwidth = $"{new Quantity(WidthByps, Prefix.Pico, "s").ToString("##0.###", true)}\n{new Tools.Quantity(UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true)}";
                condwidth = $"{cond}\t{condwidth}";
            }

            if (Source == ChannelId.AC || Source.IsDigital())
            {
                return new List<String>()
                {
                    "MaiKuan",
                    $"{Polarity}",
                   condwidth,
                };
            }
            else
            {
                return new List<String>()
                {
                    "MaiKuan",
                    $"{Polarity}\n{ new Quantity(CompPosition, PosPrefix, PosUnit).ToString(5, true)}",
                   condwidth,
                };
            }

        }
    }

    internal class SearchTransitionModel :TriggerTransModel, ISearchTypeModel
    {
        public new ChannelId Source { get => base.Source; set => base.Source = value; }
        public Int32 ResultCount { get; set; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        public ISearchTypeOptions GetOption()
        {
            return new TrigTransOptions(this.Source, this.Slope, this.Condition, this.WidthByps, this.UpperWidthByps, this.PosUpperIndex, this.PosLowerIndex)
            {
                UpperPosition = this.UpperCompPosition,
                LowerPosition = this.LowerCompPosition
            };
        }

        public List<String> GetKeyInfos()
        {
            throw new NotImplementedException();
        }
    }

    internal class SearchRuntModel :TriggerRuntModel, ISearchTypeModel
    {
        public new ChannelId Source { get => base.Source; set => base.Source = value; }
        public Int32 ResultCount { get; set; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }


        public ISearchTypeOptions GetOption()
        {
            return new TrigRuntOptions(this.Source, this.Polarity, this.Condition, this.WidthByps, this.UpperWidthByps, this.PosUpperIndex, this.PosLowerIndex)
            {
                UpperPosition = this.UpperCompPosition,
                LowerPosition = this.LowerCompPosition
            };
        }

        public List<String> GetKeyInfos()
        {
            throw new NotImplementedException();
        }
    }
    internal class SearchTimeoutModel :TriggerTimeOutModel, ISearchTypeModel
    {
        public new ChannelId Source { get => base.Source; set => base.Source = value; }
        public Int32 ResultCount { get; set; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        public ISearchTypeOptions GetOption()
        {
            return new TrigTimeOutOptions(this.Source, this.Polarity, this.DurationByps, this.CompPosIndex)
            {
                Position = this.CompPosition
            };
        }

        public List<String> GetKeyInfos()
        {
            throw new NotImplementedException();
        }
    }
    internal class SearchWindowModel :TriggerWindowModel, ISearchTypeModel
    {
        public new ChannelId Source { get => base.Source; set => base.Source = value; }
        public Int32 ResultCount { get; set; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        public ISearchTypeOptions GetOption()
        {
            return new TrigWindowOptions(this.Source, this.PosCondition, this.TimeCondition, this.WidthByps, this.PosUpperIndex, this.PosLowerIndex)
            {
                UpperPosition = this.UpperCompPosition,
                LowerPosition = this.LowerCompPosition,
            };
        }

        public List<String> GetKeyInfos()
        {
            throw new NotImplementedException();
        }
    }
    internal class SearchSetupHoldModel :TriggerSetupHoldModel, ISearchTypeModel
    {
        public ChannelId Source { get => base.DataSource; set => base.DataSource = value; }
        public new ChannelId ClkSource { get => base.ClkSource; set => base.ClkSource = value; }

        public Int32 ResultCount { get; set; }

        public (Double[,] Result, Int32 ResultCount)? ResultPack { get; set; }

        public ISearchTypeOptions GetOption()
        {
            return new TrigSetupHoldOptions(this.ClkSource, this.ClkPolarity, this.Source, this.DataPosPolarity, this.Violation, this.TsuByps, this.ThdByps)
            {
                ClkPosition = (this.ClkCompPosIndex, this.ClkCompPosition),
                DataUpperPosition = (this.UpperDataPosIndex, this.UpperDataPosition),
                DataLowerPosition = (this.LowerDataPosIndex, this.LowerDataPosition),
            };
        }

        public List<String> GetKeyInfos()
        {
            throw new NotImplementedException();
        }
    }
}
