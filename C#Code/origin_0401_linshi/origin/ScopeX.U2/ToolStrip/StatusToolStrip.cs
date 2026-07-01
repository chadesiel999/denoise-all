using System;
using System.Globalization;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class StatusToolStrip : UserControl
    {
        public StatusToolStrip() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //利用计时器刷新日期和时间
            Timer timer = new();
            timer.Interval = 500;
            timer.Enabled = true;
            timer.Start();
            timer.Tick += (_, _) =>
            {
                timeLabel.Text = DateTime.Now.ToString("t", DateTimeFormatInfo.InvariantInfo);
                //dateLabel.Text = DateTime.Now.ToString("d", DateTimeFormatInfo.InvariantInfo);
                dateLabel.Text = DateTime.Now.ToString(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("yyyyNianMMYueddRi"), DateTimeFormatInfo.InvariantInfo);
            };

            this.Disposed += (_, _) =>
            {
                timer.Stop();
                timer.Dispose();
            };
        }

    }
}
