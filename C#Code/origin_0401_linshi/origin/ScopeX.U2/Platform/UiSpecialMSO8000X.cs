using EventBus;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Model;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.U2.KeyboardLed;

namespace ScopeX.U2
{
    internal class UiSpecialMSO8000X : IPlatformUI
    {
        public ProductType ProductType { get; } = ProductType.JiHe_MSO8000X;

        /// <summary>
        /// 通道高压报警
        /// </summary>
        public void HardwareWarningEventHandler(HardwareWarningEventMessageArgs args)
        {
            //从DsoForm的事件处理函数迁移而来，进行不同平台的界面差异化处理
            string soucenames = "";

            if (args.Channel1)
            {
                soucenames += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoNiTongDao1_");
            }

            if (args.Channel2)
            {
                soucenames += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoNiTongDao2_");
            }

            if (args.Channel3)
            {
                soucenames += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoNiTongDao3_");
            }

            if (args.Channel4)
            {
                soucenames += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MoNiTongDao4_");
            }

            if (args.ExtTrigger)
            {
                soucenames += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("WaiChuFa");
            }

            if (args.Channel1 || args.Channel2 || args.Channel3 || args.Channel4 || args.ExtTrigger)
            {
                soucenames = soucenames.TrimEnd('、');
                soucenames += ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HighVoltageWaningStr");
                WeakTip.Default.Write(null, soucenames, false, null, 5);
            }
        }

        /// <summary>
        /// Utility按键差异处理
        /// </summary>
        public void KeyEnumUtilityHandler()
        {
            var form = DsoPrsnt.DefaultDsoPrsnt.UserSettingForm;
            if (form == Forms.None)
            {
                if (Program.Oscilloscope.View is DsoForm dsoform)
                {
                    dsoform.Invoke(() =>
                    {
                        CustomizeForm custom = new CustomizeForm();
                        EventBroker.Instance.GetEvent<FormEventArgs>().Publish(custom, new() { Current = custom, Type = FormType.SettingForm });
                    });
                }
            }
            else
            {
                Keyboard.Default.PreProcess((byte)((Int32)form));
            }
        }

        public void KeyEnumTriggerHandler()
        {
            HotKnobManager.Default.Fineable = !HotKnobManager.Default.Fineable;
            Program.Oscilloscope.Cursor.MoveMode = Program.Oscilloscope.Cursor.MoveMode == CursorMoveMode.Fast ? CursorMoveMode.Fine : CursorMoveMode.Fast;
        }

        public void KeyEnumTrigForceHandler()
        {
            if (Program.Oscilloscope.CurrentTrigger is TrigEdgePrsnt edgeTrig)
            {
                uint i = (uint)edgeTrig.Slope;
                i++;
                edgeTrig.Slope = (EdgeSlope)(i % 3);
            }
        }

        /// <summary>
        /// 用于按键板初始化平台化
        /// 对datas的补充
        /// </summary>
        /// <param name="datas"></param>
        public void KeyboardInit(List<IKeyData> datas)
        {
            //补充边沿触发指示灯
            TrigEdgePrsnt edgeprsnt = Program.Oscilloscope.CurrentTrigger as TrigEdgePrsnt;
            byte state = 255;
            if ((null != edgeprsnt))
            {
                switch (edgeprsnt.Slope)
                {
                    case EdgeSlope.Rise: state = 0; break;
                    case EdgeSlope.Fall: state = 1; break;
                    case EdgeSlope.Both: state = 2; break;
                    default: state = 255; break;
                }
            }
            datas.Add(new ScopeStateData()
            {
                ScopeState = ScopeStateType.TriggerLedControl,
                State = state,
            });
        }

        public IReadOnlyList<ChannelId> GetTriggerSource(Boolean hasDigitalChnl = false, Boolean hasExtChnl = false, Boolean hasAcChnl = false, Boolean hasAuxin = false)
        {
            var sources = ChannelIdExt.GetTriggerSources().ToList();

            if (!hasDigitalChnl)
            {
                sources = sources.Where(source => !source.IsDigital()).ToList();
            }

            if (!hasExtChnl)
            {
                sources = sources.Where(source => source != ChannelId.Ext && source != ChannelId.Ext5).ToList();
            }

            if (!hasAcChnl)
            {
                sources = sources.Where(source => source != ChannelId.AC).ToList();
            }

            if (!hasAuxin)
            {
                sources = sources.Where(source => source != ChannelId.AuxIn).ToList();
            }

            return sources.AsReadOnly();
        }

