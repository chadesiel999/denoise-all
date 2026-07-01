namespace ScopeX.U2.Search
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;
    using ScopeX.UserControls.Style;

    public partial class SearchEdgeSubPage : UserControl, ITriggerView, IStylize
    {
        private Boolean _ArgToCtrl;

        public SearchEdgeSubPage(SearchItemPrsnt item)
        {
            InitializeComponent();
            Item = item;
            Presenter = item.SearchTypePrsnt as SearchEdgePrsnt;
            Init();
        }

        private SearchItemPrsnt Item
        {
            get; set;
        }

        public SearchEdgePrsnt Presenter
        {
            get;
            set;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

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

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter { get => Presenter; set => Presenter = (SearchEdgePrsnt)value; }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        public void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            string[] property = propertyName.Split(":");
            if (property.Length == 2)
            {
                if (property[0] == Presenter.Name)
                {
                    propertyName = property[1];
                }
                else
                {
                    UpdateView();
                    return;
                }
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Source):
                    CbxSource.SelectIndex = CbxSource.Items.Cast<String>().FirstIndex(x => x == Presenter.Source.ToString())!.Value;
                    NebPosition.UpdateValueString();
                    SearchSourceChanged();
                    break;
                case nameof(Presenter.Coupling):
                    CbxCoupling.SelectIndex = (Int32)Presenter.Coupling;
                    break;
                case nameof(Presenter.Impedance):
                    RdoImpedance.ChoosedButtonIndex = (Int32)Presenter.Impedance;
                    ChangeCouplingOptState();
                    break;
                case "CompPosIndex":
                    NebPosition.UpdateValueString();
                    break;
                case nameof(Presenter.Slope):
                    RdoSlope.ChoosedButtonIndex = (Int32)Presenter.Slope;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxSource.SelectIndex = CbxSource.Items.Cast<String>().FirstIndex(x => x == Presenter.Source.ToString())!.Value;
                RdoImpedance.ChoosedButtonIndex = (Int32)Presenter.Impedance;
                CbxCoupling.SelectIndex = (Int32)Presenter.Coupling;
                ChangeCouplingOptState();
                NebPosition.UpdateValueString();
                RdoSlope.ChoosedButtonIndex = (Int32)Presenter.Slope;
                SearchSourceChanged();
                _ArgToCtrl = false;
            }
        }

        private void BtnResetPosition_Click(object sender, EventArgs e)
        {
            Presenter.ResetPosIndex();
            Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
        }

        private void CbxCoupling_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Coupling = (TriggerCoupling)CbxCoupling.SelectIndex;
                Item.SetModelValue(nameof(Presenter.Coupling), Presenter.Coupling);
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = Enum.Parse<ChannelId>((String)CbxSource.Items[(int)CbxSource.SelectValue]);
                Item.Source = Presenter.Source;
            }
        }

        private void SearchSourceChanged()
        {
            if (Presenter.Source == ChannelId.AC || Presenter.Source.IsDigital())
            {
                LblSlope.Visible = true;
                RdoSlope.Visible = true;
                LblPosition.Visible = false;
                NebPosition.Visible = false;
                BtnResetPosition.Visible = false;
            }
            else if (Presenter.Source == ChannelId.AuxIn)
            {
                LblSlope.Visible = false;
                RdoSlope.Visible = false;
                LblPosition.Visible = false;
                NebPosition.Visible = false;
                BtnResetPosition.Visible = false;
            }
            else
            {
                LblSlope.Visible = true;
                RdoSlope.Visible = true;
                LblPosition.Visible = true;
                NebPosition.Visible = true;
                BtnResetPosition.Visible = true;
            }
        }

        private void ChangeCouplingOptState()
        {
            if (Presenter.Impedance == TriggerImpedance.Low50)
            {
                LblCoupling.Enabled = false;
                CbxCoupling.Enabled = false;
                CbxCoupling.SelectIndex = 0;
            }
            else
            {
                LblCoupling.Enabled = true;
                CbxCoupling.Enabled = true;
            }
        }

        private String CompPosToString()
        {
            return new Quantity(Presenter.CompPosition, Presenter.PosPrefix, Presenter.PosUnit).ToString(5, true);
        }

        private void Init()
        {
            //NebPosition
            ControlsHotKnob.Default.InitHotKnob(NebPosition);
            NebPosition.EditValueOnceClicked += (a, b) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, NebPosition);
            };
            NebPosition.StringFormatFunc = (_) => CompPosToString();
            NebPosition.AddClicked = (_, e) =>
            {
                Presenter.PosIndex += e.Step;
                Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
            };
            NebPosition.SubClicked = (_, e) =>
            {
                Presenter.PosIndex += e.Step;
                Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
            };
            NebPosition.EditValueChicked = (a, b) =>
            {
                var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebPosition);

                var onokclickeventaction = new Action<Double>((data) =>
                {
                    Presenter.CompPosition = Quantity.ConvertByPrefix(data, Prefix.Empty, Presenter.PosPrefix);
                    Item.SetModelValue(nameof(Presenter.CompPosition), Presenter.CompPosition);
                });

                nkf.SetKeyBoardValue(LblPosition.Text, Presenter.PosUnit, 3, onokclickeventaction,
                    Quantity.ConvertByPrefix(Presenter.CompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MaxCompPosition, Presenter.PosPrefix),
                    Quantity.ConvertByPrefix(Presenter.MinCompPosition, Presenter.PosPrefix));

                nkf.ShowDialogByPosition();
            };

            InitSourceList();
        }

        private void InitSourceList()
        {
            CbxSource.Items = PlatformUIManager.Default.Platform.GetTriggerSource().Select(o => o.ToString()).ToArray();
        }

        private void RdoImpedance_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Impedance = (TriggerImpedance)RdoImpedance.ChoosedButtonIndex;
                Item.SetModelValue(nameof(Presenter.Impedance), Presenter.Impedance);
            }
        }

        private void RdoSlope_ButtonSelect(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Slope = (EdgeSlope)RdoSlope.ChoosedButtonIndex;
                Item.SetModelValue(nameof(Presenter.Slope), Presenter.Slope);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (Presenter != null)
            {
                Presenter.TryRemoveView(this);
            }
        }
    }
}
