// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Core;
    using ScopeX.UserControls.Style;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using static ScopeX.UserControls.SelectComboBox;


    /// <summary>
    /// Defines the <see cref="ColorConfigPage" />.
    /// </summary>
    public partial class OtherSettingPage : UserControl, IStylize, IDisplayView
    {
        private Boolean _ArgToCtrl = false;
        public OtherSettingPage()
        {
            InitializeComponent();
            LblTouch.Visible = ChkTouch.Visible = PlatformUIManager.Default.Platform.Attribute.Touchable;
            LblTempCheck.Visible = BtnShowTemperature.Visible = AppConfig.GetIntance().TEMPERATURE_MONITOR_ENABLE;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        public DisplayPrsnt DispPresenter
        {
            get;
            set;
        }

        IDisplayPrsnt IView<IDisplayPrsnt>.Presenter
        {
            get => DispPresenter;
            set => DispPresenter = (DisplayPrsnt)value;
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(DispPresenter.TouchLock):
                    ChkTouch.Checked = DispPresenter.TouchLock;
                    break;
                case nameof(DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure):
                    ChkStopMeasure.Checked = DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure;
                    break;
                case nameof(DispPresenter.SystemTime):
                    DbxDateTime.CurrentTime = DateTime.Now;
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkTouch.Checked = DispPresenter.TouchLock;
                ChkStopMeasure.Checked = DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure;
                _ArgToCtrl = false;
            }
        }

        /// <summary>
        /// The OnLoad.
        /// </summary>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DbxDateTime.CurrentTime = DateTime.Now;
            STBSysRunTime.Text = Program.Oscilloscope.SysRunTimeMangager.SystemRunTime;
            var datasource = PlatformUIManager.Default.Platform.GetEditableColorsChannel();
            InitOnLoad(datasource);

            CbxLanguage.DataSource = Enum.GetValues<Language>().Select(w => new ComboBoxItem(w.ToString(), w)).ToList(); ;
            CbxLanguage.SelectValue = Program.Oscilloscope.SysLanguage;
            CbxLanguage.SelectedIndexChanged += (_, _) =>
            {
                Program.Oscilloscope.SysLanguage = (Language)CbxLanguage.SelectValue;

                //var l = (Language)CbxLanguage.SelectValue;
                //ScopeX.Controls.Language.LanguageManger.Instance.Language = LanguageFactory.GetLanguage(l);
                //HelpProcessManager.SendCommand((int)CbxLanguage.SelectValue);
                //System.Threading.Thread.Sleep(500);//等待500ms 重新加载Xml
                //HelpDocumentManager.Default.LoadDocumentInfo();
                //var test = ChkTouch.Text;

                //// 保存多语言值
                //var appconfig = AppConfig.GetIntance();
                //appconfig.LANGUAGEID = (int)l;
                //appconfig.SaveConfig();
            };
            //TouchController.TouchChanged -= TouchController_TouchChanged;
            //TouchController.TouchChanged += TouchController_TouchChanged;
            ChkTouch.CheckedChangedEvent += ChkTouch_CheckedChangedEvent;
            ChkStopMeasure.CheckedChangedEvent += ChkStopMeasure_CheckedChangedEvent;
            Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged -= SysRunTimeMangager_RunTimeChanged;
            Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged += SysRunTimeMangager_RunTimeChanged;
            UpdateView();
        }


        //private void TouchController_TouchChanged(bool obj)
        //{
        //    _ArgToCtrl = true;
        //    ChkTouch.Checked = !obj;
        //    _ArgToCtrl = false;
        //}

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            ChkTouch.CheckedChangedEvent -= ChkTouch_CheckedChangedEvent;
            ChkStopMeasure.CheckedChangedEvent -= ChkStopMeasure_CheckedChangedEvent;
            Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged -= SysRunTimeMangager_RunTimeChanged;
            //TouchController.TouchChanged -= TouchController_TouchChanged;
        }

        /// <summary>
        /// 使用事件时，除非非常确认不存在重复注册问题，否则请勿使用匿名函数注册事件。且记得在不需要时，反注册掉事件。
        /// Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged是一个全局事件，不需要时，一定记得反注册掉。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysRunTimeMangager_RunTimeChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
            {
                Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged -= SysRunTimeMangager_RunTimeChanged;
                return;
            }
            if (this.InvokeRequired)
            {
                if (!this.IsDisposed && !this.Disposing && !STBSysRunTime.IsDisposed && !STBSysRunTime.Disposing)
                {
                    //暂时添加异常捕获，解决试产问题
                    try
                    {
                        // 如果不在 UI 线程上，则使用 Invoke 方法执行代码
                        this.Invoke(new Action(() => STBSysRunTime.Text = Program.Oscilloscope.SysRunTimeMangager.SystemRunTime));
                    }
                    catch (ObjectDisposedException)
                    {
                        Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged -= SysRunTimeMangager_RunTimeChanged;
                        return;
                    }
                    catch (Exception ex)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                    }
                }
                else
                {
                    Program.Oscilloscope.SysRunTimeMangager.RunTimeChanged -= SysRunTimeMangager_RunTimeChanged;
                    return;
                }
            }
            else
            {
                // 在 UI 线程上执行更新操作
                if (this.IsDisposed == false)
                {
                    STBSysRunTime.Text = Program.Oscilloscope.SysRunTimeMangager.SystemRunTime;
                }
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            DbxDateTime.CurrentTime = DateTime.Now;
            STBSysRunTime.Text = Program.Oscilloscope.SysRunTimeMangager.SystemRunTime;
        }

        /// <summary>
        /// The InitOnLoad.
        /// </summary>
        /// <param name="chnls">The chnls<see cref="IEnumerable{ChannelId}"/>.</param>
        private void InitOnLoad(IEnumerable<ChannelId> chnls)
        {
            CbxChannel.DataSource = chnls.Select(c => new ComboBoxItem(c.IsDigital() ? "D" + (c - ChannelId.D0) : c.ToString().Replace("AWG", "G"), (Int32)c)).ToList();
            CbxChannel.SelectedIndexChanged += (_, _) =>
            {
                var id = (ChannelId)CbxChannel.SelectValue;
                Color color = Color.Black;
                if (Program.Oscilloscope.TryGetBadge(id, out var badge))
                {
                    color = id.IsDigital() ? (badge as DigitalPrsnt).GetColorAt(id - ChannelId.D0) : badge.DrawColor;
                }
                LblChnlColor.BackColor = color;
                LblChnlColor.Text = color.ToString();
            };
            CbxChannel.SelectValue = (Int32)chnls.FirstOrDefault(); ;

            var id = (ChannelId)CbxChannel.SelectValue;
            Color color = Color.Black;
            if (Program.Oscilloscope.TryGetBadge(id, out var badge))
            {
                color = id.IsDigital() ? (badge as DigitalPrsnt).GetColorAt(id - ChannelId.D0) : badge.DrawColor;
            }
            LblChnlColor.BackColor = color;
            LblChnlColor.Text = color.ToString();



            //CbxChannel.DataSource = chnls.Select(c => new KeyValuePair<String, ChannelId>(c.IsDigital() ? "D" + (c - ChannelId.D0) : c.ToString(), c)).ToList();
            //CbxChannel.DisplayMember = "Key";
            //CbxChannel.ValueMember = "Value";

            //CbxChannel.SelectedValueChanged += (_, _) =>
            //{
            //    var id = (ChannelId)CbxChannel.SelectedValue;
            //    Color color = Color.Black;
            //    if (Program.Oscilloscope.TryGetBadge(id, out var badge))
            //    {
            //        if (id.IsDigital())
            //        {
            //            color = (badge as DigitalPrsnt).GetColorAt(id - ChannelId.D0);
            //        }
            //        else
            //        {
            //            color = badge.DrawColor;
            //        }
            //    }
            //    LblChnlColor.BackColor = color;
            //    LblChnlColor.Text = color.ToString();

            //};

            //CbxChannel.SelectedIndex = 0;
        }

        /// <summary>
        /// The LblChnlColor_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void LblChnlColor_Click(Object sender, EventArgs e)
        {
            ColorDialog cd = new();
            cd.AllowFullOpen = true;
            cd.ShowHelp = false;

            cd.Color = LblChnlColor.BackColor;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                LblChnlColor.BackColor = cd.Color;
                LblChnlColor.Text = cd.Color.ToString();

                var id = (ChannelId)CbxChannel.SelectValue;
                if (Program.Oscilloscope.TryGetBadge(id, out var badge))
                {
                    if (id.IsDigital())
                    {
                        (badge as DigitalPrsnt).SetColorAt(id - ChannelId.D0, cd.Color);
                    }
                    else
                    {
                        badge.DrawColor = cd.Color;
                    }
                }
            }
        }


        #region ChangeTime

        private void BtnSettingTime_Click(Object sender, EventArgs e)
        {
            var time = $"{DbxDateTime.CurrentTime.Year},{DbxDateTime.CurrentTime.Month},{DbxDateTime.CurrentTime.Day},{DbxDateTime.CurrentTime.Hour},{DbxDateTime.CurrentTime.Minute},{DbxDateTime.CurrentTime.Second}";
            DispPresenter.SystemTime = time;
        }

        #endregion

        private void BtnRestoreFactory_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.DEFAULT/*KeyCode.VK_RECOVER*/);
        }

        private void ChkTouch_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                DispPresenter.TouchLock = ChkTouch.Checked;
                ParentForm?.Close();
            }
        }


        private void ChkStopMeasure_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure != ChkStopMeasure.Checked)
            {
                DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure = ChkStopMeasure.Checked;
            }
        }

        private void BtnShowTemperature_Click(object sender, EventArgs e)
        {
            var tempChcek = new TemperatureTestForm(Program.Oscilloscope);
            tempChcek.Show();
            ParentForm?.Close();
        }

        private void BtnResetOptionTime_Click(object sender, EventArgs e)
        {
            Program.Oscilloscope.OptionsManager.ResetTime(false);
        }

        private void BtnOverOptionTime_Click(object sender, EventArgs e)
        {
            Program.Oscilloscope.OptionsManager.ResetTime(true);
        }
    }
}