        public List<KeyValuePair<OptionType, (String FunctionName, String Description)>> GetOptionInfo()
        {
            var options = Enum.GetValues<OptionType>()
                .Where(o => o.GetProductTypes().Contains($"{ProductType}"))
                .Select(o => new KeyValuePair<OptionType, (String FunctionName, String Description)>(o, (o.GetDisplay(), o.GetOptionDescription().Description))).ToList();

            if (Program.Oscilloscope.OptionsManager.Is2GHz)
            {
                var item = options.FirstOrDefault(o => o.Key == OptionType.BW10T20);
                options.Remove(item);
            }
            return options!;
        }

        public List<ChannelId> GetEditableColorsChannel()
        {
            var channels = Enum.GetValues<ChannelId>().Where(c => (c.IsMath() && c <= ChannelIdExt.MaxMChId) || c.IsReference() || c.IsDecode() || c.IsDigital() || c.IsAWG()).ToList();
            return channels;
        }

        public ILanguage GetLanguage(Language language)
        {
            return language switch
            {
                Language.English => EnglishLang(),
                Language.German => GermanLang(),
                Language.French => FrenchLang(),
                Language.Spanish => SpanishLang(),
                Language.Italian => ItalianLang(),
                _ => ChineseLang()
            };

            ILanguage ChineseLang()
            {
                var xmllanguage = new ScopeX.Controls.Language.XMLLanguage("Tip.xml");
                xmllanguage.AppendOrUpdate("Tip_MSO8000X.xml");
                return xmllanguage;
            }

            ILanguage EnglishLang()
            {
                var xmllanguage = new ScopeX.Controls.Language.XMLLanguage("english.xml");
                xmllanguage.AppendOrUpdate("english_MSO8000X.xml");
                return xmllanguage;
            }

            ILanguage GermanLang()
            {
                var xmllanguage = new ScopeX.Controls.Language.XMLLanguage("German.xml");
                xmllanguage.AppendOrUpdate("German_8000X.xml");
                return xmllanguage;
            }

            ILanguage FrenchLang()
            {
                var xmllanguage = new ScopeX.Controls.Language.XMLLanguage("French.xml");
                xmllanguage.AppendOrUpdate("French_MSO8000X.xml");
                return xmllanguage;
            }

            ILanguage SpanishLang()
            {
                var xmllanguage = new ScopeX.Controls.Language.XMLLanguage("Spanish.xml");
                xmllanguage.AppendOrUpdate("Spanish_MSO8000X.xml");
                return xmllanguage;
            }

            ILanguage ItalianLang()
            {
                var xmllanguage = new ScopeX.Controls.Language.XMLLanguage("Italian.xml");
                xmllanguage.AppendOrUpdate("Italian_MSO8000X.xml");
                return xmllanguage;
            }
        }

        public GeneralAttribute Attribute => new GeneralAttribute(true, true, true)
        {
            SupportDigital = true,
            SupportKeyBoard = true,
            SupportUtilityKey = true,
            FunctionCropping = false,
            SupportGetOrSetBrightness = true,
            SupportAuxIn = true,
            AutosetWaitingMode = true,
            SupportHighImpedance = Constants.ANA_CHNL_TYPE != AnaChnlType.ANA_8G,
        };

        public String[] GetAwgTriggerSource()
        {
            return new string[]
                    {
                        LanguageManger.Instance.GetIDMessage("NeiBu"),
                        LanguageManger.Instance.GetIDMessage("ShouDong"),
                        LanguageManger.Instance.GetIDMessage("WaiBu")
                    };
        }

        public IkeyBoardDetetionView KeyboardDetectionView
        {
            get
            {
                var page = new Page8000HD
                {
                    Presenter = Program.Oscilloscope.SystemCheck,
                    Dock = DockStyle.Fill,
                    Margin = new(0),
                };
                return page;
            }
        }

        public List<FFTNumber> GetFFTNumbers()
        {
            return Enum.GetValues<FFTNumber>().ToList();
        }

        public Boolean FFTFunctionLimitWithJitter() => false;

        public (Int32 MaxFanSpeed, Int32 Scale, Int32 MaxTemperature) GetFanControlParams()
        {
            return (5500, 500, 100);
        }
    }
}
