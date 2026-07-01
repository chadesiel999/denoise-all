using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Common.Tools;

namespace Veldrid.Common.VeldridRender.LineRender
{
    internal unsafe class DataRender : BaseVeldridRender
    {
        #region PipLine
        Dictionary<PrimitiveTopology, Pipeline> _PipLines = new Dictionary<PrimitiveTopology, Pipeline>();
        [AllowNull]
        Pipeline currentPipline;
        PrimitiveTopology currentPrimitive;
        #endregion

        #region vertex
        [AllowNull]
        DeviceBuffer vertexBuffer;
        VertexLayoutDescription vertexLayout;
        #endregion

        #region Uniform
        [AllowNull]
        Shader[] shaders;
        /// <summary>
        /// rgbafloat color
        /// vec4 linerange
        /// float VerticalOffset
        /// float HorizontalOffset
        /// float Brightness
        /// float spare
        /// </summary>
        [AllowNull]
        DeviceBuffer lineInfoBuffer;

        [AllowNull]
        DeviceBuffer proviewBuffer;
        [AllowNull]
        ResourceLayout sharedLayout;
        [AllowNull]
        ResourceSet sharedSet;
        #endregion 
        public DataRender(IVeldridContent control, int maxDataCount = 5000) : base(control)
        {
            MaxDataCount = maxDataCount;
        }
        public unsafe void WriteData(uint offsetCount, Vector2[] data)
        {
            lock (_Locker)
            {
                if (data == null || data.Length == 0 || IsDisposed || vertexBuffer.IsDisposed) return;
                CheckBufferLength(Unsafe.SizeOf<Vector2>() * data.Length);
                GraphicsDevice.UpdateBuffer(vertexBuffer, offsetCount * (uint)Unsafe.SizeOf<Vector2>(), data);
            }

        }

        private void CheckBufferLength(int databufferlength)
        {
            if (vertexBuffer.SizeInBytes < databufferlength)
            {
                vertexBuffer?.Dispose();
                vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)databufferlength, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            }
        }

