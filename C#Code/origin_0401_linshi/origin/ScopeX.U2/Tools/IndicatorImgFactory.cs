using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    /// <summary>
    /// 波形指示器图标工厂
    /// </summary>
    public class IndicatorImgFactory
    {
        //图标包含文字的字体名
        private const String _FontName = AppStyleConfig.MiSansBoldFamilyName;
        //图标对应波形的ChannelId的同类型ID的最小值
        private ChannelId _SameChlTypeMin;
        //图标对应波形的ChannelId的同类型ID的最大值
        private ChannelId _SameChlTypeMax;

        private Size _ImgSize = new Size(24, 24);
        /// <summary>
        /// 图标的大小
        /// </summary>
        public Size ImgSize
        {
            get => _ImgSize;
            set
            {
                _ImgSize = value;
            }
        }

        private Int32 _EdgeDistance = 0;
        /// <summary>
        /// 图标内容离边界的距离
        /// </summary>
        public Int32 EdgeDistance
        {
            get => _EdgeDistance;
            set
            {
                _EdgeDistance = value;
            }
        }

        public IndicatorImgFactory(ChannelId sameChlTypeMin = 0, ChannelId sameChlTypeMax = 0)
        {
            _SameChlTypeMin = sameChlTypeMin;
            _SameChlTypeMax = sameChlTypeMax;
        }

        /// <summary>
        /// 获取通道号对应通道的Bitmap图标
        /// </summary>
        /// <param name="chlId">通道号</param>
        /// <param name="status">指示器状态</param>
        /// <param name="chlColor">通道对应颜色</param>
        /// <returns></returns>
        public Bitmap GetChannelImg(ChannelId chlId, IndicatorStatus status, Color chlColor)
        {
            if (chlId >= ChannelId.D0 && chlId <= ChannelId.D47)
            {
                return GetDigitalImg("D" + (chlId - ChannelId.D0).ToString(), status, chlColor);
            }
            else
            {
                return GetCommonImg(chlId.ToString(), status, chlColor);
            }
        }

        public Bitmap GetCursorImg(CursorIconKind cursorKind, Color drawColor, Boolean byText = true)
        {
            if (byText)
            {
                return GetCursorImgByText(cursorKind, drawColor);
            }
            String contentsvgpath = String.Empty;
            EdgeDistance = 1;
            IndicatorRotateAngle rAngle = IndicatorRotateAngle.AntiClockWise0;
            Bitmap retimg = new Bitmap(ImgSize.Width, ImgSize.Height);

            switch (cursorKind)
            {
                case CursorIconKind.CursorSyncV:
                    Bitmap vimg = new Bitmap(GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Resources.WaveIndicator.CusorSync.png"));
                    vimg.RotateFlip(RotateFlipType.Rotate90FlipX);
                    Rectangle targetarea = new Rectangle(retimg.Width - vimg.Width, 1, vimg.Width, retimg.Height - 2);
                    RenderBack(ref retimg, targetarea);
                    Graphics.FromImage(retimg).DrawImage(vimg, targetarea, new Rectangle(0, 0, vimg.Width, vimg.Height), GraphicsUnit.Pixel);
                    return retimg;
                case CursorIconKind.CursorSyncH:
                    Bitmap himg = new Bitmap(GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Resources.WaveIndicator.CusorSync.png"));
                    Rectangle tarea = new Rectangle(1, retimg.Height - himg.Height, retimg.Width - 2, himg.Height);
                    RenderBack(ref retimg, tarea);
                    Graphics.FromImage(retimg).DrawImage(himg, tarea, new Rectangle(0, 0, himg.Width, himg.Height), GraphicsUnit.Pixel);
                    return retimg;
                case CursorIconKind.CursorAV:
                    rAngle = IndicatorRotateAngle.AntiClockWise0;
                    contentsvgpath = Properties.Resources.WaveIndicatorCursorASvg;
                    break;
                case CursorIconKind.CursorAH:
                    rAngle = IndicatorRotateAngle.AntiClockWise90;
                    contentsvgpath = Properties.Resources.WaveIndicatorCursorASvg;
                    break;
                case CursorIconKind.CursorBV:
                    rAngle = IndicatorRotateAngle.AntiClockWise0;
                    contentsvgpath = Properties.Resources.WaveIndicatorCursorBSvg;
                    break;
                case CursorIconKind.CursorBH:
                    contentsvgpath = Properties.Resources.WaveIndicatorCursorBSvg;
                    rAngle = IndicatorRotateAngle.AntiClockWise90;
                    break;
                default:
                    throw new ArgumentException("cursorKind");
            }

            //画内容
            Bitmap contentimg = new Bitmap(retimg.Width - EdgeDistance * 2, retimg.Height - EdgeDistance * 2);

            SvgPath svgpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(Properties.Resources.WaveIndicatorCursorAreaSvg),
                StrokeWidth = 0,
                Stroke = new SvgColourServer(Color.Black),
                Fill = new SvgColourServer(drawColor),
                FillOpacity = 100,
            };

            //旋转
            svgpath.AddTransform(GetCommonMatrix(rAngle));
            svgpath.DrawInImageRatio(contentimg);
            svgpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(contentsvgpath),
                StrokeWidth = 0,
                Fill = new SvgColourServer(Color.Black),
                FillOpacity = 100,
            };
            int textoffset = 6;
            Rectangle textrect = rAngle == IndicatorRotateAngle.AntiClockWise0
                ? new Rectangle(textoffset, textoffset - 2, contentimg.Width - 2 * textoffset, contentimg.Height - 2 * textoffset)
                : new Rectangle(textoffset - 2, textoffset, contentimg.Width - 2 * textoffset, contentimg.Height - 2 * textoffset);
            svgpath.DrawInImageRatio(contentimg, textrect);

            Graphics.FromImage(retimg).DrawImage(contentimg, new Point(EdgeDistance, EdgeDistance));

            return retimg;
        }
        public Bitmap GetCursorImgByText(CursorIconKind cursorKind, Color drawColor)
        {
            String content = String.Empty;
            EdgeDistance = 1;
            IndicatorRotateAngle rAngle = IndicatorRotateAngle.AntiClockWise0;
            Bitmap retimg = new Bitmap(ImgSize.Width, ImgSize.Height);
            Point point = new Point();
            switch (cursorKind)
            {
                case CursorIconKind.CursorSyncV:
                    Bitmap vimg = new Bitmap(GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Resources.WaveIndicator.CusorSync.png"));
                    vimg.RotateFlip(RotateFlipType.Rotate90FlipX);
                    Rectangle targetarea = new Rectangle(retimg.Width - vimg.Width, 1, vimg.Width, retimg.Height - 2);
                    RenderBack(ref retimg, targetarea);
                    Graphics.FromImage(retimg).DrawImage(vimg, targetarea, new Rectangle(0, 0, vimg.Width, vimg.Height), GraphicsUnit.Pixel);
                    return retimg;
                case CursorIconKind.CursorSyncH:
                    Bitmap himg = new Bitmap(GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".Resources.WaveIndicator.CusorSync.png"));
                    Rectangle tarea = new Rectangle(1, retimg.Height - himg.Height, retimg.Width - 2, himg.Height);
                    RenderBack(ref retimg, tarea);
                    Graphics.FromImage(retimg).DrawImage(himg, tarea, new Rectangle(0, 0, himg.Width, himg.Height), GraphicsUnit.Pixel);
                    return retimg;
                case CursorIconKind.CursorAV:
                    rAngle = IndicatorRotateAngle.AntiClockWise0;
                    content = "Ax";
                    point = new Point(2, 0);
                    break;
                case CursorIconKind.CursorAH:
                    rAngle = IndicatorRotateAngle.AntiClockWise90;
                    content = "Ay";
                    point = new Point(1, 1);
                    break;
                case CursorIconKind.CursorBV:
                    rAngle = IndicatorRotateAngle.AntiClockWise0;
                    content = "Bx";
                    point = new Point(2, 0);
                    break;
                case CursorIconKind.CursorBH:
                    content = "By";
                    rAngle = IndicatorRotateAngle.AntiClockWise90;
                    point = new Point(1, 1);
                    break;
                default:
                    throw new ArgumentException("cursorKind");
            }
            //画内容
            Bitmap contentimg = new Bitmap(retimg.Width - EdgeDistance * 2, retimg.Height - EdgeDistance * 2);

            SvgPath svgpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(Properties.Resources.WaveIndicatorCursorAreaSvg),
                StrokeWidth = 0,
                Stroke = new SvgColourServer(Color.Black),
                Fill = new SvgColourServer(drawColor),
                FillOpacity = 100,
            };

            //旋转
            svgpath.AddTransform(GetCommonMatrix(rAngle));
            svgpath.DrawInImageRatio(contentimg);

            // 在图片上添加字母
            Bitmap resultImage = AddTextToImage(contentimg, content, new Font(AppStyleConfig.DefaultFontFamilyName, 10), Color.Black, Color.Transparent, point);

            return resultImage;
        }


        private Bitmap AddTextToImage(Bitmap originalImage, string text, Font font, Color textColor, Color backColor, Point position)
        {
            // 创建一个新的位图，以便在其上绘制文字
            Bitmap newImage = new Bitmap(originalImage);

            // 在新位图上创建Graphics对象
            using (Graphics g = Graphics.FromImage(newImage))
            {
                // 设置绘制质量
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // 创建画刷和背景笔刷
                SolidBrush textBrush = new SolidBrush(textColor);
                SolidBrush backBrush = new SolidBrush(backColor);

                // 在指定位置绘制文字
                g.DrawString(text, font, textBrush, position);

                // 释放资源
                textBrush.Dispose();
                backBrush.Dispose();
            }

            return newImage;
        }

        public Bitmap GetTriggerImg(IndicatorRotateAngle rAngle, Color chlColor)
        {
            EdgeDistance = 0;

            //画背景
            Bitmap retimg = new Bitmap(36, 24);

            //设置内容区域
            Bitmap contentimg = new Bitmap(retimg.Width - EdgeDistance * 2, retimg.Height - EdgeDistance * 2);

            //设置相关的Svg路径
            SvgPath borderpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(Properties.Resources.WaveIndicatorTrigger),
                StrokeWidth = 2,
                Stroke = new SvgColourServer(Color.Black),
                Fill = new SvgColourServer(chlColor/*Color.FromArgb(0, 189, 255)*/),
                FillOpacity = 1
            };

            //旋转
            borderpath.AddTransform(GetCommonMatrix(rAngle));

            //画图像
            borderpath.DrawInImageRatio(contentimg);
            Graphics.FromImage(retimg).DrawImage(contentimg, new Point(EdgeDistance, EdgeDistance));

            return retimg;
        }

        /// <summary>
        /// 获取模拟，数学，参考，解码通道的indicator图标
        /// </summary>
        /// <param name="content">图标里的文字信息</param>
        /// <param name="status">指示器状态</param>
        /// <param name="chlColor">通道颜色</param>
        /// <returns></returns>
        private Bitmap GetCommonImg(string content, IndicatorStatus status, Color chlColor)
        {
            //画背景
            Bitmap retimg = new Bitmap(36, 24);

            //画内容
            Bitmap contentimg = new Bitmap(retimg.Width - EdgeDistance * 2, retimg.Height - EdgeDistance * 2);
            IndicatorRotateAngle imgrotateangle = GetRotateAngle(status);

            //选中状态
            Boolean chosen = (Int32)status % 2 == 1;

            //设置相关的Svg路径
            SvgPath borderpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(Properties.Resources.WaveIndicatorChannelSvg),
                StrokeWidth = 2,
                Stroke = new SvgColourServer(chosen ?  /*Color.FromArgb(150, AppStyleConfig.DefaultTitleForeColor.GetBrightnessColor(-0.7))*/ Color.Black : chlColor),
                FillOpacity = 1,
            };

            borderpath.Fill = chosen ? new SvgColourServer(chlColor) : new SvgColourServer(AppStyleConfig.DefaultAreaBackColor);

            //旋转
            borderpath.AddTransform(GetCommonMatrix(imgrotateangle));

            //画图像
            borderpath.DrawInImageRatio(contentimg);

            //确定文字大小和位置
            Rectangle contentarea = GetCommonContentArea(borderpath
                , imgrotateangle, borderpath.Transforms.Select(t => t.Matrix));
            float fontsize = GetFontSize(contentarea.Size, 1.5F);
            if (content.Contains("M"))
                fontsize = 9;
            Point stringpos = GetStringPos(content, new Font(_FontName, fontsize), contentarea);

            //画文字
            TextRenderer.DrawText(Graphics.FromImage(contentimg), content,
                new Font(_FontName, fontsize),
                stringpos,
                chosen ? AppStyleConfig.DefaultAreaBackColor : chlColor,
                chosen ? chlColor : Color.Transparent);

            Graphics.FromImage(retimg).DrawImage(contentimg, new Point(EdgeDistance, EdgeDistance));

            return retimg;
        }

        /// <summary>
        /// 获取相关旋转角度对应的偏移矩阵
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="svgrect"></param>
        /// <returns></returns>
        private Matrix GetCommonMatrix(IndicatorRotateAngle angle)
        {
            switch (angle)
            {
                case IndicatorRotateAngle.AntiClockWise0:
                    return new Matrix(1, 0, 0, 1, 0, 0);
                case IndicatorRotateAngle.AntiClockWise90:
                    return new Matrix(0, -1, 1, 0, 0, 0);
                case IndicatorRotateAngle.AntiClockWise180:
                    return new Matrix(-1, 0, 0, -1, 0, 0);
                case IndicatorRotateAngle.AntiClockWise270:
                    return new Matrix(0, 1, -1, 0, 0, 0);
                default:
                    throw new ArgumentException("angle");
            }
        }

        /// <summary>
        /// 获取文字放置区域
        /// </summary>
        /// <param name="svgArea">svg对应的rectangle</param>
        /// <param name="borderArea">边界对应的rectangle</param>
        /// <param name="angle"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private Rectangle GetCommonContentArea(SvgPath svgPath, IndicatorRotateAngle angle, IEnumerable<Matrix> matrixs)
        {
            const float offsetX = 1.5F;
            RectangleF svgarea = svgPath.GetPathRect();
            var strokewidth = svgPath.StrokeWidth;
            SizeF newsize = new SizeF(svgarea.Height - strokewidth, svgarea.Height - strokewidth);  //文字区域的大小

            //找到文字旋转后对应的左上角点的当前点
            PointF newOriginPos = new PointF(svgarea.X + strokewidth / 2 + offsetX, svgarea.Y + strokewidth / 2);
            switch (angle)
            {
                case IndicatorRotateAngle.AntiClockWise0:
                    //为初始值
                    break;
                case IndicatorRotateAngle.AntiClockWise90:
                    newOriginPos = new PointF(newOriginPos.X + newsize.Width, newOriginPos.Y);
                    break;
                case IndicatorRotateAngle.AntiClockWise180:
                    newOriginPos = new PointF(newOriginPos.X + newsize.Width, newOriginPos.Y + newsize.Height);
                    break;
                case IndicatorRotateAngle.AntiClockWise270:
                    newOriginPos = new PointF(newOriginPos.X, newOriginPos.Y + newsize.Height);
                    break;
                default:
                    throw new ArgumentException("angle");
            }

            //坐标变换
            Matrix retmatrix = new Matrix(1, 0, 0, 1, 0, 0);
            foreach (var mtx in matrixs)
            {
                retmatrix.Multiply(mtx);
            }
            float finalX = newOriginPos.X * retmatrix.Elements[0] + newOriginPos.Y * retmatrix.Elements[2] + retmatrix.Elements[4];
            float finalY = newOriginPos.X * retmatrix.Elements[1] + newOriginPos.Y * retmatrix.Elements[3] + retmatrix.Elements[5];

            return new Rectangle((Int32)(Math.Round(finalX))
                , (Int32)(Math.Round(finalY))
                , (Int32)newsize.Width
                , (Int32)newsize.Height);
        }

        /// <summary>
        /// 获取数字通道的indicator图像
        /// </summary>
        /// <param name="content"></param>
        /// <param name="status"></param>
        /// <param name="chlColor"></param>
        /// <returns></returns>
        private Bitmap GetDigitalImg(String content, IndicatorStatus status, Color chlColor)
        {
            //画背景
            Bitmap retimg = new Bitmap(ImgSize.Width, ImgSize.Height);
            RenderBack(ref retimg, new Rectangle(-1, -1, retimg.Width, retimg.Height));

            //设置内容区域
            Bitmap contentimg = new Bitmap(retimg.Width - EdgeDistance * 2, retimg.Height - EdgeDistance * 2);
            Color drawcolor = status == IndicatorStatus.RightFull ? Color.Red : chlColor;    //是否选中

            //设置相关的Svg路径
            SvgPath borderpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(Properties.Resources.WaveIndicatorDigitalSvg),
                StrokeWidth = 1,
                Stroke = new SvgColourServer(drawcolor),
                Fill = new SvgColourServer(drawcolor),
                FillOpacity = 1
            };


            //画svg,把svgimg放到contentimg的底部
            Rectangle svgarea = borderpath.GetPathRect()
                .ExtendRectangleRatio(new Rectangle(new Point(0, 0), contentimg.Size));
            Rectangle drawsvgarea = new Rectangle(svgarea.X,
                svgarea.Y + (contentimg.Height - svgarea.Height) / 2, svgarea.Width, svgarea.Height);
            Bitmap svgimg = new Bitmap(drawsvgarea.Width, drawsvgarea.Height);

            //画图像
            borderpath.DrawInImageRatio(svgimg);
            Graphics.FromImage(contentimg).DrawImage(svgimg, new Point(drawsvgarea.X, drawsvgarea.Y));


            //确定文字大小和位置
            Rectangle contentarea = GetDigitalContentArea(drawsvgarea, new Rectangle(new Point(0, 0), contentimg.Size));
            float fontsize = GetFontSize(contentarea.Size);
            Point stringpos = GetStringPos(content, new Font(_FontName, fontsize), contentarea);

            //画文字
            TextRenderer.DrawText(Graphics.FromImage(contentimg), content,
                new Font(_FontName, fontsize),
                stringpos,
                drawcolor);

            Graphics.FromImage(retimg).DrawImage(contentimg, new Point(EdgeDistance, EdgeDistance));

            return retimg;
        }

        /// <summary>
        /// 获取文字放置区域
        /// </summary>
        /// <param name="svgArea"></param>
        /// <param name="borderArea"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private Rectangle GetDigitalContentArea(RectangleF svgArea, Rectangle borderArea)
        {
            int offset = 2;

            return new Rectangle(borderArea.X,
                borderArea.Y + offset,
                borderArea.Width - offset,
                (Int32)(borderArea.Height - svgArea.Height - offset));
        }

        /// <summary>
        /// 获取指示器状态对应的图标旋转角度
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private IndicatorRotateAngle GetRotateAngle(IndicatorStatus status)
        {
            if (status == IndicatorStatus.Down || status == IndicatorStatus.DownFull)
            {
                //逆时针270°
                return IndicatorRotateAngle.AntiClockWise270;
            }
            else if (status == IndicatorStatus.Left || status == IndicatorStatus.LeftFull)
            {
                //逆时针180°
                return IndicatorRotateAngle.AntiClockWise180;
            }
            else if (status == IndicatorStatus.Up || status == IndicatorStatus.UpFull)
            {
                //逆时针90°
                return IndicatorRotateAngle.AntiClockWise90;
            }
            else
            {
                //不旋转
                return IndicatorRotateAngle.AntiClockWise0;
            }
        }

        /// <summary>
        /// 获取文字的字体大小
        /// </summary>
        /// <param name="areaSize"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        private float GetFontSize(Size areaSize, float ratio = 1.3F)
        {
            float defaultfontsize = 10F;
            ChannelId maxsizeid = _SameChlTypeMin;
            Size maxsize = TextRenderer.MeasureText(maxsizeid.ToString(), AppStyleConfig.DefaultLabelFont);

            //找到通道号内容最长的文本
            foreach (ChannelId id in Enum.GetValues(typeof(ChannelId)))
            {
                if (id >= _SameChlTypeMin && id <= _SameChlTypeMax)
                {
                    Size currentsize = TextRenderer.MeasureText(id.ToString(), new Font(_FontName, defaultfontsize));
                    if (currentsize.Width > maxsize.Width)//高度都一致，只用比较宽度
                    {
                        maxsize = currentsize;
                        maxsizeid = id;
                    }
                }
            }

            //换算长度，计算字体大小
            float zoomratio = Math.Min((float)areaSize.Width / maxsize.Width, (float)areaSize.Height / maxsize.Height);
            return defaultfontsize * zoomratio * ratio;
        }

        /// <summary>
        /// 获取文字内容的位置
        /// </summary>
        /// <param name="content"></param>
        /// <param name="font"></param>
        /// <param name="contentArea"></param>
        /// <returns></returns>
        private Point GetStringPos(String content, Font font, Rectangle contentArea)
        {
            Size stringsize = TextRenderer.MeasureText(content, font);
            return new Point(contentArea.X + (contentArea.Width - stringsize.Width) / 2,
                contentArea.Y + (contentArea.Height - stringsize.Height + 4) / 2);
        }

        /// <summary>
        /// 渲染Bitmap的背景
        /// </summary>
        private void RenderBack(ref Bitmap bitmap, Rectangle area)
        {
            //画一个Bitmap内接的圆角矩形背景
            SvgPath backsvgpath = new SvgPath()
            {
                PathData = SvgPathBuilder.Parse(Properties.Resources.WaveIndicatorBackGround),
                Fill = new SvgColourServer(Color.FromArgb(150, AppStyleConfig.DefaultTitleForeColor.GetBrightnessColor(-0.7))),
                FillOpacity = 100,
            };
            backsvgpath.DrawInGraph(Graphics.FromImage(bitmap), area);
        }
    }

    /// <summary>
    /// 指示器的状态
    /// </summary>
    public enum IndicatorStatus
    {
        Right,      //箭头指向右（默认方向）
        RightFull,  //箭头指向右，并且填充
        Left,       //箭头指向左
        LeftFull,   //箭头指向左，并且填充
        Down,       //箭头指向下
        DownFull,   //箭头指向下，并且填充 
        Up,         //箭头指向上
        UpFull      //箭头指向上，并且填充
    }

    /// <summary>
    /// 指示器图标的逆时针旋转角度
    /// </summary>
    public enum IndicatorRotateAngle
    {
        AntiClockWise0,
        AntiClockWise90,
        AntiClockWise180,
        AntiClockWise270
    }

    public enum CursorIconKind
    {
        CursorAV,
        CursorAH,
        CursorBV,
        CursorBH,
        CursorSyncV,
        CursorSyncH,
    }

}
