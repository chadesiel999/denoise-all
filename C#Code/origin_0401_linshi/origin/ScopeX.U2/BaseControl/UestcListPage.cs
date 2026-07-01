using Svg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ScopeX;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ScopeX.Controls.LanguageDefinition;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    /// <summary>
    /// 专用：测量结果显示页面
    /// </summary>
    public class ScopeXListPage : Control, ILanguageControl
    {
        ScopeXListPageDropDown ScopeXListPageDropDown;
        private IList dataSource;
        private int selectedIndex;
        private string header = "header";
        private string headerInfo = "info";
        private Color headerForeColor = Color.White;
        private Color headerBackColor = Color.Green;
        private string displayMember;
        private string valueMember;
        private bool droppedDown = false;
        private bool _ShowIndicator = false;
        private bool _ShowHeader = true;
        private float percentage = 0.45f;
        private Color backColor;
        private Color borderColor = Color.Black;
        private int borderThickness = 1;
        private Color displayForeColor = Color.Green;
        private Color valueForeColor = Color.White;
        private int maxItemViewCount = 5;
        private int[] dashArray;

        public Int32 DropDownPageHeight
        {
            get => ScopeXListPageDropDown.Height;
        }

        public ScopeXListPage()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ScopeXListPageDropDown = new ScopeXListPageDropDown(this);
            ScopeXListPageDropDown.Click += (_, _) =>
            {
                if (AutoHide) ScopeXListPageDropDown.Hide();
                OnClick(EventArgs.Empty);
            };
            ScopeXListPageDropDown.MouseClick += (_, e) =>
            {
                if (AutoHide) ScopeXListPageDropDown.Hide();
                OnMouseClick(e);
            };
            AutoHide = false;
            //需要激活本控件
            this.Click += (s, e) => this.Select();
        }

        public void SetParent() => ScopeXListPageDropDown?.SetParent();

        protected override void OnPaint(PaintEventArgs e)
        {
            ISvgRenderer svgRenderer = SvgRenderer.FromGraphics(e.Graphics);
            SvgGroup svgGroup = new SvgGroup();
            if (DroppedDown)
            {
                if (DataSource != null && DataSource.Count > 0)
                {
                    ScopeXListPageDropDown.Refresh();
                    ScopeXListPageDropDown.Show();
                }
                return;
            }
            ScopeXListPageDropDown.Hide();
            Rectangle rectangle = e.ClipRectangle;
            List<(string, RectangleF, Color, TextFormatFlags, Font)> textinfo = new List<(string, RectangleF, Color, TextFormatFlags, Font)>();
            if (EnbleNomalBorder && BorderColor != Color.Transparent)
            {
                svgGroup.Children.Add(new SvgRectangle()
                {
                    X = 0,
                    Y = 0,
                    Height = BorderThickness,
                    Width = BorderThickness,
                    Fill = new SvgColourServer(BorderColor),
                    FillOpacity = 1,
                });
            }

            Int32 HeaderWidth = ShowHeader ? rectangle.Height + 10: 0;
            //判断是否显示头内容
            if(ShowHeader)
            {
                svgGroup.Children.Add(new SvgRectangle()
                {
                    X = EnbleNomalBorder ? borderThickness * 0.5f : 0,
                    Y = EnbleNomalBorder ? borderThickness * 0.5f : 0,
                    CornerRadiusX = 0,
                    CornerRadiusY = 0,
                    Height = rectangle.Height - (EnbleNomalBorder ? borderThickness * 1f : 0),
                    Width = HeaderWidth - (EnbleNomalBorder ? borderThickness * 1f : 0),
                    StrokeWidth = new SvgUnit(EnbleNomalBorder ? borderThickness : 0),
                    Stroke = new SvgColourServer(borderColor),
                    Fill = new SvgColourServer(HeaderBackColor),
                    FillOpacity = 1,
                    StrokeOpacity = EnbleNomalBorder ? 1 : 0
                });
                if (ShowIndicator)
                {
                    textinfo.Add(("↑", new RectangleF(0, 0, 20, Height), Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left, HeaderFont));
                }
                textinfo.Add((Header, new RectangleF(0, 0, HeaderWidth, Height / 2), HeaderForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter, HeaderFont));
                textinfo.Add((HeaderInfo, new RectangleF(0, Height / 2, HeaderWidth, Height / 2), HeaderForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter, HeaderInfoFont));
            }
            svgGroup.Children.Add(new SvgRectangle()
            {
                X = HeaderWidth,
                Y = 0,
                Height = rectangle.Height,
                Width = rectangle.Width - HeaderWidth,
                Fill = new SvgColourServer(BackColor),
                FillOpacity = 1,
            });
            if (EnbleNomalBorder)
            {
                svgGroup.Children.Add(new SvgRectangle()
                {
                    X = borderThickness * 0.5f,
                    Y = borderThickness * 0.5f,
                    Height = Height - borderThickness,
                    Width = Width - borderThickness,
                    Stroke = new SvgColourServer(borderColor),
                    StrokeWidth = new SvgUnit(borderThickness),
                    FillOpacity = 0,
                    StrokeDashArray = new SvgUnitCollection()
                });
                if (DashArray != null && DashArray.Length > 0)
                {
                    foreach (var val in DashArray) (svgGroup.Children[^1] as SvgRectangle).StrokeDashArray.Add(val);
                }
            }
            svgGroup.RenderElement(svgRenderer);

            if (SelectedIndex >= 0 && DataSource != null && DataSource.Count > SelectedIndex)
            {
                textinfo.Add((MeasItemName, new RectangleF(HeaderWidth, -2, Width - HeaderWidth, measNameItemHeight), DisplayForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, Font));
                textinfo.Add((GetValueMemberValue(SelectedIndex) + "", new RectangleF(HeaderWidth, measNameItemHeight - 6, Width - HeaderWidth, Height - measNameItemHeight + 6), ValueForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, ValueFont));
            }
            else
            {
                textinfo.Add((MeasItemName, new RectangleF(HeaderWidth, 0, Width - Height, Height), DisplayForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, Font));
            }
            textinfo.ForEach(x =>
            {
                TextRenderer.DrawText(e.Graphics, x.Item1, x.Item5, Rectangle.Round(x.Item2), x.Item3, x.Item4);
            });
        }
        /// <summary>
        /// 使项数据与数据源的内容重新同步。
        /// </summary>
        public void RefreshItems()
        {
            if (droppedDown) ScopeXListPageDropDown?.Refresh();
            else this.Invalidate(false);
        }
        internal object GetDisplayMemberValue(int index) => GetValue(index, DisplayMember);
        internal object GetValue(int index, string memberName)
        {
            if (DataSource == null) return "";
            if (DataSource.Count <= index) return "";
            if (string.IsNullOrEmpty(memberName)) return "";
            try
            {
                var memberinfo = DataSource[index].GetType().GetMember(memberName)?.First();
                if (memberinfo == null) return "";
                else
                {
                    switch (memberinfo.MemberType)
                    {
                        case System.Reflection.MemberTypes.Field:
                            return (memberinfo as FieldInfo).GetValue(DataSource[index]);
                        case MemberTypes.Property:
                            return (memberinfo as PropertyInfo).GetValue(DataSource[index]);
                        default:
                            return "";
                    }
                }
            }
            catch { }
            return "";
        }
        internal object GetValueMemberValue(int index) => GetValue(index, ValueMember);
        /// <summary>
        /// 获取或设置此<see cref="ScopeXListPage"/>的数据源。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置此ScopeXListPage的数据源"), DefaultValue(typeof(IList)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IList DataSource
        {
            get => dataSource;
            set
            {
                dataSource = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }
        /// <summary>
        /// 获取或设置当前选定项的从零开始的索引。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置当前选定项的从零开始的索引"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                if (!droppedDown) Invalidate(false);
            }
        }
        /// <summary>
        /// 获取或设置标头字符串。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置标头字符串"), DefaultValue(typeof(string)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Header
        {
            get => header;
            set
            {
                header = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置是否显示标头区域
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置是否显示标头区域"), DefaultValue(typeof(Boolean)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Boolean ShowHeader
        {
            get => _ShowHeader;
            set
            {
                if (_ShowHeader != value)
                {
                    _ShowHeader = value;
                }
            }
        }


        /// <summary>
        /// 获取或设置是否显示指示器
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置是否显示标头字符串下划线"), DefaultValue(typeof(string)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowIndicator
        {
            get => _ShowIndicator;
            set
            {
                if (_ShowIndicator != value)
                {
                    _ShowIndicator = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置标头信息字符串。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置标头信息字符串"), DefaultValue(typeof(string)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string HeaderInfo
        {
            get => headerInfo;
            set
            {
                headerInfo = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }
        /// <summary>
        /// 获取或设置<see cref="Header"/>项字体颜色。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Header项字体颜色"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HeaderForeColor
        {
            get => headerForeColor;
            set
            {
                headerForeColor = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }
        /// <summary>
        /// 获取或设置<see cref="Header"/>项背景颜色
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Header项背景颜色"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HeaderBackColor
        {
            get =>
                  headerBackColor;
            set
            {
                headerBackColor = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }
        private bool enbleNomalBorder = true;
        private LanguagePattern languagePattern;

        /// <summary>
        /// 获取或设置控制在常规状态下是否显示边框
        /// </summary>
        [Category(Const.Category), DefaultValue(typeof(bool)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("获取或设置控制在常规状态下是否显示边框")]
        public bool EnbleNomalBorder
        {
            get { return enbleNomalBorder; }
            set
            {
                enbleNomalBorder = value;
                if (!droppedDown) this.Invalidate(true);
            }
        }

        /// <summary>
        /// 获取或设置要为此<see cref="ScopeXListPage"/>显示的属性。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置要为此ScopeXListPage显示的属性"), DefaultValue(typeof(string)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string DisplayMember
        {
            get => displayMember;
            set
            {
                displayMember = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置属性的路径，它将用作<see cref="ScopeXListPage"/>中的项的实际值
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置属性的路径，它将用作ScopeXListPage中的项的实际值"), DefaultValue(typeof(string)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ValueMember
        {
            get => valueMember;
            set
            {
                valueMember = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示组合框是否正在显示其所有项。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置一个值，该值指示组合框是否正在显示其所有项"), DefaultValue(typeof(bool)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool DroppedDown
        {
            get => droppedDown;
            set
            {
                if (value != droppedDown)
                {
                    if (value && RunTimeDisplayItemCount == 0) return;
                    this.SetAndRefresh(ref droppedDown, value);
                    DroppedDownChanged?.Invoke(this, EventArgs.Empty);
                }
                if (ScopeXListPageDropDown != null)
                    ScopeXListPageDropDown.Visible = droppedDown && Visible;
            }
        }

        /// <summary>
        /// 获取或设置现实的属性宽度与实际值宽度的比例,取值范围为0%~100%
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置现实的属性宽度与实际值宽度的比例"), DefaultValue(typeof(float)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Percentage
        {
            get => Compare.CheckRange(percentage, 1f, 0f);
            set
            {
                percentage = Compare.CheckRange(value, 1f, 0f);
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示<see cref="ScopeXListPage"/>控件是否应在失去激活状态时自动关闭
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置一个值，该值指示ScopeXListPage控件是否应在失去激活状态时自动关闭"), DefaultValue(typeof(bool)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AutoHide
        {
            get => ScopeXListPageDropDown.AutoClose;
            set
            {
                ScopeXListPageDropDown.AutoClose = value;
            }
        }
        /// <summary>
        /// 获取或设置一个值，该值指示<see cref="ScopeXListPage"/>控件是否显示或者隐藏
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置一个值，该值指示ScopeXListPage控件是否显示或者隐藏"), DefaultValue(typeof(bool)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                ScopeXListPageDropDown.Visible = value && droppedDown;
                this.Refresh();

            }
        }
        /// <summary>
        /// 获取或设置控件背景颜色
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件背景颜色"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Color BackColor
        {
            get => backColor;
            set
            {
                backColor = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置控件边框颜色
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件边框颜色"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置边框大小
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置边框大小"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get => borderThickness; set
            {
                borderThickness = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
            }
        }

        /// <summary>
        /// 获取或设置属性名区域文字颜色
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置属性名区域文字颜色"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DisplayForeColor
        {
            get => displayForeColor; set
            {
                displayForeColor = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置属性值区域文字颜色。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置属性值区域文字颜色"), DefaultValue(typeof(Color)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ValueForeColor
        {
            get => valueForeColor;
            set
            {
                valueForeColor = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
                else this.Invalidate(false);
            }
        }

        /// <summary>
        /// 获取或设置控件最大显示的项目数。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件最大显示的项目数"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxItemViewCount
        {
            get => maxItemViewCount;
            set
            {
                maxItemViewCount = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
            }
        }

        /// <summary>
        /// 多语言处理方式。
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("多语言处理方式"), DefaultValue(typeof(LanguagePattern)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public LanguagePattern LanguagePattern { get => languagePattern; set => languagePattern = value; }

        /// <summary>
        /// 获取或设置控件边框的虚线样式,详细信息参考SVG文档的DashArray属性
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件边框的虚线样式,详细信息参考SVG文档的DashArray属性"), DefaultValue(typeof(int[])), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int[] DashArray
        {
            get => dashArray;
            set
            {
                dashArray = value;
                if (droppedDown) ScopeXListPageDropDown?.Refresh();
            }
        }
        /// <summary>
        /// 获取或设置控件<see cref="MeasItemName"/>项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件MeasName项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font { get => base.Font; set => base.Font = value; }

        private Font headerFont = new Font("Arial", 10.5f);
        /// <summary>
        /// 获取或设置控件<see cref="Header"/>项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件Header项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Font HeaderFont
        {
            get { return headerFont; }
            set { headerFont = value; }
        }


        private Font headerInfoFont = new Font("Arial", 10.5f);
        /// <summary>
        /// 获取或设置控件<see cref="HeaderInfo"/>项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件HeaderInfo项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Font HeaderInfoFont
        {
            get { return headerInfoFont; }
            set { headerInfoFont = value; }
        }

        private Font valueFont = new Font("Arial", 10.5f);
        /// <summary>
        /// 获取或设置控件显示项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置控件显示项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Font ValueFont
        {
            get { return valueFont; }
            set { valueFont = value; }
        }
        private Font dropedMeasNameItemFont = new Font("Arial", 10.5f);
        /// <summary>
        /// 获取或设置Drop窗口中<see cref="MeasItemName"/>项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Drop窗口中MeasName项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public Font DropedMeasNameItemFont
        {
            get { return dropedMeasNameItemFont; }
            set { dropedMeasNameItemFont = value; }
        }

        private Font dropedHeaderFont = new Font("Arial", 10.5f);
        /// <summary>
        /// 获取或设置Drop窗口中<see cref="Header"/>项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Drop窗口中Header项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font DropedHeaderFont
        {
            get { return dropedHeaderFont; }
            set { dropedHeaderFont = value; }
        }

        private Font dropedValueFont = new Font("Arial", 10.5f);
        /// <summary>
        /// 获取或设置Drop窗口中显示项的字体
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Drop窗口中显示项的字体"), DefaultValue(typeof(Font)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font DropedValueFont
        {
            get { return dropedValueFont; }
            set { dropedValueFont = value; }
        }
        private int dropedHeaderHeight = 24;
        /// <summary>
        /// 获取或设置Drop窗口中<see cref="Header"/>项的高度
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Drop窗口中Header项的高度"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DropedHeaderHeight
        {
            get { return dropedHeaderHeight; }
            set { dropedHeaderHeight = value; }
        }

        private int dropedValueItemHeight = 24;
        /// <summary>
        /// 获取或设置Drop窗口中显示项的高度
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置Drop窗口中显示项的高度"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DropedValueItemHeight
        {
            get { return dropedValueItemHeight; }
            set { dropedValueItemHeight = value; }
        }


        /// <summary>
        /// 控件前景色，当前控件无效
        /// </summary>
        [Browsable(false)]
        [Obsolete]
        private new Color ForeColor { get => base.ForeColor; set => base.ForeColor = value; }
        /// <summary>
        /// 运行时控件显示的项目数量
        /// </summary>
        [Browsable(false)]
        public int RunTimeDisplayItemCount => (DataSource != null ? (Math.Min(DataSource.Count, MaxItemViewCount)) : 0);

        private int dropedMeasNameItemHeight = 20;
        /// <summary>
        /// 获取或设置标题高度
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置标题高度"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DropedMeasNameItemHeight
        {
            get { return dropedMeasNameItemHeight; }
            set { this.SetAndRefresh(ref dropedMeasNameItemHeight, value); }
        }
        private int measNameItemHeight = 24;
        /// <summary>
        /// 获取或设置标题高度
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置标题高度"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MeasNameItemHeight
        {
            get { return measNameItemHeight; }
            set { this.SetAndRefresh(ref measNameItemHeight, value); }
        }
        private string measItemName;
        /// <summary>
        /// 获取或设置标题名称
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置标题名称"), DefaultValue(typeof(string)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string MeasItemName
        {
            get { return measItemName; }
            set { this.SetAndRefresh(ref measItemName, value); }
        }

        /// <summary>
        /// 获取或设置一个值，该值声明是否可以返回此元素作为其呈现内容的某些部分的点击测试结果。
        /// 当为<see cref="true"/>时Drop窗口可以响应鼠标事件
        /// 当为<see cref="false"/>时Drop窗口不会响应任何事件
        /// 当<see cref="DropBackOpacity"/>或者<see cref="DropOpacity"/>为<see cref="0"/>时此属性无效，不响应任何事件
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置一个值，该值声明是否可以返回此元素作为其呈现内容的某些部分的点击测试结果。"), DefaultValue(typeof(bool)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsHitTestVisible
        {
            get => ScopeXListPageDropDown.IsHitTestVisible;
            set => ScopeXListPageDropDown.IsHitTestVisible = value;
        }
        /// <summary>
        /// 获取或设置一个值，该值为Drop窗口的背景透明度
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置一个值，该值为Drop窗口的背景透明度"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DropBackOpacity { get => ScopeXListPageDropDown.Backopacity; set => ScopeXListPageDropDown.Backopacity = value; }
        /// <summary>
        /// 获取或设置一个值，该值为Drop窗口的透明度
        /// </summary>
        [Category(Const.Category), Browsable(true), Description("获取或设置一个值，该值为Drop窗口的透明度"), DefaultValue(typeof(int)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DropOpacity { get => ScopeXListPageDropDown.Opacity; set => ScopeXListPageDropDown.Opacity = value; }
        /// <summary>
        /// <see cref="DroppedDown"/>属性改变事件
        /// </summary>
        [Browsable(true), Description("DroppedDown属性改变事件")]
        public event EventHandler DroppedDownChanged;
    }

}
