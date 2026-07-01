using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.ComModel;
using ScopeX.UserControls.Style;
using ScopeX.UserControls;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.U2.Tools;
using static ScopeX.UserControls.SelectComboBox;
using System.Runtime.Intrinsics.Arm;

namespace ScopeX.U2
{
    public partial class TriggerSerialSubPage : UserControl, Core.ITriggerSerialView, IStylize
    {
        #region 属性及字段定义

        private Boolean _ConditionChangeDataSource = false;
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        //private CustomDecodeForm _SettingForm;
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
        private Boolean _ArgToCtrl = false;
        private List<ChannelId> _SerialTypes = new List<ChannelId>();
        private SerialProtocolType _SelectedSerial;
        public TrigSerialPrsnt Presenter
        {
            get => (TrigSerialPrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }
        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter { get => Presenter; set => Presenter = (TrigSerialPrsnt)value; }

        #endregion  属性及字段定义

        public TriggerSerialSubPage()
        {
            InitializeComponent();
            InitSourceList();
            //this.CbxSerialType.SelectedValueChanged += (_, _) => SelectedValueChanged();
        }


        private void InitSourceList()
        {
            this.CbxSerialType.SelectedIndexChanged -= CbxSerialType_SelectedValueChanged;

            var busids = ChannelIdExt.GetDecodes();
            foreach (var id in busids)
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var p) && p.Active)
                {
                    _SerialTypes.Add(id);
                }

            }
            if (_SerialTypes.Count == 0)
            {
                _SerialTypes.Add(ChannelId.None);
            }

