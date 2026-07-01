using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    /// <summary>
    /// Represents a 2D texture to use in <see cref="SpriteBatch{TTexture}"/>.
    /// </summary>
    public interface ITexture2D : IDisposable
    {
        /// <summary>
        /// A bool indicating whether this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; }

        /// <summary>
        /// The total size of the texture.
        /// </summary>
        public Size Size { get; }
    }
}
