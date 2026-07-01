using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class ArticialIntelligenceInfoStrip : UserControl, IPanel, IArtificialIntelligenceView
    {
        public ArticialIntelligenceInfoStrip()
        {
            base.Visible = false;
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        #region 属性成员
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

        public ArtificialIntelligencePrsnt Presenter
        {
            get;
            set;
        }

        IArtificialIntelligencePrsnt IView<IArtificialIntelligencePrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (ArtificialIntelligencePrsnt)value;
        }

        #endregion 属性成员

        #region 接口成员
        public Color HeaderBackColor { get => LblTitle.BackColor; set => LblTitle.BackColor = value; }
        public Color HeaderForeColor { get => LblTitle.ForeColor; set => LblTitle.ForeColor = value; }
        public Color ContentBackColor { get => LveMain.BackColor; set => LveMain.BackColor = value; }
        public Color ContentForeColor { get => LveMain.ForeColor; set => LveMain.ForeColor = value; }
        public Color BorderColor { get => TlpMain.ForeColor; set => TlpMain.ForeColor = value; }

        private Int32 _BorderThickness = 2;
        public int BorderThickness
        {
            get => _BorderThickness;
            set
            {
                _BorderThickness = value;
                this.Padding = new Padding(_BorderThickness, _BorderThickness, _BorderThickness, _BorderThickness);
            }
        }

        public void UpdateView(object presenter, string propertyName)
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
        #endregion

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            if (!DesignMode)
            {
                switch (propertyName)
                {
                    case nameof(Presenter.MainEnable):
                    case nameof(Presenter.CurAiSetEnable):
                        UpdateVisibleState();
                        break;
                }
            }
        }

        public override void Refresh()
        {
            UpdateView();
            base.Refresh();
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
                UpdateVisibleState();

                LveMain.BeginUpdate();
                String[] tipinfo = Presenter.AiTipInfo;
                for (Int32 row = 0; row < tipinfo.Length; row++)
                {
                    if (row == LveMain.Items.Count)
                    {
                        LveMain.Items.Add(new ListViewItem(new String[LveMain.Columns.Count]));
                    }

                    LveMain.Items[row].SubItems[0].Text = tipinfo[row];
                }
                for (Int32 row = tipinfo.Length; row < LveMain.Items.Count; row++)
                {
                    LveMain.Items[row].SubItems[0].Text = "";
                }
                LveMain.EndUpdate();
            }
        }

        private void UpdateVisibleState()
        {
            Boolean mainEnable = Presenter.MainEnable;
            Visible = mainEnable;
            if (mainEnable)
            {
                TimerMain.Enabled = true;
                TimerMain.Start();
            }
            else
            {
                TimerMain.Enabled = false;
                TimerMain.Stop();
            }
        }
        private volatile Boolean _IsUpdating = false;
        private Int64 _LastAiTipVersion = -1;
        private void TimerMain_Tick(object sender, EventArgs e)
        {
            if (_IsUpdating)
                return;

            if (Presenter == null)
                return;

            Int64 version = Presenter.AiTipVersion;
            if (_LastAiTipVersion == version)
                return;

            _IsUpdating = true;
            try
            {
                UpdateView();
                _LastAiTipVersion = version;
            }
            finally
            {
                _IsUpdating = false;
            }
        }

        public new Boolean Visible
        {
            get => base.Visible;
            set
            {
                _DesiredVisible = value;
                if (IsDisposed || Disposing)
                    return;

                DsoForm dsoForm = Program.Oscilloscope?.View as DsoForm;
                Control uiInvoker = dsoForm as Control ?? this;
                if (uiInvoker.IsDisposed)
                    return;

                if (uiInvoker.InvokeRequired)
                {
                    try
                    {
                        uiInvoker.BeginInvoke(new Action(() => ApplyVisibleState(_DesiredVisible)));
                    }
                    catch
                    { }
                    return;
                }

                ApplyVisibleState(_DesiredVisible);
            }
        }

        private volatile Boolean _DesiredVisible = false;
        private void ApplyVisibleState(Boolean value)
        {
            if (IsDisposed || Disposing)
                return;

            if (base.Visible != value)
            {
                base.Visible = value;
            }

            try
            {
                DsoForm dsoForm = Program.Oscilloscope?.View as DsoForm;
                if (dsoForm?.PanelManager == null)
                    return;

                Boolean inPanelManager = dsoForm.PanelManager.Contains(this);
                if (value && !inPanelManager)
                {
                    dsoForm.PanelManager.Add(this);
                }
                else if (!value && inPanelManager)
                {
                    dsoForm.PanelManager.Remove(this);
                }
            }
            catch
            { }
        }
    }
}
