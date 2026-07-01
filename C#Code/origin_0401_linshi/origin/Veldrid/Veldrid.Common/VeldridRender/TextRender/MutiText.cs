using FontStashSharp;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Drawing;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.Plot;
using Veldrid.Common.Tools;
using System.Net.Http.Headers;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Veldrid.Common.VeldridRender.TextRender
{
    internal class MutiText : BaseVeldridRender
    {
        //Framebuffer frameBuffer;
        //Texture textureBuffer;
        Matrix4x4 matrix4 = Matrix4x4.Identity;
        StashRenderer spriteBatch;
        [AllowNull]
        DynamicSpriteFont sizedFont;
        [AllowNull]
        FontSystem veldridFont;
        //ImageRender.VeldridSpriteBatch spriteBatchSpriteBatch;

        private Vector2 Scale => new Vector2(1f, 1f);
        private bool needupdate = true;
        private Vector2 _ImagePoint;
        private BlendStateDescription _NomalBlend = new BlendStateDescription()
        {
            AlphaToCoverageEnabled = false,
            BlendFactor = RgbaFloat.Clear,
            AttachmentStates = new BlendAttachmentDescription[]
{
                new BlendAttachmentDescription()
                {
                    AlphaFunction = BlendFunction.Maximum,
                    ColorFunction = BlendFunction.Maximum,
                    BlendEnabled = true,
                    SourceAlphaFactor= BlendFactor.Zero,
                    DestinationAlphaFactor = BlendFactor.DestinationAlpha,
                    SourceColorFactor= BlendFactor.Zero,
                    DestinationColorFactor = BlendFactor.DestinationColor,
                }
}
        };
        public MutiText(IVeldridContent device, Boolean supportbackColor = false, Boolean supportbackTransparent = true) : base(device)
        {
            spriteBatch = new StashRenderer(GraphicsDevice, CommandList, CreateShader("TextRender"), CreateShader("TextRenderBackColor"), supportbackColor ? BlendStateDescription.SingleAlphaBlend : BlendStateDescription.SingleOverrideBlend, supportbackTransparent ? BlendStateDescription.SingleAlphaBlend : BlendStateDescription.SingleOverrideBlend);
            //spriteBatchSpriteBatch = new VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"),GraphicsDevice.LinearSampler, FaceCullMode.Back,_NomalBlend);
            CreateBuffer();
        }

        private void CreateBuffer()
        {
            //var texturedesc = TextureDescription.Texture2D(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 1, 1, MainSwapchainBuffer.ColorTargets[0].Target.Format, TextureUsage.RenderTarget | TextureUsage.Sampled);
            //if (frameBuffer == null || frameBuffer.Width != MainSwapchainBuffer.Width || frameBuffer.Height != MainSwapchainBuffer.Height)
            //{
            //    textureBuffer?.Dispose();
            //    textureBuffer = ResourceFactory.CreateTexture(texturedesc);
            //    frameBuffer?.Dispose();
            //    frameBuffer = ResourceFactory.CreateFramebuffer(new FramebufferDescription(null, textureBuffer));
            //    needupdate = true;
            //}
            matrix4 = Matrix4x4.CreateOrthographic(MainSwapchainBuffer.Width, MainSwapchainBuffer.Height, 0.01f, -100f);
        }


        internal DynamicSpriteFont VeldridFont => sizedFont;
        internal Texture Texture => spriteBatch.Texture;
        public override void CreateResources()
        {
            base.CreateResources();
            LoadFont();
        }
        private void LoadFont()
        {
            if (String.IsNullOrEmpty(FontName))
            {
                _FontName = "MiSans";
            }
            if (String.IsNullOrEmpty(FontStyle))
            {
                _FontStyle = "Regular";
            }
            veldridFont?.Dispose();

            veldridFont = FontManger.Instance.LoadFromSystemDirectory(FontName, FontStyle);
            sizedFont = veldridFont.GetFont(FontSize);
            needupdate = true;

        }

        //public override Vector2 VirtualSize => GetVirtualSize();


        private Vector2 GetSize(string text)
        {
            return sizedFont.MeasureString(text, Scale);
        }
        public Vector2 GetVirtualSize(string text)
        {
            Vector2 vector = sizedFont.MeasureString(text, Scale);
            return new Vector2(vector.X / Rectangle.Width * (Range.MaxX - Range.MinX), vector.Y / Rectangle.Height * (Range.MaxY - Range.MinY));
        }

        internal override void PreDraw()
        {
            base.PreDraw();
            if (WindowSizeState)
            {
                CreateBuffer();
                WindowSizeState = false;
            }
            if (this[nameof(FontName), nameof(FontStyle)])
            {
                LoadFont();
                this[nameof(FontName), nameof(FontStyle)] = false;
            }
        }

        public TextInfo[] TextInfos
        {
            get => textInfos;
            set
            {
                if (!textInfos.SequenceEqual(value))
                {
                    textInfos = value;
                    needupdate = true;
                    foreach (var val in textInfos)
                    {
                        val.PropertyChanged += (_, _) => needupdate = true;
                    }
                }
            }
        }
        internal override void DrawData()
        {
            if (!Visibily || TextInfos == null || TextInfos.Length == 0 || spriteBatch == null) return;
            foreach (var info in TextInfos)
            {
                if (String.IsNullOrEmpty(info.Text) || !Visibily || !info.Visibily || info.Color.A == 0) continue;

                spriteBatch.Begin();
                spriteBatch.BackColor = info.BackColor.ColorConverToRGBA();
                spriteBatch.BackColorFixedSize = info.FixBackColorSize;
                Vector2 size = GetSize(info.Text);
                _ImagePoint = this.GetImagePoint(info.Local);
                sizedFont.DrawText(spriteBatch, info.Text, _ImagePoint, info.Color.ColorConverToRGBA(), Scale, info.Rotation, GetOrigin(size, info.HorizontalAlignment, info.VerticalAlignment), 1f);

                spriteBatch.End();
            }
            needupdate = false;
        }
        private Vector2 GetImagePoint(PointF point)
        {
            float x = Rectangle.Width / Range.XLenght * (point.X - Range.MinX) + Margin.Left;
            float y = Rectangle.Height / Range.YLenght * (Range.MaxY - point.Y) + Margin.Top;
            return new Vector2(x, y);
        }
        public Vector2 MeasureSize(String str)
        {
            return sizedFont.MeasureString(str, Scale);
        }
        private Vector2 GetOrigin(Vector2 size, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            Vector2 origin = Vector2.Zero;
            switch (horizontal)
            {
                case HorizontalAlignment.Left:
                default:
                    break;
                case HorizontalAlignment.Right:
                    origin.X = size.X;
                    break;
                case HorizontalAlignment.Center:
                    origin.X = size.X / 2;
                    break;
            }
            switch (vertical)
            {
                case VerticalAlignment.Top:
                default:
                    origin.Y = FontSize;
                    break;
                case VerticalAlignment.Bottom:
                    origin.Y = size.Y * 1.5f;
                    break;
                case VerticalAlignment.Center:
                    origin.Y = size.Y * 1.25f;
                    break;
            }
            return origin;
        }

        private float fontSize = 10;

        public float FontSize
        {
            get { return fontSize; }
            set
            {
                if (value != fontSize && value > 0)
                {
                    fontSize = value;
                    sizedFont = veldridFont.GetFont(value);
                }
            }
        }
        private String _FontStyle = "Normal";

        public String FontStyle
        {
            get { return _FontStyle; }
            set { Set(ref _FontStyle, value); }
        }

        private String _FontName = "MiSans";

        public String FontName
        {
            get { return _FontName; }
            set { Set(ref _FontName, value); }
        }

        private TextInfo[] textInfos = new TextInfo[0];

        public override void DisposeResources()
        {
            lock (_Locker)
            {
                veldridFont?.Dispose();
                veldridFont = null;
                sizedFont = null;
                spriteBatch?.Dispose();
                if (textInfos != null && textInfos.Length != 0)
                {
                    for (int i = 0; i < textInfos.Length; i++)
                    {
                        textInfos[i].Dispose();
                    }
                }
                spriteBatch = null;
                textInfos = new TextInfo[0];
                base.DisposeResources();
            }
        }
    }

    internal class TextInfo : BaseProperty
    {
        private string text = string.Empty;
        private Color color = Color.White;
        private bool visibily = true;
        private Color backColor = Color.Empty;
        private SizeF fixBackColorSize = SizeF.Empty;
        private float rotation = 0;
        private VerticalAlignment verticalAlignment = VerticalAlignment.Center;
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
        private PointF local = PointF.Empty;

        public PointF Local { get => local; set => Set(ref local, value); }
        public String Text { get => text; set => Set(ref text, value); }
        public Color Color { get => color; set => Set(ref color, value); }
        public Boolean Visibily { get => visibily; set => Set(ref visibily, value); }
        public Color BackColor { get => backColor; set => Set(ref backColor, value); }
        public SizeF FixBackColorSize { get => fixBackColorSize; set => Set(ref fixBackColorSize, value); }
        public float Rotation { get => rotation; set => Set(ref rotation, value); }
        public VerticalAlignment VerticalAlignment { get => verticalAlignment; set => Set(ref verticalAlignment, value); }
        public HorizontalAlignment HorizontalAlignment { get => horizontalAlignment; set => Set(ref horizontalAlignment, value); }
        public override bool Equals(object obj)
        {
            if (obj is TextInfo info)
            {
                return info.Text == text
                    && info.Local == local
                    && info.Color == color
                    && info.Visibily == visibily
                    && info.BackColor == BackColor
                    && info.FixBackColorSize == FixBackColorSize
                    && info.Rotation == Rotation
                    && info.VerticalAlignment == verticalAlignment
                    && info.HorizontalAlignment == horizontalAlignment;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
