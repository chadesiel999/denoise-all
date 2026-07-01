using System;
using System.Threading;
using ScopeX.Core;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class AutosetSettingForm : FloatForm
    {
        private static AutosetSettingForm _Instance;
        public static AutosetSettingForm Default
        {
            get
            {
                if (null == _Instance || _Instance.IsDisposed)
                {
                    _Instance = new AutosetSettingForm(Program.Oscilloscope);
                }
                //获取焦点
                _Instance.Activate();
                return _Instance;
            }
        }

        public Boolean IsShow { get; private set; } = false;
        private const Int32 ShowTimeByS = 3;
        private Thread _Thread = null;
        private readonly DsoPrsnt _Oscilloscope;
        private AutosetSettingForm(DsoPrsnt dso)
        {
            InitializeComponent();
            _Oscilloscope = dso;
        }

        public void ResetShowTime() => _ShownTime = 0;

        private void BtnOnePeriod_Click(object sender, System.EventArgs e)
        {
            _Oscilloscope.AutoSet.SetTimebase();
            Close();
        }

        private void BtnNPeriod_Click(object sender, System.EventArgs e)
        {
            _Oscilloscope.AutoSet.SetTimebase(4);
            Close();
        }

        private void BtnRise_Click(object sender, System.EventArgs e)
        {
            _Oscilloscope.AutoSet.SetTriggerEdge(ComModel.EdgeSlope.Rise);
            Close();
        }

        private void BtnFall_Click(object sender, System.EventArgs e)
        {
            _Oscilloscope.AutoSet.SetTriggerEdge(ComModel.EdgeSlope.Fall);
            Close();
        }

        private void BtnSetting_Click(object sender, System.EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, -KeyCode.SETTING);
            Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            IsShow = true;
            Timer.Enabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsShow = false;
            if (Timer != null)
            {
                Timer.Elapsed -= Timer_Tick;
                Timer.Enabled = false;
                Timer.Dispose();
                Timer = null;
            }
        }

        private Int32 _ShownTime = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }
            void TickEvent()
            {
                if (_ShownTime > ShowTimeByS)
                {
                    Timer.Enabled = false;
                    CanClose = true;
                    Close();
                }
                _ShownTime++;
            }
        }
    }
}
