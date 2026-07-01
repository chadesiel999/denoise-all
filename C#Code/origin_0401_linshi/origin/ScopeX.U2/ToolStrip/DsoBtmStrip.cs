using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.UserControls;
using ScopeX.U2.Tools;
using System.Collections.Generic;
using ScopeX.U2.AWG;
using System.Drawing.Drawing2D;

namespace ScopeX.U2
{
    public partial class DsoBtmStrip : UserControl
    {
        public DsoBtmStrip()
        {
            InitializeComponent();
            ProductAdaptation();
            InitLangControl();
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        /// <summary>
        /// 产品适配
        /// </summary>
        private void ProductAdaptation()
        {
            if (!PlatformUIManager.Default.Platform.Attribute.MutiAwg)
            {
                TlpLv1.Controls.Remove(BtnAwg2);
                TlpLv1.SetRowSpan(BtnAwg1, 2);
            }
        }

        private void InitLangControl()
        {
            BtnDigital.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LuoJi_");
            BtnMath.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShuXue_");
            BtnReference.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanKao_");
            BtnBus.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongXian_");
            BtnRadio.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShePin_");
        }

        private void SmallBtnDigitalFontSize()
        {
            if (LanguageFactory.Current == Language.English)
            {
                BtnDigital.Font = new Font("MiSans", 12f);
            }
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            SmallBtnDigitalFontSize();
        }

        [Bindable(true), Editor(), Browsable(true), DefaultValue(16), Category("CatAppearance"), Description("The maximum number of channels")]
        public Int32 MaxCount
        {
            get;
            set;
        } = 16;

        public void AddBadge(IBadgeView info)
        {
            if (GetBadge(info.Presenter.Id) is null)
            {
                ChnlInfoPanel.AddBadge(info);

                var c = info as Control;
                c.TabIndex = (Int32)info.Presenter.Id + 10;
                c.MouseDown += Badge_MouseDown;
                c.MouseUp += Badge_MouseUp;
                c.MouseMove += Badge_MouseMove;

                //!!!Get focus on appending control
                c.Focus();

                OnControlAdded(new ControlEventArgs(c));
                ChnlInfoPanel.SrcollToShowCurrent();
            }
        }

        public void AddBadgeEx(IBadgeView info)
        {
            ChnlInfoPanel.AddBadge(info);

            var c = info as Control;
            //c.TabIndex = (Int32)info.Presenter.Id + 10;
            c.MouseDown += Badge_MouseDown;
            c.MouseUp += Badge_MouseUp;
            c.MouseMove += Badge_MouseMove;

            //!!!Get focus on appending control
            c.Focus();

            OnControlAdded(new ControlEventArgs(c));
        }
        //public void RemoveBadge(IBadgeView info)
        //{
        //    ChnlInfoPanel.RemoveBadge(info);

        //    OnControlRemoved(new ControlEventArgs(info as Control));
        //}

        public void RemoveBadge(ChannelId id)
        {
            ChnlInfoPanel.RemoveBadge(id);

            OnControlRemoved(new ControlEventArgs(GetBadge(id) as Control));
        }

        public IBadgeView GetBadge(ChannelId id)
        {
            return ChnlInfoPanel.GetBadge(id);
        }

        internal IEnumerable<IBadgeView> GetBadgeControls()
        {
            var subconrols = ChnlInfoPanel.GetBadgeControls();
            if (subconrols == null)
                yield return null;

            foreach (Control item in subconrols)
            {
                if (item is IBadgeView ibv)
                    yield return ibv;
            }
        }

        /// <summary>
        /// 显示参数窗口
        /// </summary>
        /// <param name="id">当前选中的通道编号</param>
        /// <Remark>更改人：彭博 创建日期：2024/2/27 9:56:00  原因：更新FocusId时，显示参数窗口 </Remark>
        public void ShowForm(ChannelId id)
        {
            var Badge = ChnlInfoPanel.GetBadge(id);
            if (Badge is AWGInfo)
            {
                (Badge as AWGInfo).ShowForm();
            }
            else if (Badge is DecodeInfo)
            {
                (Badge as DecodeInfo).ShowForm();
            }
            else if (Badge is BadgeInfo)
            {
                (Badge as ChnlBadgeInfo).ShowForm();
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            SmallBtnDigitalFontSize();
        }
        private void Stylize()
        {
            BtnMath.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BtnReference.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BtnDigital.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BtnBus.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BtnAwg1.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BtnAwg2.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            BtnRadio.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
        }
        private void BtnDigital_Click(Object sender, EventArgs e)
        {
            if (OptionManager.Default.Checked(OptionType.LA) == false)
                return;
            if (!Constants.ENABLE_LA)
            {
                WeakTip.Default.Write("Digital", MsgTipId.FunctionDisabled);
                return;
            }
            if (!(ParentForm as DsoForm).TryAddDigiWaveform())
            {
                WeakTip.Default.Write("Digital", MsgTipId.NoMoreChannels);
            }
        }

        private void BtnMath_Click(Object sender, EventArgs e)
        {
            if (!Constants.ENABLE_Math)
            {
                WeakTip.Default.Write("Math", MsgTipId.FunctionDisabled);
                return;
            }

            //if (!DsoPrsnt.DefaultDsoPrsnt.CheckLAMutex(true))
            //    return;

            if ((ParentForm as DsoForm).TryAddMathWaveform() is null)
            {
                WeakTip.Default.Write("Math", MsgTipId.NoMoreChannels);
            }
        }

        private void BtnReference_Click(Object sender, EventArgs e)
        {
            if (!Constants.ENABLE_Ref)
            {
                WeakTip.Default.Write("Ref", MsgTipId.FunctionDisabled);
                return;
            }

            if (!(ParentForm as DsoForm).TryAddRefWaveform())
            {
                WeakTip.Default.Write("Ref", MsgTipId.NoMoreRefChannels);
            }
        }

        private void BtnBus_Click(Object sender, EventArgs e)
        {
            if (!Constants.ENABLE_BUS)
            {
                WeakTip.Default.Write("Decode", MsgTipId.FunctionDisabled);
                return;
            }

            if (!(ParentForm as DsoForm).TryAddDecodeWaveform())
            {
                WeakTip.Default.Write("Decode", MsgTipId.NoMoreChannels);
            }
        }

        private void BtnAwg1_Click(Object sender, EventArgs e)
        {
            if (OptionManager.Default.Checked(OptionType.AWG) == false)
                return;
            if (!Constants.ENABLE_AWG)
            {
                WeakTip.Default.Write("AWG", MsgTipId.FunctionDisabled);
                return;
            }
            if ((ParentForm as DsoForm).TryAddAwgInfo(ChannelId.AWG1))
            {
                return;
            }

            WeakTip.Default.Write("AWG", MsgTipId.NoMoreChannels);
        }

        private void BtnAwg2_Click(Object sender, EventArgs e)
        {
            if (OptionManager.Default.Checked(OptionType.AWG) == false)
                return;
            if (!Constants.ENABLE_AWG)
            {
                WeakTip.Default.Write("AWG", MsgTipId.FunctionDisabled);
                return;
            }
            for (ChannelId id = ChannelId.AWG2; id <= ChannelIdExt.MaxAwgId; id++)
            {
                if ((ParentForm as DsoForm).TryAddAwgInfo(id))
                {
                    return;
                }
            }
            WeakTip.Default.Write("AWG", MsgTipId.NoMoreChannels);
        }

        private void BtnRF_Click(Object sender, EventArgs e)
        {
            //if ((ParentForm as DsoForm).TryAddRadioInfo(ChannelId.RF))
            //{
            //    return;
            //}

            //WeakTip.Default.Write("RF", MsgTipId.NoMoreChannels);

            if (!(ParentForm as DsoForm).TryAddRFWaveform())
            {
                WeakTip.Default.Write("RF", MsgTipId.NoMoreChannels);
            }
        }

        #region ToolTip
        //public void SetToolTip(ToolTip toolTip)
        //{
        //    toolTip.SetToolTip(BtnDigital, Properties.ToolTips.NewDigital);
        //    toolTip.SetToolTip(BtnMath, Properties.ToolTips.NewMath);
        //    toolTip.SetToolTip(BtnReference, Properties.ToolTips.NewRef);
        //    toolTip.SetToolTip(BtnBus, Properties.ToolTips.NewBus);
        //    toolTip.SetToolTip(BtnAwg1, Properties.ToolTips.OpenAwg1);
        //    toolTip.SetToolTip(BtnAwg2, Properties.ToolTips.OpenAwg2);
        //}
        #endregion

        #region DragDrop
        //private IBadgeView _DeletedBadge;

        //private Rectangle _DragBox;

        //private Int64 _DragDropStamp;


        private Point _StartPosition;
        private Boolean _IsMouseDown;
        private void Badge_MouseDown(Object sender, MouseEventArgs e)
        {
            //if (sender is IBadgeView)
            //{
            //    //_DragBox = (sender as Control).ClientRectangle;

            //    _DragDropStamp = DateTime.Now.Ticks + (5_00 * 10_000);
            //}
            if (e.Button == MouseButtons.Left)
            {
                _StartPosition = e.Location;
                _IsMouseDown = true;
            }
        }

        private void Badge_MouseUp(Object sender, MouseEventArgs e)
        {
            //_DragBox = Rectangle.Empty;
            if (e.Button == MouseButtons.Left)
            {
                _StartPosition = e.Location;
                _IsMouseDown = false;
            }
        }

        private void Badge_MouseMove(Object sender, MouseEventArgs e)
        {
            //if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            //{
            //    if (_DragDropStamp < DateTime.Now.Ticks && e.Y < (sender as Control).Top - 10)
            //    {
            //        _DeletedBadge = sender as IBadgeView;
            //        if (_DeletedBadge?.Presenter.Id.IsAnalog() == false)
            //        {
            //            DoDragDrop(new DataObject(nameof(IBadgeView), _DeletedBadge), DragDropEffects.Move);
            //        }
            //    }
            //}

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && _IsMouseDown)
            {
                var ctl = sender as IBadgeView;
                var offsety = e.Location.Y - _StartPosition.Y;
                var offsetx = e.Location.X - _StartPosition.X;
                if (offsety > 0 && Math.Abs(offsetx) <= 10)
                {
                    ctl.Presenter.Active = false;

                    if (ctl.Presenter.Id.IsAWG())
                    {
                        (Program.Oscilloscope.View as DsoForm).RemoveWaveformUI(ctl.Presenter);
                        ((ArbWfmGenPrsnt)ctl.Presenter).TryRemoveView((IWfmGenView)ctl);
                    }
                }
                _IsMouseDown = false;
            }
        }
        #endregion


        private void Btn_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            Color colorUp = AppStyleConfig.DefaultGradualBackColorOne;
            Color colorDown = AppStyleConfig.DefaultGradualBackColorTwo;
            LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, colorUp, colorDown, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, btn.ClientRectangle);
            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            if (btn.Tag == "1")
            {
                //btn.BackColor = Color.FromArgb(72, 77, 85);
                ControlPaint.DrawBorder(
                  e.Graphics,
                  btn.ClientRectangle,
                  Color.FromArgb(0, 191, 255),      // 左侧线颜色
                  2,               // 左侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 右侧线颜色
                  2,               // 右侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 上侧线颜色
                  2,               // 上侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 下侧线颜色
                  2,               // 下侧线宽度
                  ButtonBorderStyle.Solid);

            }


        }

        private void btnMouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;

            btn.Padding = new Padding(btn.Padding.Left + 1,
                                           btn.Padding.Top + 1,
                                           btn.Padding.Right + 1,
                                           btn.Padding.Bottom + 1);
            //btn.BackColor = Color.Gray;
            btn.Tag = "1";
            Invalidate();

        }

        private void btnMouseUp(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;

            button.Padding = new Padding(button.Padding.Left - 1,
                                           button.Padding.Top - 1,
                                           button.Padding.Right - 1,
                                           button.Padding.Bottom - 1);
            //button.BackColor = Color.White;
            //button.BackColor = Color.FromArgb(72, 77, 85);
            button.Tag = "0";
            Invalidate();

        }
    }
}
