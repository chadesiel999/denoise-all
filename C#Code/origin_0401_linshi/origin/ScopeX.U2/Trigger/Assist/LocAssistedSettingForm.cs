
namespace ScopeX.U2
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.U2.LanguageSupoort;
    using ScopeX.UserControls;

    public partial class LocAssistedSettingForm : FloatForm, ILocationAssistedView
    {
        private Boolean _ArgToCtrl;

        public LocAssistedSettingForm()
        {
            InitializeComponent();
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(LocAssistedSettingForm)));
            };
            InitSourceList();
            InitHotKnob();
        }

        private void InitSourceList()
        {
            CbxSource.Items.Clear();
            CbxSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Source = (ChannelId)CbxSource.SelectedIndex;
                }
            };

            CbxCondition.Items.Clear();
            CbxCondition.Items.AddRange(Enum.GetValues<CompareCondition>().Select(o => o.GetAlias()).ToArray());
            CbxCondition.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    Presenter.Condition = (CompareCondition)CbxCondition.SelectedIndex;
                }
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

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

        public LocationAssistedPrsnt Presenter { get; set; }

        ILocationAssistedPrsnt IView<ILocationAssistedPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (LocationAssistedPrsnt)value;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        private void Stylize()
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        private void ChangeThroldState()
        {
            switch (Presenter.Condition)
            {
                case CompareCondition.GreaterThan or CompareCondition.LessThan:
                    LblThreshold.Visible = BtnLowerThreshold.Visible = true;
                    LblLowerThreshold.Visible = false;
                    LblUpperThreshold.Visible = BtnUpperThreshold.Visible = false;
                    break;
                case CompareCondition.InRange or CompareCondition.OutRange:
                    LblThreshold.Visible = false;
                    LblLowerThreshold.Visible = BtnLowerThreshold.Visible = true;
                    LblUpperThreshold.Visible = BtnUpperThreshold.Visible = true;
                    break;
                default:
                    LblThreshold.Visible = false;
                    LblLowerThreshold.Visible = BtnLowerThreshold.Visible = false;
                    LblUpperThreshold.Visible = BtnUpperThreshold.Visible = false;
                    break;
            }
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

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.Source):
                    CbxSource.SelectedIndex = (Int32)Presenter.Source;
                    BtnLowerThreshold.Text = $"{Presenter.LowerThreshold} {Presenter.MeasUnit}";
                    BtnUpperThreshold.Text = $"{Presenter.UpperThreshold} {Presenter.MeasUnit}";
                    break;
                case nameof(Presenter.LowerThreshold):
                    BtnLowerThreshold.Text = $"{Presenter.LowerThreshold} {Presenter.MeasUnit}";
                    break;
                case nameof(Presenter.UpperThreshold):
                    BtnUpperThreshold.Text = $"{Presenter.UpperThreshold} {Presenter.MeasUnit}";
                    break;
                case nameof(Presenter.MeasName):
                    BtnMeasName.Text = Presenter.MeasName;
                    BtnLowerThreshold.Text = $"{Presenter.LowerThreshold} {Presenter.MeasUnit}";
                    BtnUpperThreshold.Text = $"{Presenter.UpperThreshold} {Presenter.MeasUnit}";
                    break;
                case nameof(Presenter.Condition):
                    CbxCondition.SelectedIndex = (Int32)Presenter.Condition;
                    ChangeThroldState();
                    break;

            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxSource.SelectedIndex = (Int32)Presenter.Source;
                BtnMeasName.Text = Presenter.MeasName;
                CbxCondition.SelectedIndex = (Int32)Presenter.Condition;
                BtnLowerThreshold.Text = $"{Presenter.LowerThreshold} {Presenter.MeasUnit}";
                BtnUpperThreshold.Text = $"{Presenter.UpperThreshold} {Presenter.MeasUnit}";
                ChangeThroldState();
                _ArgToCtrl = false;
            }
        }
        private void InitHotKnob()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnLowerThreshold);
            BtnLowerThreshold.Click += (_, _) => 
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnLowerThreshold, nameof(Presenter.LowerThreshold),20);
            };

            ControlsHotKnob.Default.InitHotKnob(BtnUpperThreshold);
            BtnUpperThreshold.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnUpperThreshold, nameof(Presenter.UpperThreshold),20);
            };
        }

        private void BtnMeasName_Click(object sender, EventArgs e)
        {
            MeasSelectionForm sf = new("Pk2Pk", DsoPrsnt.FocusId, (n, s) =>
            {
                Presenter.MeasName = n;
                return true;
            })
            {
                StartPosition = FormStartPosition.CenterScreen,
            };

            sf.ShowDialogByPosition();
            UpdateView();
        }

        private void BtnLowerThreshold_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnLowerThreshold);
            var onokclickeventaction = new Action<Double>((data) => Presenter.LowerThreshold = data);

            nkf.SetKeyBoardValue(LblThreshold.Text, Presenter.MeasUnit, 12, onokclickeventaction,
                Presenter.LowerThreshold,
                Presenter.MaxThreshold,
                Presenter.MinThreshold);

            nkf.ShowDialogByPosition();
        }

        private void BtnUpperThreshold_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnUpperThreshold);
            var onokclickeventaction = new Action<Double>((data) => Presenter.UpperThreshold = data);

            nkf.SetKeyBoardValue(LblThreshold.Text, Presenter.MeasUnit, 3, onokclickeventaction,
                Presenter.UpperThreshold,
                Presenter.MaxThreshold,
                Presenter.MinThreshold);

            nkf.ShowDialogByPosition();
        }
    }
}
