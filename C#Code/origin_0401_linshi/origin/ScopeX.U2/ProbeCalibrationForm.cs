using ScopeX.ComModel;
using ScopeX.U2.LanguageSupoort;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class ProbeCalibrationForm : Form
    {
        public ProbeCalibrationForm()
        {
            InitializeComponent();
            ProgressBar.CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 此种方式实现通道面板和校准窗口通道设置 不理想，暂时不知怎么调整
        /// </summary>
        public ChannelId ProbeAtChlId { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            PgBar.ValueColor = AppStyleConfig.DefaultCheckedBackColor;
            PgBar.MaxValue = Program.Oscilloscope.ProbeCalibration.GetProbeCaliTotalItemCount();
            PgBar.Value = 0;
            base.OnLoad(e);
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            var isfinish = false;
            Parallel.Invoke(
                () =>
                {
                    Program.Oscilloscope.ProbeCalibration.ClearProbeCaliFinishedCount();
                    while (!isfinish)
                    {
                        Thread.Sleep(100);
                        if (PgBar.Value >= PgBar.MaxValue)
                        {
                            isfinish = true;
                        }
                        PgBar.Value = Program.Oscilloscope.ProbeCalibration.GetProbeCaliFinishedCount();                        
                    }
                    PgBar.DescriptionInfo = Program.Oscilloscope.ProbeCalibration.CalibMessage;
                    Close();
                },
                () => {
                    Program.Oscilloscope.ProbeCalibration.Run(ProbeAtChlId);
                    Program.Oscilloscope.ProbeCalibration.ForceProbeCaliFinished();
                }
             );
        }
    }
}
