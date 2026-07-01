using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class DateTimeBox : UserControl, IStylize
    {
        #region 私有字段

        private Color _OriginalForeColor;//存储字体颜色

        #endregion 私有字段

        #region 属性

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        private Color _BaseColor = Color.FromArgb(53, 54, 58);

        [Description("背景色"), Category(default)]
        public Color BaseColor
        {
            get { return _BaseColor; }
            set
            {
                _BaseColor = value;
                foreach (Control c in Controls)
                {
                    foreach (Control cInC in c.Controls)
                    {
                        cInC.BackColor = value;
                    }
                    c.BackColor = value;
                }
            }
        }

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        private Color _FontColor = Color.White;

        [Description("字体颜色"), Category(default)]
        public Color FontColor
        {
            get { return _FontColor; }
            set
            {
                TbxSeparator1.ForeColor = TbxSeparator2.ForeColor = TbxSeparator3.ForeColor = TbxSeparator4.ForeColor
                    = TbxYear.ForeColor = TbxMonth.ForeColor = TbxDay.ForeColor = TbxHour.ForeColor = TbxMinute.ForeColor = TbxSecond.ForeColor = value;
                _FontColor = value;
            }
        }

        //掩盖不需要设置的属性
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        private Font _BaseFont;

        [Description("字体"), Category(default)]
        public Font BaseFont
        {
            get { return _BaseFont; }
            set
            {
                _BaseFont = value;
                foreach (Control c in Controls)
                {
                    foreach (Control cInC in c.Controls)
                    {
                        cInC.Font = value;
                    }
                    c.Font = value;
                }
            }
        }

        private Boolean _ReadOnly = false;

        [Description("是否只读"), Category(default)]
        [DefaultValue(false)]
        public Boolean ReadOnly
        {
            get => _ReadOnly;
            set
            {
                _ReadOnly = value;
                TbxYear.ReadOnly = TbxMonth.ReadOnly = TbxDay.ReadOnly = TbxHour.ReadOnly = TbxMinute.ReadOnly = TbxSecond.ReadOnly = value;
                FontColor = value ? FontColor.GetBrightnessColor(-0.5) : _OriginalForeColor;
            }
        }

        /// <summary>
        /// The current time
        /// </summary>
        private DateTime _CurrentTime = DateTime.Now;

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>The current time.</value>
        [Description("时间"), Category("自定义")]
        [Localizable(true)]
        public DateTime CurrentTime
        {
            get { return _CurrentTime; }
            set
            {
                _CurrentTime = value;
                SetTimeToControl();
            }
        }

        #endregion 属性

        public DateTimeBox()
        {
            InitializeComponent();
            _OriginalForeColor = FontColor;
        }

        #region override

        protected override void OnLoad(EventArgs e)
        {
            SetTimeToControl();
            TbxSecond.LostFocus += TbxSecond_LostFocus;
            base.OnLoad(e);
        }

        #endregion override

        #region Private Method

        /// <summary>
        /// Sets the time to control.
        /// </summary>
        private void SetTimeToControl()
        {
            var y = CurrentTime.Year;
            var M = CurrentTime.Month;
            var d = CurrentTime.Day;
            var h = CurrentTime.Hour;
            var m = CurrentTime.Minute;
            var s = CurrentTime.Second;
            TbxYear.Text = y.ToString();
            TbxMonth.Text = M.ToString().PadLeft(2, '0');
            TbxDay.Text = d.ToString().PadLeft(2, '0');
            TbxHour.Text = h.ToString().PadLeft(2, '0');
            TbxMinute.Text = m.ToString().PadLeft(2, '0');
            TbxSecond.Text = s.ToString().PadLeft(2, '0');
        }

        /// <summary>
        /// Handles the TextChanged event of the TbxYear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxYear_TextChanged(Object sender, EventArgs e)
        {
            if (TbxYear.Text.Length == 4)
            {
                TbxMonth.Focus();
            }
        }

        /// <summary>
        /// Handles the Leave event of the TbxYear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxYear_Leave(Object sender, EventArgs e)
        {
            if (TbxYear.Text.ToInt() < 1990)
            {
                TbxYear.Text = CurrentTime.Year.ToString();
            }
            CurrentTime = (TbxYear.Text + CurrentTime.ToString("-MM-dd HH:mm:ss")).ToDate();
        }

        private void TbxYear_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "。" || e.KeyChar.ToString() == " ")
            {
                if (TbxYear.SelectedText.ToString() == "" && TbxYear.Text.ToString() != "")
                {
                    SendKeys.Send("{Tab}");
                }
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the TbxMonth control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxMonth_TextChanged(Object sender, EventArgs e)
        {
            if (TbxMonth.Text.Length == 2 || TbxMonth.Text.ToInt() >= 2)
            {
                TbxDay.Focus();
            }
        }

        private void TbxMonth_Leave(Object sender, EventArgs e)
        {
            if (TbxMonth.Text.ToInt() < 1)
            {
                TbxMonth.Text = CurrentTime.Month.ToString().PadLeft(2, '0');
            }
            TbxMonth.Text = TbxMonth.Text.PadLeft(2, '0');
            CurrentTime = (CurrentTime.ToString("yyyy-" + TbxMonth.Text + "-dd HH:mm:ss")).ToDate();
        }

        private void TbxMonth_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "。" || e.KeyChar.ToString() == " ")
            {
                if (TbxMonth.SelectedText.ToString() == "" && TbxMonth.Text.ToString() != "")
                {
                    SendKeys.Send("{Tab}");
                }
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the TbxDay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxDay_TextChanged(Object sender, EventArgs e)
        {
            if (TbxDay.Text.Length == 2 || TbxDay.Text.ToInt() >= 4)
            {
                TbxHour.Focus();
            }
        }

        private void TbxDay_Leave(Object sender, EventArgs e)
        {
            if (TbxDay.Text.ToInt() < 1 || TbxDay.Text.ToInt() > DateTime.DaysInMonth(TbxYear.Text.ToInt(), TbxMonth.Text.ToInt()))
            {
                TbxDay.Text = DateTime.DaysInMonth(CurrentTime.Year, TbxMonth.Text.ToInt()).ToString().PadLeft(2, '0');
            }
            TbxDay.Text = TbxDay.Text.PadLeft(2, '0');
            CurrentTime = (CurrentTime.ToString("yyyy-MM-" + TbxDay.Text + " HH:mm:ss")).ToDate();
        }

        private void TbxDay_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "。" || e.KeyChar.ToString() == " ")
            {
                if (TbxDay.SelectedText.ToString() == "" && TbxDay.Text.ToString() != "")
                {
                    SendKeys.Send("{Tab}");
                }
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the TbxHour control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxHour_TextChanged(Object sender, EventArgs e)
        {
            if (TbxHour.Text.Length == 2 || TbxHour.Text.ToInt() >= 3)
            {
                TbxMinute.Focus();
            }
        }

        private void TbxHour_Leave(Object sender, EventArgs e)
        {
            if (TbxHour.Text.ToInt() < 1 || TbxHour.Text.ToInt() > TbxHour.MaxValue)
            {
                TbxHour.Text = TbxHour.MaxValue.ToString().PadLeft(2, '0');
            }
            TbxHour.Text = TbxHour.Text.PadLeft(2, '0');
            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + TbxHour.Text + ":mm:ss")).ToDate();
        }

        private void TbxHour_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "。" || e.KeyChar.ToString() == " ")
            {
                if (TbxHour.SelectedText.ToString() == "" && TbxHour.Text.ToString() != "")
                {
                    SendKeys.Send("{Tab}");
                }
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the TbxMinute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxMinute_TextChanged(Object sender, EventArgs e)
        {
            if (TbxMinute.Text.Length == 2 || TbxMinute.Text.ToInt() >= 6)
            {
                TbxSecond.Focus();
            }
        }

        private void TbxMinute_Leave(Object sender, EventArgs e)
        {
            if (TbxMinute.Text.ToInt() < 1)
            {
                TbxMinute.Text = CurrentTime.Minute.ToString().PadLeft(2, '0');
            }
            TbxMinute.Text = TbxMinute.Text.PadLeft(2, '0');
            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:" + TbxMinute.Text + ":ss")).ToDate();
        }

        private void TbxMinute_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "。" || e.KeyChar.ToString() == " ")
            {
                if (TbxMinute.SelectedText.ToString() == "" && TbxMinute.Text.ToString() != "")
                {
                    SendKeys.Send("{Tab}");
                }
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the TbxSecond control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TbxSecond_TextChanged(Object sender, EventArgs e)
        {
            if (TbxSecond.Text.Length == 2 || TbxSecond.Text.ToInt() >= 6)
            {
                //SendKeys.Send("{Tab}");
            }
        }

        private void TbxSecond_LostFocus(Object sender, EventArgs e)
        {
            if (TbxSecond.Text.ToInt() < 1 || TbxSecond.Text.ToInt() > TbxSecond.MaxValue)
            {
                TbxSecond.Text = CurrentTime.Minute.ToString().PadLeft(2, '0');
            }
            TbxSecond.Text = TbxSecond.Text.PadLeft(2, '0');
            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:mm:" + TbxSecond.Text)).ToDate();
        }

        #endregion Private Method
    }
}
