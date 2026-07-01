using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.VeldridRender.ImageRender
{
    /// <summary>
    /// A wrapper with <see cref="ITexture2D"/> interface.
    /// </summary>
    public struct TextureWrapper : ITexture2D, IEquatable<TextureWrapper>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TextureWrapper"/>.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TextureWrapper(Texture texture)
        {
            if (texture is null)
                throw new ArgumentNullException(nameof(texture));

            Texture = texture;
        }

        /// <summary>
        /// Texture
        /// </summary>
        public Texture Texture { get; }

        /// <summary>
        /// Size of texture
        /// </summary>
        public Size Size => new Size((int)Texture.Width, (int)Texture.Height);

        /// <inheritdoc/>
        public bool IsDisposed => Texture.IsDisposed;

        /// <inheritdoc/>
        public override int GetHashCode() => Texture.GetHashCode();

        /// <inheritdoc/>
        public bool Equals(TextureWrapper other) => Texture == other.Texture;

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj) =>
             obj is TextureWrapper tw && Equals(tw);

        /// <inheritdoc/>
        public override string? ToString() => Texture.ToString();

        /// <inheritdoc/>
        public void Dispose() => Texture.Dispose();

        /// <inheritdoc/>
        public static implicit operator TextureWrapper(Texture t) => new TextureWrapper(t);

        /// <inheritdoc/>
        public static implicit operator Texture(TextureWrapper t) => t.Texture;
    }
}
