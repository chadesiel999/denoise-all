using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    //不同控件的风格属性/字段描述
    public enum AppStyleFiled
    {
        //常规
        BackColor,
        ForeColor,
        BorderThickness,
        BorderColor,
        Font,

        //控件区分

        //ScopeXTextBox
        ScopeXTextBox_SelectedColor,
        ScopeXTextBox_BorderColor,
        ScopeXTextBox_ClickedBorderColor,
        ScopeXTextBox_BorderThickness,
        ScopeXTextBox_Height,
        ScopeXTextBox_BackColor,
        ScopeXTextBox_ForeColor,
        ScopeXTextBox_Font,
        //ScopeXGroupBox
        ScopeXGroupBox_BorderColor,
        ScopeXGroupBox_ForeColor,
        ScopeXGroupBox_Font,
        //UIRadioButtonGroup
        UIRadioButtonGroup_BorderColor,
        UIRadioButtonGroup_BorderThickness,
        UIRadioButtonGroup_ButtonBackColor,
        UIRadioButtonGroup_ButtonOffset,
        UIRadioButtonGroup_Height,
        UIRadioButtonGroup_ButtonTextColor,
        UIRadioButtonGroup_ChoosedButtonColor,
        UIRadioButtonGroup_ChoosedButtonTextColor,
        UIRadioButtonGroup_ContentBackColor,
        UIRadioButtonGroup_ContentPadding,
        UIRadioButtonGroup_FocusBorderColor,
        UIRadioButtonGroup_ButtonFont,
        //ScopeXSwitchButton
        ScopeXSwitchButton_BackColor,
        ScopeXSwitchButton_BorderColor,
        ScopeXSwitchButton_BorderThickness,
        ScopeXSwitchButton_Height,
        ScopeXSwitchButton_SliderButtonWidth,
        ScopeXSwitchButton_CheckedBackColor,
        ScopeXSwitchButton_CheckedForeColor,
        ScopeXSwitchButton_CheckedSliderColor,
        ScopeXSwitchButton_FocusBorderColor,
        ScopeXSwitchButton_ForeColor,
        ScopeXSwitchButton_SliderColor,
        ScopeXSwitchButton_UseAnimation,
        ScopeXSwitchButton_Font,
        //ComboBoxEx
        ComboBoxEx_BackColor,
        ComboBoxEx_BorderColor,
        ComboBoxEx_BorderThickness,
        //ComboBoxEx_DropDownWidth,
        ComboBoxEx_FocusColor,
        ComboBoxEx_ForeColor,
        ComboBoxEx_Font,
        ComboBoxEx_Height,
        ComboBoxEx_ItemHeight,
        ComboBoxEx_RectBtnWidth,
        ComboBoxEx_SelectedBackColor,
        //ScopeXNumericEditBox
        ScopeXNumericEditBox_BackColor,
        ScopeXNumericEditBox_BorderColor,
        ScopeXNumericEditBox_BorderThickness,
        ScopeXNumericEditBox_Height,
        ScopeXNumericEditBox_IconWidthProportion,
        ScopeXNumericEditBox_FocusBoederColor,
        ScopeXNumericEditBox_ClickedBorderColor,
        ScopeXNumericEditBox_FocusForeColor,
        ScopeXNumericEditBox_ForeColor,
        ScopeXNumericEditBox_Font,
        ScopeXNumericEditBox_AddBtnNormalBackColor,
        ScopeXNumericEditBox_AddBtnMouseInBackColor,
        ScopeXNumericEditBox_AddBtnMouseClickBackColor,
        ScopeXNumericEditBox_SubBtnNormalBackColor,
        ScopeXNumericEditBox_SubBtnMouseInBackColor,
        ScopeXNumericEditBox_SubBtnMouseClickBackColor,
        ScopeXNumericEditBox_AddBtnNormalForeColor,
        ScopeXNumericEditBox_AddBtnMouseInForeColor,
        ScopeXNumericEditBox_AddBtnMouseClickForeColor,
        ScopeXNumericEditBox_SubBtnNormalForeColor,
        ScopeXNumericEditBox_SubBtnMouseInForeColor,
        ScopeXNumericEditBox_SubBtnMouseClickForeColor,
        //ScopeXIconButton
        ScopeXIconButton_BackColor,
        ScopeXIconButton_BorderColor,
        ScopeXIconButton_SVGForeColor,
        ScopeXIconButton_BorderThickness,
        ScopeXIconButton_Font,
        ScopeXIconButton_ChoosedBackColor,
        ScopeXIconButton_ChoosedForeColor,
        ScopeXIconButton_ChoosedMouseinBackColor,
        ScopeXIconButton_ChoosedPressedBackColor,
        ScopeXIconButton_ForeColor,
        ScopeXIconButton_FocusedBorderColor,
        ScopeXIconButton_MouseinBackColor,
        ScopeXIconButton_MouseinBorderColor,
        ScopeXIconButton_MouseInBorderThickness,
        ScopeXIconButton_MouseinForeColor,
        ScopeXIconButton_MouseinSvgForeColor,
        ScopeXIconButton_PressedBackColor,
        ScopeXIconButton_PressedBorderColor,
        ScopeXIconButton_PressedBorderThickness,
        ScopeXIconButton_Height,
        ScopeXIconButton_PressedForeColor,
        ScopeXIconButton_PressedSvgForeColor,
        //SelectComboBox
        SelectComboBox_Height,
        SelectComboBox_Font,
        SelectComboBox_ComBorderColor,
        //ScopeXLabel
        ScopeXLabel_BackColor,
        ScopeXLabel_BorderColor,
        ScopeXLabel_BorderThickness,
        ScopeXLabel_ForeColor,
        ScopeXLabel_Font,
        ScopeXLabel_TextAlign,
        ScopeXLabel_Height,
        //NavBarGroup
        NavBarGroup_NavBarColor,
        NavBarGroup_NavGroupColor,
        NavBarGroup_NavForeColor,
        NavBarGroup_SplitColor,
        NavBarGroup_NavBarHeight,
        NavBarGroup_Font,
        //FloatForm
        FloatForm_ContentBackColor,
        FloatForm_HeadBackColor,
        FloatForm_IconInterval,
        FloatForm_IconWidth,
        FloatForm_ToolIconSize,
        FloatForm_HeadHeight,
        FloatForm_IconSideDistance,
        FloatForm_TitleAlign,
        FloatForm_TitleColor,
        FloatForm_TitleFont,

        //KeyboardForm
        KeyboardForm_ButtonNomalBackColor,
        KeyboardForm_ButtonMouseInBackColor,
        KeyboardForm_ButtonClickedBackColor,
        KeyboardForm_UnSelectedForeColor,
        KeyboardForm_ContentForeColor,
        KeyboardForm_ContentFont,
        KeyboardForm_Font,
        KeyboardForm_ContentBackColor,
        KeyboardForm_BackColor,
        KeyboardForm_ForeColor,
    }

    partial class AppStyleConfig
    {
        //所有的默认值参考自《控件风格属性文档》

        //int
        public static int DefaultLabelHeight = 25;
        public static int DefaultElementHeight = 30;
        public static int DefaultElementInverval = 10;
        public static int DefaultBorderThickness = 1;
        public static int DefaultIconWidth = 28;
        public static int DefaultIconHeight = 28;

        //font
        //public const string DefaultFontFamilyName = "微软雅黑";
        //public const string DefaultFontFamilyName = "Arial";
        public const string DefaultFontFamilyName = "MiSans";

        public const string MiSansBoldFamilyName = "MiSans Medium";
        public static Font DefaultButtonFont = new Font(DefaultFontFamilyName, 13F, FontStyle.Regular, GraphicsUnit.Point);
        public static Font DefaultTitleFont = new Font(DefaultFontFamilyName, 15F, FontStyle.Regular, GraphicsUnit.Point);
        public static Font DefaultLabelFont = new Font(DefaultFontFamilyName, 12.5F, FontStyle.Regular, GraphicsUnit.Point);
        public static Font DefaultContextFont = new Font(DefaultFontFamilyName, 12.5F, FontStyle.Regular, GraphicsUnit.Point);
        public static Font DefaultKeyFont = new Font(DefaultFontFamilyName, 24);
        public static Font DefaultMeasureFont = new Font(MiSansBoldFamilyName, 10f, GraphicsUnit.Point);
        public static Font DefaultMeasureFontPlus = new Font(DefaultFontFamilyName, 13f, GraphicsUnit.Point);
        public static Font DefaultBoldFont
        {
            get
            {
                return DefaultFontFamilyName == "MiSans" ?
                    new Font(MiSansBoldFamilyName, 9.5F, GraphicsUnit.Point) :
                    new Font(DefaultFontFamilyName,9.5F, FontStyle.Bold, GraphicsUnit.Point);
            }
        }

        //color

        public static Color DefaultTitleForeColor = Color.White; //Color.FromArgb(0xea, 0xea, 0xea);
        public static Color DefaultContextForeColor = Color.White;//Color.FromArgb(0xc0, 0xc0, 0xc0);
        public static Color DefaultCheckedForeColor = DefaultContextForeColor;
        public static Color DefaultIconForeColor = DefaultTitleForeColor;
        public static Color DefaultComBorderColor = Color.DeepSkyBlue;
        public static Color DefaultDisableBackColor = Color.FromArgb(0x2e, 0x2e, 0x2e);
        public static Color DefaultTitleBackColor = Color.FromArgb(61, 62, 69);//Color.FromArgb(0x36, 0x36, 0x36);
        public static Color DefaultContextBackColor = Color.FromArgb(41, 43, 50);// Color.FromArgb(0x2a, 0x2a, 0x2a);
        public static Color DefaultContextDarkBackColor = Color.FromArgb(0x20, 0x20, 0x20);
        public static Color DefaultAreaBackColor = Color.Black;

        public static Color DefaultCheckedBackColor = Color.FromArgb(0, 171, 209);//Color.FromArgb(0x28, 0x47, 0xC1);
        public static Color DefaultKeybordBackColor = Color.FromArgb(0, 162, 198);

        public static Color DefaultFocusBorderColor = DefaultTitleBackColor;
        public static Color DefaultBorderColor = Color.FromArgb(67, 69, 76); //DefaultTitleBackColor;
        public static Color DefaultEditeBorderColor = Color.DeepSkyBlue;
        public static Color DefaultKeyBoardUnSelctedForeColor = Color.Gray;
        public static Color DefaultFormActiveBorderColor = Color.FromArgb(93, 95, 94);

        public static Color DefaultGradualBackColorOne= Color.FromArgb(72, 77, 85);
        public static Color DefaultGradualBackColorTwo = Color.FromArgb(42, 43, 45);

        public static Color DefaultRunSotpRunBackColor = Color.FromArgb(0,214,56);
        public static Color DefaultRunSotpSingleStopBackColor = Color.FromArgb(200, 204, 54);
        public static Color DefaultRunSotpStopBackColor = Color.FromArgb(142, 11, 11);

        public static Color DefaultFormContentBackColor = Color.FromArgb(41, 43, 50);
        public static Color DefaultFormHeadBackColor = Color.FromArgb(50, 55, 65);
        public static Color DefaultComboBoxExBackColor = Color.FromArgb(33, 33, 38);
        public static Color DefaultNumberEditBackColor = Color.FromArgb(53, 54, 58);
        public static Color DefaultNavBarColor = Color.FromArgb(38, 38, 46);
        public static Color DefaultNavSplitColor = Color.FromArgb(50, 55, 65);

        public static Color ScopeXListPageBackColor = Color.FromArgb(50, 55, 65);
        public static Color ScopeXListPageHeaderBackColor = Color.FromArgb(72, 77, 85);

        //WindowDockPanel Color
        public static Color MainWindowActiveBackground = Color.Black;
        public static Color ToolWindowCaptionActiveBackground = Color.FromArgb(62, 62, 62);
        public static Color TabSelectedActiveBackground = Color.FromArgb(0, 209, 255);
        public static Color TabSelectedInactiveBackground = Color.FromArgb(38, 38, 46);
        public static Color TabSelectedActiveForeColor = Color.Black;
        public static Color TabSelectedInactiveForeColor = Color.White;
        public static Color SelectedActiveBorderBackColor = Color.FromArgb(0, 85, 104);
        public static Color SelectedInactiveBorderBackColor = Color.FromArgb(38, 38, 46);
    }

    public partial class AppStyleConfig : BaseStyleConfig
    {
        private static AppStyleConfig _Singleton = null;

        [JsonIgnore]
        public static AppStyleConfig Singleton
        {
            get
            {
                if (_Singleton == null)
                {
                    _Singleton = new AppStyleConfig();
                }
                return _Singleton;
            }
        }

        private AppStyleConfig()
        {
            InitControlStyleDir();
            InitStyleData();
            FreshStyleValueDir();
        }

        private void InitControlStyleDir()
        {
            Type type;
            Dictionary<string, string> dir;

            //ScopeXTextBox
            type = typeof(ScopeXTextBox);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.SelectedColor), AppStyleFiled.ScopeXTextBox_SelectedColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.BorderColor), AppStyleFiled.ScopeXTextBox_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.ClickedBorderColor), AppStyleFiled.ScopeXTextBox_ClickedBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.BorderThickness), AppStyleFiled.ScopeXTextBox_BorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.Height), AppStyleFiled.ScopeXTextBox_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.BackColor), AppStyleFiled.ScopeXTextBox_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.ForeColor), AppStyleFiled.ScopeXTextBox_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXTextBox>(x => x.Font), AppStyleFiled.ScopeXTextBox_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //ScopeXGroupBox
            type = typeof(ScopeXGroupBox);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXGroupBox>(x => x.BorderColor), AppStyleFiled.ScopeXGroupBox_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXGroupBox>(x => x.ForeColor), AppStyleFiled.ScopeXGroupBox_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXGroupBox>(x => x.Font), AppStyleFiled.ScopeXGroupBox_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //UIRadioButtonGroup
            type = typeof(UIRadioButtonGroup);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.BorderColor), AppStyleFiled.UIRadioButtonGroup_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.BorderThickness), AppStyleFiled.UIRadioButtonGroup_BorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ButtonBackColor), AppStyleFiled.UIRadioButtonGroup_ButtonBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ButtonOffset), AppStyleFiled.UIRadioButtonGroup_ButtonOffset.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.Height), AppStyleFiled.UIRadioButtonGroup_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ButtonTextColor), AppStyleFiled.UIRadioButtonGroup_ButtonTextColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ChoosedButtonColor), AppStyleFiled.UIRadioButtonGroup_ChoosedButtonColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ChoosedButtonTextColor), AppStyleFiled.UIRadioButtonGroup_ChoosedButtonTextColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ContentBackColor), AppStyleFiled.UIRadioButtonGroup_ContentBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ContentPadding), AppStyleFiled.UIRadioButtonGroup_ContentPadding.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.FocusBorderColor), AppStyleFiled.UIRadioButtonGroup_FocusBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<UIRadioButtonGroup>(x => x.ButtonFont), AppStyleFiled.UIRadioButtonGroup_ButtonFont.ToString());
            _controlStyleDir.Add(type, dir);

            //ScopeXSwitchButton
            type = typeof(ScopeXSwitchButton);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.BackColor), AppStyleFiled.ScopeXSwitchButton_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.BorderColor), AppStyleFiled.ScopeXSwitchButton_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.BorderThickness), AppStyleFiled.ScopeXSwitchButton_BorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.Height), AppStyleFiled.ScopeXSwitchButton_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.SliderButtonWidth), AppStyleFiled.ScopeXSwitchButton_SliderButtonWidth.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.CheckedBackColor), AppStyleFiled.ScopeXSwitchButton_CheckedBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.CheckedForeColor), AppStyleFiled.ScopeXSwitchButton_CheckedForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.CheckedSliderColor), AppStyleFiled.ScopeXSwitchButton_CheckedSliderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.FocusBorderColor), AppStyleFiled.ScopeXSwitchButton_FocusBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.ForeColor), AppStyleFiled.ScopeXSwitchButton_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.SliderColor), AppStyleFiled.ScopeXSwitchButton_SliderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.UseAnimation), AppStyleFiled.ScopeXSwitchButton_UseAnimation.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXSwitchButton>(x => x.Font), AppStyleFiled.ScopeXSwitchButton_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //ComboBoxEx
            type = typeof(ComboBoxEx);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.BackColor), AppStyleFiled.ComboBoxEx_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.BorderColor), AppStyleFiled.ComboBoxEx_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.BorderThickness), AppStyleFiled.ComboBoxEx_BorderThickness.ToString());
            //dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.DropDownWidth), AppStyleFiled.ComboBoxEx_DropDownWidth.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.FocusColor), AppStyleFiled.ComboBoxEx_FocusColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.ForeColor), AppStyleFiled.ComboBoxEx_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.Font), AppStyleFiled.ComboBoxEx_Font.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.Height), AppStyleFiled.ComboBoxEx_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.ItemHeight), AppStyleFiled.ComboBoxEx_ItemHeight.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.RectBtnWidth), AppStyleFiled.ComboBoxEx_RectBtnWidth.ToString());
            dir.Add(PropertyName.GetPropertyName<ComboBoxEx>(x => x.SelectedBackColor), AppStyleFiled.ComboBoxEx_SelectedBackColor.ToString());
            _controlStyleDir.Add(type, dir);

            //ScopeXNumericEditBox
            type = typeof(ScopeXNumericEditBox);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.BackColor), AppStyleFiled.ScopeXNumericEditBox_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.BorderColor), AppStyleFiled.ScopeXNumericEditBox_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.BorderThickness), AppStyleFiled.ScopeXNumericEditBox_BorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.Height), AppStyleFiled.ScopeXNumericEditBox_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.IconWidthProportion), AppStyleFiled.ScopeXNumericEditBox_IconWidthProportion.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.FocusBoederColor), AppStyleFiled.ScopeXNumericEditBox_FocusBoederColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.ClickedBorderColor), AppStyleFiled.ScopeXNumericEditBox_ClickedBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.FocusForeColor), AppStyleFiled.ScopeXNumericEditBox_FocusForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.ForeColor), AppStyleFiled.ScopeXNumericEditBox_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.Font), AppStyleFiled.ScopeXNumericEditBox_Font.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.AddButtonStyle.NomalStyle.BackColor), AppStyleFiled.ScopeXNumericEditBox_AddBtnNormalBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.AddButtonStyle.MouseInStyle.BackColor), AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseInBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.AddButtonStyle.MouseClickStyle.BackColor), AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseClickBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.SubButtonStyle.NomalStyle.BackColor), AppStyleFiled.ScopeXNumericEditBox_SubBtnNormalBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.SubButtonStyle.MouseInStyle.BackColor), AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseInBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.SubButtonStyle.MouseClickStyle.BackColor), AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseClickBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.AddButtonStyle.NomalStyle.ForeColor), AppStyleFiled.ScopeXNumericEditBox_AddBtnNormalForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.AddButtonStyle.MouseInStyle.ForeColor), AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseInForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.AddButtonStyle.MouseClickStyle.ForeColor), AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseClickForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.SubButtonStyle.NomalStyle.ForeColor), AppStyleFiled.ScopeXNumericEditBox_SubBtnNormalForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.SubButtonStyle.MouseInStyle.ForeColor), AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseInForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXNumericEditBox>(x => x.SubButtonStyle.MouseClickStyle.ForeColor), AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseClickForeColor.ToString());
            _controlStyleDir.Add(type, dir);

            //ScopeXIconButton
            type = typeof(ScopeXIconButton);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.BackColor), AppStyleFiled.ScopeXIconButton_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.BorderColor), AppStyleFiled.ScopeXIconButton_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.SVGForeColor), AppStyleFiled.ScopeXIconButton_SVGForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.BorderThickness), AppStyleFiled.ScopeXIconButton_BorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.Font), AppStyleFiled.ScopeXIconButton_Font.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.ForeColor), AppStyleFiled.ScopeXIconButton_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.FocusedBorderColor), AppStyleFiled.ScopeXIconButton_FocusedBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.ChoosedBackColor), AppStyleFiled.ScopeXIconButton_ChoosedBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.ChoosedForeColor), AppStyleFiled.ScopeXIconButton_ChoosedForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.ChoosedMouseinBackColor), AppStyleFiled.ScopeXIconButton_ChoosedMouseinBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.ChoosedPressedBackColor), AppStyleFiled.ScopeXIconButton_ChoosedPressedBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.MouseinBackColor), AppStyleFiled.ScopeXIconButton_MouseinBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.MouseinBorderColor), AppStyleFiled.ScopeXIconButton_MouseinBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.MouseInBorderThickness), AppStyleFiled.ScopeXIconButton_MouseInBorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.MouseinForeColor), AppStyleFiled.ScopeXIconButton_MouseinForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.MouseinSvgForeColor), AppStyleFiled.ScopeXIconButton_MouseinSvgForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.PressedBackColor), AppStyleFiled.ScopeXIconButton_PressedBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.PressedBorderColor), AppStyleFiled.ScopeXIconButton_PressedBorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.PressedBorderThickness), AppStyleFiled.ScopeXIconButton_PressedBorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.Height), AppStyleFiled.ScopeXIconButton_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.PressedForeColor), AppStyleFiled.ScopeXIconButton_PressedForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXIconButton>(x => x.PressedSvgForeColor), AppStyleFiled.ScopeXIconButton_PressedSvgForeColor.ToString());
            _controlStyleDir.Add(type, dir);
            
            //TabControlMenu
            type = typeof(TabControlMenu);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<TabControlMenu>(x => x.Font), AppStyleFiled.ScopeXIconButton_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //TabControlMenuPage
            type = typeof(TabControlMenuPage);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<TabControlMenuPage>(x => x.Font), AppStyleFiled.ScopeXIconButton_Font.ToString());
            _controlStyleDir.Add(type, dir);
            //ScopeXDateTimeBox
            type = typeof(ScopeXDateTimeBox);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXDateTimeBox>(x => x.Font), AppStyleFiled.ScopeXIconButton_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //checkbox
            type = typeof(CheckBox);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<CheckBox>(x => x.Font), AppStyleFiled.ScopeXIconButton_Font.ToString());
            //dir.Add(PropertyName.GetPropertyName<CheckBox>(x => x.ForeColor), AppStyleFiled.ScopeXIconButton_ForeColor.ToString());
            //dir.Add(PropertyName.GetPropertyName<CheckBox>(x => x.Height), AppStyleFiled.ScopeXIconButton_Height.ToString());
            _controlStyleDir.Add(type, dir);

            //UserControls.UserControls.SelectComboBox
            type = typeof(ScopeX.UserControls.SelectComboBox);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeX.UserControls.SelectComboBox>(x => x.Font), AppStyleFiled.SelectComboBox_Font.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeX.UserControls.SelectComboBox>(x => x.Height), AppStyleFiled.SelectComboBox_Height.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeX.UserControls.SelectComboBox>(x => x.ComBorderColor), AppStyleFiled.SelectComboBox_ComBorderColor.ToString());
            _controlStyleDir.Add(type, dir);
            

            //ScopeXLabel
            type = typeof(ScopeXLabel);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.BackColor), AppStyleFiled.ScopeXLabel_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.BorderColor), AppStyleFiled.ScopeXLabel_BorderColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.BorderThickness), AppStyleFiled.ScopeXLabel_BorderThickness.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.ForeColor), AppStyleFiled.ScopeXLabel_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.Font), AppStyleFiled.ScopeXLabel_Font.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.TextAlign), AppStyleFiled.ScopeXLabel_TextAlign.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXLabel>(x => x.Height), AppStyleFiled.ScopeXLabel_Height.ToString());
            _controlStyleDir.Add(type, dir);

            //Label
            type = typeof(Label);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<Label>(x => x.Font), AppStyleFiled.ScopeXLabel_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //ListViewNF
            type = typeof(ListViewNF);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ListViewNF>(x => x.Font), AppStyleFiled.ScopeXLabel_Font.ToString());
            _controlStyleDir.Add(type, dir);

            

            //ListViewEx
            type = typeof(ListViewEx);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ListViewEx>(x => x.Font), AppStyleFiled.ScopeXLabel_Font.ToString());
            _controlStyleDir.Add(type, dir);

            //NavBarGroup
            type = typeof(NavBarGroup);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<NavBarGroup>(x => x.NavBarColor), AppStyleFiled.NavBarGroup_NavBarColor.ToString());
            dir.Add(PropertyName.GetPropertyName<NavBarGroup>(x => x.NavGroupColor), AppStyleFiled.NavBarGroup_NavGroupColor.ToString());
            dir.Add(PropertyName.GetPropertyName<NavBarGroup>(x => x.NavForeColor), AppStyleFiled.NavBarGroup_NavForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<NavBarGroup>(x => x.SplitColor), AppStyleFiled.NavBarGroup_SplitColor.ToString());
            dir.Add(PropertyName.GetPropertyName<NavBarGroup>(x => x.NavBarHeight), AppStyleFiled.NavBarGroup_NavBarHeight.ToString());
            dir.Add(PropertyName.GetPropertyName<NavBarGroup>(x => x.Font), AppStyleFiled.NavBarGroup_Font.ToString());
            _controlStyleDir.Add(type, dir);
            //FloatForm
            type = typeof(FloatForm);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.ContentBackColor), AppStyleFiled.FloatForm_ContentBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.HeadBackColor), AppStyleFiled.FloatForm_HeadBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.IconInterval), AppStyleFiled.FloatForm_IconInterval.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.IconWidth), AppStyleFiled.FloatForm_IconWidth.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.ToolIconSize), AppStyleFiled.FloatForm_ToolIconSize.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.HeadHeight), AppStyleFiled.FloatForm_HeadHeight.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.IconSideDistance), AppStyleFiled.FloatForm_IconSideDistance.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.TitleAlign), AppStyleFiled.FloatForm_TitleAlign.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.TitleColor), AppStyleFiled.FloatForm_TitleColor.ToString());
            dir.Add(PropertyName.GetPropertyName<FloatForm>(x => x.TitleFont), AppStyleFiled.FloatForm_TitleFont.ToString());
            _controlStyleDir.Add(type, dir);

            //KeyboardForm
            type = typeof(KeyboardForm);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.BackColor), AppStyleFiled.KeyboardForm_BackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ForeColor), AppStyleFiled.KeyboardForm_ForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.UnSelectedForeColor), AppStyleFiled.KeyboardForm_UnSelectedForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ButtonClickedBackColor), AppStyleFiled.KeyboardForm_ButtonClickedBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ButtonMouseInBackColor), AppStyleFiled.KeyboardForm_ButtonMouseInBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ButtonNomalBackColor), AppStyleFiled.KeyboardForm_ButtonNomalBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.Font), AppStyleFiled.KeyboardForm_Font.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ContentBackColor), AppStyleFiled.KeyboardForm_ContentBackColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ContentForeColor), AppStyleFiled.KeyboardForm_ContentForeColor.ToString());
            dir.Add(PropertyName.GetPropertyName<KeyboardForm>(x => x.ContentFont), AppStyleFiled.KeyboardForm_ContentFont.ToString());
            _controlStyleDir.Add(type, dir);
          


            type = typeof(ScopeXListView);
            dir = new Dictionary<string, string>();
            dir.Add(PropertyName.GetPropertyName<ScopeXListView>(x => x.HeaderBackColor), AppStyleFiled.NavBarGroup_NavBarColor.ToString());
            dir.Add(PropertyName.GetPropertyName<ScopeXListView>(x => x.Font), AppStyleFiled.ScopeXTextBox_Font.ToString());
            _controlStyleDir.Add(type, dir);
        }

        private void InitStyleData()
        {
            //ScopeXTextBox
            colorDate.Add(AppStyleFiled.ScopeXTextBox_SelectedColor.ToString(), DefaultBorderColor);
            colorDate.Add(AppStyleFiled.ScopeXTextBox_BorderColor.ToString(), DefaultBorderColor);
            colorDate.Add(AppStyleFiled.ScopeXTextBox_ClickedBorderColor.ToString(), DefaultEditeBorderColor);
            intDate.Add(AppStyleFiled.ScopeXTextBox_BorderThickness.ToString(), DefaultBorderThickness);
            intDate.Add(AppStyleFiled.ScopeXTextBox_Height.ToString(), DefaultElementHeight);
            colorDate.Add(AppStyleFiled.ScopeXTextBox_BackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXTextBox_ForeColor.ToString(), DefaultContextForeColor);
            fontDate.Add(AppStyleFiled.ScopeXTextBox_Font.ToString(), DefaultContextFont);
            //ScopeXGroupBox
            colorDate.Add(AppStyleFiled.ScopeXGroupBox_BorderColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXGroupBox_ForeColor.ToString(), DefaultTitleForeColor);
            fontDate.Add(AppStyleFiled.ScopeXGroupBox_Font.ToString(), DefaultTitleFont);
            //UIRadioButtonGroup
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_BorderColor.ToString(), DefaultBorderColor);
            intDate.Add(AppStyleFiled.UIRadioButtonGroup_BorderThickness.ToString(), 2);
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_ButtonBackColor.ToString(), DefaultTitleBackColor);
            intDate.Add(AppStyleFiled.UIRadioButtonGroup_ButtonOffset.ToString(), 2);
            intDate.Add(AppStyleFiled.UIRadioButtonGroup_Height.ToString(), DefaultElementHeight);
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_ButtonTextColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_ChoosedButtonColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_ChoosedButtonTextColor.ToString(), Color.Black);
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_ContentBackColor.ToString(), DefaultBorderColor);
            paddingDate.Add(AppStyleFiled.UIRadioButtonGroup_ContentPadding.ToString(), new Padding(0));
            colorDate.Add(AppStyleFiled.UIRadioButtonGroup_FocusBorderColor.ToString(), DefaultFocusBorderColor);
            fontDate.Add(AppStyleFiled.UIRadioButtonGroup_ButtonFont.ToString(), DefaultContextFont);
            //ScopeXSwitchButton
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_BackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_BorderColor.ToString(), DefaultBorderColor);
            intDate.Add(AppStyleFiled.ScopeXSwitchButton_BorderThickness.ToString(), DefaultBorderThickness);
            intDate.Add(AppStyleFiled.ScopeXSwitchButton_Height.ToString(), DefaultElementHeight);
            intDate.Add(AppStyleFiled.ScopeXSwitchButton_SliderButtonWidth.ToString(), DefaultElementHeight);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_CheckedBackColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_CheckedForeColor.ToString(), Color.Black);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_CheckedSliderColor.ToString(), DefaultIconForeColor);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_FocusBorderColor.ToString(), DefaultFocusBorderColor);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_ForeColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.ScopeXSwitchButton_SliderColor.ToString(), DefaultIconForeColor);
            boolDate.Add(AppStyleFiled.ScopeXSwitchButton_UseAnimation.ToString(), false);
            fontDate.Add(AppStyleFiled.ScopeXSwitchButton_Font.ToString(), DefaultContextFont);
            //ComboBoxEx
            colorDate.Add(AppStyleFiled.ComboBoxEx_BackColor.ToString(), DefaultComboBoxExBackColor);
            colorDate.Add(AppStyleFiled.ComboBoxEx_BorderColor.ToString(), DefaultBorderColor);
            intDate.Add(AppStyleFiled.ComboBoxEx_BorderThickness.ToString(), DefaultBorderThickness);
            //intDate.Add(AppStyleFiled.ComboBoxEx_DropDownWidth.ToString(), 95);
            colorDate.Add(AppStyleFiled.ComboBoxEx_FocusColor.ToString(), DefaultFocusBorderColor);
            colorDate.Add(AppStyleFiled.ComboBoxEx_ForeColor.ToString(), DefaultContextForeColor);
            fontDate.Add(AppStyleFiled.ComboBoxEx_Font.ToString(), DefaultContextFont);
            intDate.Add(AppStyleFiled.ComboBoxEx_Height.ToString(), DefaultElementHeight);
            intDate.Add(AppStyleFiled.ComboBoxEx_ItemHeight.ToString(), 35);
            intDate.Add(AppStyleFiled.ComboBoxEx_RectBtnWidth.ToString(), 20);
            colorDate.Add(AppStyleFiled.ComboBoxEx_SelectedBackColor.ToString(), DefaultCheckedBackColor);
            //ScopeXNumericEditBox
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_BackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_BorderColor.ToString(), DefaultTitleBackColor);
            intDate.Add(AppStyleFiled.ScopeXNumericEditBox_BorderThickness.ToString(), DefaultBorderThickness);
            intDate.Add(AppStyleFiled.ScopeXNumericEditBox_Height.ToString(), DefaultElementHeight);
            floatDate.Add(AppStyleFiled.ScopeXNumericEditBox_IconWidthProportion.ToString(), 1.6F);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_FocusBoederColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_ClickedBorderColor.ToString(), DefaultEditeBorderColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_FocusForeColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_ForeColor.ToString(), DefaultContextForeColor);
            fontDate.Add(AppStyleFiled.ScopeXNumericEditBox_Font.ToString(), DefaultContextFont);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_AddBtnNormalBackColor.ToString(), DefaultNumberEditBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseInBackColor.ToString(), DefaultNumberEditBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseClickBackColor.ToString(), DefaultNumberEditBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_SubBtnNormalBackColor.ToString(), DefaultNumberEditBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseInBackColor.ToString(), DefaultNumberEditBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseClickBackColor.ToString(), DefaultNumberEditBackColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_AddBtnNormalForeColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseInForeColor.ToString(), DefaultTitleForeColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_AddBtnMouseClickForeColor.ToString(), DefaultTitleForeColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_SubBtnNormalForeColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseInForeColor.ToString(), DefaultTitleForeColor);
            colorDate.Add(AppStyleFiled.ScopeXNumericEditBox_SubBtnMouseClickForeColor.ToString(), DefaultTitleForeColor);
            //ScopeXIconButton
            colorDate.Add(AppStyleFiled.ScopeXIconButton_BackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_BorderColor.ToString(), DefaultBorderColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_SVGForeColor.ToString(), DefaultContextForeColor);
            intDate.Add(AppStyleFiled.ScopeXIconButton_BorderThickness.ToString(), DefaultBorderThickness);
            fontDate.Add(AppStyleFiled.ScopeXIconButton_Font.ToString(), DefaultContextFont);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_ForeColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_MouseinBackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_ChoosedBackColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_ChoosedForeColor.ToString(), DefaultCheckedForeColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_ChoosedMouseinBackColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_ChoosedPressedBackColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_FocusedBorderColor.ToString(), DefaultEditeBorderColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_MouseinBorderColor.ToString(), DefaultFocusBorderColor);
            intDate.Add(AppStyleFiled.ScopeXIconButton_MouseInBorderThickness.ToString(), DefaultBorderThickness);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_MouseinForeColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_MouseinSvgForeColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_PressedBackColor.ToString(), Color.Gray);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_PressedBorderColor.ToString(), DefaultFocusBorderColor);
            intDate.Add(AppStyleFiled.ScopeXIconButton_PressedBorderThickness.ToString(), DefaultBorderThickness);
            intDate.Add(AppStyleFiled.ScopeXIconButton_Height.ToString(), DefaultElementHeight);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_PressedForeColor.ToString(), DefaultContextForeColor);
            colorDate.Add(AppStyleFiled.ScopeXIconButton_PressedSvgForeColor.ToString(), DefaultCheckedBackColor);
            //SelectComboBox
            fontDate.Add(AppStyleFiled.SelectComboBox_Font.ToString(), DefaultContextFont);
            intDate.Add(AppStyleFiled.SelectComboBox_Height.ToString(), DefaultElementHeight);
            colorDate.Add(AppStyleFiled.SelectComboBox_ComBorderColor.ToString(), DefaultComBorderColor);
            //ScopeXLabel
            colorDate.Add(AppStyleFiled.ScopeXLabel_BackColor.ToString(), DefaultContextBackColor);
            colorDate.Add(AppStyleFiled.ScopeXLabel_BorderColor.ToString(), DefaultBorderColor);
            intDate.Add(AppStyleFiled.ScopeXLabel_BorderThickness.ToString(), 0);
            colorDate.Add(AppStyleFiled.ScopeXLabel_ForeColor.ToString(), DefaultTitleForeColor);
            fontDate.Add(AppStyleFiled.ScopeXLabel_Font.ToString(), DefaultLabelFont);
            contentAlignmentDate.Add(AppStyleFiled.ScopeXLabel_TextAlign.ToString(), ContentAlignment.MiddleLeft);
            intDate.Add(AppStyleFiled.ScopeXLabel_Height.ToString(), DefaultLabelHeight);
            //NavBarGroup
            colorDate.Add(AppStyleFiled.NavBarGroup_NavBarColor.ToString(), DefaultNavBarColor);
            colorDate.Add(AppStyleFiled.NavBarGroup_NavGroupColor.ToString(), DefaultContextBackColor);
            colorDate.Add(AppStyleFiled.NavBarGroup_NavForeColor.ToString(), DefaultTitleForeColor);
            colorDate.Add(AppStyleFiled.NavBarGroup_SplitColor.ToString(), DefaultNavSplitColor);
            intDate.Add(AppStyleFiled.NavBarGroup_NavBarHeight.ToString(), 40);
            fontDate.Add(AppStyleFiled.NavBarGroup_Font.ToString(), DefaultTitleFont);
            //FloatForm
            colorDate.Add(AppStyleFiled.FloatForm_ContentBackColor.ToString(), DefaultFormContentBackColor);
            colorDate.Add(AppStyleFiled.FloatForm_HeadBackColor.ToString(), DefaultFormHeadBackColor);
            intDate.Add(AppStyleFiled.FloatForm_IconInterval.ToString(), 21);
            intDate.Add(AppStyleFiled.FloatForm_IconWidth.ToString(), DefaultIconWidth);
            sizeDate.Add(AppStyleFiled.FloatForm_ToolIconSize.ToString(), new Size(DefaultIconWidth, DefaultIconHeight));
            intDate.Add(AppStyleFiled.FloatForm_HeadHeight.ToString(), 45);
            intDate.Add(AppStyleFiled.FloatForm_IconSideDistance.ToString(), 10);
            contentAlignmentDate.Add(AppStyleFiled.FloatForm_TitleAlign.ToString(), ContentAlignment.MiddleLeft);
            colorDate.Add(AppStyleFiled.FloatForm_TitleColor.ToString(), DefaultTitleForeColor);
            fontDate.Add(AppStyleFiled.FloatForm_TitleFont.ToString(), DefaultTitleFont);

            //KeyboardForm
            colorDate.Add(AppStyleFiled.KeyboardForm_BackColor.ToString(), DefaultFormContentBackColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_ForeColor.ToString(), DefaultCheckedForeColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_UnSelectedForeColor.ToString(), DefaultKeyBoardUnSelctedForeColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_ButtonClickedBackColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_ButtonMouseInBackColor.ToString(), DefaultCheckedBackColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_ButtonNomalBackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_ContentBackColor.ToString(), DefaultTitleBackColor);
            colorDate.Add(AppStyleFiled.KeyboardForm_ContentForeColor.ToString(), Color.White);
            fontDate.Add(AppStyleFiled.KeyboardForm_Font.ToString(), DefaultKeyFont);
            fontDate.Add(AppStyleFiled.KeyboardForm_ContentFont.ToString(), DefaultKeyFont);
        }

    }
}
