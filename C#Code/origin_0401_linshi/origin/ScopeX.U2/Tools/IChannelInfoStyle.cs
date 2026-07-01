using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.UserControls.Style;
using ScopeX.UserControls;
using ScopeX.ComModel;

namespace ScopeX.U2
{
    /// <summary>
    /// 通道信息栏风格接口
    /// 实际上是当成一个抽象类来用
    /// </summary>
    internal interface IChannelInfoStyle
    {
        //需实现的属性
        Color TitleBackColor { set; get; }
        Color TitleForeColor { set; get; }
        Color ContentBackColor { set; get; }
        Color ContentForeColor { set; get; }
        Color BottomBarBackColor { set; get; }
        int BottomBarBackThickness { set; get; }

        //需实现的方法
        void SetContentStyle();
        void SetUnfocusedTitleStyle();
        void SetDeactiveTitleStyle();
        void SetFocusedTitleStyle();

        //实现的默认方法
        public void DefaultSetContentStyle()
        {
            ContentBackColor = ChannelInfoStyleDefine.ContentBackColor;
            ContentForeColor = ChannelInfoStyleDefine.ContentForeColor;
        }

        public void DefaultSetFocusedTitleStyle(Color channelColor)
        {
            TitleBackColor = channelColor;
            TitleForeColor = ChannelInfoStyleDefine.TitleDeactiveForeColor;

            BottomBarBackThickness = 4;
            BottomBarBackColor = channelColor.GetBrightnessColor(0.5D);
        }

        public void DefaultSetUnfocusedTitleStyle(Color channelColor)
        {
            TitleBackColor = channelColor;
            TitleForeColor = ChannelInfoStyleDefine.TitleDeactiveForeColor;

            BottomBarBackThickness = 0;
        }

        public void DefaultSetDeactiveTitleStyle()
        {
            TitleBackColor = ChannelInfoStyleDefine.TitleDeactiveBackColor;
            TitleForeColor = ChannelInfoStyleDefine.TitleDeactiveForeColor;

            BottomBarBackThickness = 0;
        }
    }

    internal class ChannelInfoStyleDefine
    {
        //通道信息标签的大小
        public static Size ChannelInfoSize = new Size(120, 110);

        //标题栏相关属性定义
        public static Color TitleDeactiveBackColor = Color.FromArgb(72, 77, 85);//SystemColors.ControlDarkDark;
        public static Color TitleDeactiveForeColor = AppStyleConfig.DefaultTitleForeColor;
        public static int TitleHeight = 28;

        //内容区域相关属性定义
        public static Color ContentForeColor = AppStyleConfig.DefaultContextForeColor;
        public static Color ContentBackColor = Color.FromArgb(53, 57, 68);//AppStyleConfig.DefaultContextDarkBackColor;

        //字体属性定义
        public static Font BoldFont = new Font("MiSans", 12.5F, GraphicsUnit.Point);//AppStyleConfig.DefaultBoldFont;
        public static Font NormalFont = AppStyleConfig.DefaultLabelFont;
        //小字体 ljw
        public static Font Smallfont = new Font("MiSans Medium", 11F, FontStyle.Regular, GraphicsUnit.Point);

        public static int BottomBarThickness = 4;
    }
}
