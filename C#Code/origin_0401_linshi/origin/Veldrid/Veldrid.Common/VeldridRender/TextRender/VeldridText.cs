using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Veldrid.SPIRV;
using System.Numerics;
using System.IO;
using FontStashSharp;
using FontStashSharp.Interfaces;
using Veldrid.Common.VeldridRender.ImageRender;
using Veldrid.Common.Tools;
using System.Diagnostics.CodeAnalysis;
using Veldrid.Common.Plot;

namespace Veldrid.Common.VeldridRender.TextRender
{
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
    }
    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom,
    }
    internal class VeldridText : BaseVeldridRender
    {
        //Framebuffer frameBuffer;
        Matrix4x4 matrix4 = Matrix4x4.Identity;
        //Texture textureBuffer;
        StashRenderer spriteBatch;
        [AllowNull]
        DynamicSpriteFont sizedFont;
        //VeldridSpriteBatch veldridSpriteBatch;
        [AllowNull]
        FontSystem veldridFont;
        bool needupdate = true;

        private Vector2 Scale => new Vector2(1f, 1f);
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
        public VeldridText(IVeldridContent device,Boolean supportbackColor=false,Boolean supportbackTransparent = true) : base(device)
        {
            spriteBatch = new StashRenderer(GraphicsDevice, CommandList, CreateShader("TextRender"), CreateShader("TextRenderBackColor"),supportbackColor?BlendStateDescription.SingleAlphaBlend:BlendStateDescription.SingleOverrideBlend, supportbackTransparent? BlendStateDescription.SingleAlphaBlend:BlendStateDescription.SingleOverrideBlend);
            //veldridSpriteBatch = new VeldridSpriteBatch(GraphicsDevice, MainSwapchainBuffer.OutputDescription, CreateShader("ImageRender"),GraphicsDevice.LinearSampler, FaceCullMode.Back,_NomalBlend);
            BackColor = Color.Transparent;
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
            //matrix4 = Matrix4x4.CreateOrthographic(frameBuffer.Width, frameBuffer.Height, 0.01f, -100f);
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

        public HorizontalAlignment HorizontalAlignment
        { 
            get => horizontalAlignment;
            set
            {
                if(horizontalAlignment!=value)
                {
                horizontalAlignment = value;
                    needupdate = true;
                }
            }
        }
        public VerticalAlignment VerticalAlignment 
        {
            get => verticalAlignment;
            set
            {
                if (verticalAlignment != value)
                {
                    verticalAlignment = value;
                    needupdate = true;
                }
            }
        }
        public override Vector2 VirtualSize => GetVirtualSize();
        private Vector2 GetImagePoint(PointF point)
        {
            float x = Rectangle.Width / Range.XLenght * (point.X - Range.MinX) + Rectangle.Left;
            float y = Rectangle.Height / Range.YLenght * (Range.MaxY - point.Y) + Rectangle.Top;
            return new Vector2(x, y);
        }

        public Vector2 GetSize()
        {
            if (sizedFont == null) return Vector2.Zero;
            return sizedFont.MeasureString(Text, Scale);
        }
        public Vector2 GetVirtualSize()
        {
            if(sizedFont ==null) return Vector2.Zero;
            Vector2 vector = sizedFont.MeasureString(Text, Scale);
            return this.LocalSizeToVirtualSize(vector); 
        }

        internal override void PreDraw()
        {
            base.PreDraw();
            if(WindowSizeState)
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
        internal override void DrawData()
        {
            if (!String.IsNullOrEmpty(Text) && Visibily && spriteBatch!=null)
            {
                spriteBatch.Begin();
                Vector2 size = GetSize();
                _ImagePoint = this.GetImagePoint(Local);
                sizedFont.DrawText(spriteBatch, Text, _ImagePoint, Color.ColorConverToRGBA(), Scale, 0, GetOrigin(size), 1f);
                spriteBatch.End();
                needupdate = false;
            }
        }
        public Vector2 MeasureSize(String str)
        {
            if (sizedFont == null) return Vector2.Zero;
            return sizedFont.MeasureString(str, Scale);
        }
        private Vector2 GetOrigin(Vector2 size)
        {
            Vector2 origin = Vector2.Zero;
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    break;
                case HorizontalAlignment.Right:
                    origin.X = size.X;
                    break;
                case HorizontalAlignment.Center:
                    origin.X = size.X/2;
                    break;
            }
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                default:
                    origin.Y = FontSize;
                    break;
                case VerticalAlignment.Bottom:
                    origin.Y = size.Y*1.5f;
                    break;
                case VerticalAlignment.Center:
                    origin.Y = size.Y*1.25f;
                    break;
            }
            return origin;
        }
        private string text = "Veldrid Control";

        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    Set(ref text, value);
                    needupdate = true;
                }
            }
        }
        private System.Drawing.Color color = System.Drawing.Color.Black;

        public System.Drawing.Color Color
        {
            get { return color; }
            set 
            {
                if (color != value)
                {
                    Set(ref color, value);
                    needupdate = true;
                }
            }
        }
        public SizeF BackColorFixedSize { get => spriteBatch.BackColorFixedSize; set => spriteBatch.BackColorFixedSize = value; }
        public Color BackColor
        {
            get => backColor;
            set
            {
                if (value != backColor && spriteBatch!=null)
                {
                    backColor = value;
                    spriteBatch.BackColor = value.ColorConverToRGBA();
                    needupdate = true;
                }
            }
        }
        private float rotation = 0;

        public float Rotation
        {
            get { return rotation; }
            set 
            {
                if (rotation != value)
                {
                    Set(ref rotation, value);
                    needupdate = true;
                }
            }
        }

        private float fontSize = 10;

        public float FontSize
        {
            get { return fontSize; }
            set
            {
                if (value != fontSize && value > 0 && sizedFont!=null)
                {
                    fontSize = value;
                    sizedFont = veldridFont.GetFont(value);
                    needupdate = true;
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
            set 
            { 
                Set(ref _FontName, value);
            }
        }

        private PointF local = new PointF();
        private Color backColor = Color.Transparent;
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
        private VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;

        public override PointF Local 
        { 
            get => local;
            set
            {
                if (local != value)
                {
                    Set(ref local, value);
                    needupdate = true;
                }
            }
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                spriteBatch?.Dispose();
                spriteBatch = null;
                veldridFont?.Dispose();
                veldridFont = null;
                sizedFont = null;
                base.DisposeResources();
            }
        }
    }
}
