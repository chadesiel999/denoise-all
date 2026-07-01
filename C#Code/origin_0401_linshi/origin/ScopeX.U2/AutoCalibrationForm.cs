using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class AutoCalibrationForm : Form
    {
        public AutoCalibrationForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            PgBar.ValueColor = AppStyleConfig.DefaultCheckedBackColor;
            //PgBar.MaxValue = Program.Oscilloscope.AutoCalibration.GetTotalItemCount();
            //PgBar.Value = 0;
            base.OnLoad(e);
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            (Program.Oscilloscope.View as DsoForm).MultiWindowManager.SetDrawEnable(false);
            (Program.Oscilloscope.View as DsoForm).ManualUpdateTriggerState(ComModel.SysState.Stop);
            var max = Program.Oscilloscope.AutoCalibration.GetTotalItemCount();
            Program.Oscilloscope.AutoCalibration.ClearFinishedCount();

            PgBar.Value = 0;
            PgBar.MaxValue = max;
            PgBar.Refresh();
            PgBar.Visible = false;
            var isfinish = false;
            Task.Run(() =>
            {
                Program.Oscilloscope.AutoCalibration.Run();
            });
            Task.Run(() =>
            {
                while (!isfinish)
                {
                    if (PgBar.Value >= PgBar.MaxValue)
                    {
                        isfinish = true;
                    }

                    var current = Program.Oscilloscope.AutoCalibration.GetFinishedCount();

                    BeginInvoke(() =>
                    {
                        PgBar.Value = current;
                        PgBar.Refresh();
                    });

                    Thread.Sleep(30);
                }
                Program.Oscilloscope.AutoCalibration.ClearFinishedCount();
                (Program.Oscilloscope.View as DsoForm).BeginInvoke(() =>
                {
                    (Program.Oscilloscope.View as DsoForm).ManualUpdateTriggerState(ComModel.SysState.Auto);
                    (Program.Oscilloscope.View as DsoForm).MultiWindowManager.SetDrawEnable(true);
                    Close();
                });
            });

            //Parallel.Invoke(
            //    () => Program.Oscilloscope.AutoCalibration.Run(),
            //    () =>
            //        {
            //            while (!isfinish)
            //            {
            //                if (PgBar.Value >= PgBar.MaxValue)
            //                {
            //                    isfinish = true;
            //                }
            //                PgBar.Value = Program.Oscilloscope.AutoCalibration.GetFinishedCount();
            //                Thread.Sleep(10);
            //            }
            //            Program.Oscilloscope.AutoCalibration.ClearFinishedCount();
            //            Close();
            //        }
            // );
        }
    }
}
