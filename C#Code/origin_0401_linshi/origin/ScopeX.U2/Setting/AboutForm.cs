using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.U2.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ScopeX.U2
{
    public partial class AboutForm : FloatForm
    {
        private OptionType? _SelectedOption = null;

        private readonly DsoPrsnt _Oscilloscope;

        private IReadOnlyList<KeyValuePair<OptionType, (String ModelName, String Description)>> _AllOptionInfo = new List<KeyValuePair<OptionType, (String ModelName, String Description)>>();
        private readonly String _VersionPrefix = "V";

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private List<String> _DllsName = new List<String>();

        private void InitControlLang()
        {
            scopexLabel1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YingJianBanBen_");
            LblOption.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YiAnZhuangDeXuanJian_");
            Lbl1.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XingHao_");
            Lbl2.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("RuanJianBanBen_");
            Lbl3.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuJianBanBen_");
            Lbl4.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ChanPinXuHao_");
            Lbl5.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShengChanRiQi_");
            BtnInfo.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiangXiXinXi");
            BtnRemoveLiscense.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YiChuXuKeZhengShu");
            BtnInstallLicense.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AnZhuangXuKeZhengShu");
            Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiTongXinXi");
            Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiTongXinXi");
        }

        public AboutForm()
        {
            InitializeComponent();
            InitControlLang();
            this.FixedToolIconInfos[2].IsShow = false;
            _Oscilloscope = Program.Oscilloscope;
            Stylize();
            InitInfo();
            InitLvOption();
            UpdateView();
            if (Constants.ENABLE_DEBUG == false)
            {
                BtnInfo.Visible = false;
            }
        }

        /// <summary>
        /// 初始化各标签的内容信息
        /// </summary>
        private void InitInfo()
        {
            if (String.IsNullOrEmpty(VersionManager.HardWareVersion) == false)
            {
                var hardware = VersionManager.HardWareVersion.Split("：");
                LblHardWare.Text = hardware.Length >= 1 ? (_VersionPrefix + hardware[0]) : "NULL";
            }
            LblSN.Text = Program.Oscilloscope.OptionsManager.SerialNumber.ToString();
            LblOptionUsingTime.Visible = _Oscilloscope.OptionsManager.GetRemainingTimeByHour() > 0 && !_Oscilloscope.OptionsManager.IsAllActive();
            LblOptionUsingTime.Text = $"{ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ShiYongShiJianShengYu")} {_Oscilloscope.OptionsManager.GetRemainingTimeByHour().ToString("#0.00")} {ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XiaoShi")}";
            LblOptionUsingTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            ReadXmlInfo();
            LblSoftWareVersion.Text = _VersionPrefix + Application.ProductVersion + (!String.IsNullOrWhiteSpace(VersionManager.ExtraVersionInfo) ? $"-{VersionManager.ExtraVersionInfo}" : String.Empty);
        }

        /// <summary>
        /// 初始化选件的信息
        /// </summary>
        private void InitLvOption()
        {
            //BtnActive.Font = AppStyleConfig.DefaultLabelFont;
            //BtnActive.ForeColor = AppStyleConfig.DefaultContextForeColor;

            //设置Listview的外观
            LvOption.HeaderForeColor = AppStyleConfig.DefaultTitleForeColor;
            LvOption.HeaderBackColor = AppStyleConfig.DefaultTitleBackColor.GetBrightnessColor(0.05);
            LvOption.ForeColor = AppStyleConfig.DefaultContextForeColor;
            LvOption.BackColor = AppStyleConfig.DefaultContextBackColor.GetBrightnessColor(0.05);
            //LvOption.HeaderForeColor = Color.White;
            //LvOption.HeaderBackColor = Color.Gray;
            //LvOption.GridLines = true;
            //LvOption.GridLinesColor = Color.Gray;
            //LvOption.BackColor = Color.FromArgb(41, 42, 45);
            //LvOption.ForeColor = AppStyleConfig.DefaultContextForeColor;
            LvOption.SelectedRowColor = AppStyleConfig.DefaultCheckedBackColor;
            LvOption.Font = AppStyleConfig.DefaultLabelFont;
            LvOption.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuanJianMing"), 100);
            LvOption.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhuangTai"), 110);
            LvOption.Columns.Add(ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MiaoShu"), -2);

            InitAllOptionInfo();
        }

        private void InitAllOptionInfo()
        {
            var alloptioninfo = PlatformUIManager.Default.Platform.GetOptionInfo();
            _AllOptionInfo = alloptioninfo.AsReadOnly();
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                LvOption.BeginUpdate();
                Int32 row = 0;
                foreach (var option in _AllOptionInfo)
                {
                    if (row == LvOption.Items.Count)
                    {
                        LvOption.Items.Add(new ListViewItem(new String[LvOption.Columns.Count]));
                    }

                    var name = option.Value.ModelName;
                    if (LvOption.Items[row].SubItems[0].Text != name)
                    {
                        LvOption.Items[row].SubItems[0].Text = name;
                    }
                    LvOption.Items[row].SubItems[1].Text = _Oscilloscope.OptionsManager.GetOptionActive(option.Key) ? ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("YiJiHuo") : ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WeiJiHuo");
                    //LvOption.Items[row].SubItems[2].Text = option.Value.Description;
                    LvOption.Items[row].SubItems[2].Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage($"AboutForm_OptionType_{option.Value.ModelName}");
                    row++;
                }
                LvOption.EndUpdate();

                LblOptionUsingTime.Visible = !_Oscilloscope.OptionsManager.IsAllActive() && _Oscilloscope.OptionsManager.GetRemainingTimeByHour() > 0;
            }
        }

        //读取Xml文件里面的配置信息
        private void ReadXmlInfo()
        {
            Type type = typeof(AboutForm);
            String Type = String.Empty;
            _DllsName.Clear();
            try
            {
                Stream sm;
                var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\AboutInfo.xml");
                if (!System.IO.File.Exists(filepath))
                {
                    sm = type.Assembly.GetManifestResourceStream(type.Namespace + ".Resources.AboutInfo.xml");
                }
                else
                    sm = System.IO.File.OpenRead(filepath);

                XmlReader reader = XmlReader.Create(sm);
                reader.ReadToFollowing("AboutInfo");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        String name = reader.Name;
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Text)
                        {
                            switch (name)
                            {
                                case "Type":
                                    Type = reader.Value.Trim();
                                    break;
                                case "FireWare":
                                    LblFireWare.Text = _VersionPrefix + reader.Value;
                                    break;
                                case "SN":
                                    //LblSN.Text = reader.Value;
                                    break;
                                case "Date":
                                    // LblDate.Text = reader.Value;
                                    break;
                                case "Extra":
                                    VersionManager.ExtraVersionInfo = reader.Value.Trim();
                                    break;
                                default:
                                    break;

                            }
                        }
                        else if (reader.NodeType == XmlNodeType.Whitespace)
                        {
                            switch (name)
                            {
                                case "Dlls":
                                    while (reader.Read())
                                    {
                                        name = reader.Name;
                                        reader.Read();
                                        if (reader.NodeType == XmlNodeType.Text)
                                        {
                                            switch (name)
                                            {
                                                case "Dll":
                                                    _DllsName.Add(reader.Value);
                                                    break;
                                            }

                                        }
                                    }
                                    break;

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //@todo：提示并记录日志
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.Message, EventBus.LogLevel.Error));
            }

            LblType.Text = Program.Oscilloscope.GetProductModel();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //Stylize();
#if SaveLanguage
            // LanguageFactory.CacheFormLanguageControls(this);
#endif
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _SelectedOption = null;
            (_Oscilloscope.View as DsoForm).Activate();
            base.OnFormClosed(e);
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            //this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        }

        private void BtnInfo_Click(object sender, EventArgs e)
        {
            DetailsForm detailsForm = new DetailsForm();
            detailsForm.ShowDialog();
        }

        private async void BtnInstallLicense_Click(object sender, EventArgs e)
        {
            _Oscilloscope.OptionsManager.IsActiveOption = 2;

            var keyboardenable = DsoPrsnt.KeyBoardLockEnable;
            DsoPrsnt.KeyBoardLockEnable = true;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("QingXuanZeXuKeWenJian");
            ofd.Filter = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("XuKeWenJian___lic");
            ofd.Multiselect = false;
            var result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    _Oscilloscope.OptionsManager.UpdateCompleteFlag = false;
                    _Oscilloscope.OptionsManager.ActiveOption(ofd.FileName, out var updateview);
                    if (updateview)
                    {
                        var i = 0;
                        var complete = await Task.Run(() =>
                        {
                            while (!_Oscilloscope.OptionsManager.UpdateCompleteFlag && i <= 50)//超过50*500 ms
                            {
                                Thread.Sleep(500);
                                i++;
                            }
                            return _Oscilloscope.OptionsManager.UpdateCompleteFlag;
                        });
                        UpdateView();
                    }
                }
                catch (Exception ex)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Error (Setting IPAddress): " + ex.Message, EventBus.LogLevel.Error));
                }
                finally
                {
                    _Oscilloscope.OptionsManager.UpdateCompleteFlag = false;
                    DsoPrsnt.KeyBoardLockEnable = keyboardenable;
                }
            }

            if (_SelectedOption != null)
            {
                BtnRemoveLiscense.Visible = _Oscilloscope.OptionsManager.GetOpitonActiveEx((OptionType)_SelectedOption);
            }
            _Oscilloscope.OptionsManager.UpdateCompleteFlag = false;
            DsoPrsnt.KeyBoardLockEnable = keyboardenable;
        }

        private async void BtnRemoveLiscense_Click(object sender, EventArgs e)
        {
            _Oscilloscope.OptionsManager.IsActiveOption = 1;

            var keyboardenable = DsoPrsnt.KeyBoardLockEnable;
            DsoPrsnt.KeyBoardLockEnable = true;

            var result = StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.LicenseRemoveYesOrNo, MessageType.Asking);

            Thread.Sleep(1000);
            if (result && _SelectedOption != null)
            {
                try
                {
                    _Oscilloscope.OptionsManager.UpdateCompleteFlag = false;
                    _Oscilloscope.OptionsManager.RemoveLicense((OptionType)_SelectedOption);
                    var i = 0;
                    var complete = await Task.Run(() =>
                     {
                         while (!_Oscilloscope.OptionsManager.UpdateCompleteFlag && i <= 50) //超过50 * 500 ms
                         {
                             Thread.Sleep(500);
                             i++;
                         }
                         return _Oscilloscope.OptionsManager.UpdateCompleteFlag;
                     });

                    UpdateView();
                    UpdateOptionEnable();
                    BtnRemoveLiscense.Visible = _Oscilloscope.OptionsManager.GetOpitonActiveEx((OptionType)_SelectedOption);
                }
                catch (Exception ex)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Error (Setting IPAddress): " + ex.Message, EventBus.LogLevel.Error));
                }
                finally
                {
                    _Oscilloscope.OptionsManager.UpdateCompleteFlag = false;
                    DsoPrsnt.KeyBoardLockEnable = keyboardenable;
                }
            }
            _Oscilloscope.OptionsManager.UpdateCompleteFlag = false;
            DsoPrsnt.KeyBoardLockEnable = keyboardenable;
        }

        /// <summary>
        /// 移除许可后 关闭正在使用的选件
        /// </summary>
        private void UpdateOptionEnable()
        {
            var bActive = false;
            if ((Int32)_SelectedOption >= (Int32)OptionType.Jitter)
            {
                //Jitter
                bActive = _Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.Jitter);
                if (!bActive && _Oscilloscope.Jitter.Active)
                {
                    _Oscilloscope.Jitter.Active = false;
                }

                //Pwr
                bActive = _Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.Pwr);
                foreach (var pwr in _Oscilloscope.PwrAnalysisDictionary)
                {
                    if (!bActive && pwr.Value.Active)
                    {
                        pwr.Value.Active = false;
                    }
                }

                //BUS
                UpdateBusOptionEnable();

                return;
            }

            if (_SelectedOption == OptionType.LA)
            {
                bActive = _Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.LA);
                var activedigital = _Oscilloscope.TryGetRange(c => c.Id.IsDigital()).Where(p => p.Active).ToList();
                if (activedigital.Count > 0)
                {
                    activedigital.ForEach(p =>
                    {
                        p.Active = false;
                    });
                }
            }
            if (_SelectedOption == OptionType.AWG)
            {
                bActive = _Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.AWG);
                foreach (var awgprsnt in _Oscilloscope.ArbWfmGens)
                {
                    if (!bActive && awgprsnt.Active)
                    {
                        awgprsnt.Active = false;
                        (_Oscilloscope.View as DsoForm).RemoveWaveformUI(awgprsnt);
                    }
                }
            }
            if (_SelectedOption == OptionType.BW10T20)
            {
                bActive = _Oscilloscope.OptionsManager.GetOptionActive(OptionType.BW10T20);
            }
        }

        private void UpdateBusOptionEnable()
        {
            var bus = _Oscilloscope.TryGetRange(c => c.Id.IsDecode()).Where(p => p.Active).ToList();
            bus.ForEach(p =>
            {
                if (p is DecodePrsnt prsnt)
                {
                    var serialtype = prsnt.DecodeChPrsnt.ProtocolType;
                    if (serialtype == SerialProtocolType.CAN_FD)
                    {
                        CheckBusOptionActive(prsnt, OptionType.Decode_CanFD);
                    }
                    else if (serialtype == SerialProtocolType.FlexRay)
                    {
                        CheckBusOptionActive(prsnt, OptionType.Decode_FlexRay);
                    }
                    else if (serialtype == SerialProtocolType.SENT)
                    {
                        CheckBusOptionActive(prsnt, OptionType.Decode_SENT);
                    }
                    else if (serialtype == SerialProtocolType.MIL || serialtype == SerialProtocolType.ARINC429)
                    {
                        CheckBusOptionActive(prsnt, OptionType.Decode_AERO);
                    }
                    else if (serialtype == SerialProtocolType.AudioBus)
                    {
                        CheckBusOptionActive(prsnt, OptionType.Decode_AudioBus);
                    }
                }
            });
        }

        private void CheckBusOptionActive(DecodePrsnt prsnt, OptionType optiontype)
        {
            if (OptionManager.Default.Checked(optiontype) == false)
            {
                prsnt.DecodeChPrsnt.ProtocolType = SerialProtocolType.Close;
            }
        }

        private void LvOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LvOption.SelectedItems.Count <= 0)
            {
                _SelectedOption = null;
                return;
            }
            var _selectitem = LvOption.SelectedItems[0];
            var optioninfo = _AllOptionInfo.FirstOrDefault(option => option.Value.ModelName == _selectitem.Text);
            _SelectedOption = optioninfo.Key;
            BtnRemoveLiscense.Visible = _Oscilloscope.OptionsManager.GetOpitonActiveEx(optioninfo.Key);
        }

        private void LblOption_DoubleClick(object sender, EventArgs e)
        {
            _Oscilloscope.OptionsManager.ResetTime(true);
        }

        private enum LicenseFormat
        {
            [Alias("lic")]
            License,
        }

    }
}