            _SelectedSerial = SerialProtocolType.Close;
            this.CbxSerialType.SelectedIndexChanged += CbxSerialType_SelectedValueChanged;
        }


        private void CbxSerialType_SelectedValueChanged(object sender, EventArgs e) => SelectedValueChanged();

        public void ReLoadSource()
        {
            InitSourceList();
        }
        private void Init()
        {
            //_SelectedSerial = SerialProtocolType.Close;
            this.CbxSerialType.SelectedIndexChanged -= CbxSerialType_SelectedValueChanged;
            CbxSerialType.DataSource = _SerialTypes.Select(x => new ComboBoxItem(x == ChannelId.None ? SerialProtocolType.Close.ToString() : x.ToString(), x)).ToList();
            if (!_SerialTypes.Contains(ChannelId.None))
            {
                DecodePrsnt dp = null;

                if (Presenter.Source != ChannelId.None)
                {
                    if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Presenter.Source, out var p) && p is DecodePrsnt)
                    {
                        dp = p as DecodePrsnt;
                    }
                }
                else
                {
                    if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SerialTypes[0], out var p) && p is DecodePrsnt)
                    {
                        dp = p as DecodePrsnt;
                    }
                }

                Presenter.Source = dp.Id;
                TriggerPrsnt.UpdateTrigSerialType(dp.ProtocolType);
            }
            CbxSerialType.SelectValue = Presenter.Source;
            CbxSerialType.SelectedIndexChanged += CbxSerialType_SelectedValueChanged;
            this.CbxCondition.Visible = Presenter.SerialType != SerialProtocolType.Close;
            this.LblCondition.Visible = this.CbxCondition.Visible;
            HeaderPanel.Controls.Cast<Control>().ToList().ForEach(x =>
            {
                DefaultStyleManager.Instance.RegisterControlRecursion(x, StyleFlag.FontSize);
            });
            PCustom.Controls.Cast<Control>().ToList().ForEach(x =>
            {
                DefaultStyleManager.Instance.RegisterControlRecursion(x, StyleFlag.FontSize);
            });
            SelectedValueChanged();
        }

        private void ShowCustom()
        {
            if (_ArgToCtrl)
                return;
            //if (Presenter != null && _SettingForm != null && Presenter.SerialType != SerialProtocolType.Close)
            //{
            //    //CustomDecodeForm frm = new CustomDecodeForm(Presenter.SerialType);
            //    //frm.FormClosed += (_, _) =>
            //    //{
            //    //    //Focus();
            //    //    (ParentForm as TriggerForm)?.Activate();
            //    //};
            //    //frm.ShowInTaskbar = false;
            //    //frm.IsShowPin = false;
            //    //frm.ShowDialogByPosition();

            //    _SettingForm.ShowInTaskbar = false;
            //    _SettingForm.IsShowPin = false;
            //    _SettingForm.ShowDialogByPosition();
            //}
            //else
            //{

            //}
        }

        private string GetConditionName()
        {
            string conditionName = Presenter.ConditionName;
            if (Presenter is AudioBusTrigSerialPrsnt audioprsnt)
                conditionName = audioprsnt.ConditionName;
            if (Presenter is SENTTrigSerialPrsnt senttrigp)
                conditionName = senttrigp.ConditionName;

            return conditionName;
        }

        private void ConditionChanged()
        {
            string conditionName = GetConditionName();
            if (Presenter != null && !String.IsNullOrEmpty(conditionName) && !_ConditionChangeDataSource)
            {
                var conditionproterty = Presenter.GetType().GetProperty(conditionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (conditionproterty != null)
                {
                    conditionproterty.SetValue(Presenter, CbxCondition.SelectValue);
                }
                else
                {

                }
            }
            else
            {

            }
        }
        private void UpdateCondition()
        {
            string conditionName = GetConditionName();
            if (Presenter != null && !String.IsNullOrEmpty(conditionName))
            {
                var conditionproterty = Presenter.GetType().GetProperty(conditionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (conditionproterty != null)
                {
                    CbxCondition.SelectValue = conditionproterty.GetValue(Presenter);
                }
                else
                {

                }
            }
            else
            {

            }

        }
        private void InitCondition()
        {
            string conditionName = GetConditionName();
            if (Presenter != null && !String.IsNullOrEmpty(conditionName))
            {
                var conditionproterty = Presenter.GetType().GetProperty(conditionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (conditionproterty != null)
                {
                    var condition = (Enum)conditionproterty.GetValue(Presenter);
                    if (condition != null)
                    {
                        var str = ContentControl.Content.GetType().Assembly.GetName().Name + ".Properties.Resources";
                        global::System.Resources.ResourceManager resource = new global::System.Resources.ResourceManager(str, ContentControl.Content.GetType().Assembly);
                        // ☆☆☆再次提醒：☆☆☆，请勿使用匿名函数注册事件，这种取消事件注册的方式是无效的
                        //CbxCondition.SelectedIndexChanged -= (_, _) => ConditionChanged();
                        CbxCondition.SelectedIndexChanged -= CbxCondition_SelectedValueChanged;
                        _ConditionChangeDataSource = true;
                        CbxCondition.DataSource = Enum.GetValues(condition.GetType()).Cast<Enum>().Select(x =>
                        {
                            String key = String.Empty;
                            Type type = x.GetType();
                            try
                            {
                                //在获取资源属性时才会抛出找不到资源文件的错误
                                //key = resource?.GetString(type.ReflectedType.FullName + "." + type.Name + "." + x.ToString());
                                string lan_key = type.ReflectedType.FullName + "." + type.Name + "." + x.ToString();
                                var result = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage(lan_key);
                                if (result == lan_key)
                                {
                                    key = resource?.GetString(lan_key);
                                }
                                else
                                {
                                    key = result;
                                }
                            }
                            catch
                            {

                            }
                            if (String.IsNullOrEmpty(key))
                            {
                                key = x.ToString();
                            }
                            return new ComboBoxItem(key, x);
                        }).ToList();
                        CbxCondition.SelectValue = conditionproterty.GetValue(Presenter);
                        CbxCondition.SelectedIndexChanged += CbxCondition_SelectedValueChanged;
                        //CbxCondition.SelectedValueChanged += (_, _) => ConditionChanged();
                        _ConditionChangeDataSource = false;
                        CbxCondition.SelectValue = conditionproterty.GetValue(Presenter);
                        if (_SelectedSerial == SerialProtocolType.NRZ)
                        {
                            //NRZ没有触发功能，因此先屏蔽掉触发选择控件
                            CbxCondition.Visible = false;
                            LblCondition.Visible = false;
                        }
                        else
                        {
                            CbxCondition.Visible = true;
                            LblCondition.Visible = true;
                        }
                        BtnCustom.Visible = false;
                    }
                    else
                    {
                        CbxCondition.SelectedIndexChanged -= CbxCondition_SelectedValueChanged;
                        CbxCondition.Visible = false;
                        LblCondition.Visible = false;
                        BtnCustom.Visible = false;

                    }
                }
            }
            else
            {
                CbxCondition.SelectedIndexChanged -= CbxCondition_SelectedValueChanged;
                CbxCondition.Visible = false;
                LblCondition.Visible = false;
                BtnCustom.Visible = false;
            }
        }

        private void CbxCondition_SelectedValueChanged(object sender, EventArgs e) => ConditionChanged();

        private void SelectedValueChanged(Boolean forceUpdate = false)
        {
            var serialtype = Presenter.GetSerialTypeByChannelId((ChannelId)this.CbxSerialType.SelectValue, true);
            if (_SelectedSerial == SerialProtocolType.Close)
            {
                this.ContentControl.Controls.Clear();
                this.CbxCondition.Visible = false;
                this.LblCondition.Visible = false;
                this.BtnCustom.Visible = false;
            }

            if ((!_ArgToCtrl || forceUpdate) && ParentForm != null)
            {
                CbxCondition.SelectedIndexChanged -= CbxCondition_SelectedValueChanged;
                _SelectedSerial = serialtype;

                if (this.ContentControl.Content is Control view)
                {
                    this.ContentControl.Controls.Remove(view);

                    if (view is ITriggerView triggerview)
                    {
                        Presenter?.TryRemoveView(triggerview);
                    }
                    Presenter.GetViewList()
                        .OfType<ITriggerSerialView>()
                        .ToList()
                        .ForEach(x => Presenter?.TryRemoveView(x));
                }


                TriggerPrsnt.SetTrigSerialSource((ChannelId)this.CbxSerialType.SelectValue);
                TrigSerialPrsnt serialPrsnt = null;
                IEnumerable<ITriggerView> tv = Presenter.GetViewList();
                serialPrsnt = TrigSerialPrsnt.GetOrMakeTriggerSerial(Presenter.Dso, _SelectedSerial);
                serialPrsnt.AddViewList(tv);

                foreach (ITriggerView val in tv)
                    val.Presenter = serialPrsnt;

                switch (_SelectedSerial)
                {
                    case SerialProtocolType.Close:
                        this.ContentControl.Controls.Clear();
                        this.CbxCondition.Visible = false;
                        this.LblCondition.Visible = false;
                        this.BtnCustom.Visible = false;
                        break;
                    default:
                        var result = DecodeApp.Default.TriggerViews[_SelectedSerial];

                        if (result != null)
                        {
                            if (result.Presenter == null) result.Presenter = Presenter;
                            this.ContentControl.Content = (Control)result;
                            this.ContentControl.Content.Dock = DockStyle.Fill;
                            result?.ReLoadSource();

                            if (((Control)result).Created)
                                result?.UpdateView(Presenter, "");
                        }
                        else
                        {
                            this.ContentControl.Controls.Clear();
                            throw new Exception($"{_SelectedSerial} trigger is not support");
                        }
                        InitCondition();
                        //_SettingForm = new CustomDecodeForm(_SelectedSerial);
                        break;
                }
            }
        }

        private void CheckOptionActive(OptionType optiontype)
        {
            if (OptionManager.Default.Checked(optiontype) == false)
            {
                _ArgToCtrl = true;
                Presenter.SerialType = SerialProtocolType.Close;
                this.CbxSerialType.SelectValue = Presenter.Source;
                _ArgToCtrl = false;
            }
        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(TrigSerialPrsnt.Source):

                    CbxSerialType.SelectValue = Presenter.Source;
                    SelectedValueChanged(true);
                    break;


                case nameof(AudioBusTrigSerialPrsnt.ConditionName):
                    // 条件类型改变
                    InitCondition();
                    //UpdateCondition();
                    break;
                default:
                    if (this.ContentControl.Content is ITriggerSerialView view)
                    {
                        view?.UpdateView(presenter, propertyName);

                    }
                    string conditionName = GetConditionName();
                    if (propertyName == conditionName)
                    {
                        UpdateCondition();
                    }
                    break;
            }
            _ArgToCtrl = false;
        }
        public override void Refresh()
        {
            this.UpdateView();
            base.Refresh();
        }
        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxSerialType.SelectValue = Presenter.Source;
                _ArgToCtrl = false;

                if (this.ContentControl.Content is ITriggerSerialView view)
                {
                    view?.UpdateView(Presenter, "");
                }
            }
        }
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (ParentForm == null) return;
            Init();
            UpdateView();
        }

        private void InitControlLang()
        {
            LblCondition.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChuFaFangShi");
            LblSerialType.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZongXianLeiXing");
            BtnCustom.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CanShuSheZhi");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitControlLang();
            //page背景透明设置
            this.BackColor = Color.Transparent;
            //Init();
            //UpdateView();
        }

        private void BtnCustom_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                ShowCustom();
            }

        }

    }
}
