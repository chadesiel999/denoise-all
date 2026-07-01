using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.MathExt;
using static ScopeX.Core.MathModel;

namespace ScopeX.Core
{
    /// <summary>
    /// 提供标准接口，用于强制占用数学通道
    /// </summary>
    internal class AdvancedMathModel
    {
        public AdvancedMathModel( String formula)
        {
            DrawMethod = DrawMethod.Plot;
            _Enabled = false;
            _Formula = formula;

            Conditioning = new ConditioningModel(/*this*/);
            Sampling = new SamplingModelEx(MathType.Custom);
        }
        public AdvancedMathModel(String formula, DrawMethod drawMethod)
        {
            DrawMethod = drawMethod;
            _Enabled = false;
            _Formula = formula;

            Conditioning = new ConditioningModel(/*this*/);
            Sampling = new SamplingModelEx(MathType.Custom);
        }

        public virtual void InitChannelProperties(String formula)
        {
            
        }

        public virtual WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            return new WfmProperties("null");
        }

        public ConditioningModel Conditioning { get; }
        public SamplingModelEx Sampling { get; }

        //public ChannelId Source { get; set; }

        public DrawMethod DrawMethod { get; }
        private String _Formula;

        public String Formula 
        {
            get { return _Formula; }
        }

        private Boolean _Enabled;
        internal Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value)
                    return;
                _Enabled = value;

                var channel = DsoModel.Default.GetChannel(MathChannelId);
                if (channel == null || channel is not MathModel)
                {
                    return;
                }
                var mch = (MathModel)channel;
                if (mch.Args == null)
                {
                    return;
                }

                if (value)
                {
                    if (mch.Args.Occupier == null)
                    {
                        mch.Args.Occupier = this;
                        _Enabled = true;
                        mch.Formula = $"{MathType.Custom}:Execute.{_Formula}";
                        mch.Active = _Enabled;
                        mch.Label = _Formula;
                        mch.Conditioning.Prefix = Tools.Prefix.Milli;
                    }
                }
                else
                {
                    if (mch.Args.Occupier == this)
                    {
                        mch.Args.Occupier = null;
                        _Enabled = false;
                        mch.Active = _Enabled;
                        mch.Label = "";
                    }
                }
            }
        }

        public Boolean IsExternMethod = true;



        public virtual Vector? Take() 
        {
            return null;
        }

        /// <summary>
        /// 用来显示波形的数学通道
        /// </summary>
        internal ChannelId MathChannelId { get; set; } = ChannelId.M1;

        public override String ToString()
        {
            return _Formula;
        }
    }
}
