using EventBus;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Common.Structs;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace ScopeX.U2
{
    [Description(nameof(MathType.Histgram))]
    public partial class MathHistSubPage :MathSubPageBase
    {
        MathHistArg Arg;

        public MathHistSubPage()
        {
            InitializeComponent();
            ViewInit();
        }

        #region Base Method
        protected override void ViewInit()
        {
            //NebNBins
            NebNBins.AddClicked = (a, b) => Arg.NBins++;
            NebNBins.SubClicked = (a, b) => Arg.NBins--;
            NebNBins.StringFormatFunc = (value) => Arg.NBins.ToString() + "#";
            NebNBins.EditValueChicked = (_, _) =>
            {
                NumberKeybordForm nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebNBins);
                Action<Double> onokclickeventaction = (data) =>
                    Arg.NBins = (Int32)data;

                nkf.SetKeyBoardValue(LblNBins.Text, Presenter.Unit, 0, onokclickeventaction,
                    Arg.NBins, MathHistArg.MaxNBins, MathHistArg.MinNBins);
                nkf.ShowDialogByPosition();
            };
            //ChkAutoScale.CheckedChangedEvent -= ChkAutoScale_CheckedChangedEvent;
          
        }

        private void ChkAutoScale_CheckedChangedEvent(object sender, EventArgs e)
        {
            //if (!_ArgToCtrl)
            //{
            //    Presenter.AutoScale = ChkAutoScale.Checked;
            //    NebNBins.Enabled = !ChkAutoScale.Checked;
            //}
        }


        protected override void SubPageUpdateView(Object prsnt, String propertyName)
        {
            UpdateView();
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();
            //ChkAutoScale.Checked = Presenter.AutoScale;
            //ChkAutoScale.CheckedChangedEvent += ChkAutoScale_CheckedChangedEvent;
            Arg = (MathHistArg)Presenter.GetOrMakeArg(MathType.Histgram);
            if (Parent is MathPage page && page != null)
            {
                page.ToAutoScale(Presenter.AutoScale);

            }
            UpdateView();
            //UpdateHScale();
            //UpdateVScale();
        }
        #endregion

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (Arg is null)
                {
                    return;
                }
                
                _ArgToCtrl = true;
                CbxSource.SelectedIndex = CbxSource.FindStringExact(Arg.Source.ToString());
                //ChkAutoScale.Checked = Presenter.AutoScale;
                //NebNBins.Enabled = !ChkAutoScale.Checked;
                NebNBins.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Arg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedItem);
            }
        }

        private void BtnMeasure_Click(Object sender, EventArgs e)
        {
            var exist = Presenter.GetViewList().Exists(o => o is HistParametersForm);

            if (!exist)
            {
                var hpf = new HistParametersForm()
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new(100, 100),
                };

                hpf.Presenter = Presenter;
                hpf.Presenter.TryAddView(hpf);

                EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = hpf, Type = FormType.InfoForm });
            }
        }

        private void UpdateVScale()
        {
            var pm = Arg.CalcParameters();
            if (pm == null)
            {
                return;
            }

            var vmax = pm.MaxPop;
            //从小到大找到合适的刻度
            for (Int32 i = Presenter.ScaleMinIndex; i < Presenter.ScaleMaxIndex; i++)
            {
                var scale = Presenter.QueryScaleValue(i);
                var div = Quantity.ConvertByPrefix(scale, Presenter.Prefix);

                if ((vmax <= (div * 4) && vmax >= (div * 2)) || i == Presenter.ScaleMaxIndex)
                {
                    Presenter.ScaleIndex = i;
                    //设置到合适的中间位置
                    Presenter.PosIndexBymDiv = -1 * vmax / Presenter.Scale * Presenter.PosIdxPerDiv;
                    break;
                }
            }
        }

        private void UpdateHScale()
        {
            var pm = Arg.CalcParameters();
            if (pm == null)
            {
                return;
            }

            var hmax = pm.Max;
            var hmin = pm.Min;
            if (Program.Oscilloscope.TryGetChannel(Arg.Source, out var cp))
            {
                var (p, u) = cp.Pack.Properties.ChnlUnit;
                hmin = Quantity.ConvertByPrefix(hmin, p);
                hmax = Quantity.ConvertByPrefix(hmax, p);
            }
            Presenter.Sampling.ScaleIndex = (Int32)Presenter.Sampling.ScaleMinIndex;
            Presenter.Sampling.PosIndexBymDiv = Constants.VIS_XDIVS_NUM / 2 * Constants.IDX_PER_XDIV;
            var firstdiv = 0D;
            var lastdiv = 0D;

            //从小到大找到合适的刻度
            for (var i = Presenter.Sampling.ScaleMinIndex; i < Presenter.Sampling.ScaleMaxIndex; i++)
            {
                var scale = Presenter.Sampling.QueryScaleValue((int)i);
                var div = Quantity.ConvertByPrefix(scale, Presenter.Sampling.Prefix);

                firstdiv = (Constants.MAX_XPOS_IDX / 10 - Presenter.Sampling.PosIndexBymDiv) * div / Presenter.Sampling.PosIdxPerDiv;
                lastdiv = (Constants.MAX_XPOS_IDX * 9 / 10 - Presenter.Sampling.PosIndexBymDiv) * div / Presenter.Sampling.PosIdxPerDiv;
                if (firstdiv <= hmin && lastdiv >= hmax)
                {
                    Presenter.Sampling.ScaleIndex = (Int32)i;
                    return;
                }
            }
        }

        /// <summary>
        /// 清空直方图数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetHist_Click(object sender, EventArgs e)
        {
            if(Arg.Source.IsMeasure())
            {
                var src = Arg.Source - ChannelId.P1;
                DsoPrsnt.DefaultDsoPrsnt.Measure.ResetStat(src);
            }
            else
            {
                switch(Arg.Source)
                {
                    case ChannelId.DVM:
                        DsoPrsnt.DefaultDsoPrsnt.Voltmeter.ResetStatistics();
                        break;
                    case ChannelId.CYM:
                        DsoPrsnt.DefaultDsoPrsnt.Cymometer.ResetStatistics();
                        break;
                }
            }
        }
    }
}
