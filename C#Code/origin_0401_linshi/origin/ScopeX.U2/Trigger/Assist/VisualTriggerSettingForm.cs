using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Channels;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class VisualTriggerSettingForm : FloatForm, IVisualTriggerView
    {
        private Boolean _ArgToCtrl;
        public VisualTriggerSettingForm()
        {
            InitializeComponent();
            LangKey_Init();
            #region 属性绑定

            //LblZoneRelation.DataBindings.Add("Visible", CbxZoneRelation, "Visible");

            #endregion
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged += Timebase_PublisherChanged;
            HelpClick += (_, _) =>
            {
                //var res = Int32.TryParse(HelpLabel, out var index);
                //if (!res)
                //{
                //    HelpProcessManager.SendCommand();
                //    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"Failed to obtain help index information({HelpLabel})!", EventBus.LogLevel.Debug));
                //    return;
                //}
                HelpProcessManager.SendCommand(HelpDocumentManager.Default.GetCommand(nameof(VisualTriggerSettingForm)));
            };
            InitSourceList();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged -= Timebase_PublisherChanged;
        }

        private void Timebase_PublisherChanged(object sender, CustomEventArg e)
        {
            if (e.Message == nameof(DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode) && DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode != AnaChnlStorageMode.Fast)
            {
                DsoPrsnt.DefaultDsoPrsnt.Timebase.PublisherChanged -= Timebase_PublisherChanged;
                this.Close();
            }
        }

        private void LangKey_Init()
        {
            ScopeX.UserControls.RadioButtonItem radioButtonItem1 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem2 = new ScopeX.UserControls.RadioButtonItem();
            radioButtonItem1.Icon = null;
            radioButtonItem1.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem1.Tag = null;
            radioButtonItem1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYuNei");
            radioButtonItem2.Icon = null;
            radioButtonItem2.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem2.Tag = null;
            radioButtonItem2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYuWai");
            this.RdoCondition_A.ButtonItems = new ScopeX.UserControls.RadioButtonItem[]
            {
                radioButtonItem1,
                radioButtonItem2
            };

            ScopeX.UserControls.RadioButtonItem radioButtonItem3 = new ScopeX.UserControls.RadioButtonItem();
            ScopeX.UserControls.RadioButtonItem radioButtonItem4 = new ScopeX.UserControls.RadioButtonItem();
            radioButtonItem3.Icon = null;
            radioButtonItem3.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem3.Tag = null;
            radioButtonItem3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYuNei");
            radioButtonItem4.Icon = null;
            radioButtonItem4.Padding = new System.Windows.Forms.Padding(0);
            radioButtonItem4.Tag = null;
            radioButtonItem4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYuWai");
            this.RdoCondition_B.ButtonItems = new ScopeX.UserControls.RadioButtonItem[]
            {
                radioButtonItem3,
                radioButtonItem4
            };

            this.lblCondition_A.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoJian");
            this.lblCondition_B.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("TiaoJian");
            this.LblSource_A.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");
            this.LblSource_B.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Yuan");

            /*this.CbxAreaShape.Items.AddRange(new object[] {
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SanJiaoXing"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WindowType_Rectangle"),
            ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("polygon")});
            this.CbxAreaShape.SelectedIndexChanged += this.CbxAreaShape_SelectedIndexChanged;*/

            this.LblRegionA.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYu") + "A";
            this.LblAreaB.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QuYu") + "B";
            this.BtnAreaReset_A.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhong__Zhi");
            this.BtnAreaReset_B.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Zhong__Zhi");
            this.ChkActive_A.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkActive_A.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.ChkActive_B.CheckedText = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Kai");
            this.ChkActive_B.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Guan");
            this.LblActive_A.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            this.LblActive_B.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XianShi");
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KeShiChuFa");
            this.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KeShiChuFa");
            this.LblDescription.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ABRegionalDescription");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

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

        public VisualTriggerPrsnt Presenter { get; set; }
        IVisualTriggerPrsnt IView<IVisualTriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (VisualTriggerPrsnt)value;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Presenter.TryRemoveView(this);
            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            //ChkActive_CheckedChangedEvent(null, null);
            // LanguageFactory.CacheFormLanguageControls(this);
            Change2FirstEnabledItem();
        }

        private void InitSourceList()
        {
            CbxSource_A.SelectedIndexChanged -= CbxSource_SelectedIndexChanged;
            CbxSource_A.Items.Clear();
            CbxSource_A.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource_A.SelectedIndexChanged += CbxSource_SelectedIndexChanged;

            CbxSource_B.SelectedIndexChanged -= CbxSource_B_SelectedIndexChanged;
            CbxSource_B.Items.Clear();
            CbxSource_B.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxSource_B.SelectedIndexChanged += CbxSource_B_SelectedIndexChanged;

            /*var relationtypes = Enum.GetValues(typeof(VisualTriggerRelation));
            List<string> relationtypes_list = new List<string>();
            foreach (VisualTriggerRelation item in relationtypes)
            {
                relationtypes_list.Add(item.GetDescription_Lang());
            }*/
        }

        private void CbxSource_B_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 1;
                var channelid = (ChannelId)CbxSource_B.SelectedIndex;
                Presenter[1].Source = channelid;
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(channelid, out IChnlPrsnt cprsnt) && cprsnt is AnalogPrsnt analogprsnt)
                {
                    Presenter[1].VerticalScale = analogprsnt.ScaleBymV;
                }
            }
        }

        private void CbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 0;
                var channelid = (ChannelId)CbxSource_A.SelectedIndex;
                Presenter[0].Source = channelid;
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(channelid, out IChnlPrsnt cprsnt) && cprsnt is AnalogPrsnt analogprsnt)
                {
                    Presenter[0].VerticalScale = analogprsnt.ScaleBymV;
                }
            }
        }

        public void UpdateView(Object presenter, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }
        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            var vip = Presenter[Presenter.CurrentSelected];
            switch (propertyName)
            {
                case nameof(vip.Source):
                    if (Presenter.CurrentSelected == 0)
                    {
                        CbxSource_A.SelectedIndex = (Int32)Presenter[Presenter.CurrentSelected].Source;
                    }
                    else if (Presenter.CurrentSelected == 1)
                    {
                        CbxSource_B.SelectedIndex = (Int32)Presenter[Presenter.CurrentSelected].Source;
                    }

                    break;
                case nameof(vip.TriggerState):
                    if (Presenter.CurrentSelected == 0)
                    {

                        RdoCondition_A.ChoosedButtonIndex = (Int32)Presenter[Presenter.CurrentSelected].TriggerState;
                    }
                    else if (Presenter.CurrentSelected == 1)
                    {
                        RdoCondition_B.ChoosedButtonIndex = (Int32)Presenter[Presenter.CurrentSelected].TriggerState;
                    }
                    break;
                case nameof(vip.Enabled):
                    if (Presenter.CurrentSelected == 0)
                    {
                        ChkActive_A.Checked = Presenter[Presenter.CurrentSelected].Enabled;
                    }
                    else if (Presenter.CurrentSelected == 1)
                    {
                        ChkActive_B.Checked = Presenter[Presenter.CurrentSelected].Enabled;
                    }

                    var enableditemcount = Presenter.SelectedItems.Where(c => c.Enabled).Count();
                    if (enableditemcount == 0)
                    {
                        // 没有任何区域启用时，窗体自动关闭
                        this.Close();
                    }
                    else if (enableditemcount == 1)
                    {
                        // 当关闭了其中一个区域，另一个区域启用时，切换到启用的区域配置界面去。
                        Change2FirstEnabledItem();
                    }
                    break;
                default:
                    break;
            }

            _ArgToCtrl = false;
        }

        private void Change2FirstEnabledItem()
        {
            if (Presenter.SelectedItems.Count(c => c.Enabled) != 1)
                return;

            var enableditem = Presenter.SelectedItems.FirstOrDefault(c => c.Enabled);
            if (enableditem == null)
                return;

            var index = Presenter.SelectedItems.IndexOf(enableditem);
            if (Presenter.CurrentSelected != index)
            {
                Presenter.CurrentSelected = index;
                UpdateView();
            }
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoCondition_A.ChoosedButtonIndex = (Int32)Presenter[0].TriggerState;
                CbxSource_A.SelectedIndex = (Int32)Presenter[0].Source;
                ChkActive_A.Checked = Presenter[0].Enabled;

                RdoCondition_B.ChoosedButtonIndex = (Int32)Presenter[1].TriggerState;
                CbxSource_B.SelectedIndex = (Int32)Presenter[1].Source;
                ChkActive_B.Checked = Presenter[1].Enabled;
                _ArgToCtrl = false;
            }
        }

        private void RdoCondition_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 0;
                Presenter[0].TriggerState = (VisualTriggerState)RdoCondition_A.ChoosedButtonIndex;
            }
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
            //this.ContentBackColor = AppStyleConfig.DefaultContextBackColor;
        }

        private void BtnAreaReset_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 0;
                Presenter[0].ReSet = true;
            }
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 0;
                var channelid = (ChannelId)CbxSource_A.SelectedIndex;
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(channelid, out IChnlPrsnt cprsnt) && cprsnt is AnalogPrsnt analogprsnt)
                {
                    Presenter[0].VerticalScale = analogprsnt.ScaleBymV;
                    Presenter[0].VerticalPosIndexBymDiv = analogprsnt.PosIndexBymDiv;
                    Presenter[0].TimeBasePosIndexBymDiv = DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                }
                Presenter[0].TimebaseScale = DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus;
                Presenter[0].Enabled = ChkActive_A.Checked;
                if (ChkActive_A.Checked)
                {
                    Presenter.Enabled = true;
                }
            }
        }

        private void RdoCondition_B_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 1;
                Presenter[1].TriggerState = (VisualTriggerState)RdoCondition_B.ChoosedButtonIndex;
            }
        }

        private void BtnAreaReset_B_Click(object sender, EventArgs e)
        {
            Presenter.CurrentSelected = 1;
            Presenter[1].ReSet = true;
        }

        private void ChkActive_B_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelected = 1;
                var channelid = (ChannelId)CbxSource_B.SelectedIndex;
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(channelid, out IChnlPrsnt cprsnt) && cprsnt is AnalogPrsnt analogprsnt)
                {
                    Presenter[1].VerticalScale = analogprsnt.ScaleBymV;
                    Presenter[1].VerticalPosIndexBymDiv = analogprsnt.PosIndexBymDiv;
                    Presenter[1].TimeBasePosIndexBymDiv = DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                }
                Presenter[1].TimebaseScale = DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus;
                Presenter[1].Enabled = ChkActive_B.Checked;
                if (ChkActive_B.Checked)
                {
                    Presenter.Enabled = true;
                }
            }
        }
    }
}
