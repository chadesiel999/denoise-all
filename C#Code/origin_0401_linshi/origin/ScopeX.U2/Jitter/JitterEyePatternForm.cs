using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class JitterEyePatternForm : FloatForm
    {
        public static EyePatternParameterForm eyePatternParameterForm = null;
        public JitterEyePatternForm()
        {
            InitializeComponent();
            //PlotInit();
            FixedToolIconInfos[2].Icon = Properties.Resources.MeasureTool;
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(JitterEyePatternForm)));
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
        private void PlotInit()
        {
            var plt = Plot.Plot;
            plt.SetAxisLimits(-5, 5, -5, 5);
            plt.SetViewLimits(xMin: -32768, xMax: 32768, yMin: -10000, yMax: 10000);
            plt.Style(figureBackground: Color.Transparent, dataBackground: Color.Transparent, grid: Color.Gray, tick: Color.Gray, axisLabel: Color.Gray, titleLabel: Color.Gray);
            plt.GridCrosslineVisible(false);
            Plot.BackColor = Color.Black;
            plt.YAxis.MinorLogScale(false);
            plt.ResetChannelParameter(0, 1, "", "", 1, false);
            plt.XAxis.MinorLogScale(false);
            plt.ResetTimebaseParameter(0, 1, "", "", 1, false);
            //X轴数据

            Plot.Render();
            //formsPlot1.Refresh();
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

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);

            HeadBackColor = Color.FromArgb(255, 150, 14, 75);
            BorderBackColor = HeadBackColor;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            //GeneralSettingPage.eyePatternForm = null;
            //Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        private void JitterEyePatternForm_EmbededClick(object sender, EventArgs e)
        {
            if (eyePatternParameterForm != null)
                return;
            eyePatternParameterForm = new EyePatternParameterForm()
            {
                Anchor = AnchorStyles.Top,
                Location = new(1400, 420),
            };
            EventBus.EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = eyePatternParameterForm, Type = FormType.InfoForm });
        }
    }
}
