using Svg;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Language;
using ScopeX.U2.BaseControl;
using System.Linq;

namespace ScopeX.U2
{

    /// <summary>
    /// 初始化选择(聚焦)按钮的类型
    /// </summary>
    public enum BtnChooseType
    {
        /// <summary>
        /// 确定按钮
        /// </summary>
        Yes,

        /// <summary>
        /// 取消按钮
        /// </summary>
        Cancel,
    }

    /// <summary>
    /// 消息窗体
    /// </summary>
    public partial class MsgTipForm :FlashBorderForm //BorderColorForm
    {
        #region Field&Propert

        //字段

        private readonly MessageType _MsgType = MessageType.Error;           //消息类型（默认为错误消息）
        private readonly BtnChooseType _OriginBtnChooseType;                 //初始选择按钮类型
        private readonly StrongTipEventArgs _MsgData = null;

        //属性

        //消息图标的Y轴坐标
        private int MsgIconY => ((Height - HeadHeight - _msgIconHeight) / 3) + HeadHeight;

        /// <summary>
        /// 消息图标的颜色
        /// </summary>
        public Color MsgIconColor { get; set; }

        /// <summary>
        /// 窗体边界，图标，文字的间隔
        /// </summary>
        public int IconTextInterval { get; set; } = 30;

        private int _msgIconHeight = 40;
        /// <summary>
        /// 图标的高度：为了保证图标的长宽比只能设置高度，且高度不能大于内容区高度；大于的话设置失败；
        /// </summary>
        public int MsgIconHeight
        {
            get => _msgIconHeight;
            set
            {
                if (value >= 0 && value <= (this.Height - this.HeadHeight))
                {
                    _msgIconHeight = value;
                }
            }
        }

        private string _Message = string.Empty;
        /// <summary>
        /// 消息的内容
        /// </summary>
        public string Message
        {
            get => _Message;
            set
            {
                _Message = value;
                SetLabelMessage();
            }
        }

        private int MaxWindowWidth = 900;

        #endregion Field&Propert

        [Bindable(true), Category(Const.Category), Description("关闭点击事件"), DefaultValue(typeof(EventHandler))]
        public event EventHandler CloseClick;

        [Bindable(true), Category(Const.Category), Description("点击确认按钮事件"), DefaultValue(typeof(EventHandler))]
        public event EventHandler YesClick;

        [Bindable(true), Category(Const.Category), Description("点击取消按钮事件"), DefaultValue(typeof(EventHandler))]
        public event EventHandler CancelClick;

        public MsgTipForm()
        {
            InitializeComponent();
            IsShowHelp = false;
            //外观设置
            MsgIconColor = Color.Red;
            FixedToolIconInfos[1].IsShow = false;
            base._canDragControls.Add(this);

            //事件处理函数
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            base.CloseClick += RightPb_Click;
            BtnYes.GotFocus += (o, e) =>
            {
                SetLabelMessage();
            };
            BtnCancel.GotFocus += (o, e) =>
            {
                SetLabelMessage();
            };
        }

        /// <summary>
        /// 指定消息类型的构造函数
        /// </summary>
        /// <param name="messageType"></param>
        public MsgTipForm(StrongTipEventArgs msgData) : this()
        {
            if (msgData == null)
            {
                return;
            }
            _MsgType = msgData.MsgType;
            MsgIconColor = _MsgType switch
            {
                MessageType.Information => Color.LightBlue,
                MessageType.Error => Color.Red,
                MessageType.Warning => Color.Yellow,
                //MessageType.Asking
                _ => Color.White,
            };
            _MsgData = msgData;

            if (_MsgData != null)
            {
                this.Title = LanguageManger.Instance.GetIDMessage(_MsgData.MsgTitleName);
                if (_MsgData.Args != null && _MsgData.Args.Length > 0)
                {
                    this.Message = string.Format(LanguageManger.Instance.GetIDMessage(_MsgData.MsgContentName), _MsgData.Args);
                }
                else
                {
                    this.Message = LanguageManger.Instance.GetIDMessage(_MsgData.MsgContentName);
                }
                if (_MsgData.MsgTitleName.Contains("MsgTipId.PowerAnalysis"))
                {
                    this.BtnYes.Text = LanguageManger.Instance.GetIDMessage("XiaYiBu");
                    this.BtnCancel.Text = LanguageManger.Instance.GetIDMessage("QuXiao");
                }
                var wordsize = TextRenderer.MeasureText(this.Message, LbMsg.Font);

                var lableminwidth = wordsize.Width / 2; // 只显示两行
                //lableminwidth = Math.Clamp(lableminwidth, lableminwidth, MaxWindowWidth);
                if (lableminwidth >= MaxWindowWidth)
                    lableminwidth = MaxWindowWidth;

                var ddwidth = Width - 150; // 更改成150，150为图标加间距的宽度
                if (ddwidth < lableminwidth)
                {
                    // 由于英文状态下如果有换行，会导致需要更多的宽度，故而这里默认加一百
                    Width += lableminwidth - ddwidth + 100;
                }
            }
        }

