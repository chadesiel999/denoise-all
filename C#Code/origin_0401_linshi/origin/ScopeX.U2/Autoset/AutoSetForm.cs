using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class AutoSetForm : Form
    {
        public AutoSetForm(Action<CancellationToken> autoset)
        {
            InitializeComponent();
            _AutoSetCallback = autoset;
            _AutoSetTimer = new System.Threading.Timer(AutoSet, _ClosingEvent, Timeout.Infinite, Timeout.Infinite);
        }

        private readonly Action<CancellationToken> _AutoSetCallback;

        private readonly System.Threading.Timer _AutoSetTimer;

        private readonly CancellationTokenSource _Source = new();

        private readonly AutoResetEvent _ClosingEvent = new(false);

        private void AutoSet(Object state)
        {
            try
            {
                _AutoSetCallback(_Source.Token);

                DialogResult = DialogResult.OK;
            }
            catch (OperationCanceledException)
            {
                //if (InvokeRequired)
                //{
                //    BeginInvoke(new Action(() =>
                //    {
                //        DialogResult = DialogResult.OK;
                //        Close();
                //    }));
                //}
                //else
                //{
                //    DialogResult = DialogResult.OK;
                //    Close();
                //}
                

                DialogResult = DialogResult.Cancel;
            }
            finally
            {
                _ClosingEvent.Set();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _AutoSetTimer.Change(10, Timeout.Infinite);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _Source.Cancel();

            _ClosingEvent.WaitOne();

            base.OnFormClosed(e);

            _AutoSetTimer.Dispose();
            _ClosingEvent.Dispose();
            _Source.Dispose();
            Dispose();
        }
    }
}
