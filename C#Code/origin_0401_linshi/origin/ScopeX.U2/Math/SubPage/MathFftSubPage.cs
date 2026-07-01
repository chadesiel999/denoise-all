using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    [Description(nameof(MathType.FFT))]
    public partial class MathFftSubPage : MathSubPageBase
    {
        MathFftArg Arg;

        public MathFftSubPage()
        {
            InitializeComponent();
            ViewInit();
        }
        //private FrequencyAdapter _Adapter;



        #region Base Method
        //初始换页面控件
        protected override void ViewInit()
        {
            ControlsHotKnob.Default.InitHotKnob(NebFrequencyValueCenter);
            NebFrequencyValueCenter.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebFrequencyValueCenter, nameof(Presenter.FrequencyAdapter.ValueCenter));
            };
            //NebFrequencyValueStart
            NebFrequencyValueCenter.StringFormatFunc = (value) => CenterValueToString();
            NebFrequencyValueCenter.AddClicked += (_, _) =>
            {
                Presenter.FrequencyAdapter.ValueCenter += Presenter.Sampling.MinScale * 100;
                NebFrequencyValueCenter.UpdateValueString();
                //Presenter.FrequencyAdapter.FreshVerticalValue();
            };
            NebFrequencyValueCenter.SubClicked += (_, _) =>
            {
                Presenter.FrequencyAdapter.ValueCenter -= Presenter.Sampling.MinScale * 100;
                NebFrequencyValueCenter.UpdateValueString();
                //Presenter.FrequencyAdapter.FreshVerticalValue();
            };
            NebFrequencyValueCenter.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFrequencyValueCenter);
                nkf.NumberKeyboard.MaxValue = Presenter.FrequencyAdapter.ValueCenterMax(Presenter.Sampling.Prefix);
                nkf.NumberKeyboard.MinValue = Presenter.FrequencyAdapter.ValueCenterMin(Presenter.Sampling.Prefix);
                nkf.NumberKeyboard.AbsoluteMinValue = Quantity.ConvertByPrefix(1, Prefix.Nano, Prefix.Empty);
                nkf.NumberKeyboard.Unit = Presenter.Sampling.Unit;
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.FrequencyAdapter.ValueCenter, Presenter.Sampling.Prefix);
                nkf.NumberKeyboard.UseSI = true;
                nkf.Title = LblCenter.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.FrequencyAdapter.ValueCenter = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Sampling.Prefix);
                    nkf.Close();
                    NebFrequencyValueCenter.UpdateValueString();
                };

                nkf.ShowDialogByPosition();
            };

            //NebFrequencyValueEnd
            ControlsHotKnob.Default.InitHotKnob(NebFrequencyValueSpan);
            NebFrequencyValueSpan.EditValueOnceClicked += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebFrequencyValueSpan, nameof(Presenter.FrequencyAdapter.ValueSpan));
            };
            NebFrequencyValueSpan.StringFormatFunc = (value) => SpanValueToString();
            NebFrequencyValueSpan.AddClicked += (_, e) =>
            {
                Presenter.FrequencyAdapter.ScaleIndexSpan += 1;
                NebFrequencyValueSpan.UpdateValueString();
            };
            NebFrequencyValueSpan.SubClicked += (_, e) =>
            {
                Presenter.FrequencyAdapter.ScaleIndexSpan -= 1;
                NebFrequencyValueSpan.UpdateValueString();
            };

            NebFrequencyValueSpan.EditValueChicked = (_, _) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebFrequencyValueSpan);
                nkf.NumberKeyboard.MaxValue = Presenter.FrequencyAdapter.ValueSpanMax(Presenter.Sampling.Prefix);
                nkf.NumberKeyboard.MinValue = Presenter.FrequencyAdapter.ValueSpanMin(Presenter.Sampling.Prefix);
                nkf.NumberKeyboard.Unit = Presenter.Sampling.Unit;
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.FrequencyAdapter.ValueSpan, Presenter.Sampling.Prefix);
                nkf.Title = LblSpan.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Presenter.FrequencyAdapter.ValueSpan = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Sampling.Prefix);
                    nkf.Close();
                    NebFrequencyValueSpan.UpdateValueString();
                    NebFrequencyValueCenter.UpdateValueString();
                };

                nkf.ShowDialogByPosition();
            };

        }

        protected override void SubPageUpdateView(object prsnt, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                //case nameof(Arg.Source):
                //    //CbxSource.SelectedIndex = (Int32)Arg.Source;
                //    CbxSource.SetItemText(Arg.Source);
                //    break;
                case nameof(Arg.Number):
                    CbxNumber.SelectValue = Arg.Number;
                    break;
                case nameof(Arg.ResultType):
                    CbxResultType.SelectValue = (Int32)Arg.ResultType;
                    break;
                case nameof(Arg.Window):
                    CbxWindowType.SelectValue = (Int32)Arg.Window;
                    break;
                case nameof(Arg.ResultUnit):
                    CbxUnit.SelectValue = (Int32)Arg.ResultUnit;
                    break;
                case "ConditioningScale":

                    break;
                case "SamplingScale":
                    NebFrequencyValueSpan.UpdateValueString();
                    NebFrequencyValueCenter.UpdateValueString();
                    break;
                case nameof(Presenter.FrequencyAdapter.ValueCenter):
                    NebFrequencyValueSpan.UpdateValueString();
                    NebFrequencyValueCenter.UpdateValueString();
                    break;
                case nameof(Presenter.FrequencyAdapter.ValueSpan):
                    NebFrequencyValueSpan.UpdateValueString();
                    NebFrequencyValueCenter.UpdateValueString();
                    break;
                default:
                    CbxSource.SelectValue = Arg.Source;
                    CbxWindowType.SelectValue = (Int32)Arg.Window;
                    CbxUnit.SelectValue = (Int32)Arg.ResultUnit;
                    CbxResultType.SelectValue = (Int32)Arg.ResultType;
                    CbxNumber.SelectValue = Arg.Number;
                    break;
            }
            NebFrequencyValueCenter.UpdateValueString();
            NebFrequencyValueSpan.UpdateValueString();
            if (Arg is null)
            {
                return;
            }
            //输出类型不同，控件显示调整
            if (Arg.ResultType == FFTResultOpt.Ampltd
               // ||Arg.ResultType == FFTResultOpt.Real ||
               //Arg.ResultType == FFTResultOpt.Imaginary
               )
            {
                CbxUnit.Visible = true;
                LblUnit.Visible = true;
            }
            else
            {
                CbxUnit.Visible = false;
                LblUnit.Visible = false;
            }
            CbxUnit.SelectValue = (Int32)Arg.ResultUnit;
            CbxUnit.Enabled = Arg.IsAutoUnit;

            //NebFrequencyValueCenter.UpdateValueString();
            //NebFrequencyValueSpan.UpdateValueString();
            _ArgToCtrl = false;
        }

        protected override void LoadMethod()
        {
            base.LoadMethod();

            Arg = (MathFftArg)Presenter.GetOrMakeArg(MathType.FFT);
            //Presenter.FrequencyAdapter = FrequencyAdapterCollection.Instance.CreateAdapter(Presenter);

            _ArgToCtrl = true;

            //var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog());
            //var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsReference()));
            //FFT源处可以选择降噪数据
            var srcs = Program.Oscilloscope.FindIdentities(c => c.Id.IsAnalog() || (c.Active && c.Id.IsMath() && c.Id != Presenter.Id));


            if (Presenter is null)
            {
                if (Presenter.PreMathChannels.Count > 0)
                {
                    srcs.Add(Presenter.PreMathChannels[0]);
                }
            }

            LoadSourceList(srcs);

            CbxUnit.Enabled = Arg.IsAutoUnit;
            _ArgToCtrl = false;

            InitCbxNumber();

            UpdateView();
        }

        #endregion

        public void FreshFrequency()
        {
            NebFrequencyValueCenter.UpdateValueString();
            NebFrequencyValueSpan.UpdateValueString();
            Presenter.FrequencyAdapter.FreshPrsnt();
            Presenter.FrequencyAdapter.FreshVerticalValue();
        }

        Boolean isautomove = false;

        private void InitCbxNumber()
        {
            if (CbxNumber.DataSource != null)
                return;
            var fftnumbers = PlatformUIManager.Default.Platform.GetFFTNumbers();
            var datasource = fftnumbers.Select(o => new ComboBoxItem(EnumEx.GetDescription(o), o, null)).ToList();
            CbxNumber.DataSource = datasource;
            CbxNumber.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Arg.Number = (FFTNumber)CbxNumber.SelectValue;
                }
            };
        }

        //public void RemoveMouseMoveHandler(Type type, Control con, string eventName)
        //{
        //    PropertyInfo pi = type.GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);   //获取type类定义的所有事件的信息
        //    EventHandlerList ehl = (EventHandlerList)pi.GetValue(con, null);    //获取con对象的事件处理程序列表
        //    FieldInfo fieldInfo = (typeof(Control)).GetField("EventText", BindingFlags.Static | BindingFlags.NonPublic); //获取Control类Click事件的字段信息
        //    Delegate d = ehl[fieldInfo.GetValue(null)];
        //    foreach (Delegate del in d.GetInvocationList())
        //    {
        //        if (del.Method.Name == eventName)
        //        {
        //            break;
        //        }
        //    }
        //}

        //public void GetCustomEventList(object obj)
        //{
        //    BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
        //    EventInfo ei = obj.GetType().GetEvent("MouseMove",bindingFlags);
        //    var fis = ei.DeclaringType.GetFields(bindingFlags).Where(x=>x.Name.Contains("mouseMove"));
        //}

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                if (Arg is null)
                    return;

                _ArgToCtrl = true;
                //CbxSource.SelectedIndex = (Int32)_FftArg.Source;
                //CbxSource.SetItemText(_FftArg.Source);
                CbxSource.SelectValue = Arg.Source;
                //CbxNumber.SelectedIndex = (Int32)_FftArg.Number;
                //CbxNumber.SelectedIndex = CbxNumber.FindString((_FftArg.Number).ToString());
                //CbxNumber.SelectedValue = _FftArg.Number;
                CbxNumber.SelectValue = Arg.Number;
                //CbxResultType.SelectedIndex = (Int32)_FftArg.ResultType;
                CbxResultType.SelectValue = (Int32)Arg.ResultType;
                //CbxWindowType.SelectedIndex = (Int32)_FftArg.Window;
                CbxWindowType.SelectValue = (Int32)Arg.Window;

                if (ParentForm != null)
                {
                    NebFrequencyValueCenter.UpdateValueString();
                    NebFrequencyValueSpan.UpdateValueString();
                }

                //输出类型不同，控件显示调整
                if (Arg.ResultType == FFTResultOpt.Ampltd
                   // ||Arg.ResultType == FFTResultOpt.Real ||
                   //Arg.ResultType == FFTResultOpt.Imaginary
                   )
                {
                    CbxUnit.Visible = true;
                    LblUnit.Visible = true;
                }
                else
                {
                    CbxUnit.Visible = false;
                    LblUnit.Visible = false;
                }
                //CbxUnit.SelectedIndex = (Int32)_FftArg.ResultUnit;
                //CbxUnit.SelectedIndex = CbxNumber.FindString((_FftArg.ResultUnit).ToString());
                //CbxUnit.Enabled = _FftArg.IsAutoUnit;
                CbxUnit.SelectValue = (Int32)Arg.ResultUnit;
                CbxUnit.Enabled = Arg.IsAutoUnit;
                NebFrequencyValueCenter.UpdateValueString();
                NebFrequencyValueSpan.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        /// <summary>
        /// The HScaleToString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        private String CenterValueToString()
        {
            return new Quantity(Presenter.FrequencyAdapter.ValueCenter, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString("#0.000", true);
        }

        /// <summary>
        /// The HPosToString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        private String SpanValueToString()
        {
            return new Quantity(Presenter.FrequencyAdapter.ValueSpan, Presenter.Sampling.Prefix, Presenter.Sampling.Unit).ToString("#0.###", true);
        }

        private void LoadSourceList(IEnumerable<ChannelId> sources)
        {
            if (CbxSource.DataSource != null)
            {
                if (((CbxSource.DataSource is IEnumerable<KeyValuePair<String, ChannelId>> channels) && channels.Select(x => x.Value.ToString()).ToArray().SequenceEqual(sources.Select(x => x.ToString()))))
                {
                    return;
                }
            }
            CbxSource.DataSource = sources.OrderBy(x => x).Select(x => new ComboBoxItem(x.ToString(), x, null)).ToList();
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if (Program.Oscilloscope.TryGetChannel((ChannelId)CbxSource.SelectValue, out var chnlPrsnt))
                    {
                        //if (!chnlPrsnt.Active)
                        //{
                        //    chnlPrsnt.Active = true;
                        //}
                    }
                    //_FftArg.Source = Enum.Parse<ChannelId>((String)CbxSource.SelectedValue);
                    Arg.Source = (ChannelId)CbxSource.SelectValue;
                }
            };

        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
            }
        }

        //private void CbxResultType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        _FftArg.ResultType = (FFTResultOpt)CbxResultType.SelectedIndex;
        //    }
        //}

        //private void CbxWindowType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        _FftArg.Window = (WindowType)CbxWindowType.SelectedIndex;
        //    }
        //}



        private void RdoFrequencyTypeStart_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                NebFrequencyValueCenter.UpdateValueString();
                NebFrequencyValueSpan.UpdateValueString();
            }
        }

        private void RdoFrequencyTypeEnd_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //RdoFrequencyTypeStart.ChoosedButtonIndex = RdoFrequencyTypeEnd.ChoosedButtonIndex;
            }
        }

        //private void CbxUnit_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!_ArgToCtrl)
        //    {
        //        _FftArg.ResultUnit = (FFTCoordUnit)CbxUnit.SelectedIndex;
        //    }
        //}
        private void CbxSource_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                //var srcs = Program.Oscilloscope.FindIdentities(c => (c.Active && (c.Id.IsAnalog() || c.Id.IsReference())));

                //var _availableList = new List<String>();
                //if (Presenter.PreMathChannels.Count > 0)
                //{
                //    srcs.Add(Presenter.PreMathChannels[0]);
                //}
                //foreach (var item in srcs)
                //{
                //    _availableList.Add(item.ToString());
                //}
                //CbxSource.DataSource = _availableList;
            }
        }

        private void CbxResultType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Arg.ResultType = (FFTResultOpt)CbxResultType.SelectValue;
            }
        }

        private void CbxUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Arg.ResultUnit = (FFTCoordUnit)CbxUnit.SelectValue;
            }
        }

        private void CbxWindowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Arg.Window = (WindowType)CbxWindowType.SelectValue;
            }
        }
        private void BtnConfig_Click(object sender, EventArgs e)
        {
            Presenter.InitFlag();
            Presenter.ResetFFT();
        }
    }
}
