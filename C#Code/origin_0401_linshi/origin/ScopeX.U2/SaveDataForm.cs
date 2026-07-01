using ScopeX.Core.Tools;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class SaveDataForm : Form
    {
        /// <summary>
        /// //假如保存位置为U盘，实际U盘写入速度会影响整个保存时间，因此超时放宽一些
        /// </summary>
        private const Int32 TimeOutBySecond = 20 * 60;

        private CancellationTokenSource _TokenSource;
        public SaveDataForm()
        {
            InitializeComponent();
            PgBar.DescriptionInfo = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoCunShuJuZhong_QingDengDai");
            ProgressBar.CheckForIllegalCrossThreadCalls = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            PgBar.ValueColor = AppStyleConfig.DefaultCheckedBackColor;

            base.OnLoad(e);
            // LanguageFactory.CacheFormLanguageControls(this);
        }
        private DateTime _StartTime = DateTime.MinValue;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _TokenSource = new CancellationTokenSource();
            _StartTime = DateTime.Now;
            PgBar.Value = 0;
            PgBar.MaxValue = Program.Oscilloscope.SaveDataSoure.GetTotalSegementCount();
            (Program.Oscilloscope.View as DsoForm).MultiWindowManager.SetDrawEnable(false);
            (Program.Oscilloscope.View as DsoForm).ManualUpdateTriggerState(ComModel.SysState.Stop);
            var isfinish = false;
            var res = 0;

            Task.Run(() =>
            {
                res = Program.Oscilloscope.SaveDataSoure.Run(_TokenSource.Token);
            });

            Task.Run(() =>
            {
                while (!isfinish && !_TokenSource.Token.IsCancellationRequested)
                {
                    if (PgBar.Value >= PgBar.MaxValue)
                    {
                        isfinish = true;
                    }
                    PgBar.Value = Program.Oscilloscope.SaveDataSoure.CompletedSegementCount;

                    var runtime = DateTime.Now.Subtract(_StartTime).TotalSeconds;
                    runtime = runtime < 0 ? 0 : runtime;
                    if (runtime > TimeOutBySecond)
                    {
                        _TokenSource.Cancel();
                    }
                    Thread.Sleep(100);
                }
                        (Program.Oscilloscope.View as DsoForm).MultiWindowManager.SetDrawEnable(true);
                Program.Oscilloscope.SaveDataSoure.ClearCompletedSegementCount();

                Task.Run(() =>
                {
                    Thread.Sleep(300);
                    switch (res)
                    {
                        case -1:
                            (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.UnknownErrorSaveData));
                            break;
                        case 0:
                            (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.SavingFailed));
                            break;
                        case 1:
                            (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.SavingSuccess, path: Path.GetDirectoryName(Program.Oscilloscope.SaveDataSoure.SaveFileName)));
                            break;
                        case 2:
                            (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.TimeOutSaveData));
                            break;
                        default:
                            break;
                    };
                });
                _TokenSource.Dispose();
                Close();
            });

  



            //交换执行顺序 优先执行不耗资源的线程
            //Parallel.Invoke(

            //    () =>
            //        {
            //            while (!isfinish && !_TokenSource.Token.IsCancellationRequested)
            //            {
            //                if (PgBar.Value >= PgBar.MaxValue)
            //                {
            //                    isfinish = true;
            //                }
            //                PgBar.Value = Program.Oscilloscope.SaveDataSoure.CompletedSegementCount;

            //                var runtime = DateTime.Now.Subtract(_StartTime).TotalSeconds;
            //                runtime = runtime < 0 ? 0 : runtime;
            //                if (runtime > TimeOutBySecond)
            //                {
            //                    _TokenSource.Cancel();
            //                }
            //                Thread.Sleep(100);
            //            }
            //            (Program.Oscilloscope.View as DsoForm).MultiWindowManager.SetDrawEnable(true);
            //            Program.Oscilloscope.SaveDataSoure.ClearCompletedSegementCount();

            //            Task.Run(() =>
            //            {
            //                Thread.Sleep(300);
            //                switch (res)
            //                {
            //                    case -1:
            //                        (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.UnknownErrorSaveData));
            //                        break;
            //                    case 0:
            //                        (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.SavingFailed));
            //                        break;
            //                    case 1:
            //                        (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.SavingSuccess, path: Path.GetDirectoryName(Program.Oscilloscope.SaveDataSoure.SaveFileName)));
            //                        break;
            //                    case 2:
            //                        (Program.Oscilloscope.View as DsoForm).Invoke(() => WeakTip.Default.Write("Save Waveform", MsgTipId.TimeOutSaveData));
            //                        break;
            //                    default:
            //                        break;
            //                };
            //            });
            //            _TokenSource.Dispose();
            //            Close();
            //        },
            //     () =>
            //        {
            //            res = Program.Oscilloscope.SaveDataSoure.Run(_TokenSource.Token);
            //        }
            //    );
        }
    }
}