        /// <summary>
        /// 指定消息类型和选择按钮的类型的构造函数
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="btnChooseType"></param>
        public MsgTipForm(StrongTipEventArgs msgData, BtnChooseType btnChooseType) : this(msgData)
        {
            _OriginBtnChooseType = btnChooseType;
        }

        #region protected&private

        //事件处理函数

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //风格统一修改
            Stylize();

            #region//开启设置默认按钮聚焦的任务
            //Task.Run(async delegate
            //{
            //    await Task.Delay(100);
            //    switch (_OriginBtnChooseType)
            //    {
            //        case BtnChooseType.Yes:
            //            this.Invoke(new Action(() => BtnYes.Focus()));
            //            break;
            //        case BtnChooseType.Cancel:
            //            this.Invoke(new Action(() => BtnCancel.Focus()));
            //            break;
            //        default:
            //            throw new ArgumentException("chooseType");
            //    }
            //});
            #endregion
        }

        private void Stylize()
        {
            IsShowHelp = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
            this.ContentBackColor = AppStyleConfig.DefaultContextBackColor;
            this.HeadBackColor = Color.FromArgb(50, 55, 65);
            this.TitleColor = Color.White;
            BtnYes.MouseinForeColor = Color.Black;
            BtnYes.ForeColor = Color.Black;
            BtnYes.BackColor = AppStyleConfig.DefaultCheckedBackColor;
            BtnYes.MouseinBackColor = AppStyleConfig.DefaultCheckedBackColor;
            BtnCancel.MouseinForeColor = Color.Black;
            BtnCancel.MouseinBackColor = AppStyleConfig.DefaultCheckedBackColor;
            BtnCancel.BackColor = AppStyleConfig.DefaultTitleBackColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //画消息图标
            DrawSvgIcon(GetMsgIconSvgPath(_MsgType), e.Graphics);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    BtnYes_Click(sender, e);
                    break;
                case Keys.Escape:
                    BtnCancel_Click(sender, e);
                    break;
                default:
                    break;
            }
        }

        private void RightPb_Click(object sender, EventArgs e)
        {
            CloseClick?.Invoke(sender, e);
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("BtnYes_Click");
            YesClick?.Invoke(sender, e);
            this.DialogResult = DialogResult.Yes;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("BtnCancel_Click");
            CancelClick?.Invoke(sender, e);
            this.DialogResult = DialogResult.Cancel;
        }


        //其他

        private void SetLbMsgRect(Rectangle iconRect)
        {
            Double scale = 1.5; //LabelRect的高度位IconRect高度的倍数
            int locationy = Math.Max((int)(iconRect.Y - (iconRect.Height * (scale - 1) / 2)), this.HeadHeight);
            int sizeh = Math.Min((int)(iconRect.Height * scale), BtnYes.Location.Y - locationy);

            LbMsg.Location = new Point(iconRect.X + iconRect.Width + IconTextInterval, locationy);
            LbMsg.Size = new Size(Width - LbMsg.Location.X - IconTextInterval, sizeh);
        }

        private string GetMsgIconSvgPath(MessageType msgType)
        {
            return msgType switch
            {
                MessageType.Information => Properties.Resources.TipMsgMarkMessageSvg,
                MessageType.Error => Properties.Resources.TipMsgErrorMessageSvg,
                MessageType.Warning => Properties.Resources.TipMsgWarningMessageSvg,
                MessageType.Asking => Properties.Resources.TipMsgAskMessageSvg,
                _ => throw new ArgumentException("msgType"),
            };
        }

        private void DrawSvgIcon(string svgString, Graphics g)
        {
            try
            {
                //初始化Svg路径和样式
                SvgPath svgpath = new SvgPath()
                {
                    PathData = SvgPathBuilder.Parse(svgString),
                    Fill = new SvgColourServer(MsgIconColor),
                };

                //画图
                int iconwidth = (int)(_msgIconHeight * svgpath.GetPathRectWHRatio());
                Rectangle iconrect = new Rectangle(IconTextInterval, MsgIconY, iconwidth, _msgIconHeight);
                svgpath.DrawInGraphRatio(g, iconrect);

                //设置LbMsg的位置和大小
                SetLbMsgRect(iconrect);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.StackTrace, exp.Message);
            }
        }

        private void SetLabelMessage()
        {
            LbMsg.Text = _Message;
        }

        #endregion protected&private

        private void BtnCancel_MouseMove(object sender, MouseEventArgs e)
        {
            BtnYes.BackColor = AppStyleConfig.DefaultTitleBackColor;
            BtnYes.ForeColor = AppStyleConfig.DefaultContextForeColor;
        }
    }
}
