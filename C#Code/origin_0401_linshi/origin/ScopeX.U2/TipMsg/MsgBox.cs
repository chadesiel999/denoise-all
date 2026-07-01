using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Controls.Language;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using Svg;
using Timer = System.Timers.Timer;

namespace ScopeX.U2
{
    /// <summary>
    /// 消息窗体
    /// </summary>
    public partial class MsgBox : BorderColorForm
    {
        #region Field&Propert

        //字段

        private readonly MessageType _MsgType = MessageType.Error;           //消息类型（默认为错误消息）
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

        /// <summary>
        /// 超时
        /// </summary>
        private Int32 _TimeOut;

        /// <summary>
        /// 计数器
        /// </summary>
        private Int32 _Counter = 0;

        /// <summary>
        /// Automatically update window position
        /// </summary>
        public Boolean IsAutoMove { get; set; } = false;

        private Timer _MoveTimer = new Timer()
        {
            Enabled = false,
            Interval = 3000,
        };

        private Timer _Timer = new Timer()
        {
            Enabled = false,
            Interval = 1000,
        };

        private String _BtnYesText = "";

        #endregion Field&Propert

        public MsgBox(Int32 timeout = 10)
        {
            InitializeComponent();
            _BtnYesText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QueDing");
            //外观设置
            MsgIconColor = Color.Red;
            FixedToolIconInfos[1].IsShow = false;
            FixedToolIconInfos[0].IsShow = false;
            base._canDragControls.Add(this);

            //事件处理函数
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            BtnYes.GotFocus += (o, e) =>
            {
                SetLabelMessage();
            };
            if (timeout >= 3)
            {
                _TimeOut = (Int32)timeout;
                _Timer.Enabled = true;
                _Timer.Start();
                _Timer.Elapsed -= Timer_Tick;
                _Timer.Elapsed += Timer_Tick;
            }
        }

        private Random _random = new Random();
        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }
            void TickEvent()
            {
                if (Owner != null)
                {
                    var x = _random.Next(0, Owner.Width - Width);
                    var y = _random.Next(0, Owner.Height - Height);

                    // 更新窗口位置
                    Location = new Point(x, y);
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }
            void TickEvent()
            {
                if (_Counter <= _TimeOut)
                {

                    BtnYes.Text = $"{_BtnYesText}（{_TimeOut - _Counter}）";
                    _Counter++;
                }
                else
                {
                    _Timer.Stop();
                    _Timer.Enabled = false;
                    this.DialogResult = DialogResult.Yes;
                }
            }
        }

        public MsgBox(MessageType type, Int32 timeout = 10)
        {
            InitializeComponent();
            _BtnYesText = LanguageManger.Instance.GetIDMessage("QueDing");
            _MsgType = type;
            MsgIconColor = _MsgType switch
            {
                MessageType.Information => Color.LightBlue,
                MessageType.Error => Color.Red,
                MessageType.Warning => Color.Yellow,
                //MessageType.Asking
                _ => Color.White,
            };
            FixedToolIconInfos[1].IsShow = false;
            FixedToolIconInfos[0].IsShow = false;
            base._canDragControls.Add(this);

            //事件处理函数
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            BtnYes.GotFocus += (o, e) =>
            {
                SetLabelMessage();
            };
            if (timeout >= 3)
            {
                _TimeOut = (Int32)timeout;
                _Timer.Enabled = true;
                _Timer.Start();
                _Timer.Elapsed -= Timer_Tick;
                _Timer.Elapsed += Timer_Tick;
            }
        }

        /// <summary>
        /// 指定消息类型的构造函数
        /// </summary>
        /// <param name="messageType"></param>
        public MsgBox(StrongTipEventArgs msgData, Int32 timeout = 10) : this()
        {
            _BtnYesText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QueDing");

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
            }

            if (_MsgType == MessageType.Error && timeout >= 3)
            {
                _TimeOut = (Int32)timeout;
                _Timer.Enabled = true;
                _Timer.Start();
                _Timer.Elapsed -= Timer_Tick;
                _Timer.Elapsed += Timer_Tick;
            }

        }
        #region protected&private

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            _Timer.Stop();
            _Timer.Enabled = false;
        }

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
            //风格统一修改
            Stylize();

            base.OnLoad(e);

            //开启设置默认按钮聚焦的任务
            Task.Run(async delegate
            {
                await Task.Delay(100);
                if (this.Created)
                    this.Invoke(new Action(() => BtnYes.Focus()));
            });

            if (IsAutoMove)
            {
                _MoveTimer.Elapsed += MoveTimer_Tick;
                _MoveTimer.Enabled = true;
            }
            else
            {
                _MoveTimer.Elapsed -= MoveTimer_Tick;
                _MoveTimer.Enabled = false;
            }
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this);
            this.ContentBackColor = AppStyleConfig.DefaultContextBackColor;
            this.HeadBackColor = Color.FromArgb(50, 55, 65); ;
            this.TitleColor = Color.White;
            BtnYes.MouseinForeColor = Color.Black;
            BtnYes.MouseinBackColor = AppStyleConfig.DefaultCheckedBackColor;
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
                default:
                    break;
            }
        }



        private void BtnYes_Click(object sender, EventArgs e)
        {
            _Timer.Stop();
            _Timer.Enabled = false;
            this.DialogResult = DialogResult.Yes;
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
    }
}
