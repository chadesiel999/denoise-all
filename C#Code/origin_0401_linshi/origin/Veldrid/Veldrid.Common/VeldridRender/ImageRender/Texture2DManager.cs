using FontStashSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    internal class Texture2DManager : ITexture2DManager,IDisposable
    {
        private readonly GraphicsDevice _gd;
        private readonly List<WeakReference<Texture>> _textures;
        private bool _disposed;
        public Texture2DManager(GraphicsDevice gd)
        {
            _gd = gd;
            _textures = new List<WeakReference<Texture>>();
        }

        ~Texture2DManager()
        {
            Dispose(false);
        }

        public Veldrid.Texture CreateTexture(int width, int height)
        {
            var texture = _gd.ResourceFactory.CreateTexture(
                new TextureDescription(
                    (uint)width, (uint)height, 1,
                    1, 1,
                    PixelFormat.B8_G8_R8_A8_UNorm,
                    TextureUsage.Sampled,
                    TextureType.Texture2D,
                    TextureSampleCount.Count1));

            _textures.Add(new WeakReference<Texture>(texture));
            return texture;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;

            if (disposing)
            {
                foreach (var wt in _textures)
                    if (wt.TryGetTarget(out var texture))
                        texture.Dispose();
            }
        }

        public Size GetTextureSize(Veldrid.Texture texture)
        {
            var t = texture as Texture;
            if (t is null)
                return Size.Empty;

            return new Size((int)t.Width, (int)t.Height);
        }

        public void SetTextureData(Veldrid.Texture texture, System.Drawing.Rectangle bounds, byte[] data)
        {
            if (texture is  Texture t)
                _gd.UpdateTexture(t, data, (uint)bounds.X, (uint)bounds.Y, 0, (uint)bounds.Width, (uint)bounds.Height, 1, 0, 0);
        }
    }
}
