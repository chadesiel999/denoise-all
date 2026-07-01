using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.LanguageDefinition;
using ScopeX.U2.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public class ScopeXDateTimeBox : Control, IStylize, ILanguageControl
    {
        private const Int32 OFFSET = 5;

        private readonly TextItemInfo _Year;
        private readonly TextItemInfo _Month;
        private readonly TextItemInfo _Day;
        private readonly TextItemInfo _Hour;
        private readonly TextItemInfo _Minute;
        private readonly TextItemInfo _Second;
        private readonly TextItemInfo _Separator1;
        private readonly TextItemInfo _Separator2;
        private readonly TextItemInfo _Separator3;
        private readonly TextItemInfo _Separator4;
        private readonly TextItemInfo _CursorLine;
        private readonly List<TextItemInfo> _TextInfos;

        private readonly Timer _Timer;
        private Boolean _ShowCursor = true;
        private Boolean _ArgToCtrl = false;
        private Boolean _IsDiposed = true;//防止冗余调用

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public LanguagePattern LanguagePattern { get => LanguagePattern.Ignore; set => _ = value; }

        [Browsable(true)]
        [Description("背景颜色"), DefaultValue(typeof(Color))]
        public override Color BackColor { get; set; } = Color.FromArgb(53, 54, 58);

        private Color _ForeColor = Color.White;

        [Browsable(true)]
        [Description("字体颜色"), DefaultValue(typeof(Color))]
        public override Color ForeColor { get; set; } = Color.White;

        [Browsable(true)]
        [Description("文本字体"), DefaultValue(typeof(Font))]
        public override Font Font
        {
            get => base.Font; set
            {
                if (base.Font != value)
                {
                    CalcTextInfoRegion();
                    CalcSeparatorRegion();
                    Refresh();
                }
                base.Font = value;
            }
        }

        [Browsable(true)]
        [Description("是否只读"), DefaultValue(typeof(Boolean))]
        public Boolean ReadOnly { get; set; } = false;

        [Browsable(true)]
        [Description("光标样式"), DefaultValue(typeof(Cursors))]
        public override Cursor Cursor { get; set; } = Cursors.IBeam;

        [Browsable(true)]
        [Description("选中区域颜色"), DefaultValue(typeof(Color))]
        public Color SelectedColor { get; set; } = Color.FromArgb(40, 71, 193);

        /// <summary>
        /// 选中区域
        /// </summary>
        private Rectangle SelectedRegion { get; set; }

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
            get => _CurrentTime;
            set
            {
                if (_CurrentTime != value)
                {
                    _CurrentTime = value;
                    SetTimeToControl();
                }
            }
        }

        public ScopeXDateTimeBox()
        {
            SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);

            _Year = new() { ItemIndex = 1 };
            _Month = new() { ItemIndex = 2 };
            _Day = new() { ItemIndex = 3 };
            _Hour = new() { ItemIndex = 4 };
            _Minute = new() { ItemIndex = 5 };
            _Second = new() { ItemIndex = 6 };
            SetTimeToControl();

            _Separator1 = new() { Text = "-" };
            _Separator2 = new() { Text = "-" };
            _Separator3 = new() { Text = ":" };
            _Separator4 = new() { Text = ":" };

            _CursorLine = new() { Text = "|" };

            _TextInfos = new List<TextItemInfo>();
            _TextInfos.Add(_Year);
            _TextInfos.Add(_Month);
            _TextInfos.Add(_Day);
            _TextInfos.Add(_Hour);
            _TextInfos.Add(_Minute);
            _TextInfos.Add(_Second);

            _Timer = new Timer();
            _Timer.Interval = 1000;
            _Timer.Tick += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    _ShowCursor = !_ShowCursor;
                    Refresh();
                }
            };
            _Timer.Start();
            _Timer.Enabled = false;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!IsAnySelected() && !IsAnyFocused())
            {
                _Year.Selected = true;
                CalcSelectedRegion();
                Refresh();
            }
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            SetAllselected(false);
            SetAllFocused(false);
            _ShowCursor = _Timer.Enabled = false;
            Refresh();
            //SystemSoftKeyboard.Close();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            _ArgToCtrl = true;
            var mousepoint = this.PointToClient(Control.MousePosition);
            var item = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Bound.Contains(mousepoint));
            if (item != null)
            {
                item.Focused = true;
                var otheritem = _TextInfos.Where((TextItemInfo x) => x.ItemIndex != item.ItemIndex).ToList();
                foreach (var other in otheritem)
                {
                    other.Focused = !item.Focused;
                }
                SystemSoftKeyboard.Show();
            }
            else
            {
                SetAllFocused(false);
            }
            _ArgToCtrl = false;

            if (IsAnyFocused())
            {
                SetAllselected(false);
                _ShowCursor = true;
                Refresh();//先刷新一次 光标立即显示
                _Timer.Enabled = true;
                this.Focus();
            }

        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            _ArgToCtrl = true;
            var mousepoint = this.PointToClient(Control.MousePosition);
            var item = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Bound.Contains(mousepoint));
            if (item != null)
            {
                item.Selected = true;
                var otheritem = _TextInfos.Where((TextItemInfo x) => x.ItemIndex != item.ItemIndex).ToList();
                foreach (var other in otheritem)
                {
                    other.Selected = !item.Selected;
                }
            }
            else
            {
                SetAllselected(false);
            }

            _ArgToCtrl = false;

            if (IsAnySelected())
            {
                _ShowCursor = _Timer.Enabled = false;
                CalcSelectedRegion();
                Refresh();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                _ArgToCtrl = true;
                if (_Year.Selected || _Year.Focused)
                {
                    if (String.IsNullOrEmpty(_Year.Text))
                    {
                        _Year.Text = DateTime.Now.Year.ToString();
                    }
                    _Year.Focused = _Year.Selected = false;
                    _Month.Focused = _Month.Selected = true;
                }
                else if (_Month.Selected || _Month.Focused)
                {
                    if (String.IsNullOrEmpty(_Month.Text))
                    {
                        _Month.Text = DateTime.Now.Month.ToString().PadLeft(2, '0');
                    }
                    _Month.Focused = _Month.Selected = false;
                    _Day.Focused = _Day.Selected = true;
                }
                else if (_Day.Selected || _Day.Focused)
                {
                    if (String.IsNullOrEmpty(_Day.Text))
                    {
                        _Day.Text = DateTime.Now.Day.ToString().PadLeft(2, '0');
                    }
                    _Day.Focused = _Day.Selected = false;
                    _Hour.Focused = _Hour.Selected = true;
                }
                else if (_Hour.Selected || _Hour.Focused)
                {
                    if (String.IsNullOrEmpty(_Hour.Text))
                    {
                        _Hour.Text = DateTime.Now.Hour.ToString().PadLeft(2, '0');
                    }
                    _Hour.Focused = _Hour.Selected = false;
                    _Minute.Focused = _Minute.Selected = true;
                }
                else if (_Minute.Selected || _Minute.Focused)
                {
                    if (String.IsNullOrEmpty(_Minute.Text))
                    {
                        _Minute.Text = DateTime.Now.Minute.ToString().PadLeft(2, '0');
                    }
                    _Minute.Focused = _Minute.Selected = false;
                    _Second.Focused = _Second.Selected = true;
                }
                else
                {
                    if (String.IsNullOrEmpty(_Second.Text))
                    {
                        _Second.Text = DateTime.Now.Second.ToString().PadLeft(2, '0');
                    }
                    _Second.Focused = _Second.Selected = false;
                    _ShowCursor = _Timer.Enabled = false;
                    Refresh();
                }
                _ArgToCtrl = false;
                if (IsAnySelected())
                {
                    CalcSelectedRegion();
                    _ShowCursor = _Timer.Enabled = false;
                }
                Refresh();
            }
            return keyData == Keys.Tab;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // 阻止消息冒泡，从而出现按一下按键，出现两个字符的情况。Bug:2427
            e.Handled = true;
            if (ReadOnly)
            {
                return;
            }
            //如果输入的不是退格和数字，则屏蔽输入
            if (!(e.KeyChar == '\b' || (e.KeyChar >= '0' && e.KeyChar <= '9')))
            {
                return;
            }
            if (_Year.Selected || _Year.Focused)
            {
                KeyFilterYear(e);
            }
            else if (_Month.Selected || _Month.Focused)
            {
                KeyFilterMonth(e);
            }
            else if (_Day.Selected || _Day.Focused)
            {
                KeyFilterDay(e);
            }
            else if (_Hour.Selected || _Hour.Focused)
            {
                KeyFilterHour(e);
            }
            else if (_Minute.Selected || _Minute.Focused)
            {
                KeyFilterMinute(e);
            }
            else if (_Second.Selected || _Second.Focused)
            {
                KeyFilterSecond(e);
            }

            if (!IsAnySelected())
            {
                if (IsAnyFocused())
                {
                    _Timer.Enabled = _ShowCursor = true;
                }
                else
                {
                    _Timer.Enabled = _ShowCursor = false;
                }
            }
            else
            {
                CalcSelectedRegion();
                _Timer.Enabled = _ShowCursor = false;
            }

            Refresh();
        }

        private void KeyFilterYear(KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (_Year.Selected)//选中状态
                {
                    _Year.Selected = false;
                    _Year.Text = e.KeyChar.ToString();
                }// end if (_Year.Selected)
                else
                {
                    if (_Year.Text.Length == 4)
                    {
                        if (_Year.Text.ToInt() < 1990)
                        {
                            _Year.Text = DateTime.Now.Year.ToString();
                        }
                        CurrentTime = (_Year.Text + CurrentTime.ToString("-MM-dd HH:mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                    else
                    {
                        _Year.Text += e.KeyChar;
                        if (_Year.Text.Length == 4)
                        {
                            if (_Year.Text.ToInt() < 1990)
                            {
                                _Year.Text = DateTime.Now.Year.ToString();
                            }
                            CurrentTime = (_Year.Text + CurrentTime.ToString("-MM-dd HH:mm:ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }

                    }
                }
            }
            else
            {
                Remove(_Year);
            }
        }

        private void KeyFilterMonth(KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (_Month.Selected)//选中状态
                {
                    _Month.Selected = false;
                    _Month.Text = e.KeyChar.ToString();
                    if (_Month.Text.ToInt() > 1)
                    {
                        _Month.Text = _Month.Text.PadLeft(2, '0');
                        CurrentTime = (CurrentTime.ToString("yyyy-" + _Month.Text + "-dd HH:mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                }// end if (_Month.Selected)
                else
                {
                    if (_Month.Text.Length == 2)
                    {
                        if (_Month.Text.ToInt() > 12)
                        {
                            _Month.Text = DateTime.Now.Month.ToString();
                        }
                        CurrentTime = (CurrentTime.ToString("yyyy-" + _Month.Text + "-dd HH:mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                    else
                    {
                        _Month.Text += e.KeyChar;
                        if (_Month.Text.Length < 2 && _Month.Text.ToInt() > 1)
                        {
                            _Month.Text = _Month.Text.PadLeft(2, '0');
                            CurrentTime = (CurrentTime.ToString("yyyy-" + _Month.Text + "-dd HH:mm:ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                        else if (_Month.Text.Length == 2)
                        {
                            if (_Month.Text.ToInt() > 12)
                            {
                                _Month.Text = DateTime.Now.Month.ToString().PadLeft(2, '0');
                            }
                            CurrentTime = (CurrentTime.ToString("yyyy-" + _Month.Text + "-dd HH:mm:ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                    }
                }
            }
            else
            {
                Remove(_Month);
            }
        }

        private void KeyFilterDay(KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (_Day.Selected)//选中状态
                {
                    _Day.Selected = false;
                    _Day.Text = e.KeyChar.ToString();
                    if (Int32.TryParse(_Day.Text, out Int32 m) && m > 3)
                    {
                        _Day.Text = _Day.Text.PadLeft(2, '0');
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-" + _Day.Text + " HH:mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                }// end if (_Month.Selected)
                else
                {
                    if (_Day.Text.Length == 2)
                    {
                        if (_Day.Text.ToInt() < 1 || _Day.Text.ToInt() > DateTime.DaysInMonth(_Year.Text.ToInt(), _Month.Text.ToInt()))
                        {
                            _Day.Text = DateTime.DaysInMonth(CurrentTime.Year, _Month.Text.ToInt()).ToString().PadLeft(2, '0');
                        }
                        _Day.Text = _Day.Text.PadLeft(2, '0');
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-" + _Day.Text + " HH:mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                    else
                    {
                        _Day.Text += e.KeyChar;
                        if (_Day.Text.Length == 1)
                        {
                            if (_Day.Text.ToInt() > 3)
                            {
                                _Day.Text = _Day.Text.PadLeft(2, '0');
                                CurrentTime = (CurrentTime.ToString("yyyy-MM-" + _Day.Text + " HH:mm:ss")).ToDate();
                                SendKeys.SendWait("{TAB}");
                            }
                        }
                        else if (_Day.Text.Length == 2)
                        {
                            if (_Day.Text.ToInt() < 1 || _Day.Text.ToInt() > DateTime.DaysInMonth(_Year.Text.ToInt(), _Month.Text.ToInt()))
                            {
                                _Day.Text = DateTime.DaysInMonth(CurrentTime.Year, _Month.Text.ToInt()).ToString().PadLeft(2, '0');
                            }
                            _Day.Text = _Day.Text.PadLeft(2, '0');
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-" + _Day.Text + " HH:mm:ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                    }
                }
            }
            else
            {
                Remove(_Day);
            }
        }

        private void KeyFilterHour(KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (_Hour.Selected)
                {
                    _Hour.Selected = false;
                    _Hour.Text = e.KeyChar.ToString();
                    if (_Hour.Text.ToInt() > 2)
                    {
                        _Hour.Text = _Hour.Text.PadLeft(2, '0');
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + _Hour.Text + ":mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                }//end if _Hour.Selected
                else
                {
                    if (_Hour.Text.Length == 2)
                    {
                        if (_Hour.Text.ToInt() < 1 || _Hour.Text.ToInt() > 23)
                        {
                            _Hour.Text = DateTime.Now.Hour.ToString().PadLeft(2, '0');
                        }
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + _Hour.Text + ":mm:ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                    else
                    {
                        _Hour.Text += e.KeyChar;
                        if (_Hour.Text.Length <= 1 && _Hour.Text.ToInt() > 2)
                        {
                            _Hour.Text = _Hour.Text.PadLeft(2, '0');
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + _Hour.Text + ":mm:ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                        else if (_Hour.Text.Length >= 2)
                        {
                            if (_Hour.Text.ToInt() < 1 || _Hour.Text.ToInt() > 23)
                            {
                                _Hour.Text = DateTime.Now.Hour.ToString().PadLeft(2, '0');
                            }
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + _Hour.Text + ":mm:ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                    }
                }
            }
            else
            {
                Remove(_Hour);
            }
        }

        private void KeyFilterMinute(KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (_Minute.Selected)
                {
                    _Minute.Selected = false;
                    _Minute.Text = e.KeyChar.ToString();
                    if (_Minute.Text.ToInt() > 5)
                    {
                        _Minute.Text = _Minute.Text.PadLeft(2, '0');
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:" + _Minute.Text + ":ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                }
                else
                {
                    if (_Minute.Text.Length == 2)
                    {
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:" + _Minute.Text + ":ss")).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                    else
                    {
                        _Minute.Text += e.KeyChar;
                        if (_Minute.Text.Length <= 1 && _Minute.Text.ToInt() > 5)
                        {
                            _Minute.Text = _Minute.Text.PadLeft(2, '0');
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:" + _Minute.Text + ":ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                        else if (_Minute.Text.Length >= 2)
                        {
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:" + _Minute.Text + ":ss")).ToDate();
                            SendKeys.SendWait("{TAB}");
                        }
                    }
                }
            }
            else
            {
                Remove(_Minute);
            }
        }

        private void KeyFilterSecond(KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')
            {
                if (_Second.Selected)
                {
                    _Second.Selected = false;
                    _Second.Text = e.KeyChar.ToString();
                    if (_Second.Text.ToInt() > 5)
                    {
                        _Second.Text = _Second.Text.PadLeft(2, '0');
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:mm:" + _Second.Text)).ToDate();
                        SendKeys.SendWait("{TAB}");
                    }
                }
                else
                {
                    if (_Second.Text.Length >= 2)
                    {
                        CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:mm:" + _Second.Text)).ToDate();
                        _Second.Selected = _Second.Focused = false;
                        SendKeys.SendWait("{TAB}");
                    }
                    else
                    {
                        _Second.Text += e.KeyChar;
                        if (_Second.Text.Length <= 1 && _Second.Text.ToInt() > 5)
                        {
                            _Second.Text = _Second.Text.PadLeft(2, '0');
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:mm:" + _Second.Text)).ToDate();
                            _Second.Selected = _Second.Focused = false;
                            SendKeys.SendWait("{TAB}");
                        }
                        else if (_Second.Text.Length >= 2)
                        {
                            CurrentTime = (CurrentTime.ToString("yyyy-MM-dd " + "HH:mm:" + _Second.Text)).ToDate();
                            _Second.Selected = _Second.Focused = false;
                            SendKeys.SendWait("{TAB}");
                        }
                    }
                }
            }
            else
            {
                Remove(_Second);
            }
        }

        private void Remove(TextItemInfo info)
        {
            if (info.Selected)
            {
                info.Focused = !(info.Selected = false);
                info.Text = "";
            }
            else
            {
                if (info.Text.Length == 0)
                {
                    info.Text = "";
                }
                else
                {
                    info.Text = info.Text.Substring(0, info.Text.Length - 1);
                }
            }
        }

        public override void Refresh()
        {
            try
            {
                Draw(Graphics.FromHwnd(Handle));
            }
            catch (Exception)
            { }
            base.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            CalcTextInfoRegion();
            CalcSeparatorRegion();
            CalcCursorRegion();
            Draw(e.Graphics);
        }

        private void Draw(Graphics graphics)
        {
            graphics.Clear(BackColor);

            if (IsAnySelected())
            {
                graphics.FillRectangle(new SolidBrush(SelectedColor), SelectedRegion);
            }
            else
            {
                graphics.FillRectangle(new SolidBrush(BackColor), SelectedRegion);
            }

            TextRenderer.DrawText(graphics, _Year.Text, Font, _Year.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(graphics, _Separator1.Text, Font, _Separator1.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(graphics, _Month.Text, Font, _Month.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(graphics, _Separator2.Text, Font, _Separator2.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(graphics, _Day.Text, Font, _Day.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(graphics, _Hour.Text, Font, _Hour.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(graphics, _Separator3.Text, Font, _Separator3.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(graphics, _Minute.Text, Font, _Minute.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(graphics, _Separator4.Text, Font, _Separator4.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            TextRenderer.DrawText(graphics, _Second.Text, Font, _Second.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            if (_ShowCursor)
            {
                TextRenderer.DrawText(graphics, _CursorLine.Text, Font, _CursorLine.DrawRegion, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);//draw Cursor
            }
        }

        protected override void Dispose(Boolean disposing)
        {
            if (_IsDiposed)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    _Timer.Enabled = false;
                    _Timer?.Dispose();
                    if (Font != null)
                    {
                        Font = null;
                    }
                }

                _IsDiposed = false;
            }
        }

        #region Calc

        /// <summary>
        /// 计算文本绘制区域
        /// </summary>
        private void CalcTextInfoRegion()
        {
            var separator1length = TextRenderer.MeasureText(_Separator1.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            var separator2length = TextRenderer.MeasureText(_Separator3.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            var width = Width - separator1length * 2 - separator2length * 2;
            var itemwidth = width / 6;//年 月 日 时 分 秒

            //年
            var textwidth = TextRenderer.MeasureText(_Year.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Year.Bound = new Rectangle(itemwidth * 0, 0, itemwidth, Height);
            _Year.DrawRegion = new Rectangle(itemwidth - textwidth - OFFSET * 2, 0, textwidth, Height);

            //月
            textwidth = TextRenderer.MeasureText(_Month.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Month.Bound = new Rectangle(itemwidth * 1 + separator1length, 0, itemwidth, Height);
            _Month.DrawRegion = new Rectangle(itemwidth * 2 + separator1length - textwidth - OFFSET * 2, 0, textwidth, Height);

            //日
            textwidth = TextRenderer.MeasureText(_Day.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Day.Bound = new Rectangle(itemwidth * 2 + separator1length * 2, 0, itemwidth, Height);
            _Day.DrawRegion = new Rectangle(itemwidth * 3 + separator1length * 2 - textwidth - OFFSET * 2, 0, textwidth, Height);

            //时
            textwidth = TextRenderer.MeasureText(_Hour.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Hour.Bound = new Rectangle(itemwidth * 3 + separator1length * 3, 0, itemwidth, Height);
            _Hour.DrawRegion = new Rectangle(itemwidth * 4 + separator1length - textwidth - OFFSET * 2, 0, textwidth, Height);

            //分
            textwidth = TextRenderer.MeasureText(_Minute.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Minute.Bound = new Rectangle(itemwidth * 4 + separator1length * 3 + separator2length, 0, itemwidth, Height);
            _Minute.DrawRegion = new Rectangle(itemwidth * 5 + separator1length * 3 + separator2length - textwidth - OFFSET * 2, 0, textwidth, Height);

            //秒
            textwidth = TextRenderer.MeasureText(_Second.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Second.Bound = new Rectangle(itemwidth * 5 + separator1length * 3 + separator2length * 2, 0, itemwidth, Height);
            _Second.DrawRegion = new Rectangle(itemwidth * 6 + separator1length * 3 + separator2length * 2 - textwidth - OFFSET * 2, 0, textwidth, Height);
        }

        /// <summary>
        /// 计算分隔符
        /// </summary>
        private void CalcSeparatorRegion()
        {
            var separator1length = TextRenderer.MeasureText(_Separator1.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            var separator2length = TextRenderer.MeasureText(_Separator3.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            _Separator1.DrawRegion = new Rectangle(_Year.DrawRegion.X + _Year.DrawRegion.Width + OFFSET * 3, 0, separator1length, Height);
            _Separator2.DrawRegion = new Rectangle(_Month.DrawRegion.X + _Month.DrawRegion.Width + OFFSET * 3, 0, separator1length, Height);
            _Separator3.DrawRegion = new Rectangle(_Hour.DrawRegion.X + _Hour.DrawRegion.Width + OFFSET * 3, 0, separator2length, Height);
            _Separator4.DrawRegion = new Rectangle(_Minute.DrawRegion.X + _Minute.DrawRegion.Width + OFFSET * 3, 0, separator2length, Height);
        }

        /// <summary>
        /// 计算光标绘制区域
        /// </summary>
        private void CalcCursorRegion()
        {
            var length = TextRenderer.MeasureText(_CursorLine.Text + "X", Font).Width - TextRenderer.MeasureText("X", Font).Width;
            var focusedtext = GetFocusedText();
            if (focusedtext != null)
            {
                _CursorLine.DrawRegion = new Rectangle(focusedtext.DrawRegion.X + focusedtext.DrawRegion.Width, 0, length, Height);
            }
        }

        /// <summary>
        /// 计算选中文本绘制区域
        /// </summary>
        private void CalcSelectedRegion()
        {
            var textinfo = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Selected == true);
            if (textinfo == null)
            {
                return;
            }
            var size = TextRenderer.MeasureText(textinfo.Text, Font);
            SelectedRegion = new Rectangle(textinfo.DrawRegion.X, textinfo.DrawRegion.Y + (textinfo.DrawRegion.Height - size.Height) / 2, size.Width - OFFSET * 2, size.Height);
        }

        #endregion

        /// <summary>
        /// 获取一个值 指示是否有区域被选中
        /// </summary>
        /// <returns></returns>
        private Boolean IsAnySelected()
        {
            if (_Year.Selected || _Month.Selected || _Day.Selected || _Hour.Selected || _Minute.Selected || _Second.Selected)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取一个值 指示是否有区域获取到输入焦点
        /// </summary>
        /// <returns></returns>
        private Boolean IsAnyFocused()
        {
            if (_Year.Focused || _Month.Focused || _Day.Focused || _Hour.Focused || _Minute.Focused || _Second.Focused)
            {
                return true;
            }
            return false;
        }

        private void SetAllselected(Boolean selected = true)
        {
            foreach (var item in _TextInfos)
            {
                item.Selected = selected;
            }
        }

        private void SetAllFocused(Boolean focused = true)
        {
            foreach (var item in _TextInfos)
            {
                item.Focused = focused;
            }
        }

        /// <summary>
        /// 获取得到输入焦点的TextInfo
        /// </summary>
        /// <returns></returns>
        private TextItemInfo GetFocusedText()
        {
            var textifo = _TextInfos.FirstOrDefault((TextItemInfo x) => x.Focused == true);
            return textifo;
        }

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
            _Year.Text = y.ToString();
            _Month.Text = M.ToString().PadLeft(2, '0');
            _Day.Text = d.ToString().PadLeft(2, '0');
            _Hour.Text = h.ToString().PadLeft(2, '0');
            _Minute.Text = m.ToString().PadLeft(2, '0');
            _Second.Text = s.ToString().PadLeft(2, '0');
        }
    }
}
