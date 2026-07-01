using System;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.Controls.Language;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class WeakTipForm : Form
    {
        private Int32 _Duration = 5;
        private String _Path = "";
        private Int32 _Margin = 15;
        private Object _Mark = null;

        public WeakTipForm(Form mainForm = null)
        {
            LanguageManger.Instance.LanguageChanged += OnLanguageChanged;
            Owner = mainForm;
            TopMost = true;
            InitializeComponent();

            Deactivate += WeakTipForm_Deactivate;
            VisibleChanged += WeakTipForm_VisibleChanged;
        }

        private void WeakTipForm_VisibleChanged(object sender, EventArgs e)
        {
            if (_Duration < 0 && Visible == true)
            {
                Visible = false;
            }
        }

        private void WeakTipForm_Deactivate(object sender, EventArgs e)
        {
            if (Form.ActiveForm == null && Visible == false)
            {
                Owner.Activate();
            }
        }

        private void OnLanguageChanged(object sender, ILanguage e)
        { }

        public void Show(WeakTipEventArgs ea, TipMsgAttribute tma = null)
        {
            if (ea == null)
            {
                return;
            }
            if (this?.Visible == true && String.IsNullOrEmpty(ea.ContentName) && String.IsNullOrEmpty(ea.Path) && ea.Mark != null && String.IsNullOrEmpty(ea.Mark.ToString()) && ea.Duration == 0)
            {
                TmUpdate.Enabled = false;
                _Duration = 0;
                this.Visible = false;
                return;
            }

            _Path = ea.Path;
            _Mark = ea.Mark;
            if (String.IsNullOrEmpty(_Path))
            {
                BtnOpenFile.Visible = false;
            }
            else
            {
                BtnOpenFile.Visible = true;
            }
            //Load Tip.xml
            String msg = LanguageManger.Instance.GetIDMessage(ea.ContentName);
            if (ea.Args?.Length > 0)
            {
                msg = String.Format(msg, ea.Args);
            }

            if (String.IsNullOrEmpty(msg))
            {
                return;
            }

            if (_Mark != null)
            {
                msg = msg + " " + _Mark.ToString();
            }

            _Duration = ea.Duration;

            if (tma != null)
            {
                LblMsg.Font = tma.LabelFont;
                LblMsg.ForeColor = tma.ForeColor;

                PnlLeft.Width = tma.BorderThickness;
                PnlBottom.Height = tma.BorderThickness;
                PnlRight.Width = tma.BorderThickness;
                PnlTop.Height = tma.BorderThickness;

                PnlLeft.BackColor = tma.BorderColor;
                PnlBottom.BackColor = tma.BorderColor;
                PnlRight.BackColor = tma.BorderColor;
                PnlTop.BackColor = tma.BorderColor;
            }
            else
            {
                LblMsg.Font = AppStyleConfig.DefaultButtonFont;
                LblMsg.ForeColor = Color.Yellow;
            }


            //解决第一次提示框不在中心位置的问题
            Size size = GetStringSize(msg, LblMsg.Size, LblMsg.Font);
            LblMsg.Text = msg;
            //LblMsg.TextAlign = ContentAlignment.MiddleLeft;
            var contentwidth = size.Width + BtnOpenFile.Size.Width;
            var needwidth = 0;
            Point location;
            Size maxsize;
            if (Owner != null)
            {
                maxsize = new Size(Owner.Width / 2, Owner.Height / 3);

            }
            else
            {
                maxsize = new Size(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 3);
            }
            needwidth = contentwidth + _Margin * 2;
            if (needwidth > Width)
            {
                if (needwidth < maxsize.Width)
                {
                    Width = needwidth;
                }
                else
                {
                    Width = maxsize.Width;
                    Height = size.Height * 2 + 20;
                }
            }
            needwidth = needwidth > maxsize.Width ? maxsize.Width : needwidth;
            if (size.Height > Height)
            {
                if (size.Height < maxsize.Height)
                {
                    Height = size.Height;
                }
                else
                {
                    Height = maxsize.Height;
                }
            }
            this.Size = new System.Drawing.Size(needwidth, Height);
            if (Owner != null)
            {
                location = new Point(Owner.Location.X + ((Owner.Width - Width) / 2), Owner.Location.Y + (Owner.Height * 2 / 3));
            }
            else
            {
                location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, Screen.PrimaryScreen.Bounds.Height * 2 / 3);
            }
            if (!Visible && !IsDisposed)
            {
                Show();
            }

            if (ea.Duration > 0 && !TmUpdate.Enabled)
            {
                TmUpdate.Enabled = true;
            }

            Location = location;

            static Size GetStringSize(String str, Size sz, Font font)
            {
                return TextRenderer.MeasureText(str, font, sz, TextFormatFlags.NoPadding);
            }
        }

        /// <summary>
        /// 重置控件的大小
        /// </summary>
        /// <param name="msg">显示的文本信息</param>
        /// <param name="size">文件信息的大小</param>
        /// <Remark>作者：彭博 创建日期：2023/11/23 15:14:00 </Remark>
        /// <Remark>创建原因：解决提示的文本显示不完整的bug </Remark>
        private void ResetFormSize(string msg, Size size)
        {
            //判断显示的内容是否一致，若否则隐藏显示
            if (msg != LblMsg.Text)
            {
                //隐藏显示
                Hide();

                //判断文本长度度和控件的宽度,文本长度是否大于控件宽度
                if (size.Width > LblMsg.Size.Width)
                {
                    //为控件重新赋值
                    LblMsg.Text = msg;
                    //自动扩展长度
                    LblMsg.AutoSize = true;
                    //关闭自动扩展
                    LblMsg.AutoSize = false;
                }

                //当本次显示内容小于610，并且小于上一次显示的长度时，重新将控件绘制到最初大小
                if (size.Width <= 610 && size.Width < LblMsg.Size.Width)
                {
                    //重新将控件绘制到最初大小
                    //LblMsg.Size = new System.Drawing.Size(610, 46);
                }
            }
        }

        private void TmUpdate_Tick(object sender, EventArgs e)
        {
            _Duration--;
            if (_Duration < 0)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(HideTip));
                }
                else
                {
                    HideTip();
                }
            }

            void HideTip()
            {
                Visible = false;
                TmUpdate.Enabled = false;
            }
        }

        private void WeakTipForm_Load(object sender, EventArgs e)
        {
            this.BackColor = AppStyleConfig.DefaultContextBackColor;
            LblMsg.BackColor = Color.Transparent;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
        }

        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            String filename = ExplorerExtension.GetLatestFile(_Path, "");
            if (String.IsNullOrEmpty(filename))
            {
                ExplorerExtension.ExplorerDic(_Path);
            }
            else
            {
                ExplorerExtension.ExplorerFile(filename);
            }
            this.Visible = false;
        }
    }

    public class TipMsgAttribute
    {
        public TipMsgAttribute()
        {
            LabelFont = new Font(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiRuanYaHei"), 9);
            ForeColor = Color.FromArgb(232, 234, 237);
            BorderThickness = 1;
            BorderColor = Color.Gray;
        }
        /// <summary>
        /// 字体设置
        /// </summary>
        public Font LabelFont { get; set; }
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color ForeColor { get; set; }
        /// <summary>
        /// 边框大小
        /// </summary>
        public int BorderThickness { get; set; }
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color BorderColor { get; set; }
    }
}
