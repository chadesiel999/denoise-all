using FontStashSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.VeldridRender.ImageRender;

namespace Veldrid.Common.VeldridRender.TextRender
{
    internal class ThreeDimMutiText : BaseVeldridRender
    {

        ThreeStashRenderer spriteBatch;
        [AllowNull]
        DynamicSpriteFont sizedFont;
        [AllowNull]
        FontSystem veldridFont;

        private bool needupdate = true;
        public ThreeDimMutiText(IVeldridContent control) : base(control)
        {
            spriteBatch = new ThreeStashRenderer(GraphicsDevice, CommandList, CreateShader("TextRender"));
        }
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
        public TextInfo[] TextInfos { get; set; } = new TextInfo[0];
        internal override void PreDraw()
        {
            base.PreDraw();
            if(needupdate)
            {
                needupdate = false;
            }
        }

        internal unsafe override void DrawData()
        {
            if (!Visibily) return;

            Matrix4x4 matrix = Camera.ProjectionMatrix*Camera.ViewMatrix*Camera.ModelMatrix;
            if (TextInfos != null && TextInfos.Length > 0)
            {
                spriteBatch.Begin();
                foreach (var val in TextInfos.Where(x => x.Visibily && !String.IsNullOrEmpty(x.Text) && x.Color != RgbaFloat.Clear))
                {
                    var bounds = sizedFont.TextBounds(val.Text, val.Position);
                    float sc = 0.001f/(bounds.Y2 - bounds.Y);
                    sc = 1;
                    //sc = 10;
                    sizedFont.DrawText(spriteBatch, val.Text, val.Position, val.Color, new Vector2(sc,sc), val.Rotation, layerDepth: val.LayerDepth);
                }
                spriteBatch.End(matrix);
            }
        }
        public override void DisposeResources()
        {
            base.DisposeResources();
            spriteBatch?.Dispose();
            veldridFont?.Dispose();
            veldridFont= null;
            sizedFont = null;
            
        }
        private float fontSize = 12;

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

        public class TextInfo
        {
            public string Text { get; set; } = string.Empty;
            public Vector2 Position { get; set; } = Vector2.Zero;
            public float Height { get; set; } = 0.1f;
            public RgbaFloat Color { get; set; } = RgbaFloat.White;
            public Boolean Visibily { get; set; } = true;
            public float LayerDepth { get; set; } = 0;
            public float Rotation { get; set; } = 0;
        }
    }
}
