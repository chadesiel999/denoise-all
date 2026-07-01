using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    class BatchTask_Demo: BatchTaskBase
    {
        public override int MaxStepCount
        {
            get => 100;
        }
        public override string ResultTipMessage
        {
            get => "文件保存在.....";
        }

        public override bool CheckPrepareOk(ref string fileMessage, ref string InstrumentationInfo)
        {
            return MessageBox.Show("你确认要执行该任务吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
        protected override void TaskBody()
        {
            state= BatchTaskState.Running;
            for(int i=0;i<100;i++)
            {
                Thread.Sleep(100);
                updateAction?.Invoke(i, $"正在处理{i}...", "上步处理OK");
                if (cancelTokenSrc != null)
                {
                    try
                    {
                        cancelTokenSrc.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        state = BatchTaskState.Canceled;
                        return;
                    }
                }
            }
            state = BatchTaskState.FinishedOK;
        }
    }
}
