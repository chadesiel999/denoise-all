using ScopeX.Controls.Language;
using ScopeX.Core.Tools.Tips;
using ScopeX.U2.TipMsg;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScopeX.U2
{
    /// <summary>
    /// 可控制的弱提示框
    /// </summary>
    public partial class ControlableWeakTipForm : Form
    {
        private String _Path = "";
        private string _contentName;
        private Guid _id;

        public ControlableWeakTipForm(Form mainForm = null)
        {
            LanguageManger.Instance.LanguageChanged += OnLanguageChanged;
            Owner = mainForm;
            TopMost = true;
            InitializeComponent();
            Deactivate += WeakTipForm_Deactivate;
            this.FormClosing += ControlableWeakTipForm_FormClosing;
        }

        private void ControlableWeakTipForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ControlableWeakTipCacher.RemoveForm(_id);
        }

        private void WeakTipForm_Deactivate(object sender, EventArgs e)
        {
            if (Form.ActiveForm == null && Visible == false)
            {
                Owner.Activate();
            }
        }

        private void OnLanguageChanged(object sender, ILanguage e)
        {
            UpdateContent(_contentName);
        }

        /// <summary>
        /// 更新显示内容
        /// </summary>
        /// <param name="newContentName"></param>
        public void UpdateContent(string newContentName)
        {
            String msg = LanguageManger.Instance.GetIDMessage(newContentName);
            if (String.IsNullOrEmpty(msg) || msg == newContentName)
                return;

            msgSize = GetStringSize(msg, LblMsg.Size, LblMsg.Font);
            ResetFormSize(msg, msgSize);
            LblMsg.Text = msg;
        }

        public void Show(ControlableWeakTipEventArgs ea, TipMsgAttribute tma = null)
        {
            if (ea == null || ea.type != ControlableWeakTipEventControlType.Create)
                return;

            String msg = LanguageManger.Instance.GetIDMessage(ea.ContentName);
            if (ea.Args?.Length > 0)
                msg = String.Format(msg, ea.Args);

            if (String.IsNullOrEmpty(msg))
                return;

            _id = ea.TipId;
            _Path = ea.Path;
            if (String.IsNullOrEmpty(_Path))
            {
                BtnOpenFile.Visible = false;
            }
            else
            {
                BtnOpenFile.Visible = true;
            }
            _contentName = ea.ContentName;


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

            msgSize = GetStringSize(msg, LblMsg.Size, LblMsg.Font);
            ResetFormSize(msg, msgSize);
            LblMsg.Text = msg;

            /*Point location;
            Size maxsize;
            if (Owner != null)
            {
                location = new Point(Owner.Location.X + ((Owner.Width - Width) / 2), Owner.Location.Y + (Owner.Height * 2 / 3));
                maxsize = new Size(Owner.Width / 3, Owner.Height / 3);
            }
            else
            {
                location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, Screen.PrimaryScreen.Bounds.Height * 2 / 3);
                maxsize = new Size(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3);
            }

            if (msgSize.Width > Width)
            {
                if (msgSize.Width < maxsize.Width)
                {
                    Width = msgSize.Width;
                }
                else
                {
                    Width = maxsize.Width;
                }
            }
            if (msgSize.Height > Height)
            {
                if (msgSize.Height < maxsize.Height)
                {
                    Height = msgSize.Height;
                }
                else
                {
                    Height = maxsize.Height;
                }
            }*/

            UpdateFormLocationAndSize();

            if (!Visible && !IsDisposed)
            {
                Show();
            }

            //Location = location;
        }

        private Size msgSize;

        /// <summary>
        /// 更新窗体位置
        /// </summary>
        private void UpdateFormLocationAndSize()
        {
            Point location;
            int? minlocation = null;
            Size maxsize;
            var forms = ControlableWeakTipCacher.AllCachedForm.Value.Select(c => c.Value);
            if (forms.Any())
                minlocation = forms.Min(c => c.Location.Y);

            if (Owner != null)
            {
                location = new Point(Owner.Location.X + ((Owner.Width - Width) / 2), Owner.Location.Y + (Owner.Height * 2 / 3));
                maxsize = new Size(Owner.Width / 3, Owner.Height / 3);
            }
            else
            {
                location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, Screen.PrimaryScreen.Bounds.Height * 2 / 3);
                maxsize = new Size(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3);
            }

            if (msgSize.Width > Width)
            {
                if (msgSize.Width < maxsize.Width)
                {
                    Width = msgSize.Width;
                }
                else
                {
                    Width = maxsize.Width;
                }
            }
            if (msgSize.Height > Height)
            {
                if (msgSize.Height < maxsize.Height)
                {
                    Height = msgSize.Height;
                }
                else
                {
                    Height = maxsize.Height;
                }
            }

            if (minlocation != null)
                location.Y = minlocation.Value - Height;
            Location = location;
        }

        private Size GetStringSize(String str, Size sz, Font font)
        {
            return TextRenderer.MeasureText(str, font, sz, TextFormatFlags.NoPadding);
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
                    LblMsg.Size = new System.Drawing.Size(610, 46);
                }
            }
        }

        private void WeakTipForm_Load(object sender, EventArgs e)
        {
            this.BackColor = AppStyleConfig.DefaultContextBackColor;
            LblMsg.BackColor = Color.Transparent;
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
        }
    }
}
