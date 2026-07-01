using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    public partial class TimebasePage : UserControl, ITimebaseView, IStylize
    {
        public TimebasePage()
        {
            InitializeComponent();
            Init();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        private Boolean _NebDelayEditValueChicked = false;
        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        public TimebasePrsnt Presenter
        {
            get => (TimebasePrsnt)(ParentForm as ITimebaseView).Presenter;
            set => (ParentForm as ITimebaseView).Presenter = value;
        }

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TimebasePrsnt)value;
        }

        private void Init()
        {
            //NebScale
            ControlsHotKnob.Default.InitHotKnob(NebScale);
            NebScale.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebScale);
            };
            NebScale.AddClicked = (s, e) => Presenter.ScaleIndex++;
            NebScale.SubClicked = (s, e) => Presenter.ScaleIndex--;
            NebScale.StringFormatFunc = (d) => ScaleToString();
            NebScale.EditValueChicked += (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebScale);
                nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.MaxScale, Presenter.Prefix);
                nkf.NumberKeyboard.MinValue = Quantity.ConvertByPrefix(Presenter.MinScale, Presenter.Prefix);
                nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.ScaleByus, Presenter.Prefix);
                nkf.NumberKeyboard.DecimalNumber = 3;
                nkf.NumberKeyboard.Unit = Presenter.Unit;
                nkf.Title = LblTimebase.Text;

                nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                {
                    Double scalebyus = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Prefix);
                    Presenter.ScaleByus = Math.Round(scalebyus,9); //Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Prefix);
                    nkf.Close();
                };

                DialogResult dialogresult = nkf.ShowDialogByPosition();
            };

            //NebDelay
            ControlsHotKnob.Default.InitHotKnob(NebDelay);
            NebDelay.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebDelay);
            };
            NebDelay.AddClicked = (_, e) => Presenter.PosIndexBymDiv += e.Step;
            NebDelay.SubClicked = (_, e) => Presenter.PosIndexBymDiv += e.Step;
            NebDelay.StringFormatFunc = (d) => PosToString();
            NebDelay.EditValueChicked += (a, b) =>
            {
                if (!_NebDelayEditValueChicked)
                {
                    _NebDelayEditValueChicked = true;
                    var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebDelay);
                    //nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(Presenter.MaxPosition, Presenter.Prefix);
                    nkf.NumberKeyboard.MaxValue = Quantity.ConvertByPrefix(1_000_000, Presenter.Prefix);
                    nkf.NumberKeyboard.MinValue = (Double)(decimal)Quantity.ConvertByPrefix(Presenter.MinPosition, Presenter.Prefix);
                    nkf.NumberKeyboard.DefaultValue = Quantity.ConvertByPrefix(Presenter.PositionByus, Presenter.Prefix);
                    nkf.NumberKeyboard.DecimalNumber = 3;
                    nkf.NumberKeyboard.Unit = Presenter.Unit;
                    nkf.Title = LblDelay.Text;

                    nkf.NumberKeyboard.OkClickEvent += (sender, args) =>
                    {
                        Presenter.PositionByus = Quantity.ConvertByPrefix(args.Data, Prefix.Empty, Presenter.Prefix);
                        nkf.Close();
                    };
                    DialogResult dialogresult = nkf.ShowDialogByPosition();
                    _NebDelayEditValueChicked = false;
                }
            };
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            switch (propertyName)
            {
                case "SamplingScale":
                    NebScale.UpdateValueString();
                    //NebDelay.UpdateValueString();
                    break;
                case "SamplingPosition":
                    NebDelay.UpdateValueString();
                    break;
                case nameof(TimebasePrsnt.IsScan):

                    Boolean enabled = true;
                    if(Presenter.IsScan && TriggerPrsnt.State != SysState.Stop)
                    {
                        enabled = false;
                    }
                    NebDelay.Enabled = enabled;
                    LblDelay.Enabled = enabled;
                    BtnResetDelay.Enabled = enabled;
                    break;
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                Boolean enabled = true;
                if (Presenter.IsScan && TriggerPrsnt.State != SysState.Stop)
                {
                    enabled = false;
                }
                NebDelay.Enabled = enabled;
                LblDelay.Enabled = enabled;
                BtnResetDelay.Enabled = enabled;
                NebScale.UpdateValueString();
                NebDelay.UpdateValueString();
            }
        }

        private String ScaleToString()
        {
            return new Quantity(Presenter.ScaleByus, Presenter.Prefix, Presenter.Unit).ToString("##0.###", true);
        }

        private String PosToString()
        {
            return new Quantity(Presenter.PositionByus, Presenter.Prefix, Presenter.Unit).ToString("##0.###", true);
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void BtnResetDelay_Click(object sender, EventArgs e)
        {
            Presenter.ResetPosIndex();
        }
    }
}
