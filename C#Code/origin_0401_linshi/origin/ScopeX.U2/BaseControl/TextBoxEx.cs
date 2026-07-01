using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    /// <summary>
    /// Class TextBoxEx.
    /// Implements the <see cref="System.Windows.Forms.ScopeXTextBox" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.ScopeXTextBox" />
    public partial class TextBoxEx : ScopeXTextBox
    {
        #region 私有字段

        /// <summary>
        /// The BLN focus
        /// </summary>
        private Boolean blnFocus = false;

        /// <summary>
        /// The m string old value
        /// </summary>
        private String m_strOldValue = string.Empty;

        #endregion

        #region 属性

        /// <summary>
        /// The input type
        /// </summary>
        private TextInputType _InputType = TextInputType.NotControl;
        /// <summary>
        /// Gets or sets the type of the input.
        /// </summary>
        /// <value>The type of the input.</value>
        [Description("获取或设置一个值，该值指示文本框中的文本输入类型。")]
        public TextInputType InputType
        {
            get
            {
                return _InputType;
            }
            set
            {
                _InputType = value;
                if (value != TextInputType.NotControl)
                {
                    TextChanged -= new EventHandler(this.TextBoxEx_TextChanged);
                    TextChanged += new EventHandler(this.TextBoxEx_TextChanged);
                }
                else
                {
                    TextChanged -= new EventHandler(this.TextBoxEx_TextChanged);
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示当输入类型InputType=Regex时，使用的正则表达式。
        /// </summary>
        /// <value>The regex pattern.</value>
        [Description("获取或设置一个值，该值指示当输入类型InputType=Regex时，使用的正则表达式。")]
        public String RegexPattern { get; set; } = "";

        /// <summary>
        /// 当InputType为数字类型时，能输入的最大值
        /// </summary>
        /// <value>The maximum value.</value>
        [Description("当InputType为数字类型时，能输入的最大值。")]
        public Decimal MaxValue { get; set; } = 1000000m;

        /// <summary>
        /// 当InputType为数字类型时，能输入的最小值
        /// </summary>
        /// <value>The minimum value.</value>
        [Description("当InputType为数字类型时，能输入的最小值。")]
        public Decimal MinValue { get; set; } = -1000000m;

        /// <summary>
        /// 当InputType为数字类型时，能输入的最小值
        /// </summary>
        /// <value>The length of the decimal.</value>
        [Description("当InputType为数字类型时，小数位数。")]
        public Int32 DecLength { get; set; } = 2;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxEx" /> class.
        /// </summary>
        public TextBoxEx()
        {
            this.InitializeComponent();
            base.GotFocus += new EventHandler(this.TextBoxEx_GotFocus);
            base.MouseUp += new MouseEventHandler(this.TextBoxEx_MouseUp);
            base.KeyPress += TextBoxEx_KeyPress;
        }

        #region private

        /// <summary>
        /// Handles the KeyPress event of the TextBoxEx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyPressEventArgs" /> instance containing the event data.</param>
        private void TextBoxEx_KeyPress(Object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == System.Convert.ToChar(13) || e.KeyChar == System.Convert.ToChar(27))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the TextBoxEx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void TextBoxEx_MouseUp(Object sender, MouseEventArgs e)
        {
            if (this.blnFocus)
            {
                this.blnFocus = false;
            }
        }

        /// <summary>
        /// Handles the GotFocus event of the TextBoxEx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TextBoxEx_GotFocus(Object sender, EventArgs e)
        {
            this.blnFocus = true;
        }

        /// <summary>
        /// Handles the TextChanged event of the TextBoxEx control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void TextBoxEx_TextChanged(Object sender, EventArgs e)
        {
            if (this.Text == "")
            {
                this.m_strOldValue = this.Text;
            }
            else if (this.m_strOldValue != this.Text)
            {
                if (CheckInputType(this.Text, InputType, MaxValue, MinValue, DecLength, RegexPattern))
                {
                    Int32 num = base.SelectionStart;
                    if (this.m_strOldValue.Length < this.Text.Length)
                    {
                        num--;
                    }
                    else
                    {
                        num++;
                    }
                    base.TextChanged -= new EventHandler(this.TextBoxEx_TextChanged);
                    this.Text = this.m_strOldValue;
                    base.TextChanged += new EventHandler(this.TextBoxEx_TextChanged);
                    if (num < 0)
                    {
                        num = 0;
                    }
                    base.SelectionStart = num;
                }
                else
                {
                    this.m_strOldValue = this.Text;
                }
            }
        }

        #endregion

        #region override

        /// <summary>
        /// Handles the <see cref="E:TextChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            base.Invalidate();
        }

        #endregion

        #region public

        /// <summary>
        /// 检查文本控件输入类型是否有效
        /// </summary>
        /// <param name="strValue">值</param>
        /// <param name="inputType">控制类型</param>
        /// <param name="decMaxValue">最大值</param>
        /// <param name="decMinValue">最小值</param>
        /// <param name="intLength">小数位长度</param>
        /// <param name="strRegexPattern">正则</param>
        /// <returns>返回值</returns>
        public static Boolean CheckInputType(
            String strValue,
            TextInputType inputType,
            Decimal decMaxValue = default(Decimal),
            Decimal decMinValue = default(Decimal),
            Int32 intLength = 2,
            String strRegexPattern = null)
        {
            Boolean result;
            switch (inputType)
            {
                case TextInputType.NotControl:
                    result = true;
                    return result;
                case TextInputType.UnsignNumber:
                    if (String.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (strValue.IndexOf("-") >= 0)
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.Number:
                    if (String.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (!Regex.IsMatch(strValue, "^-?\\d*(\\.?\\d*)?$"))
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.Integer:
                    if (String.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (!Regex.IsMatch(strValue, "^-?\\d*$"))
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.PositiveInteger:
                    if (String.IsNullOrEmpty(strValue))
                    {
                        result = true;
                        return result;
                    }
                    else
                    {
                        if (!Regex.IsMatch(strValue, "^\\d+$"))
                        {
                            result = false;
                            return result;
                        }
                    }
                    break;
                case TextInputType.Regex:
                    result = (String.IsNullOrEmpty(strRegexPattern) || Regex.IsMatch(strValue, strRegexPattern));
                    return result;
            }
            if (strValue == "-")
            {
                return true;
            }
            Decimal d;
            if (!Decimal.TryParse(strValue, out d))
            {
                result = false;
            }
            else if (d < decMinValue || d > decMaxValue)
            {
                result = false;
            }
            else
            {
                if (inputType == TextInputType.Number || inputType == TextInputType.UnsignNumber || inputType == TextInputType.PositiveNumber)
                {
                    if (strValue.IndexOf(".") >= 0)
                    {
                        String text = strValue.Substring(strValue.IndexOf("."));
                        if (text.Length > intLength + 1)
                        {
                            result = false;
                            return result;
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        #endregion
    }

    /// <summary>
    /// 文本控件输入类型
    /// </summary>
    public enum TextInputType
    {
        /// <summary>
        /// 不控制输入
        /// </summary>
        [Description("不控制输入")]
        NotControl = 1,
        /// <summary>
        /// 任意数字
        /// </summary>
        [Description("任意数字")]
        Number = 2,
        /// <summary>
        /// 非负数
        /// </summary>
        [Description("非负数")]
        UnsignNumber = 4,
        /// <summary>
        /// 正数
        /// </summary>
        [Description("正数")]
        PositiveNumber = 8,
        /// <summary>
        /// 整数
        /// </summary>
        [Description("整数")]
        Integer = 16,
        /// <summary>
        /// 非负整数
        /// </summary>
        [Description("非负整数")]
        PositiveInteger = 32,
        /// <summary>
        /// 正则验证
        /// </summary>
        [Description("正则验证")]
        Regex = 64
    }
}
