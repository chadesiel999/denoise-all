using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class AcquireForm : FloatForm, ITimebaseView
    {
        private readonly AcquirePage _AcquirePage;

        private readonly FastFramePage _FastFramePage;

        public Int32 GroupIndex { get; set; } = -1;
        private String _RecordKey = String.Empty;

        public AcquireForm()
        {
            InitializeComponent();

            _AcquirePage = new()
            {
                BackColor = Color.Transparent,
                //Dock = DockStyle.Fill,
            };
            //_FastFramePage = new()
            //{
            //    BackColor = Color.Transparent,
            //    //Dock = DockStyle.Fill,
            //};

            NbgAcquire.SetGroupContent(0, _AcquirePage);
            //NbgAcquire.SetGroupContent(1, _FastFramePage);
            Size = new(_AcquirePage.Size.Width, _AcquirePage.Size.Height + HeadHeight + NbgAcquire.CurrentGroupNum * NbgAcquire.NavBarHeight);
            _RecordKey = $"{this.Name}_{NbgAcquire.Name}";
            HelpClick += (_, _) =>
            {
                var res = Int32.TryParse(HelpLabel, out var index);
                if (!res)
                {
                    HelpProcessManager.SendCommand();
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                    return;
                }
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(AcquireForm)));
            };
            NbgAcquire.CurrentGroupIndexChanged += (obj, index) =>
            {
                switch (NbgAcquire.CurrentGroupIndex)
                {
                    case 0:
                        _AcquirePage?.Refresh();
                        break;
                    //case 1:
                    //    _FastFramePage?.Refresh();
                    //    break;
                    default:
                        break;
                }

            };
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DsoPrsnt.NavBarGroupRecords[_RecordKey] = NbgAcquire.CurrentGroupIndex;
            base.OnHandleDestroyed(e);
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public TimebasePrsnt Presenter
        {
            get;
            set;
        }

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TimebasePrsnt)value;
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
                return;
            }
            _AcquirePage.UpdateView(prsnt, propertyName);
            //_FastFramePage.UpdateView(prsnt, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            SetGroupIndex(GroupIndex);
            // LanguageFactory.CacheFormLanguageControls(this);
        }
 
        private void Stylize()
        {
            _AcquirePage.StylizeFlag = true;
            //_FastFramePage.StylizeFlag = true;
            IsShowHelp = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //HeadBackColor = Color.FromArgb(50, 55, 65);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }

        private void SetGroupIndex(Int32 index = 0)
        {
            if (index < 0)
            {
                if (!DsoPrsnt.NavBarGroupRecords.ContainsKey(_RecordKey))
                {
                    DsoPrsnt.NavBarGroupRecords.AddOrUpdate(_RecordKey, 0, (k, v) => 0);
                }
                index = DsoPrsnt.NavBarGroupRecords[_RecordKey];
            }
            if (index > NbgAcquire.CurrentGroupNum)
            {
                index = NbgAcquire.CurrentGroupNum - 1;
            }
            NbgAcquire.CurrentGroupIndex = index;
        }
    }
}