        public override void CreateResources()
        {
            base.CreateResources();
            vertexBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<Vector2>() * MaxDataCount), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            vertexLayout = new VertexLayoutDescription(new VertexElementDescription("in_Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
            shaders = CreateShader("DataRender");
            ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
            {
                new ResourceLayoutElementDescription("ProView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("LineInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            };
            proviewBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 2, BufferUsage.Dynamic | BufferUsage.UniformBuffer));
            lineInfoBuffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>() * 2), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
            sharedLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescription);
            BindableResource[] bindableResources = new BindableResource[] { proviewBuffer, lineInfoBuffer };
            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
            sharedSet = ResourceFactory.CreateResourceSet(resourceSetDescription);
            this[nameof(Range), nameof(Margin)] = true;
        }
        public BlendStateDescription BlendState { get; set; } = BlendStateDescription.SingleAlphaBlend;
        private Pipeline GetPipeline(PrimitiveTopology primitiveTopology)
        {
            if (currentPipline != null && currentPrimitive == primitiveTopology)
            {
                return currentPipline;
            }
            if (_PipLines.TryGetValue(primitiveTopology, out Pipeline pipeline))
            {
                currentPipline = pipeline;
                currentPrimitive = primitiveTopology;
                return pipeline;
            }
            else
            {
                pipeline = CreatePipLine(primitiveTopology, sharedLayout, vertexLayout, shaders, BlendState);
                _PipLines[primitiveTopology] = pipeline;
                currentPipline = pipeline;
                currentPrimitive = primitiveTopology;
                return pipeline;
            }
        }
        public int MaxDataCount { get; private set; }
        public DataRenderConfig[] DataRenderConfigs { get; set; } = new DataRenderConfig[0];

        private uint _offsiteBrightness = (uint)(Unsafe.SizeOf<RgbaFloat>() * 2 + Unsafe.SizeOf<float>() * 2);
        internal override void DrawData()
        {
            if (DataRenderConfigs.Length == 0 || IsDisposed || vertexBuffer.IsDisposed || lineInfoBuffer.IsDisposed || proviewBuffer.IsDisposed) return;

            CommandList.SetVertexBuffer(0, vertexBuffer);
            uint offset = 0;
            for (int i = 0; i < DataRenderConfigs.Length; i++)
            {
                if (DataRenderConfigs[i].DataLenght == 0)
                {
                    offset += DataRenderConfigs[i].FixedDataLenght;
                    continue;
                }
                var pipline = GetPipeline(DataRenderConfigs[i].Primitive);
                CommandList.SetPipeline(pipline);
                CommandList.SetGraphicsResourceSet(0, sharedSet);

                uint start = 0;
                foreach (var p in DataRenderConfigs[i].PointConfigs)
                {
                    if (p == null)
                        continue;
                    CommandList.UpdateBuffer(lineInfoBuffer, 0, p.Color.ColorConverToRGBA());
                    CommandList.UpdateBuffer(lineInfoBuffer, _offsiteBrightness, p.Brightness);

                    if (p.PointCounts == null && p.VertexCount != null)
                    {
                        if (p.Visibily)
                            CommandList.Draw(p.VertexCount.Value, 1, offset, 0);

                        start += p.VertexCount.Value;
                    }
                    else if (p.PointCounts != null && p.PointCounts.Length > 0)
                    {
                        foreach (var count in p.PointCounts)
                        {
                            if (count == null)
                                continue;

                            if (p.Visibily && count.Visibily)
                                CommandList.Draw(count.PointCount, 1, start + offset, 0);
                            start += count.FixedPointCount;
                        }
                    }
                    else
                    {
                        CommandList.Draw((uint)DataRenderConfigs[i].DataLenght, 1, offset, 0);
                    }
                }

                offset += (uint)DataRenderConfigs[i].FixedDataLenght;
            }
        }
        private float horizontalOffset;

        public float HorizontalOffset
        {
            get { return horizontalOffset; }
            set { Set(ref horizontalOffset, value); }
        }
        private float verticalOffset;

        public float VerticalOffset
        {
            get { return verticalOffset; }
            set { Set(ref verticalOffset, value); }
        }
        public override void DisposeResources()
        {
            lock (_Locker)
            {
                vertexBuffer?.Dispose();
                lineInfoBuffer?.Dispose();
                sharedLayout?.Dispose();
                sharedSet?.Dispose();
                _PipLines.Values.ToList().ForEach(x => x.Dispose());
                base.DisposeResources();
            }
        }
        internal override void PreDraw()
        {
            if (IsDisposed || proviewBuffer.IsDisposed || lineInfoBuffer.IsDisposed) return;
            base.PreDraw();
            if (this[nameof(Range), nameof(Margin)] || WindowSizeState)
            {
                CommandList.UpdateBuffer(proviewBuffer, 0, Camera.OrthographicMatrix);
                CommandList.UpdateBuffer(proviewBuffer, (uint)Unsafe.SizeOf<Matrix4x4>(), Camera.GetLineMatrix(Margin, Range));
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)Unsafe.SizeOf<RgbaFloat>(), (Vector4)Range);
                this[nameof(Range), nameof(Margin)] = false;
                if (WindowSizeState) WindowSizeState = false;
            }
            if (this[nameof(HorizontalOffset)])
            {
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>() + Unsafe.SizeOf<float>()), HorizontalOffset);
                this[nameof(HorizontalOffset)] = false;
            }
            if (this[nameof(VerticalOffset)])
            {
                CommandList.UpdateBuffer(lineInfoBuffer, (uint)(Unsafe.SizeOf<RgbaFloat>() + Unsafe.SizeOf<Vector4>()), VerticalOffset);
                this[nameof(VerticalOffset)] = false;
            }
        }
    }
    internal class DataRenderConfig
    {
        /// <summary>
        /// 实际数据长度
        /// </summary>
        public uint DataLenght { get; set; }
        //public Color Color { get; set; }
        public PrimitiveTopology Primitive { get; set; }
        //public List<uint> TranglePointCount { get; } = new List<uint>();
        public PointConfig[] PointConfigs { get; set; } = new PointConfig[0];
        /// <summary>
        /// 修正后数据长度
        /// 不能比<see cref="DataLenght"/>小
        /// </summary>
        public uint FixedDataLenght { get; set; }
    }
    public class PointConfig
    {
        public Color Color;
        public float Brightness;
        public PointVisibily[] PointCounts;

        /// <summary>
        /// 顶点数量
        /// </summary>
        public uint? VertexCount { get; set; }

        /// <summary>
        /// 是否绘制
        /// </summary>
        public Boolean Visibily { get; set; } = true;

        public PointConfig()
        {
            Color = Color.Transparent;
            Brightness = 100;
            PointCounts = new PointVisibily[0];
        }
    }
    public class PointVisibily
    {
        public uint PointCount;
        public Boolean Visibily;
        public uint FixedPointCount;
        public PointVisibily() : this(0)
        {

        }
        public PointVisibily(uint count, bool visibily = true)
        {
            PointCount = count;
            Visibily = visibily;
            FixedPointCount = count;
        }

        public static implicit operator PointVisibily(uint count)
        {
            return new PointVisibily(count);
        }
        public static implicit operator PointVisibily(int count)
        {
            return new PointVisibily((uint)count);
        }
    }
}
